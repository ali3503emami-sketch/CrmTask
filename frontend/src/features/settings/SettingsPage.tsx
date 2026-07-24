import { Button, Card, Form, InputNumber, message } from 'antd'
import { useAppSettings, useUpdateAppSettings } from './useAppSettings'
import type { AppSettings } from './types'

export function SettingsPage() {
  const { data: settings, isLoading } = useAppSettings()
  const updateSettings = useUpdateAppSettings()

  const handleSubmit = async (values: AppSettings) => {
    await updateSettings.mutateAsync(values)
    message.success('تنظیمات ذخیره شد.')
  }

  if (isLoading || !settings) {
    return <Card size="small" title="تنظیمات" loading style={{ maxWidth: 480 }} />
  }

  return (
    <Card size="small" title="تنظیمات" style={{ maxWidth: 480 }}>
      <Form layout="vertical" onFinish={handleSubmit} initialValues={settings}>
        <Form.Item
          name="taskUpcomingWindowDays"
          label="نمایش کارهای چند روز آینده در داشبورد"
          rules={[{ required: true, message: 'این فیلد الزامی است' }]}
        >
          <InputNumber min={0} style={{ width: '100%' }} />
        </Form.Item>
        <Form.Item
          name="contractEndingWindowDays"
          label="نمایش قراردادهای در حال خاتمه از چند روز قبل"
          rules={[{ required: true, message: 'این فیلد الزامی است' }]}
        >
          <InputNumber min={0} style={{ width: '100%' }} />
        </Form.Item>
        <Form.Item>
          <Button type="primary" htmlType="submit" loading={updateSettings.isPending}>
            ذخیره
          </Button>
        </Form.Item>
      </Form>
    </Card>
  )
}
