import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import App from './App'

describe('App', () => {
  it('shows the CRM page title', async () => {
    const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
    render(
      <QueryClientProvider client={queryClient}>
        <App />
      </QueryClientProvider>,
    )

    expect(
      screen.getByRole('heading', { name: 'سیستم مدیریت ارتباط با مشتری' }),
    ).toBeInTheDocument()
  })

  it('navigates to the staff tab', async () => {
    const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
    render(
      <QueryClientProvider client={queryClient}>
        <App />
      </QueryClientProvider>,
    )
    const user = userEvent.setup()

    await user.click(screen.getByRole('tab', { name: 'پرسنل' }))

    expect(await screen.findByText('تعداد پرسنل')).toBeInTheDocument()
  })
})
