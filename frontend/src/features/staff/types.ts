export interface StaffMember {
  id: string
  fullName: string
  phoneNumber: string
  isActive: boolean
}

export interface CreateStaffMemberInput {
  fullName: string
  phoneNumber: string
}
