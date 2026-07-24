import { useState } from 'react'
import { Button, Card, Form, Input, Modal, Space, Statistic, Table } from 'antd'
import type { ColumnsType } from 'antd/es/table'
import { useCreateReferenceListItem, useReferenceList, useUpdateReferenceListItem } from './useReferenceList'
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
  const updateItem = useUpdateReferenceListItem(queryKey, basePath)
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [editingItem, setEditingItem] = useState<ReferenceListItem | null>(null)
  const [form] = Form.useForm<CreateFormValues>()

  const openCreateModal = () => {
    setEditingItem(null)
    form.resetFields()
    setIsModalOpen(true)
  }

  const openEditModal = (item: ReferenceListItem) => {
    setEditingItem(item)
    form.setFieldsValue({ title: item.title })
    setIsModalOpen(true)
  }

  const handleSubmit = async (values: CreateFormValues) => {
    if (editingItem) {
      await updateItem.mutateAsync({ id: editingItem.id, input: values })
    } else {
      await createItem.mutateAsync(values)
    }
    form.resetFields()
    setIsModalOpen(false)
    setEditingItem(null)
  }

  const columns: ColumnsType<ReferenceListItem> = [
    { title: columnLabel, dataIndex: 'title', key: 'title' },
    {
      title: 'عملیات',
      key: 'actions',
      width: 100,
      render: (_, item) => (
        <Space>
          <Button size="small" onClick={() => openEditModal(item)}>
            ویرایش
          </Button>
        </Space>
      ),
    },
  ]

  return (
    <div>
      <Card
        size="small"
        title={pageTitle}
        extra={
          <Button type="primary" onClick={openCreateModal}>
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

      <Modal
        title={editingItem ? 'ویرایش' : modalTitle}
        open={isModalOpen}
        onCancel={() => setIsModalOpen(false)}
        footer={null}
        destroyOnHidden
      >
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
          <Form.Item name="title" label={fieldLabel} rules={[{ required: true, message: 'این فیلد الزامی است' }]}>
            <Input />
          </Form.Item>
          <Form.Item>
            <Button type="primary" htmlType="submit" loading={editingItem ? updateItem.isPending : createItem.isPending} block>
              {editingItem ? 'ذخیره تغییرات' : 'ثبت'}
            </Button>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  )
}
