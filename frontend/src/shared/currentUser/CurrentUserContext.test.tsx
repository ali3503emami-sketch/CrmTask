import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it } from 'vitest'
import { CurrentUserProvider, useCurrentUser } from './CurrentUserContext'

function Probe() {
  const { currentStaffId, setCurrentStaffId } = useCurrentUser()
  return (
    <div>
      <span>{currentStaffId ?? 'خالی'}</span>
      <button onClick={() => setCurrentStaffId('staff-1')}>انتخاب</button>
      <button onClick={() => setCurrentStaffId(null)}>پاک کردن</button>
    </div>
  )
}

describe('CurrentUserContext', () => {
  beforeEach(() => {
    sessionStorage.clear()
  })

  it('defaults to null when nothing is stored', () => {
    render(
      <CurrentUserProvider>
        <Probe />
      </CurrentUserProvider>,
    )

    expect(screen.getByText('خالی')).toBeInTheDocument()
  })

  it('updates state and persists to sessionStorage when set', async () => {
    render(
      <CurrentUserProvider>
        <Probe />
      </CurrentUserProvider>,
    )
    const user = userEvent.setup()

    await user.click(screen.getByRole('button', { name: 'انتخاب' }))

    expect(screen.getByText('staff-1')).toBeInTheDocument()
    expect(sessionStorage.getItem('crmtask.currentStaffId')).toBe('staff-1')
  })

  it('reads a previously stored value on mount', () => {
    sessionStorage.setItem('crmtask.currentStaffId', 'staff-2')

    render(
      <CurrentUserProvider>
        <Probe />
      </CurrentUserProvider>,
    )

    expect(screen.getByText('staff-2')).toBeInTheDocument()
  })

  it('clears sessionStorage when set to null', async () => {
    sessionStorage.setItem('crmtask.currentStaffId', 'staff-2')
    render(
      <CurrentUserProvider>
        <Probe />
      </CurrentUserProvider>,
    )
    const user = userEvent.setup()

    await user.click(screen.getByRole('button', { name: 'پاک کردن' }))

    expect(screen.getByText('خالی')).toBeInTheDocument()
    expect(sessionStorage.getItem('crmtask.currentStaffId')).toBeNull()
  })
})
