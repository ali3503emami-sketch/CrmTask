import { Calendar } from 'react-multi-date-picker'
import DateObject from 'react-date-object'
import gregorian from 'react-date-object/calendars/gregorian'
import gregorian_en from 'react-date-object/locales/gregorian_en'
import persian from 'react-date-object/calendars/persian'
import persian_fa from 'react-date-object/locales/persian_fa'

interface PersianCalendarProps {
  value?: string | null
  onChange: (isoDate: string) => void
}

/**
 * An always-visible (non-popup) Jalali calendar for day-based navigation —
 * e.g. the Tasks page's "pick a day to filter/add tasks" affordance. See
 * PersianDateField for why incoming/outgoing values must cross the
 * gregorian/gregorian_en calendar+locale pair explicitly.
 */
export function PersianCalendar({ value, onChange }: PersianCalendarProps) {
  const dateValue = value ? new DateObject({ date: value, format: 'YYYY-MM-DD', calendar: gregorian }) : undefined

  return (
    <Calendar
      calendar={persian}
      locale={persian_fa}
      value={dateValue}
      onChange={(date: DateObject | null) => {
        if (date) {
          onChange(date.convert(gregorian, gregorian_en).format('YYYY-MM-DD'))
        }
      }}
    />
  )
}
