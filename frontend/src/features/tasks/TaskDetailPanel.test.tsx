import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it } from 'vitest'
import { TaskDetailPanel } from './TaskDetailPanel'
import { CurrentUserProvider } from '../../shared/currentUser/CurrentUserContext'
import type { TaskItem } from './types'
import { sampleTasks } from '../../test/mocks/handlers'

const creatorId = '22222222-2222-2222-2222-222222222222'
const otherStaffId = '44444444-4444-4444-4444-444444444444'

const staff = [
  { id: creatorId, fullName: 'سارا محمدی' },
  { id: otherStaffId, fullName: 'علی رضایی' },
]

const openTask: TaskItem = {
  id: '33333333-3333-3333-3333-333333333333',
  title: 'بررسی سرور',
  description: 'بررسی وضعیت سرور اصلی',
  dueAt: '2026-08-01T12:00:00.000Z',
  dueAtShamsi: '1405/05/10',
  customerId: null,
  assignedToStaffId: creatorId,
  createdByStaffId: creatorId,
  status: 'Open',
  checklistItems: [{ id: 'c1', label: 'چک شد؟', fieldType: 'Checkbox', options: [], value: null }],
  referrals: [],
}

function renderPanel(task: TaskItem) {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <CurrentUserProvider>
        <TaskDetailPanel
          task={task}
          assigneeName="سارا محمدی"
          customerName={undefined}
          customers={[]}
          staff={staff}
          open
          onClose={() => {}}
        />
      </CurrentUserProvider>
    </QueryClientProvider>,
  )
}

describe('TaskDetailPanel', () => {
  beforeEach(() => {
    sessionStorage.clear()
  })

  it('shows the full task: title, description, due date, assignee, and checklist', () => {
    renderPanel(openTask)

    expect(screen.getByText('بررسی سرور')).toBeInTheDocument()
    expect(screen.getByText('بررسی وضعیت سرور اصلی')).toBeInTheDocument()
    expect(screen.getByText(/1405\/05\/10/)).toBeInTheDocument()
    expect(screen.getByText(/سارا محمدی/)).toBeInTheDocument()
    expect(screen.getByText('چک شد؟')).toBeInTheDocument()
  })

  it('shows an edit button for the creator and lets them edit all fields plus the checklist', async () => {
    sessionStorage.setItem('crmtask.currentStaffId', creatorId)
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

  it('hides the edit button from someone other than the creator', () => {
    sessionStorage.setItem('crmtask.currentStaffId', otherStaffId)
    renderPanel(openTask)

    expect(screen.queryByRole('button', { name: 'ویرایش' })).not.toBeInTheDocument()
  })

  it('lets the assignee refer the task to someone else, with a note', async () => {
    sessionStorage.setItem('crmtask.currentStaffId', creatorId)
    sampleTasks.push({ ...openTask, id: '55555555-5555-5555-5555-555555555555' })
    renderPanel({ ...openTask, id: '55555555-5555-5555-5555-555555555555' })
    const user = userEvent.setup()

    await user.click(screen.getByRole('button', { name: 'ارجاع به شخص دیگر' }))
    const dialog = screen.getByRole('button', { name: 'ثبت ارجاع' }).closest('form')!
    await user.type(screen.getByLabelText('متن ارجاع'), 'لطفاً پیگیری کنید')
    const staffSelect = screen.getByLabelText('ارجاع به')
    await user.click(staffSelect)
    const option = (await screen.findAllByText('علی رضایی')).pop()!
    await user.click(option)
    await user.click(screen.getByRole('button', { name: 'ثبت ارجاع' }))

    await waitFor(() => {
      expect(dialog).not.toBeInTheDocument()
    })
    expect(await screen.findByText('لطفاً پیگیری کنید')).toBeInTheDocument()
  })

  it('hides the edit button and disables checklist inputs once the task is Done', () => {
    renderPanel({ ...openTask, status: 'Done' })

    expect(screen.queryByRole('button', { name: 'ویرایش' })).not.toBeInTheDocument()
    expect(screen.getByRole('checkbox')).toBeDisabled()
  })
})
