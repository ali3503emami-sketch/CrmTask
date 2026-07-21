import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen, waitFor, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { CustomersPage } from './CustomersPage'
import { sampleCustomers } from '../../test/mocks/handlers'

function renderPage() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <CustomersPage />
    </QueryClientProvider>,
  )
}

describe('CustomersPage', () => {
  it('shows customers loaded from the API', async () => {
    renderPage()

    expect(await screen.findAllByText(sampleCustomers[0].name)).not.toHaveLength(0)
  })

  it('creates a new customer through the form and shows it in the list', async () => {
    renderPage()
    await screen.findAllByText(sampleCustomers[0].name)
    const user = userEvent.setup()

    await user.click(screen.getByRole('button', { name: 'مشتری جدید' }));
    const dialog = await screen.findByRole('dialog');

    await user.type(within(dialog).getByLabelText('نام مشتری'), 'مهندس رضا کیانی')
    await user.type(within(dialog).getByLabelText('تلفن'), '09121234567')
    await user.click(within(dialog).getByRole('button', { name: 'ثبت' }))

    await waitFor(async () => {
      expect(await screen.findAllByText('مهندس رضا کیانی')).not.toHaveLength(0)
    })
  })

  it('opens the contacts panel for a customer via the row action', async () => {
    renderPage()
    await screen.findAllByText(sampleCustomers[0].name)
    const user = userEvent.setup()

    await user.click(screen.getByRole('button', { name: 'تماس‌ها' }))

    expect(await screen.findByText(`تماس‌های ${sampleCustomers[0].name}`)).toBeInTheDocument()
  })

  it('opens the contracts panel for a customer via the row action', async () => {
    renderPage()
    await screen.findAllByText(sampleCustomers[0].name)
    const user = userEvent.setup()

    await user.click(screen.getByRole('button', { name: 'قراردادها' }))

    expect(await screen.findByText(`قراردادهای ${sampleCustomers[0].name}`)).toBeInTheDocument()
  })
})
