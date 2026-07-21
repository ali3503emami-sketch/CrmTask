import { Checkbox, Drawer, Empty, Input, Radio, Select, Typography } from 'antd'
import { useSetChecklistItemValue } from './useSetChecklistItemValue'
import type { ChecklistItem, TaskItem } from './types'

const { Text } = Typography

interface ChecklistPanelProps {
  task: TaskItem
  open: boolean
  onClose: () => void
}

function ChecklistField({ taskId, item }: { taskId: string; item: ChecklistItem }) {
  const setValue = useSetChecklistItemValue()
  const commit = (value: string | null) => setValue.mutate({ taskId, checklistItemId: item.id, value })

  switch (item.fieldType) {
    case 'Checkbox':
      return (
        <Checkbox checked={item.value === 'true'} onChange={(e) => commit(e.target.checked ? 'true' : 'false')}>
          {item.label}
        </Checkbox>
      )
    case 'Dropdown':
      return (
        <div>
          <Text>{item.label}</Text>
          <Select
            style={{ width: '100%', marginTop: 4 }}
            value={item.value ?? undefined}
            onChange={commit}
            options={item.options.map((o) => ({ value: o, label: o }))}
          />
        </div>
      )
    case 'ListBox':
      return (
        <div>
          <Text>{item.label}</Text>
          <Radio.Group
            style={{ display: 'flex', flexDirection: 'column', marginTop: 4 }}
            value={item.value ?? undefined}
            onChange={(e) => commit(e.target.value)}
          >
            {item.options.map((o) => (
              <Radio key={o} value={o}>
                {o}
              </Radio>
            ))}
          </Radio.Group>
        </div>
      )
    case 'TextBox':
      return (
        <div>
          <Text>{item.label}</Text>
          <Input
            style={{ marginTop: 4 }}
            defaultValue={item.value ?? ''}
            onBlur={(e) => commit(e.target.value || null)}
          />
        </div>
      )
    default:
      return null
  }
}

export function ChecklistPanel({ task, open, onClose }: ChecklistPanelProps) {
  return (
    <Drawer title={`چک‌لیست: ${task.title}`} open={open} onClose={onClose} size="default">
      {task.checklistItems.length === 0 ? (
        <Empty description="این کار چک‌لیستی ندارد" />
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
          {task.checklistItems.map((item) => (
            <ChecklistField key={item.id} taskId={task.id} item={item} />
          ))}
        </div>
      )}
    </Drawer>
  )
}
