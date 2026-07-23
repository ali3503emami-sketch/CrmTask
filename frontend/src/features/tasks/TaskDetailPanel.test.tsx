import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { TaskDetailPanel } from './TaskDetailPanel'
import type { TaskItem } from './types'
import { sampleTasks } from '../../test/mocks/handlers'

const openTask: TaskItem = {
  id: '33333333-3333-3333-3333-333333333333',
  title: 'بررسی سرور',
  description: 'بررسی وضعیت سرور اصلی',
  dueAt: '2026-08-01T12:00:00.000Z',
  dueAtShamsi: '1405/05/10',
  customerId: null,
  assignedToStaffId: '22222222-2222-2222-2222-222222222222',
  createdByStaffId: '22222222-2222-2222-2222-222222222222',
  status: 'Open',
  checklistItems: [{ id: 'c1', label: 'چک شد؟', fieldType: 'Checkbox', options: [], value: null }],
}

function renderPanel(task: TaskItem) {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <TaskDetailPanel task={task} assigneeName="سارا محمدی" customerName={undefined} customers={[]} open onClose={() => {}} />
    </QueryClientProvider>,
  )
}

describe('TaskDetailPanel', () => {
  it('shows the full task: title, description, due date, assignee, and checklist', () => {
    renderPanel(openTask)

    expect(screen.getByText('بررسی سرور')).toBeInTheDocument()
    expect(screen.getByText('بررسی وضعیت سرور اصلی')).toBeInTheDocument()
    expect(screen.getByText(/1405\/05\/10/)).toBeInTheDocument()
    expect(screen.getByText(/سارا محمدی/)).toBeInTheDocument()
    expect(screen.getByText('چک شد؟')).toBeInTheDocument()
  })

  it('shows an edit button for an open task and lets it be edited', async () => {
    sampleTasks.push(openTask)
    renderPanel(openTask)
    const user = userEvent.setup()

    await user.click(screen.getByRole('button', { name: 'ویرایش' }))
    const titleInput = screen.getByLabelText('عنوان')
    await user.clear(titleInput)
    await user.type(titleInput, 'بررسی سرور پشتیبان')
    await user.click(screen.getByRole('button', { name: 'ذخیره تغییرات' }))

    await waitFor(() => {
      expect(screen.queryByRole('button', { name: 'انصراف' })).not.toBeInTheDocument()
    })
  })

  it('hides the edit button and disables checklist inputs once the task is Done', () => {
    renderPanel({ ...openTask, status: 'Done' })

    expect(screen.queryByRole('button', { name: 'ویرایش' })).not.toBeInTheDocument()
    expect(screen.getByRole('checkbox')).toBeDisabled()
  })
})
