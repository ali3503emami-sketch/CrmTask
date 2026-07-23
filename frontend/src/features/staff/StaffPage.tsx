import { useState } from 'react'
import { Button, Card, Form, Input, Modal, Select, Statistic, Table, Tag } from 'antd'
import type { ColumnsType } from 'antd/es/table'
import { useStaff } from './useStaff'
import { useCreateStaffMember } from './useCreateStaffMember'
import { useReferenceList } from '../../shared/referenceData/useReferenceList'
import type { StaffMember } from './types'

interface CreateStaffFormValues {
  fullName: string
  phoneNumber: string
  position?: string
}

export function StaffPage() {
  const { data: staff, isLoading } = useStaff()
  const createStaffMember = useCreateStaffMember()
  const { data: positions } = useReferenceList('positions', '/api/positions')
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [form] = Form.useForm<CreateStaffFormValues>()

  const handleSubmit = async (values: CreateStaffFormValues) => {
    await createStaffMember.mutateAsync({ ...values, position: values.position ?? null })
    form.resetFields()
    setIsModalOpen(false)
  }

  const columns: ColumnsType<StaffMember> = [
    { title: 'نام و نام خانوادگی', dataIndex: 'fullName', key: 'fullName' },
    { title: 'سمت', dataIndex: 'position', key: 'position', width: 140, render: (position: string | null) => position ?? '—' },
    { title: 'شماره تماس', dataIndex: 'phoneNumber', key: 'phoneNumber', width: 160 },
    {
      title: 'وضعیت',
      dataIndex: 'isActive',
      key: 'isActive',
      width: 110,
      render: (isActive: boolean) => (
        <Tag color={isActive ? 'success' : 'default'}>{isActive ? 'فعال' : 'غیرفعال'}</Tag>
      ),
    },
  ]

  return (
    <div>
      <Card
        size="small"
        title="پرسنل"
        extra={
          <Button type="primary" onClick={() => setIsModalOpen(true)}>
            کارمند جدید
          </Button>
        }
        style={{ marginBottom: 16 }}
      >
        <Statistic title="تعداد پرسنل" value={staff?.length ?? 0} />
      </Card>

      <Card size="small">
        <Table size="small" rowKey="id" loading={isLoading} columns={columns} dataSource={staff} pagination={false} />
      </Card>

      <Modal title="کارمند جدید" open={isModalOpen} onCancel={() => setIsModalOpen(false)} footer={null} destroyOnHidden>
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
          <Form.Item
            name="fullName"
            label="نام و نام خانوادگی"
            rules={[{ required: true, message: 'نام الزامی است' }]}
          >
            <Input />
          </Form.Item>
          <Form.Item name="phoneNumber" label="شماره تماس" rules={[{ required: true, message: 'شماره تماس الزامی است' }]}>
            <Input />
          </Form.Item>
          <Form.Item name="position" label="سمت">
            <Select
              allowClear
              placeholder="انتخاب سمت"
              options={(positions ?? []).map((p) => ({ value: p.title, label: p.title }))}
            />
          </Form.Item>
          <Form.Item>
            <Button type="primary" htmlType="submit" loading={createStaffMember.isPending} block>
              ثبت
            </Button>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  )
}
