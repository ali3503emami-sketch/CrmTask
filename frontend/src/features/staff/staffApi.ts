import { httpClient } from '../../shared/api/httpClient'
import type { CreateStaffMemberInput, StaffMember } from './types'

export const staffApi = {
  getActive: () => httpClient.get<StaffMember[]>('/api/staff'),
  create: (input: CreateStaffMemberInput) => httpClient.post<StaffMember>('/api/staff', input),
  update: (id: string, input: CreateStaffMemberInput) => httpClient.put<StaffMember>(`/api/staff/${id}`, input),
}
