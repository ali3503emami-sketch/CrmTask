import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it } from 'vitest'
import { DashboardPage } from './DashboardPage'
import { CurrentUserProvider } from '../../shared/currentUser/CurrentUserContext'
import { sampleContracts, sampleCustomers, sampleStaff, sampleTasks, sampleSettings } from '../../test/mocks/handlers'

const staffId = '22222222-2222-2222-2222-222222222222'
const otherStaffId = '44444444-4444-4444-4444-444444444444'

function renderDashboard() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <CurrentUserProvider>
        <DashboardPage />
      </CurrentUserProvider>
    </QueryClientProvider>,
  )
}

describe('DashboardPage', () => {
  beforeEach(() => {
    sessionStorage.clear()
  })

  it('prompts to pick a current user when none is selected', async () => {
    renderDashboard()

    expect(await screen.findByText('لطفاً ابتدا مشخص کنید شما کیستید.')).toBeInTheDocument()
  })

  it('shows only tasks assigned to or created by the current user, on the my-tasks tab', async () => {
    // Due "tomorrow" — inside the default 3-day upcoming window (see
    // sampleSettings in handlers.ts) regardless of what day this suite runs on.
    const dueAt = new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString()
    sampleStaff.push({ id: otherStaffId, fullName: 'علی رضایی', phoneNumber: '09120000000', position: null, isActive: true })
    sampleTasks.push(
      { id: 't1', title: 'کار ارجاع شده به من', description: '', dueAt, dueAtShamsi: '1405/05/10', customerId: null, assignedToStaffId: staffId, createdByStaffId: otherStaffId, status: 'Open', checklistItems: [], referrals: [] },
      { id: 't2', title: 'کاری که خودم ثبت کردم', description: '', dueAt, dueAtShamsi: '1405/05/10', customerId: null, assignedToStaffId: otherStaffId, createdByStaffId: staffId, status: 'Open', checklistItems: [], referrals: [] },
      { id: 't3', title: 'کار بی‌ربط به من', description: '', dueAt, dueAtShamsi: '1405/05/10', customerId: null, assignedToStaffId: otherStaffId, createdByStaffId: otherStaffId, status: 'Open', checklistItems: [], referrals: [] },
    )
    sessionStorage.setItem('crmtask.currentStaffId', staffId)

    renderDashboard()

    expect(await screen.findByText('کار ارجاع شده به من')).toBeInTheDocument()
    expect(screen.getByText('کاری که خودم ثبت کردم')).toBeInTheDocument()
    expect(screen.queryByText('کار بی‌ربط به من')).not.toBeInTheDocument()
  })

  it('hides a my-task whose due date is beyond the configured upcoming window', async () => {
    sampleSettings.taskUpcomingWindowDays = 3
    const nearDueAt = new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString()
    const farDueAt = new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString()
    sampleTasks.push(
      {
        id: 't4',
        title: 'کار نزدیک',
        description: '',
        dueAt: nearDueAt,
        dueAtShamsi: '1405/05/11',
        customerId: null,
        assignedToStaffId: staffId,
        createdByStaffId: staffId,
        status: 'Open',
        checklistItems: [],
        referrals: [],
      },
      {
        id: 't5',
        title: 'کار خیلی دور',
        description: '',
        dueAt: farDueAt,
        dueAtShamsi: '1405/06/10',
        customerId: null,
        assignedToStaffId: staffId,
        createdByStaffId: staffId,
        status: 'Open',
        checklistItems: [],
        referrals: [],
      },
    )
    sessionStorage.setItem('crmtask.currentStaffId', staffId)

    renderDashboard()

    // Wait for a within-window task to render first, proving tasks/settings
    // have both loaded, before asserting the far-out one stayed hidden.
    expect(await screen.findByText('کار نزدیک')).toBeInTheDocument()
    expect(screen.queryByText('کار خیلی دور')).not.toBeInTheDocument()
  })

  it('shows ended contracts on the second tab', async () => {
    sampleCustomers.push({
      id: 'c2',
      name: 'مشتری دوم',
      category: 'Legal',
      phone: '021',
      createdAt: '2026-01-01T00:00:00Z',
      createdAtShamsi: '1404/10/11',
      managerName: null,
      managerBirthDate: null,
      managerBirthDateShamsi: null,
      address: null,
      fax: null,
      notes: null,
      nationalId: null,
      categoryTitle: null,
      activityField: null,
      personnel: [],
    })
    sampleContracts.push({
      id: 'contract-1',
      customerId: 'c2',
      title: 'قرارداد خاتمه‌یافته',
      amount: 1000,
      startDate: '2020-01-01',
      startDateShamsi: '1398/10/11',
      endDate: '2021-01-01',
      endDateShamsi: '1399/10/12',
      status: 'Ended',
    })
    sessionStorage.setItem('crmtask.currentStaffId', staffId)
    renderDashboard()
    const user = userEvent.setup()

    await user.click(await screen.findByRole('tab', { name: 'قراردادهای خاتمه‌یافته' }))

    expect(await screen.findByText('قرارداد خاتمه‌یافته')).toBeInTheDocument()
    expect(screen.getByText('مشتری دوم')).toBeInTheDocument()
  })

  it('also shows ExpiringSoon contracts on the same tab, not just already-ended ones', async () => {
    sampleCustomers.push({
      id: 'c2',
      name: 'مشتری دوم',
      category: 'Legal',
      phone: '021',
      createdAt: '2026-01-01T00:00:00Z',
      createdAtShamsi: '1404/10/11',
      managerName: null,
      managerBirthDate: null,
      managerBirthDateShamsi: null,
      address: null,
      fax: null,
      notes: null,
      nationalId: null,
      categoryTitle: null,
      activityField: null,
      personnel: [],
    })
    sampleContracts.push({
      id: 'contract-2',
      customerId: 'c2',
      title: 'قرارداد رو به اتمام',
      amount: 2000,
      startDate: '2026-01-01',
      startDateShamsi: '1404/10/11',
      endDate: '2026-08-01',
      endDateShamsi: '1405/05/10',
      status: 'ExpiringSoon',
    })
    sessionStorage.setItem('crmtask.currentStaffId', staffId)
    renderDashboard()
    const user = userEvent.setup()

    await user.click(await screen.findByRole('tab', { name: 'قراردادهای خاتمه‌یافته' }))

    expect(await screen.findByText('قرارداد رو به اتمام')).toBeInTheDocument()
    expect(screen.getByText('رو به اتمام')).toBeInTheDocument()
  })
})
