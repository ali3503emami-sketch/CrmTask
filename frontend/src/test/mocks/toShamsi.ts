import DateObject from 'react-date-object'
import gregorian from 'react-date-object/calendars/gregorian'
import gregorian_en from 'react-date-object/locales/gregorian_en'
import persian from 'react-date-object/calendars/persian'

/**
 * Mimics the real API's `PersianDateConverter.ToShamsi` (backend,
 * `CrmTask.Domain/Shared/PersianDateConverter.cs`) so MSW-mocked responses
 * carry a realistic Shamsi mirror string — ASCII digits, "YYYY/MM/DD" — for
 * pages that render it directly instead of re-deriving it client-side.
 */
export function toShamsi(isoDate: string): string {
  return new DateObject({ date: isoDate, calendar: gregorian }).convert(persian, gregorian_en).format('YYYY/MM/DD')
}
