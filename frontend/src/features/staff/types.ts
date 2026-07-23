export interface StaffMember {
  id: string
  fullName: string
  phoneNumber: string
  position: string | null
  isActive: boolean
}

export interface CreateStaffMemberInput {
  fullName: string
  phoneNumber: string
  position: string | null
}
