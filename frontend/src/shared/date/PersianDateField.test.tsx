import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import { PersianDateField } from './PersianDateField'

describe('PersianDateField', () => {
  it('renders as a readonly input with the given placeholder', () => {
    render(<PersianDateField placeholder="تاریخ تولد" />)

    const input = screen.getByPlaceholderText('تاریخ تولد')
    expect(input).toHaveAttribute('readonly')
  })

  it('shows the Gregorian value converted to its Jalali display string', () => {
    render(<PersianDateField value="2024-03-20" />)

    expect(screen.getByDisplayValue('۱۴۰۳/۰۱/۰۱')).toBeInTheDocument()
  })

  it('opens a Jalali calendar on focus and reports the picked day as an ISO date', async () => {
    const handleChange = vi.fn()
    render(<PersianDateField value="2024-03-20" onChange={handleChange} />)
    const user = userEvent.setup()

    await user.click(screen.getByDisplayValue('۱۴۰۳/۰۱/۰۱'))
    await user.click(screen.getByText('۲'))

    expect(handleChange).toHaveBeenCalledWith('2024-03-21')
  })

  it('disables days before minDate', async () => {
    // 2024-03-25 is Jalali 1403/01/06; minDate 2024-03-24 is Jalali 1403/01/05,
    // so day ۱ (1403/01/01) of the same displayed month must be disabled.
    render(<PersianDateField value="2024-03-25" minDate="2024-03-24" />)
    const user = userEvent.setup()

    await user.click(screen.getByDisplayValue('۱۴۰۳/۰۱/۰۶'))

    expect(screen.getByText('۱').closest('div.rmdp-day')).toHaveClass('rmdp-disabled')
  })
})
