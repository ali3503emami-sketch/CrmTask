import { Button, Space, Table, Tag } from 'antd'
import type { ColumnsType } from 'antd/es/table'
import type { TaskItem, TaskItemStatus } from './types'

const statusLabel: Record<TaskItemStatus, { text: string; color: string }> = {
  Open: { text: 'باز', color: 'processing' },
  Done: { text: 'انجام‌شده', color: 'success' },
}

interface TaskListTableProps {
  tasks: TaskItem[]
  isLoading: boolean
  staffNameById: Map<string, string>
  onView: (taskId: string) => void
  onMarkDone: (taskId: string) => void
}

/**
 * The table + per-row "مشاهده"/"اتمام کار" actions — shared by TasksPage (full
 * CRUD page) and the Dashboard's read-only "my tasks" tab, so the row markup
 * and status/action logic exist in exactly one place.
 */
export function TaskListTable({ tasks, isLoading, staffNameById, onView, onMarkDone }: TaskListTableProps) {
  const columns: ColumnsType<TaskItem> = [
    { title: 'عنوان', dataIndex: 'title', key: 'title' },
    { title: 'سررسید', dataIndex: 'dueAtShamsi', key: 'dueAtShamsi', width: 160 },
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
          <Button size="small" onClick={() => onView(task.id)}>
            مشاهده
          </Button>
          {task.status === 'Open' && (
            <Button size="small" onClick={() => onMarkDone(task.id)}>
              اتمام کار
            </Button>
          )}
        </Space>
      ),
    },
  ]

  return <Table size="small" rowKey="id" loading={isLoading} columns={columns} dataSource={tasks} pagination={false} />
}
