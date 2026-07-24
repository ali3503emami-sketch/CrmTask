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

    // Submenus are collapsed by default (only the top-level "کاربر" group is
    // open) — expand "اطلاعات پایه" before its children become clickable.
    await user.click(screen.getByRole('menuitem', { name: 'اطلاعات پایه' }))
    await user.click(await screen.findByRole('menuitem', { name: 'معرفی پرسنل' }))

    expect(await screen.findByText('تعداد پرسنل')).toBeInTheDocument()
  })

  it('navigates to معرفی مشتریان under امور مشتریان', async () => {
    renderApp()
    const user = userEvent.setup()

    await user.click(screen.getByRole('menuitem', { name: 'امور مشتریان' }))
    await user.click(await screen.findByRole('menuitem', { name: 'معرفی مشتریان' }))

    expect(await screen.findByText('تعداد مشتریان')).toBeInTheDocument()
  })

  it('navigates to انجام کار', async () => {
    renderApp()
    const user = userEvent.setup()

    await user.click(screen.getByRole('menuitem', { name: 'انجام کار' }))

    expect(await screen.findByText('تعداد کارهای باز')).toBeInTheDocument()
  })

  it('opens the mobile menu drawer via the hamburger button', async () => {
    renderApp()
    const user = userEvent.setup()

    await user.click(screen.getByRole('button', { name: 'باز کردن منو' }))

    expect(await screen.findByText('منو')).toBeInTheDocument()
    expect(screen.getAllByRole('menuitem', { name: 'انجام کار' }).length).toBeGreaterThan(0)
  })

  it('closes the mobile menu drawer and navigates after picking an item from it', async () => {
    renderApp()
    const user = userEvent.setup()

    await user.click(screen.getByRole('button', { name: 'باز کردن منو' }))
    await screen.findByText('منو')
    const basicInfoGroups = screen.getAllByRole('menuitem', { name: 'اطلاعات پایه' })
    await user.click(basicInfoGroups[basicInfoGroups.length - 1])
    const drawerItems = await screen.findAllByRole('menuitem', { name: 'معرفی پرسنل' })
    await user.click(drawerItems[drawerItems.length - 1])

    expect(await screen.findByText('تعداد پرسنل')).toBeInTheDocument()
    // The Drawer's exit animation never settles under jsdom (no real CSS
    // transitions/rAF timing), so its content stays mounted — assert on the
    // `ant-drawer-open` class instead, which antd's underlying rc-drawer
    // toggles synchronously off the `open` prop, independent of the motion.
    expect(document.querySelector('.ant-drawer')).not.toHaveClass('ant-drawer-open')
  })
})
