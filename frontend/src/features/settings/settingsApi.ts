import { httpClient } from '../../shared/api/httpClient'
import type { AppSettings, UpdateAppSettingsInput } from './types'

export const settingsApi = {
  get: () => httpClient.get<AppSettings>('/api/settings'),
  update: (input: UpdateAppSettingsInput) => httpClient.put<AppSettings>('/api/settings', input),
}
