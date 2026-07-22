import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import { PersianDateTimeField } from './PersianDateTimeField'

describe('PersianDateTimeField', () => {
  it('renders as a readonly input with the given placeholder', () => {
    render(<PersianDateTimeField placeholder="مهلت انجام" />)

    const input = screen.getByPlaceholderText('مهلت انجام')
    expect(input).toHaveAttribute('readonly')
  })

  it('shows the Gregorian value converted to its Jalali display string with time', () => {
    render(<PersianDateTimeField value="2024-03-20T14:30:00.000Z" />)

    expect(screen.getByRole('textbox').getAttribute('value')).toContain('۱۴۰۳/۰۱/۰۱')
  })

  it('reports the picked day as a full ISO datetime string', async () => {
    const handleChange = vi.fn()
    render(<PersianDateTimeField value="2024-03-20T14:30:00.000Z" onChange={handleChange} />)
    const user = userEvent.setup()

    await user.click(screen.getByRole('textbox'))
    await user.click(screen.getByText('۲'))

    expect(handleChange).toHaveBeenCalledTimes(1)
    const [isoDateTime] = handleChange.mock.calls[0] as [string]
    expect(isoDateTime).toMatch(/^2024-03-21T/)
  })
})
