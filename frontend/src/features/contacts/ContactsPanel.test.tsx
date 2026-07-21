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
})
