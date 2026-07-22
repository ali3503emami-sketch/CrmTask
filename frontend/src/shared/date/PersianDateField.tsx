import { Input } from 'antd'
import DateObject from 'react-date-object'
import DatePicker from 'react-multi-date-picker'
import gregorian from 'react-date-object/calendars/gregorian'
import gregorian_en from 'react-date-object/locales/gregorian_en'
import persian from 'react-date-object/calendars/persian'
import persian_fa from 'react-date-object/locales/persian_fa'

interface PersianDateFieldProps {
  value?: string | null
  onChange?: (isoDate: string | null) => void
  placeholder?: string
  /** Gregorian "YYYY-MM-DD" — days before this are disabled in the calendar. */
  minDate?: string
}

/**
 * A date-only picker (no time-of-day) rendered as a Jalali calendar, styled to
 * match antd's Input via react-multi-date-picker's `render` prop — value in/out
 * is always a plain Gregorian "YYYY-MM-DD" string, what the API expects. The
 * incoming value must be parsed with the `gregorian` calendar explicitly:
 * DatePicker otherwise parses plain strings using its own display calendar
 * (persian), silently misreading e.g. "2024-03-20" as a Jalali date.
 */
export function PersianDateField({ value, onChange, placeholder, minDate }: PersianDateFieldProps) {
  const dateValue = value ? new DateObject({ date: value, format: 'YYYY-MM-DD', calendar: gregorian }) : undefined
  const minDateValue = minDate ? new DateObject({ date: minDate, format: 'YYYY-MM-DD', calendar: gregorian }) : undefined

  return (
    <DatePicker
      calendar={persian}
      locale={persian_fa}
      value={dateValue}
      minDate={minDateValue}
      onChange={(date: DateObject | null) =>
        onChange?.(date ? date.convert(gregorian, gregorian_en).format('YYYY-MM-DD') : null)
      }
      render={(inputValue, openCalendar) => (
        <Input value={inputValue} onFocus={openCalendar} placeholder={placeholder} readOnly />
      )}
    />
  )
}
