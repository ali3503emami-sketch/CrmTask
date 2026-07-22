import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import { PersianCalendar } from './PersianCalendar'

describe('PersianCalendar', () => {
  it('renders the Jalali month/year for the given value', () => {
    // 2024-03-20 is Nowruz (Persian new year) 1403 — a fixed calendar fact.
    render(<PersianCalendar value="2024-03-20" onChange={() => {}} />)

    expect(screen.getAllByText('فروردین').length).toBeGreaterThan(0)
    expect(screen.getAllByText('۱۴۰۳').length).toBeGreaterThan(0)
  })

  it('reports the clicked day as a Gregorian ISO date', async () => {
    const handleChange = vi.fn()
    render(<PersianCalendar value="2024-03-20" onChange={handleChange} />)
    const user = userEvent.setup()

    await user.click(screen.getByText('۲'))

    expect(handleChange).toHaveBeenCalledWith('2024-03-21')
  })
})
