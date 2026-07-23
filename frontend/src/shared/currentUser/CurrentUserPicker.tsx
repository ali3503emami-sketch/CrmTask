import { Select, Space, Typography } from 'antd'
import { useStaff } from '../../features/staff/useStaff'
import { useCurrentUser } from './CurrentUserContext'

const { Text } = Typography

export function CurrentUserPicker() {
  const { data: staff } = useStaff()
  const { currentStaffId, setCurrentStaffId } = useCurrentUser()

  return (
    <Space>
      <Text type="secondary">شما کیستید؟</Text>
      <Select
        style={{ minWidth: 180 }}
        placeholder="انتخاب کاربر"
        allowClear
        value={currentStaffId ?? undefined}
        onChange={(value) => setCurrentStaffId(value ?? null)}
        options={(staff ?? []).map((s) => ({ value: s.id, label: s.fullName }))}
      />
    </Space>
  )
}
