import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { ContractsPanel } from './ContractsPanel'

const customerId = '11111111-1111-1111-1111-111111111111'

function renderPanel() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <ContractsPanel customerId={customerId} customerName="شرکت فناوران البرز" open onClose={() => {}} />
    </QueryClientProvider>,
  )
}

describe('ContractsPanel', () => {
  it('shows an empty state when the customer has no contracts yet', async () => {
    renderPanel()

    expect(await screen.findByText('هنوز قراردادی ثبت نشده است')).toBeInTheDocument()
  })

  it('creates a new contract and shows it with its computed status', async () => {
    renderPanel()
    await screen.findByText('هنوز قراردادی ثبت نشده است')
    const user = userEvent.setup()

    await user.type(screen.getByLabelText('عنوان قرارداد'), 'قرارداد پشتیبانی سالانه')
    await user.type(screen.getByLabelText('مبلغ (تومان)'), '50000000')
    await user.click(screen.getByPlaceholderText('انتخاب تاریخ شروع'))
    await user.click(screen.getByText('۵'))
    await user.click(screen.getByPlaceholderText('انتخاب تاریخ پایان'))
    await user.click(screen.getByText('۲۰'))
    await user.click(screen.getByRole('button', { name: 'ثبت قرارداد' }))

    await waitFor(async () => {
      expect(await screen.findAllByText('قرارداد پشتیبانی سالانه')).not.toHaveLength(0)
    })
  })
})
