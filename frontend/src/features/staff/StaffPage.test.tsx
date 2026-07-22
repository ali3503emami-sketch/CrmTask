import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen, waitFor, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { StaffPage } from './StaffPage'
import { sampleStaff } from '../../test/mocks/handlers'

function renderPage() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <StaffPage />
    </QueryClientProvider>,
  )
}

describe('StaffPage', () => {
  it('shows staff members loaded from the API', async () => {
    renderPage()

    expect(await screen.findAllByText(sampleStaff[0].fullName)).not.toHaveLength(0)
  })

  it('creates a new staff member through the form and shows it in the list', async () => {
    renderPage()
    await screen.findAllByText(sampleStaff[0].fullName)
    const user = userEvent.setup()

    await user.click(screen.getByRole('button', { name: 'کارمند جدید' }))
    const dialog = await screen.findByRole('dialog')

    await user.type(within(dialog).getByLabelText('نام و نام خانوادگی'), 'علی رضایی')
    await user.type(within(dialog).getByLabelText('شماره تماس'), '09123334455')
    await user.click(within(dialog).getByRole('button', { name: 'ثبت' }))

    await waitFor(async () => {
      expect(await screen.findAllByText('علی رضایی')).not.toHaveLength(0)
    })
  })
})
