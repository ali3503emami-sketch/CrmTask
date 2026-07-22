import { useState } from 'react'
import { Button, Checkbox, Drawer, Empty, Form, Input, Radio, Select, Space, Tag, Typography } from 'antd'
import { PersianDateTimeField } from '../../shared/date/PersianDateTimeField'
import { useSetChecklistItemValue } from './useSetChecklistItemValue'
import { useUpdateTask } from './useUpdateTask'
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
    case 'ListBox':
      return (
        <div>
          <Text>{item.label}</Text>
          <Radio.Group
            disabled={disabled}
            style={{ display: 'flex', flexDirection: 'column', marginTop: 4 }}
            value={item.value ?? undefined}
            onChange={(e) => commit(e.target.value)}
          >
            {item.options.map((o) => (
              <Radio key={o} value={o}>
                {o}
              </Radio>
            ))}
          </Radio.Group>
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
    default:
      return null
  }
}

interface EditTaskFormValues {
  title: string
  description?: string
  dueAt: string
  customerId?: string
}

interface TaskDetailPanelProps {
  task: TaskItem
  assigneeName: string
  customerName: string | undefined
  customers: { id: string; name: string }[]
  open: boolean
  onClose: () => void
}

export function TaskDetailPanel({ task, assigneeName, customerName, customers, open, onClose }: TaskDetailPanelProps) {
  const [isEditing, setIsEditing] = useState(false)
  const updateTask = useUpdateTask()
  const [form] = Form.useForm<EditTaskFormValues>()

  const handleSubmit = async (values: EditTaskFormValues) => {
    await updateTask.mutateAsync({
      taskId: task.id,
      input: {
        title: values.title,
        description: values.description ?? '',
        dueAt: values.dueAt,
        customerId: values.customerId ?? null,
      },
    })
    setIsEditing(false)
  }

  const isLocked = task.status === 'Done'

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
            <Button style={{ marginTop: 12 }} onClick={() => setIsEditing(true)}>
              ویرایش
            </Button>
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
