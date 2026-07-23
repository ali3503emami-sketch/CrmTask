import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import { TaskListTable } from './TaskListTable'
import type { TaskItem } from './types'

const task: TaskItem = {
  id: '33333333-3333-3333-3333-333333333333',
  title: 'بررسی سرور',
  description: '',
  dueAt: '2026-08-01T12:00:00.000Z',
  dueAtShamsi: '1405/05/10',
  customerId: null,
  assignedToStaffId: '22222222-2222-2222-2222-222222222222',
  createdByStaffId: '22222222-2222-2222-2222-222222222222',
  status: 'Open',
  checklistItems: [],
}

describe('TaskListTable', () => {
  it('shows the task row with resolved assignee name and status', () => {
    render(
      <TaskListTable
        tasks={[task]}
        isLoading={false}
        staffNameById={new Map([[task.assignedToStaffId, 'سارا محمدی']])}
        onView={() => {}}
        onMarkDone={() => {}}
      />,
    )

    expect(screen.getByText('بررسی سرور')).toBeInTheDocument()
    expect(screen.getByText('سارا محمدی')).toBeInTheDocument()
    expect(screen.getByText('باز')).toBeInTheDocument()
  })

  it('calls onView and onMarkDone from the row actions', async () => {
    const onView = vi.fn()
    const onMarkDone = vi.fn()
    render(
      <TaskListTable
        tasks={[task]}
        isLoading={false}
        staffNameById={new Map()}
        onView={onView}
        onMarkDone={onMarkDone}
      />,
    )
    const user = userEvent.setup()

    await user.click(screen.getByRole('button', { name: 'مشاهده' }))
    await user.click(screen.getByRole('button', { name: 'اتمام کار' }))

    expect(onView).toHaveBeenCalledWith(task.id)
    expect(onMarkDone).toHaveBeenCalledWith(task.id)
  })

  it('hides the mark-done button for a Done task', () => {
    render(
      <TaskListTable
        tasks={[{ ...task, status: 'Done' }]}
        isLoading={false}
        staffNameById={new Map()}
        onView={() => {}}
        onMarkDone={() => {}}
      />,
    )

    expect(screen.queryByRole('button', { name: 'اتمام کار' })).not.toBeInTheDocument()
  })
})
