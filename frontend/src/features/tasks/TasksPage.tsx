import { useState } from 'react'
import { Button, Card, Form, Input, Modal, Select, Space, Statistic, Table, Tag } from 'antd'
import type { ColumnsType } from 'antd/es/table'
import dayjs from 'dayjs'
import { useTasks } from './useTasks'
import { useCreateTask } from './useCreateTask'
import { useMarkTaskDone } from './useMarkTaskDone'
import { useStaff } from '../staff/useStaff'
import { useCustomers } from '../customers/useCustomers'
import { ChecklistPanel } from './ChecklistPanel'
import type { ChecklistFieldType, TaskItem, TaskItemStatus } from './types'

const statusLabel: Record<TaskItemStatus, { text: string; color: string }> = {
  Open: { text: 'باز', color: 'processing' },
  Done: { text: 'انجام‌شده', color: 'success' },
}

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
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [form] = Form.useForm<CreateTaskFormValues>()
  const [selectedTask, setSelectedTask] = useState<TaskItem | null>(null)

  const staffNameById = new Map((staff ?? []).map((s) => [s.id, s.fullName]))

  const handleSubmit = async (values: CreateTaskFormValues) => {
    await createTask.mutateAsync({
      title: values.title,
      description: values.description ?? '',
      dueAt: new Date(values.dueAt).toISOString(),
      customerId: values.customerId ?? null,
      assignedToStaffId: values.assignedToStaffId,
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

  const columns: ColumnsType<TaskItem> = [
    { title: 'عنوان', dataIndex: 'title', key: 'title' },
    {
      title: 'سررسید',
      dataIndex: 'dueAt',
      key: 'dueAt',
      width: 160,
      render: (dueAt: string) => dayjs(dueAt).format('YYYY/MM/DD HH:mm'),
    },
    {
      title: 'مسئول',
      key: 'assignee',
      width: 140,
      render: (_, task) => staffNameById.get(task.assignedToStaffId) ?? '—',
    },
    {
      title: 'وضعیت',
      dataIndex: 'status',
      key: 'status',
      width: 110,
      render: (status: TaskItemStatus) => <Tag color={statusLabel[status].color}>{statusLabel[status].text}</Tag>,
    },
    {
      title: 'عملیات',
      key: 'actions',
      width: 200,
      render: (_, task) => (
        <Space>
          <Button size="small" onClick={() => setSelectedTask(task)}>
            چک‌لیست
          </Button>
          {task.status === 'Open' && (
            <Button size="small" onClick={() => markDone.mutate(task.id)}>
              اتمام کار
            </Button>
          )}
        </Space>
      ),
    },
  ]

  return (
    <div>
      <Card
        size="small"
        title="کارهای جاری"
        extra={
          <Button type="primary" onClick={() => setIsModalOpen(true)}>
            کار جدید
          </Button>
        }
        style={{ marginBottom: 16 }}
      >
        <Statistic title="تعداد کارهای باز" value={openTasksCount} />
      </Card>

      <Card size="small">
        <Table size="small" rowKey="id" loading={isLoading} columns={columns} dataSource={tasks} pagination={false} />
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
            <Input placeholder="2026-08-01T12:00" />
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

      {selectedTask && <ChecklistPanel task={selectedTask} open onClose={() => setSelectedTask(null)} />}
    </div>
  )
}
