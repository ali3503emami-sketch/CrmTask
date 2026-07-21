import { useState } from 'react'
import { Button, Card, Form, Input, Modal, Select, Space, Statistic, Table, Tag } from 'antd'
import type { ColumnsType } from 'antd/es/table'
import { useCustomers } from './useCustomers'
import { useCreateCustomer } from './useCreateCustomer'
import type { Customer, CustomerCategory } from './types'
import { ContactsPanel } from '../contacts/ContactsPanel'
import { ContractsPanel } from '../contracts/ContractsPanel'

const categoryLabel: Record<CustomerCategory, { text: string; color: string }> = {
  Legal: { text: 'حقوقی', color: 'blue' },
  Individual: { text: 'حقیقی', color: 'default' },
}

type ActivePanel = 'contacts' | 'contracts' | null

interface CreateCustomerFormValues {
  name: string
  category: CustomerCategory
  phone: string
}

export function CustomersPage() {
  const { data: customers, isLoading } = useCustomers()
  const createCustomer = useCreateCustomer()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [form] = Form.useForm<CreateCustomerFormValues>()
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null)
  const [activePanel, setActivePanel] = useState<ActivePanel>(null)

  const handleSubmit = async (values: CreateCustomerFormValues) => {
    await createCustomer.mutateAsync(values)
    form.resetFields()
    setIsModalOpen(false)
  }

  const openPanel = (customer: Customer, panel: ActivePanel) => {
    setSelectedCustomer(customer)
    setActivePanel(panel)
  }

  const columns: ColumnsType<Customer> = [
    { title: 'نام مشتری', dataIndex: 'name', key: 'name' },
    {
      title: 'دسته‌بندی',
      dataIndex: 'category',
      key: 'category',
      width: 110,
      render: (category: CustomerCategory) => (
        <Tag color={categoryLabel[category].color}>{categoryLabel[category].text}</Tag>
      ),
    },
    { title: 'تلفن', dataIndex: 'phone', key: 'phone', width: 150 },
    {
      title: 'عملیات',
      key: 'actions',
      width: 180,
      render: (_, customer) => (
        <Space>
          <Button size="small" onClick={() => openPanel(customer, 'contacts')}>
            تماس‌ها
          </Button>
          <Button size="small" onClick={() => openPanel(customer, 'contracts')}>
            قراردادها
          </Button>
        </Space>
      ),
    },
  ]

  return (
    <div>
      <Card
        size="small"
        title="مشتریان"
        extra={
          <Button type="primary" onClick={() => setIsModalOpen(true)}>
            مشتری جدید
          </Button>
        }
        style={{ marginBottom: 16 }}
      >
        <Statistic title="تعداد مشتریان" value={customers?.length ?? 0} />
      </Card>

      <Card size="small">
        <Table
          size="small"
          rowKey="id"
          loading={isLoading}
          columns={columns}
          dataSource={customers}
          pagination={false}
        />
      </Card>

      <Modal
        title="مشتری جدید"
        open={isModalOpen}
        onCancel={() => setIsModalOpen(false)}
        footer={null}
        destroyOnHidden
      >
        <Form form={form} layout="vertical" onFinish={handleSubmit} initialValues={{ category: 'Legal' }}>
          <Form.Item name="name" label="نام مشتری" rules={[{ required: true, message: 'نام مشتری الزامی است' }]}>
            <Input />
          </Form.Item>
          <Form.Item name="category" label="دسته‌بندی" rules={[{ required: true }]}>
            <Select
              options={[
                { value: 'Legal', label: 'حقوقی' },
                { value: 'Individual', label: 'حقیقی' },
              ]}
            />
          </Form.Item>
          <Form.Item name="phone" label="تلفن" rules={[{ required: true, message: 'تلفن الزامی است' }]}>
            <Input />
          </Form.Item>
          <Form.Item>
            <Button type="primary" htmlType="submit" loading={createCustomer.isPending} block>
              ثبت
            </Button>
          </Form.Item>
        </Form>
      </Modal>

      {selectedCustomer && (
        <ContactsPanel
          customerId={selectedCustomer.id}
          customerName={selectedCustomer.name}
          open={activePanel === 'contacts'}
          onClose={() => setActivePanel(null)}
        />
      )}

      {selectedCustomer && (
        <ContractsPanel
          customerId={selectedCustomer.id}
          customerName={selectedCustomer.name}
          open={activePanel === 'contracts'}
          onClose={() => setActivePanel(null)}
        />
      )}
    </div>
  )
}
