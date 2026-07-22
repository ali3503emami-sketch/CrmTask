import { Button, Drawer, Form, Input, Select, Space } from 'antd'
import { PersianDateField } from '../../shared/date/PersianDateField'
import { useUpdateCustomer } from './useUpdateCustomer'
import type { Customer, CustomerCategory, CustomerPersonnelInput } from './types'

interface CustomerProfileFormValues {
  name: string
  category: CustomerCategory
  phone: string
  managerName?: string
  managerBirthDate?: string | null
  address?: string
  fax?: string
  notes?: string
  nationalId?: string
  personnel?: CustomerPersonnelInput[]
}

interface CustomerProfilePanelProps {
  customer: Customer
  open: boolean
  onClose: () => void
}

export function CustomerProfilePanel({ customer, open, onClose }: CustomerProfilePanelProps) {
  const updateCustomer = useUpdateCustomer()
  const [form] = Form.useForm<CustomerProfileFormValues>()

  const handleSubmit = async (values: CustomerProfileFormValues) => {
    await updateCustomer.mutateAsync({
      id: customer.id,
      input: {
        name: values.name,
        category: values.category,
        phone: values.phone,
        managerName: values.managerName?.trim() || null,
        managerBirthDate: values.managerBirthDate ?? null,
        address: values.address?.trim() || null,
        fax: values.fax?.trim() || null,
        notes: values.notes?.trim() || null,
        nationalId: values.nationalId?.trim() || null,
        personnel: (values.personnel ?? []).map((p) => ({
          fullName: p.fullName,
          position: p.position || null,
          phone: p.phone || null,
          mobile: p.mobile || null,
          email: p.email || null,
        })),
      },
    })
    onClose()
  }

  return (
    <Drawer title={`ویرایش ${customer.name}`} open={open} onClose={onClose} size="large" destroyOnHidden>
      <Form
        form={form}
        layout="vertical"
        onFinish={handleSubmit}
        initialValues={{
          name: customer.name,
          category: customer.category,
          phone: customer.phone,
          managerName: customer.managerName ?? undefined,
          managerBirthDate: customer.managerBirthDate ?? undefined,
          address: customer.address ?? undefined,
          fax: customer.fax ?? undefined,
          notes: customer.notes ?? undefined,
          nationalId: customer.nationalId ?? undefined,
          personnel: customer.personnel.map((p) => ({
            fullName: p.fullName,
            position: p.position ?? undefined,
            phone: p.phone ?? undefined,
            mobile: p.mobile ?? undefined,
            email: p.email ?? undefined,
          })),
        }}
      >
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
        <Form.Item name="managerName" label="نام مدیرعامل">
          <Input />
        </Form.Item>
        <Form.Item name="managerBirthDate" label="تاریخ تولد مدیرعامل">
          <PersianDateField placeholder="انتخاب تاریخ تولد" />
        </Form.Item>
        <Form.Item name="address" label="آدرس">
          <Input.TextArea rows={2} />
        </Form.Item>
        <Form.Item name="fax" label="فکس">
          <Input />
        </Form.Item>
        <Form.Item name="nationalId" label="شماره ملی / شناسه ملی">
          <Input />
        </Form.Item>
        <Form.Item name="notes" label="یادداشت‌ها">
          <Input.TextArea rows={2} />
        </Form.Item>

        <Form.List name="personnel">
          {(fields, { add, remove }) => (
            <>
              {fields.map((field, index) => (
                <Space key={field.key} align="baseline" style={{ display: 'flex', marginBottom: 8 }} wrap>
                  <Form.Item
                    name={[field.name, 'fullName']}
                    label={`نام پرسنل ${index + 1}`}
                    rules={[{ required: true, message: 'نام الزامی است' }]}
                  >
                    <Input />
                  </Form.Item>
                  <Form.Item name={[field.name, 'position']} label="سمت">
                    <Input />
                  </Form.Item>
                  <Form.Item name={[field.name, 'phone']} label="تلفن">
                    <Input />
                  </Form.Item>
                  <Form.Item name={[field.name, 'mobile']} label="موبایل">
                    <Input />
                  </Form.Item>
                  <Form.Item name={[field.name, 'email']} label="ایمیل">
                    <Input />
                  </Form.Item>
                  <Button onClick={() => remove(field.name)}>حذف</Button>
                </Space>
              ))}
              <Button onClick={() => add()} block style={{ marginBottom: 16 }}>
                افزودن پرسنل
              </Button>
            </>
          )}
        </Form.List>

        <Form.Item>
          <Button type="primary" htmlType="submit" loading={updateCustomer.isPending} block>
            ذخیره تغییرات
          </Button>
        </Form.Item>
      </Form>
    </Drawer>
  )
}
