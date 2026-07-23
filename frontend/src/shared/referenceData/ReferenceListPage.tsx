import { useState } from 'react'
import { Button, Card, Form, Input, Modal, Statistic, Table } from 'antd'
import type { ColumnsType } from 'antd/es/table'
import { useCreateReferenceListItem, useReferenceList } from './useReferenceList'
import type { ReferenceListItem } from './types'

interface CreateFormValues {
  title: string
}

interface ReferenceListPageProps {
  queryKey: string
  basePath: string
  pageTitle: string
  countLabel: string
  columnLabel: string
  createButtonLabel: string
  modalTitle: string
  fieldLabel: string
}

/**
 * Positions, Customer Categories, and Activity Fields are all "just a list of
 * titles" (list + create only) — one generic page instead of tripling
 * near-identical table+modal boilerplate. See ReferenceListControllerBase on
 * the backend for the matching API-side consolidation.
 */
export function ReferenceListPage({
  queryKey,
  basePath,
  pageTitle,
  countLabel,
  columnLabel,
  createButtonLabel,
  modalTitle,
  fieldLabel,
}: ReferenceListPageProps) {
  const { data: items, isLoading } = useReferenceList(queryKey, basePath)
  const createItem = useCreateReferenceListItem(queryKey, basePath)
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [form] = Form.useForm<CreateFormValues>()

  const handleSubmit = async (values: CreateFormValues) => {
    await createItem.mutateAsync(values)
    form.resetFields()
    setIsModalOpen(false)
  }

  const columns: ColumnsType<ReferenceListItem> = [{ title: columnLabel, dataIndex: 'title', key: 'title' }]

  return (
    <div>
      <Card
        size="small"
        title={pageTitle}
        extra={
          <Button type="primary" onClick={() => setIsModalOpen(true)}>
            {createButtonLabel}
          </Button>
        }
        style={{ marginBottom: 16 }}
      >
        <Statistic title={countLabel} value={items?.length ?? 0} />
      </Card>

      <Card size="small">
        <Table size="small" rowKey="id" loading={isLoading} columns={columns} dataSource={items} pagination={false} />
      </Card>

      <Modal title={modalTitle} open={isModalOpen} onCancel={() => setIsModalOpen(false)} footer={null} destroyOnHidden>
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
          <Form.Item name="title" label={fieldLabel} rules={[{ required: true, message: 'این فیلد الزامی است' }]}>
            <Input />
          </Form.Item>
          <Form.Item>
            <Button type="primary" htmlType="submit" loading={createItem.isPending} block>
              ثبت
            </Button>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  )
}
