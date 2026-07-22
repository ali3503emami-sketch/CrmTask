import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen, waitFor, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { TasksPage } from './TasksPage'

function renderPage() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <TasksPage />
    </QueryClientProvider>,
  )
}

// The page always renders an inline PersianCalendar (for day filtering) *and*
// opens a PersianDateTimeField popup calendar inside the create-task Modal —
// both use the same rmdp day markup, so day-number text alone is ambiguous.
// Only the popup is wrapped in react-multi-date-picker's floating positioner
// (an inline `position: absolute` div), so scope queries to that container.
function withinDueDatePopup() {
  const wrappers = Array.from(document.querySelectorAll('.rmdp-wrapper'))
  const popupWrapper = wrappers.find((w) => w.closest('div[style*="position: absolute"]'))
  return within(popupWrapper as HTMLElement)
}

async function createTask(user: ReturnType<typeof userEvent.setup>, title: string) {
  await user.click(screen.getByRole('button', { name: 'کار جدید' }))
  const dialog = await screen.findByRole('dialog', { name: 'کار جدید' })

  await user.type(within(dialog).getByLabelText('عنوان'), title)
  await user.click(within(dialog).getByPlaceholderText('انتخاب سررسید'))
  await user.click(withinDueDatePopup().getAllByText('۱۰')[0])

  const assigneeSelect = within(dialog).getByLabelText('مسئول')
  await user.click(assigneeSelect)
  await user.click(await screen.findByText('سارا محمدی'))

  await user.click(within(dialog).getByRole('button', { name: 'ثبت کار' }))
}

describe('TasksPage', () => {
  it('shows the open-tasks count starting at zero', async () => {
    renderPage()

    expect(await screen.findByText('تعداد کارهای باز')).toBeInTheDocument()
    expect(screen.getByText('0')).toBeInTheDocument()
  })

  it('creates a task with a checklist field and shows it in the list', async () => {
    renderPage()
    await screen.findByText('تعداد کارهای باز')
    const user = userEvent.setup()

    await user.click(screen.getByRole('button', { name: 'کار جدید' }))
    const dialog = await screen.findByRole('dialog', { name: 'کار جدید' })

    await user.type(within(dialog).getByLabelText('عنوان'), 'بررسی سرور')
    await user.click(within(dialog).getByPlaceholderText('انتخاب سررسید'))
    await user.click(withinDueDatePopup().getAllByText('۱۰')[0])

    const assigneeSelect = within(dialog).getByLabelText('مسئول')
    await user.click(assigneeSelect)
    await user.click(await screen.findByText('سارا محمدی'))

    await user.click(within(dialog).getByRole('button', { name: 'افزودن آیتم چک‌لیست' }))
    await user.type(within(dialog).getByLabelText('برچسب آیتم 1'), 'چک شد؟')

    await user.click(within(dialog).getByRole('button', { name: 'ثبت کار' }))

    await waitFor(async () => {
      expect(await screen.findAllByText('بررسی سرور')).not.toHaveLength(0)
    })
  })

  it('marks a task as done via the row action', async () => {
    renderPage()
    await screen.findByText('تعداد کارهای باز')
    const user = userEvent.setup()

    await createTask(user, 'کار برای اتمام')
    await screen.findAllByText('کار برای اتمام')

    await user.click(screen.getByRole('button', { name: 'اتمام کار' }))

    await waitFor(async () => {
      expect(await screen.findAllByText('انجام‌شده')).not.toHaveLength(0)
    })
  })

  it('opens the task detail panel showing the full task via the مشاهده button', async () => {
    renderPage()
    await screen.findByText('تعداد کارهای باز')
    const user = userEvent.setup()

    await createTask(user, 'کار قابل مشاهده')
    await screen.findAllByText('کار قابل مشاهده')

    await user.click(screen.getByRole('button', { name: 'مشاهده' }))

    expect(await screen.findByText('مشاهده کار')).toBeInTheDocument()
    expect(screen.getAllByText('کار قابل مشاهده').length).toBeGreaterThan(0)
  })

  it('filters the task list to a calendar day, hiding tasks due on other days', async () => {
    renderPage()
    await screen.findByText('تعداد کارهای باز')
    const user = userEvent.setup()

    await createTask(user, 'کار امروز نیست')
    await screen.findAllByText('کار امروز نیست')

    const dayFive = screen.getAllByText('۵').find((el) => el.closest('div.rmdp-day') && !el.closest('.rmdp-day-hidden'))
    await user.click(dayFive!)

    expect(screen.queryByText('کار امروز نیست')).not.toBeInTheDocument()
  })
})
