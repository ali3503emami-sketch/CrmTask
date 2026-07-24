import { useState } from 'react'
import { Button, Checkbox, Drawer, Empty, Form, Input, Select, Space, Tag, Typography, message } from 'antd'
import { PersianDateTimeField } from '../../shared/date/PersianDateTimeField'
import { useCurrentUser } from '../../shared/currentUser/CurrentUserContext'
import { useSetChecklistItemValue } from './useSetChecklistItemValue'
import { useUpdateTask } from './useUpdateTask'
import { useReferTask } from './useReferTask'
import { ChecklistFieldsEditor } from './ChecklistFieldsEditor'
import { checklistItemsToFormValues, choiceFieldTypes, parseDropdownOptions, type ChecklistFieldFormValue } from './checklistFieldTypes'
import type { ChecklistItem, TaskItem, TaskItemStatus } from './types'

const { Text, Title } = Typography

const statusLabel: Record<TaskItemStatus, { text: string; color: string }> = {
  Open: { text: 'باز', color: 'processing' },
  Done: { text: 'انجام‌شده', color: 'success' },
}

interface ChecklistFieldProps {
  taskId: string
  item: ChecklistItem
  disabled: boolean
}

function ChecklistField({ taskId, item, disabled }: ChecklistFieldProps) {
  const setValue = useSetChecklistItemValue()
  const commit = (value: string | null) => setValue.mutate({ taskId, checklistItemId: item.id, value })

  switch (item.fieldType) {
    case 'Checkbox':
      return (
        <Checkbox
          disabled={disabled}
          checked={item.value === 'true'}
          onChange={(e) => commit(e.target.checked ? 'true' : 'false')}
        >
          {item.label}
        </Checkbox>
      )
    case 'Dropdown':
      return (
        <div>
          <Text>{item.label}</Text>
          <Select
            disabled={disabled}
            style={{ width: '100%', marginTop: 4 }}
            value={item.value ?? undefined}
            onChange={commit}
            options={item.options.map((o) => ({ value: o, label: o }))}
          />
        </div>
      )
    case 'TextBox':
      return (
        <div>
          <Text>{item.label}</Text>
          <Input
            disabled={disabled}
            style={{ marginTop: 4 }}
            defaultValue={item.value ?? ''}
            onBlur={(e) => commit(e.target.value || null)}
          />
        </div>
      )
    case 'MultilineText':
      return (
        <div>
          <Text>{item.label}</Text>
          <Input.TextArea
            disabled={disabled}
            rows={3}
            style={{ marginTop: 4 }}
            defaultValue={item.value ?? ''}
            onBlur={(e) => commit(e.target.value || null)}
          />
        </div>
      )
    default:
      return null
  }
}

interface EditTaskFormValues {
  title: string
  description?: string
  dueAt: string
  customerId?: string
  assignedToStaffId: string
  checklistFields?: ChecklistFieldFormValue[]
}

interface ReferFormValues {
  referredToStaffId: string
  note: string
}

interface TaskDetailPanelProps {
  task: TaskItem
  assigneeName: string
  customerName: string | undefined
  customers: { id: string; name: string }[]
  staff: { id: string; fullName: string }[]
  open: boolean
  onClose: () => void
}

export function TaskDetailPanel({ task, assigneeName, customerName, customers, staff, open, onClose }: TaskDetailPanelProps) {
  const { currentStaffId } = useCurrentUser()
  const [isEditing, setIsEditing] = useState(false)
  const [isReferring, setIsReferring] = useState(false)
  const updateTask = useUpdateTask()
  const referTask = useReferTask()
  const [form] = Form.useForm<EditTaskFormValues>()
  const [referForm] = Form.useForm<ReferFormValues>()

  const staffNameById = new Map(staff.map((s) => [s.id, s.fullName]))
  const isLocked = task.status === 'Done'
  const isCreator = currentStaffId === task.createdByStaffId
  const canRefer =
    currentStaffId != null &&
    (currentStaffId === task.assignedToStaffId || task.referrals.some((r) => r.referredToStaffId === currentStaffId))

  const handleSubmit = async (values: EditTaskFormValues) => {
    if (!currentStaffId) {
      message.warning('لطفاً ابتدا مشخص کنید شما کیستید.')
      return
    }
    await updateTask.mutateAsync({
      taskId: task.id,
      input: {
        title: values.title,
        description: values.description ?? '',
        dueAt: values.dueAt,
        customerId: values.customerId ?? null,
        assignedToStaffId: values.assignedToStaffId,
        requestedByStaffId: currentStaffId,
        checklistFields: (values.checklistFields ?? []).map((f) => ({
          label: f.label,
          fieldType: f.fieldType,
          options: choiceFieldTypes.includes(f.fieldType) ? parseDropdownOptions(f.options ?? '') : null,
        })),
      },
    })
    setIsEditing(false)
  }

  const handleRefer = async (values: ReferFormValues) => {
    if (!currentStaffId) {
      message.warning('لطفاً ابتدا مشخص کنید شما کیستید.')
      return
    }
    await referTask.mutateAsync({
      taskId: task.id,
      input: { referredByStaffId: currentStaffId, referredToStaffId: values.referredToStaffId, note: values.note },
    })
    referForm.resetFields()
    setIsReferring(false)
  }

  return (
    <Drawer title="مشاهده کار" open={open} onClose={onClose} size="default">
      {isEditing ? (
        <Form
          form={form}
          layout="vertical"
          onFinish={handleSubmit}
          initialValues={{
            title: task.title,
            description: task.description,
            dueAt: task.dueAt,
            customerId: task.customerId ?? undefined,
            assignedToStaffId: task.assignedToStaffId,
            checklistFields: checklistItemsToFormValues(task.checklistItems),
          }}
        >
          <Form.Item name="title" label="عنوان" rules={[{ required: true, message: 'عنوان الزامی است' }]}>
            <Input />
          </Form.Item>
          <Form.Item name="description" label="توضیحات">
            <Input.TextArea rows={2} />
          </Form.Item>
          <Form.Item name="dueAt" label="سررسید" rules={[{ required: true, message: 'سررسید الزامی است' }]}>
            <PersianDateTimeField placeholder="انتخاب سررسید" />
          </Form.Item>
          <Form.Item name="customerId" label="مشتری (اختیاری)">
            <Select
              allowClear
              placeholder="بدون مشتری (داخلی)"
              options={customers.map((c) => ({ value: c.id, label: c.name }))}
            />
          </Form.Item>
          <Form.Item
            name="assignedToStaffId"
            label="مسئول"
            rules={[{ required: true, message: 'انتخاب مسئول الزامی است' }]}
          >
            <Select options={staff.map((s) => ({ value: s.id, label: s.fullName }))} />
          </Form.Item>

          <ChecklistFieldsEditor />

          <Space>
            <Button type="primary" htmlType="submit" loading={updateTask.isPending}>
              ذخیره تغییرات
            </Button>
            <Button onClick={() => setIsEditing(false)}>انصراف</Button>
          </Space>
        </Form>
      ) : (
        <div style={{ marginBottom: 20 }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 8 }}>
            <Title level={5} style={{ margin: 0 }}>
              {task.title}
            </Title>
            <Tag color={statusLabel[task.status].color}>{statusLabel[task.status].text}</Tag>
          </div>
          {task.description && <Text>{task.description}</Text>}
          <div style={{ marginTop: 12 }}>
            <Text type="secondary">سررسید: {task.dueAtShamsi}</Text>
            <br />
            <Text type="secondary">مسئول: {assigneeName}</Text>
            <br />
            <Text type="secondary">مشتری: {customerName ?? 'بدون مشتری (داخلی)'}</Text>
          </div>
          {!isLocked && (
            <Space style={{ marginTop: 12 }}>
              {isCreator && <Button onClick={() => setIsEditing(true)}>ویرایش</Button>}
              {canRefer && <Button onClick={() => setIsReferring((v) => !v)}>ارجاع به شخص دیگر</Button>}
            </Space>
          )}

          {isReferring && (
            <Form form={referForm} layout="vertical" onFinish={handleRefer} style={{ marginTop: 12 }}>
              <Form.Item
                name="referredToStaffId"
                label="ارجاع به"
                rules={[{ required: true, message: 'انتخاب شخص الزامی است' }]}
              >
                <Select options={staff.map((s) => ({ value: s.id, label: s.fullName }))} />
              </Form.Item>
              <Form.Item name="note" label="متن ارجاع" rules={[{ required: true, message: 'متن ارجاع الزامی است' }]}>
                <Input.TextArea rows={2} />
              </Form.Item>
              <Space>
                <Button type="primary" htmlType="submit" loading={referTask.isPending}>
                  ثبت ارجاع
                </Button>
                <Button onClick={() => setIsReferring(false)}>انصراف</Button>
              </Space>
            </Form>
          )}

          {task.referrals.length > 0 && (
            <div style={{ marginTop: 16 }}>
              <Title level={5}>تاریخچه ارجاعات</Title>
              <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
                {task.referrals.map((r) => (
                  <div key={r.id}>
                    <Text type="secondary">
                      از {staffNameById.get(r.referredByStaffId) ?? '—'} به {staffNameById.get(r.referredToStaffId) ?? '—'} —{' '}
                      {r.referredAtShamsi}
                    </Text>
                    <br />
                    <Text>{r.note}</Text>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      )}

      {!isEditing && (
        <>
          <Title level={5}>چک‌لیست</Title>
          {task.checklistItems.length === 0 ? (
            <Empty description="این کار چک‌لیستی ندارد" />
          ) : (
            <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
              {task.checklistItems.map((item) => (
                <ChecklistField key={item.id} taskId={task.id} item={item} disabled={isLocked} />
              ))}
            </div>
          )}
        </>
      )}
    </Drawer>
  )
}
