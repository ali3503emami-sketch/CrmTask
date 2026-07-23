import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it } from 'vitest'
import { CurrentUserPicker } from './CurrentUserPicker'
import { CurrentUserProvider, useCurrentUser } from './CurrentUserContext'
import { sampleStaff } from '../../test/mocks/handlers'

function CurrentStaffIdProbe() {
  const { currentStaffId } = useCurrentUser()
  return <span data-testid="current-staff-id">{currentStaffId ?? 'خالی'}</span>
}

function renderPicker() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <CurrentUserProvider>
        <CurrentUserPicker />
        <CurrentStaffIdProbe />
      </CurrentUserProvider>
    </QueryClientProvider>,
  )
}

describe('CurrentUserPicker', () => {
  beforeEach(() => {
    sessionStorage.clear()
  })

  it('lets the user pick who they are from the staff list', async () => {
    renderPicker()
    const user = userEvent.setup()

    await user.click(screen.getByRole('combobox'))
    await user.click(await screen.findByText(sampleStaff[0].fullName))

    expect(screen.getByTestId('current-staff-id')).toHaveTextContent(sampleStaff[0].id)
  })
})
