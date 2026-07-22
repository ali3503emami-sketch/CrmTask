import { Input } from 'antd'
import DateObject from 'react-date-object'
import DatePicker from 'react-multi-date-picker'
import TimePicker from 'react-multi-date-picker/plugins/time_picker'
import gregorian from 'react-date-object/calendars/gregorian'
import gregorian_en from 'react-date-object/locales/gregorian_en'
import persian from 'react-date-object/calendars/persian'
import persian_fa from 'react-date-object/locales/persian_fa'

interface PersianDateTimeFieldProps {
  value?: string | null
  onChange?: (isoDateTime: string | null) => void
  placeholder?: string
}

const DISPLAY_FORMAT = 'YYYY/MM/DD HH:mm'

/**
 * A date+time picker rendered as a Jalali calendar with an attached time
 * picker plugin, styled to match antd's Input. Value in/out is always a full
 * Gregorian ISO datetime string, what the API expects. See PersianDateField
 * for why the incoming value must be parsed with the `gregorian` calendar
 * explicitly, and why the outgoing value must be formatted with the
 * `gregorian_en` locale (otherwise digits come out as Persian numerals).
 */
export function PersianDateTimeField({ value, onChange, placeholder }: PersianDateTimeFieldProps) {
  const dateValue = value ? new DateObject({ date: value, calendar: gregorian }) : undefined

  return (
    <DatePicker
      calendar={persian}
      locale={persian_fa}
      format={DISPLAY_FORMAT}
      value={dateValue}
      onChange={(date: DateObject | null) =>
        onChange?.(date ? date.convert(gregorian, gregorian_en).toDate().toISOString() : null)
      }
      plugins={[<TimePicker key="time-picker" hideSeconds />]}
      render={(inputValue, openCalendar) => (
        <Input value={inputValue} onFocus={openCalendar} placeholder={placeholder} readOnly />
      )}
    />
  )
}
