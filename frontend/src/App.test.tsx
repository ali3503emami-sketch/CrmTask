import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import App from './App'

function renderApp() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <App />
    </QueryClientProvider>,
  )
}

describe('App', () => {
  it('shows the CRM page title', () => {
    renderApp()

    expect(screen.getByRole('heading', { name: 'سیستم مدیریت ارتباط با مشتری' })).toBeInTheDocument()
  })

  it('opens on the Dashboard by default', async () => {
    renderApp()

    expect(await screen.findByRole('tab', { name: 'کارهای جاری' })).toBeInTheDocument()
    expect(screen.getByRole('tab', { name: 'قراردادهای خاتمه‌یافته' })).toBeInTheDocument()
  })

  it('navigates to معرفی پرسنل under اطلاعات پایه', async () => {
    renderApp()
    const user = userEvent.setup()

    await user.click(screen.getByRole('menuitem', { name: 'معرفی پرسنل' }))

    expect(await screen.findByText('تعداد پرسنل')).toBeInTheDocument()
  })

  it('navigates to معرفی مشتریان under امور مشتریان', async () => {
    renderApp()
    const user = userEvent.setup()

    await user.click(screen.getByRole('menuitem', { name: 'معرفی مشتریان' }))

    expect(await screen.findByText('تعداد مشتریان')).toBeInTheDocument()
  })

  it('navigates to انجام کار', async () => {
    renderApp()
    const user = userEvent.setup()

    await user.click(screen.getByRole('menuitem', { name: 'انجام کار' }))

    expect(await screen.findByText('تعداد کارهای باز')).toBeInTheDocument()
  })
})
