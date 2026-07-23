import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { ContactsPanel } from './ContactsPanel'

const customerId = '11111111-1111-1111-1111-111111111111'

function renderPanel() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <ContactsPanel customerId={customerId} customerName="شرکت فناوران البرز" open onClose={() => {}} />
    </QueryClientProvider>,
  )
}

describe('ContactsPanel', () => {
  it('shows an empty state when the customer has no contacts yet', async () => {
    renderPanel()

    expect(await screen.findByText('هنوز تماسی ثبت نشده است')).toBeInTheDocument()
  })

  it('logs a new contact and shows it in the list', async () => {
    renderPanel()
    await screen.findByText('هنوز تماسی ثبت نشده است')
    const user = userEvent.setup()

    await user.type(screen.getByLabelText('خلاصه تماس'), 'پیگیری وضعیت قرارداد')
    await user.click(screen.getByRole('button', { name: 'ثبت تماس' }))

    await waitFor(async () => {
      expect(await screen.findAllByText('پیگیری وضعیت قرارداد')).not.toHaveLength(0)
    })
  })

  it('logs a contact with a Jalali next-follow-up date and shows it back as a Shamsi string', async () => {
    renderPanel()
    await screen.findByText('هنوز تماسی ثبت نشده است')
    const user = userEvent.setup()

    await user.type(screen.getByLabelText('خلاصه تماس'), 'نیاز به پیگیری دارد')
    await user.click(screen.getByPlaceholderText('انتخاب تاریخ یادآوری'))
    const nextMonthButton = document.querySelector('.rmdp-arrow-container.rmdp-right') as HTMLElement
    await user.click(nextMonthButton)
    await user.click(screen.getAllByText('۱۰')[0])
    await user.click(screen.getByRole('button', { name: 'ثبت تماس' }))

    await waitFor(async () => {
      // Don't hardcode a specific month/year here — "next month from today" is
      // relative to whatever day the suite actually runs on.
      expect(await screen.findByText(/یادآوری پیگیری: \d{4}\/\d{2}\/\d{2}/)).toBeInTheDocument()
    })
  })
})
