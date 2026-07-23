import { useMemo, useState } from 'react'
import { Button, Card, Form, Input, message, Modal, Select, Space, Statistic } from 'antd'
import { PersianCalendar } from '../../shared/date/PersianCalendar'
import { PersianDateTimeField } from '../../shared/date/PersianDateTimeField'
import { useCurrentUser } from '../../shared/currentUser/CurrentUserContext'
import { useTasks } from './useTasks'
import { useCreateTask } from './useCreateTask'
import { useMarkTaskDone } from './useMarkTaskDone'
import { useStaff } from '../staff/useStaff'
import { useCustomers } from '../customers/useCustomers'
import { TaskDetailPanel } from './TaskDetailPanel'
import { TaskListTable } from './TaskListTable'
import type { ChecklistFieldType } from './types'

const fieldTypeOptions: { value: ChecklistFieldType; label: string }[] = [
  { value: 'Checkbox', label: 'چک‌باکس' },
  { value: 'Dropdown', label: 'کشو' },
  { value: 'ListBox', label: 'لیست‌باکس' },
  { value: 'TextBox', label: 'متن' },
]

const choiceFieldTypes: ChecklistFieldType[] = ['Dropdown', 'ListBox']

interface ChecklistFieldFormValue {
  label: string
  fieldType: ChecklistFieldType
  options?: string
}

interface CreateTaskFormValues {
  title: string
  description?: string
  dueAt: string
  customerId?: string
  assignedToStaffId: string
  checklistFields?: ChecklistFieldFormValue[]
}

export function TasksPage() {
  const { data: tasks, isLoading } = useTasks()
  const { data: staff } = useStaff()
  const { data: customers } = useCustomers()
  const createTask = useCreateTask()
  const markDone = useMarkTaskDone()
  const { currentStaffId } = useCurrentUser()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [form] = Form.useForm<CreateTaskFormValues>()
  const [selectedTaskId, setSelectedTaskId] = useState<string | null>(null)
  const [selectedDate, setSelectedDate] = useState<string | null>(null)
  const [showAllForDay, setShowAllForDay] = useState(false)

  const selectedTask = (tasks ?? []).find((t) => t.id === selectedTaskId) ?? null

  const staffNameById = new Map((staff ?? []).map((s) => [s.id, s.fullName]))
  const customerNameById = new Map((customers ?? []).map((c) => [c.id, c.name]))

  const handleDayChange = (isoDate: string) => {
    setSelectedDate(isoDate)
    setShowAllForDay(false)
  }

  const displayedTasks = useMemo(() => {
    if (!selectedDate) return tasks ?? []
    return (tasks ?? []).filter((t) => t.dueAt.slice(0, 10) === selectedDate && (showAllForDay || t.status === 'Open'))
  }, [tasks, selectedDate, showAllForDay])

  const handleOpenCreateModal = () => {
    if (!currentStaffId) {
      message.warning('لطفاً ابتدا مشخص کنید شما کیستید.')
      return
    }
    form.resetFields()
    if (selectedDate) {
      form.setFieldValue('dueAt', new Date(`${selectedDate}T09:00:00`).toISOString())
    }
    setIsModalOpen(true)
  }

  const handleSubmit = async (values: CreateTaskFormValues) => {
    if (!currentStaffId) {
      message.warning('لطفاً ابتدا مشخص کنید شما کیستید.')
      return
    }
    await createTask.mutateAsync({
      title: values.title,
      description: values.description ?? '',
      dueAt: values.dueAt,
      customerId: values.customerId ?? null,
      assignedToStaffId: values.assignedToStaffId,
      createdByStaffId: currentStaffId,
      checklistFields: (values.checklistFields ?? []).map((f) => ({
        label: f.label,
        fieldType: f.fieldType,
        options: choiceFieldTypes.includes(f.fieldType)
          ? (f.options ?? '')
              .split(',')
              .map((o) => o.trim())
              .filter(Boolean)
          : null,
      })),
    })
    form.resetFields()
    setIsModalOpen(false)
  }

  const openTasksCount = (tasks ?? []).filter((t) => t.status === 'Open').length

  return (
    <div>
      <Card
        size="small"
        title="کارهای جاری"
        extra={
          <Button type="primary" onClick={handleOpenCreateModal}>
            کار جدید
          </Button>
        }
        style={{ marginBottom: 16 }}
      >
        <Statistic title="تعداد کارهای باز" value={openTasksCount} />
      </Card>

      <Card size="small" title="تقویم" style={{ marginBottom: 16 }}>
        <PersianCalendar value={selectedDate} onChange={handleDayChange} />
      </Card>

      <Card
        size="small"
        extra={
          selectedDate && (
            <Button size="small" onClick={() => setShowAllForDay((v) => !v)}>
              {showAllForDay ? 'فقط کارهای باز' : 'نمایش همه'}
            </Button>
          )
        }
      >
        <TaskListTable
          tasks={displayedTasks}
          isLoading={isLoading}
          staffNameById={staffNameById}
          onView={setSelectedTaskId}
          onMarkDone={(taskId) => markDone.mutate(taskId)}
        />
      </Card>

      <Modal
        title="کار جدید"
        open={isModalOpen}
        onCancel={() => setIsModalOpen(false)}
        footer={null}
        destroyOnHidden
        width={560}
      >
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
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
              options={(customers ?? []).map((c) => ({ value: c.id, label: c.name }))}
            />
          </Form.Item>
          <Form.Item
            name="assignedToStaffId"
            label="مسئول"
            rules={[{ required: true, message: 'انتخاب مسئول الزامی است' }]}
          >
            <Select options={(staff ?? []).map((s) => ({ value: s.id, label: s.fullName }))} />
          </Form.Item>

          <Form.List name="checklistFields">
            {(fields, { add, remove }) => (
              <>
                {fields.map((field, index) => (
                  <Space key={field.key} align="baseline" style={{ display: 'flex', marginBottom: 8 }} wrap>
                    <Form.Item
                      name={[field.name, 'label']}
                      label={`برچسب آیتم ${index + 1}`}
                      rules={[{ required: true, message: 'برچسب الزامی است' }]}
                    >
                      <Input />
                    </Form.Item>
                    <Form.Item name={[field.name, 'fieldType']} label="نوع" initialValue="TextBox">
                      <Select style={{ width: 120 }} options={fieldTypeOptions} />
                    </Form.Item>
                    <Form.Item
                      noStyle
                      shouldUpdate={(prev: CreateTaskFormValues, cur: CreateTaskFormValues) =>
                        prev.checklistFields?.[index]?.fieldType !== cur.checklistFields?.[index]?.fieldType
                      }
                    >
                      {({ getFieldValue }) => {
                        const fieldType: ChecklistFieldType = getFieldValue(['checklistFields', field.name, 'fieldType'])
                        return choiceFieldTypes.includes(fieldType) ? (
                          <Form.Item name={[field.name, 'options']} label="گزینه‌ها (با کاما جدا کنید)">
                            <Input />
                          </Form.Item>
                        ) : null
                      }}
                    </Form.Item>
                    <Button onClick={() => remove(field.name)}>حذف</Button>
                  </Space>
                ))}
                <Button onClick={() => add({ fieldType: 'TextBox' })} block style={{ marginBottom: 16 }}>
                  افزودن آیتم چک‌لیست
                </Button>
              </>
            )}
          </Form.List>

          <Form.Item>
            <Button type="primary" htmlType="submit" loading={createTask.isPending} block>
              ثبت کار
            </Button>
          </Form.Item>
        </Form>
      </Modal>

      {selectedTask && (
        <TaskDetailPanel
          task={selectedTask}
          assigneeName={staffNameById.get(selectedTask.assignedToStaffId) ?? '—'}
          customerName={selectedTask.customerId ? customerNameById.get(selectedTask.customerId) : undefined}
          customers={customers ?? []}
          open
          onClose={() => setSelectedTaskId(null)}
        />
      )}
    </div>
  )
}
