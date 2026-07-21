import { Button, Drawer, Empty, Form, Input, InputNumber, Spin, Tag, Typography } from 'antd'
import dayjs from 'dayjs'
import { useContracts } from './useContracts'
import { useCreateContract } from './useCreateContract'
import type { ContractStatus } from './types'

const { Text } = Typography

const statusLabel: Record<ContractStatus, { text: string; color: string }> = {
  Active: { text: 'فعال', color: 'success' },
  ExpiringSoon: { text: 'در حال اتمام', color: 'warning' },
  Ended: { text: 'خاتمه‌یافته', color: 'default' },
}

const datePattern = /^\d{4}-\d{2}-\d{2}$/

interface CreateContractFormValues {
  title: string
  amount: number
  startDate: string
  endDate: string
}

interface ContractsPanelProps {
  customerId: string
  customerName: string
  open: boolean
  onClose: () => void
}

export function ContractsPanel({ customerId, customerName, open, onClose }: ContractsPanelProps) {
  const { data: contracts, isLoading } = useContracts(customerId, open)
  const createContract = useCreateContract(customerId)
  const [form] = Form.useForm<CreateContractFormValues>()

  const handleSubmit = async (values: CreateContractFormValues) => {
    await createContract.mutateAsync(values)
    form.resetFields()
  }

  return (
    <Drawer title={`قراردادهای ${customerName}`} open={open} onClose={onClose} size="default">
      <Form form={form} layout="vertical" onFinish={handleSubmit} style={{ marginBottom: 20 }}>
        <Form.Item
          name="title"
          label="عنوان قرارداد"
          rules={[{ required: true, message: 'عنوان قرارداد الزامی است' }]}
        >
          <Input />
        </Form.Item>
        <Form.Item name="amount" label="مبلغ (تومان)" rules={[{ required: true, message: 'مبلغ الزامی است' }]}>
          <InputNumber style={{ width: '100%' }} min={0} />
        </Form.Item>
        <Form.Item
          name="startDate"
          label="تاریخ شروع"
          rules={[
            { required: true, message: 'تاریخ شروع الزامی است' },
            { pattern: datePattern, message: 'فرمت تاریخ باید YYYY-MM-DD باشد' },
          ]}
        >
          <Input placeholder="1405-01-01" />
        </Form.Item>
        <Form.Item
          name="endDate"
          label="تاریخ پایان"
          rules={[
            { required: true, message: 'تاریخ پایان الزامی است' },
            { pattern: datePattern, message: 'فرمت تاریخ باید YYYY-MM-DD باشد' },
          ]}
        >
          <Input placeholder="1405-12-29" />
        </Form.Item>
        <Form.Item>
          <Button type="primary" htmlType="submit" loading={createContract.isPending} block>
            ثبت قرارداد
          </Button>
        </Form.Item>
      </Form>

      <Spin spinning={isLoading}>
        {!contracts || contracts.length === 0 ? (
          <Empty description="هنوز قراردادی ثبت نشده است" />
        ) : (
          <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
            {contracts.map((contract) => (
              <div key={contract.id} style={{ borderBottom: '1px solid #e5e1d8', paddingBottom: 10 }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 4 }}>
                  <Tag color={statusLabel[contract.status].color}>{statusLabel[contract.status].text}</Tag>
                  <Text type="secondary" style={{ fontSize: 12 }}>
                    {dayjs(contract.startDate).format('YYYY/MM/DD')} تا {dayjs(contract.endDate).format('YYYY/MM/DD')}
                  </Text>
                </div>
                <div>{contract.title}</div>
                <Text type="secondary" style={{ fontSize: 12 }}>
                  {contract.amount.toLocaleString('fa-IR')} تومان
                </Text>
              </div>
            ))}
          </div>
        )}
      </Spin>
    </Drawer>
  )
}
