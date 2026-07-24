import { useState } from 'react'
import { Button, Card, Empty, Form, Input, message, Modal, Select, Statistic } from 'antd'
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
import { ChecklistFieldsEditor } from './ChecklistFieldsEditor'
import { choiceFieldTypes, parseDropdownOptions, type ChecklistFieldFormValue } from './checklistFieldTypes'
import type { TaskItem } from './types'

interface CreateTaskFormValues {
  title: string
  description?: string
  dueAt: string
  customerId?: string
  assignedToStaffId: string
  checklistFields?: ChecklistFieldFormValue[]
}

function isMyTask(task: TaskItem, currentStaffId: string): boolean {
  return (
    task.assignedToStaffId === currentStaffId ||
    task.createdByStaffId === currentStaffId ||
    task.referrals.some((r) => r.referredToStaffId === currentStaffId)
  )
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

  const myTasks = currentStaffId ? (tasks ?? []).filter((t) => isMyTask(t, currentStaffId)) : []
  const selectedTask = myTasks.find((t) => t.id === selectedTaskId) ?? null

  const staffNameById = new Map((staff ?? []).map((s) => [s.id, s.fullName]))
  const customerNameById = new Map((customers ?? []).map((c) => [c.id, c.name]))

  const handleDayChange = (isoDate: string) => {
    setSelectedDate(isoDate)
    setShowAllForDay(false)
  }

  const displayedTasks = selectedDate
    ? myTasks.filter((t) => t.dueAt.slice(0, 10) === selectedDate && (showAllForDay || t.status === 'Open'))
    : myTasks

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
        options: choiceFieldTypes.includes(f.fieldType) ? parseDropdownOptions(f.options ?? '') : null,
      })),
    })
    form.resetFields()
    setIsModalOpen(false)
  }

  const openTasksCount = myTasks.filter((t) => t.status === 'Open').length

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
        {!currentStaffId ? (
          <Empty description="لطفاً ابتدا مشخص کنید شما کیستید." />
        ) : (
          <TaskListTable
            tasks={displayedTasks}
            isLoading={isLoading}
            staffNameById={staffNameById}
            onView={setSelectedTaskId}
            onMarkDone={(taskId) => markDone.mutate(taskId)}
          />
        )}
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

          <ChecklistFieldsEditor />

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
          staff={staff ?? []}
          open
          onClose={() => setSelectedTaskId(null)}
        />
      )}
    </div>
  )
}
