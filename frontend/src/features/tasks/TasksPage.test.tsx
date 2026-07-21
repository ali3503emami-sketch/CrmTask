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
    const dialog = await screen.findByRole('dialog')

    await user.type(within(dialog).getByLabelText('عنوان'), 'بررسی سرور')
    await user.type(within(dialog).getByLabelText('سررسید'), '2026-08-01T12:00')

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

    await user.click(screen.getByRole('button', { name: 'کار جدید' }))
    const dialog = await screen.findByRole('dialog')
    await user.type(within(dialog).getByLabelText('عنوان'), 'کار برای اتمام')
    await user.type(within(dialog).getByLabelText('سررسید'), '2026-08-01T12:00')
    await user.click(within(dialog).getByLabelText('مسئول'))
    await user.click(await screen.findByText('سارا محمدی'))
    await user.click(within(dialog).getByRole('button', { name: 'ثبت کار' }))
    await screen.findAllByText('کار برای اتمام')

    await user.click(screen.getByRole('button', { name: 'اتمام کار' }))

    await waitFor(async () => {
      expect(await screen.findAllByText('انجام‌شده')).not.toHaveLength(0)
    })
  })
})
