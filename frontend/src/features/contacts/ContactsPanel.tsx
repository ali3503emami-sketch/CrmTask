import { Button, DatePicker, Drawer, Empty, Form, Input, Select, Spin, Tag, Typography } from 'antd'
import dayjs, { type Dayjs } from 'dayjs'
import { useContacts } from './useContacts'
import { useCreateContact } from './useCreateContact'
import type { ContactDirection } from './types'

const { Text } = Typography

const directionLabel: Record<ContactDirection, { text: string; color: string }> = {
  Inbound: { text: 'ورودی', color: 'blue' },
  Outbound: { text: 'خروجی', color: 'green' },
}

interface LogContactFormValues {
  direction: ContactDirection
  summary: string
  nextFollowUpAt: Dayjs | null
}

interface ContactsPanelProps {
  customerId: string
  customerName: string
  open: boolean
  onClose: () => void
}

export function ContactsPanel({ customerId, customerName, open, onClose }: ContactsPanelProps) {
  const { data: contacts, isLoading } = useContacts(customerId, open)
  const createContact = useCreateContact(customerId)
  const [form] = Form.useForm<LogContactFormValues>()

  const handleSubmit = async (values: LogContactFormValues) => {
    await createContact.mutateAsync({
      direction: values.direction,
      summary: values.summary,
      contactedAt: new Date().toISOString(),
      nextFollowUpAt: values.nextFollowUpAt ? values.nextFollowUpAt.toISOString() : null,
    })
    form.resetFields()
  }

  return (
    <Drawer title={`تماس‌های ${customerName}`} open={open} onClose={onClose} size="default">
      <Form
        form={form}
        layout="vertical"
        onFinish={handleSubmit}
        initialValues={{ direction: 'Outbound' }}
        style={{ marginBottom: 20 }}
      >
        <Form.Item name="direction" label="نوع تماس" rules={[{ required: true }]}>
          <Select
            options={[
              { value: 'Outbound', label: 'خروجی' },
              { value: 'Inbound', label: 'ورودی' },
            ]}
          />
        </Form.Item>
        <Form.Item
          name="summary"
          label="خلاصه تماس"
          rules={[{ required: true, message: 'خلاصه تماس الزامی است' }]}
        >
          <Input.TextArea rows={3} />
        </Form.Item>
        <Form.Item name="nextFollowUpAt" label="یادآوری پیگیری بعدی (اختیاری)">
          <DatePicker style={{ width: '100%' }} disabledDate={(date) => date.isBefore(dayjs(), 'day')} />
        </Form.Item>
        <Form.Item>
          <Button type="primary" htmlType="submit" loading={createContact.isPending} block>
            ثبت تماس
          </Button>
        </Form.Item>
      </Form>

      <Spin spinning={isLoading}>
        {!contacts || contacts.length === 0 ? (
          <Empty description="هنوز تماسی ثبت نشده است" />
        ) : (
          <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
            {contacts.map((contact) => (
              <div key={contact.id} style={{ borderBottom: '1px solid #e5e1d8', paddingBottom: 10 }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 4 }}>
                  <Tag color={directionLabel[contact.direction].color}>
                    {directionLabel[contact.direction].text}
                  </Tag>
                  <Text type="secondary" style={{ fontSize: 12 }}>
                    {dayjs(contact.contactedAt).format('YYYY/MM/DD HH:mm')}
                  </Text>
                </div>
                <div>{contact.summary}</div>
                {contact.nextFollowUpAt && (
                  <Text type="warning" style={{ fontSize: 12 }}>
                    یادآوری پیگیری: {dayjs(contact.nextFollowUpAt).format('YYYY/MM/DD')}
                  </Text>
                )}
              </div>
            ))}
          </div>
        )}
      </Spin>
    </Drawer>
  )
}
