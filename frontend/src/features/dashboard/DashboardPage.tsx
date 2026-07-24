import { useState } from 'react'
import { Card, Empty, Table, Tabs, Tag } from 'antd'
import type { ColumnsType } from 'antd/es/table'
import { useCurrentUser } from '../../shared/currentUser/CurrentUserContext'
import { useTasks } from '../tasks/useTasks'
import { useMarkTaskDone } from '../tasks/useMarkTaskDone'
import { useStaff } from '../staff/useStaff'
import { useCustomers } from '../customers/useCustomers'
import { useAllContracts } from '../contracts/useAllContracts'
import { useAppSettings } from '../settings/useAppSettings'
import { TaskListTable } from '../tasks/TaskListTable'
import { TaskDetailPanel } from '../tasks/TaskDetailPanel'
import type { Contract, ContractStatus } from '../contracts/types'

const contractStatusLabel: Record<Extract<ContractStatus, 'Ended' | 'ExpiringSoon'>, { text: string; color: string }> = {
  Ended: { text: 'خاتمه‌یافته', color: 'default' },
  ExpiringSoon: { text: 'رو به اتمام', color: 'warning' },
}

function EndingContractsTable({ contracts, customerNameById }: { contracts: Contract[]; customerNameById: Map<string, string> }) {
  // Includes both already-ended and about-to-end contracts — how far ahead
  // "about to end" looks is the configurable contractEndingWindowDays setting
  // (see SettingsPage), already baked into each contract's status by the backend.
  const relevantContracts = contracts.filter((c) => c.status === 'Ended' || c.status === 'ExpiringSoon')

  const columns: ColumnsType<Contract> = [
    { title: 'مشتری', key: 'customer', render: (_, contract) => customerNameById.get(contract.customerId) ?? '—' },
    { title: 'عنوان قرارداد', dataIndex: 'title', key: 'title' },
    {
      title: 'وضعیت',
      dataIndex: 'status',
      key: 'status',
      width: 120,
      render: (status: 'Ended' | 'ExpiringSoon') => (
        <Tag color={contractStatusLabel[status].color}>{contractStatusLabel[status].text}</Tag>
      ),
    },
    { title: 'تاریخ پایان', dataIndex: 'endDateShamsi', key: 'endDateShamsi', width: 140 },
    {
      title: 'مبلغ',
      dataIndex: 'amount',
      key: 'amount',
      width: 140,
      render: (amount: number) => `${amount.toLocaleString('fa-IR')} تومان`,
    },
  ]

  return <Table size="small" rowKey="id" columns={columns} dataSource={relevantContracts} pagination={false} />
}

export function DashboardPage() {
  const { currentStaffId } = useCurrentUser()
  const { data: tasks, isLoading: isLoadingTasks } = useTasks()
  const { data: staff } = useStaff()
  const { data: customers } = useCustomers()
  const { data: contracts } = useAllContracts()
  const { data: appSettings } = useAppSettings()
  const markDone = useMarkTaskDone()
  const [selectedTaskId, setSelectedTaskId] = useState<string | null>(null)

  const staffNameById = new Map((staff ?? []).map((s) => [s.id, s.fullName]))
  const customerNameById = new Map((customers ?? []).map((c) => [c.id, c.name]))
  const selectedTask = (tasks ?? []).find((t) => t.id === selectedTaskId) ?? null

  // Tasks due within taskUpcomingWindowDays (or already overdue) — a
  // configurable look-ahead window (see SettingsPage), not every task
  // assigned/created regardless of how far off its due date is.
  const upcomingThreshold = new Date()
  upcomingThreshold.setDate(upcomingThreshold.getDate() + (appSettings?.taskUpcomingWindowDays ?? 0))

  const myTasks = currentStaffId
    ? (tasks ?? []).filter(
        (t) =>
          (t.assignedToStaffId === currentStaffId ||
            t.createdByStaffId === currentStaffId ||
            t.referrals.some((r) => r.referredToStaffId === currentStaffId)) &&
          new Date(t.dueAt) <= upcomingThreshold,
      )
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
              children: <EndingContractsTable contracts={contracts ?? []} customerNameById={customerNameById} />,
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
          staff={staff ?? []}
          open
          onClose={() => setSelectedTaskId(null)}
        />
      )}
    </div>
  )
}
