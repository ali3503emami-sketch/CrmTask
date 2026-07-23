import { useState } from 'react'
import { Card, Empty, Table, Tabs } from 'antd'
import type { ColumnsType } from 'antd/es/table'
import { useCurrentUser } from '../../shared/currentUser/CurrentUserContext'
import { useTasks } from '../tasks/useTasks'
import { useMarkTaskDone } from '../tasks/useMarkTaskDone'
import { useStaff } from '../staff/useStaff'
import { useCustomers } from '../customers/useCustomers'
import { useAllContracts } from '../contracts/useAllContracts'
import { TaskListTable } from '../tasks/TaskListTable'
import { TaskDetailPanel } from '../tasks/TaskDetailPanel'
import type { Contract } from '../contracts/types'

function EndedContractsTable({ contracts, customerNameById }: { contracts: Contract[]; customerNameById: Map<string, string> }) {
  const endedContracts = contracts.filter((c) => c.status === 'Ended')

  const columns: ColumnsType<Contract> = [
    { title: 'مشتری', key: 'customer', render: (_, contract) => customerNameById.get(contract.customerId) ?? '—' },
    { title: 'عنوان قرارداد', dataIndex: 'title', key: 'title' },
    { title: 'تاریخ پایان', dataIndex: 'endDateShamsi', key: 'endDateShamsi', width: 140 },
    {
      title: 'مبلغ',
      dataIndex: 'amount',
      key: 'amount',
      width: 140,
      render: (amount: number) => `${amount.toLocaleString('fa-IR')} تومان`,
    },
  ]

  return <Table size="small" rowKey="id" columns={columns} dataSource={endedContracts} pagination={false} />
}

export function DashboardPage() {
  const { currentStaffId } = useCurrentUser()
  const { data: tasks, isLoading: isLoadingTasks } = useTasks()
  const { data: staff } = useStaff()
  const { data: customers } = useCustomers()
  const { data: contracts } = useAllContracts()
  const markDone = useMarkTaskDone()
  const [selectedTaskId, setSelectedTaskId] = useState<string | null>(null)

  const staffNameById = new Map((staff ?? []).map((s) => [s.id, s.fullName]))
  const customerNameById = new Map((customers ?? []).map((c) => [c.id, c.name]))
  const selectedTask = (tasks ?? []).find((t) => t.id === selectedTaskId) ?? null

  const myTasks = currentStaffId
    ? (tasks ?? []).filter((t) => t.assignedToStaffId === currentStaffId || t.createdByStaffId === currentStaffId)
    : []

  return (
    <div>
      <Card size="small">
        <Tabs
          items={[
            {
              key: 'my-tasks',
              label: 'کارهای جاری',
              children: !currentStaffId ? (
                <Empty description="لطفاً ابتدا مشخص کنید شما کیستید." />
              ) : (
                <TaskListTable
                  tasks={myTasks}
                  isLoading={isLoadingTasks}
                  staffNameById={staffNameById}
                  onView={setSelectedTaskId}
                  onMarkDone={(taskId) => markDone.mutate(taskId)}
                />
              ),
            },
            {
              key: 'ended-contracts',
              label: 'قراردادهای خاتمه‌یافته',
              children: <EndedContractsTable contracts={contracts ?? []} customerNameById={customerNameById} />,
            },
          ]}
        />
      </Card>

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
