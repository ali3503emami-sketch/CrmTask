import { Button, Form, Input, Select, Space } from 'antd'
import { choiceFieldTypes, fieldTypeOptions, type ChecklistFieldFormValue } from './checklistFieldTypes'
import type { ChecklistFieldType } from './types'

interface FormValuesWithChecklist {
  checklistFields?: ChecklistFieldFormValue[]
}

/**
 * The checklist-fields `Form.List` UI — shared between TasksPage's create form
 * and TaskDetailPanel's edit form (which reuses the exact same field shape,
 * see checklistFieldTypes.ts) so the two can't drift apart. Assumes it's
 * rendered inside a `Form` whose values include a `checklistFields` array.
 */
export function ChecklistFieldsEditor() {
  return (
    <Form.List name="checklistFields">
      {(fields, { add, remove }) => (
        <>
          {fields.map((field, index) => (
            <Space key={field.key} align="baseline" style={{ display: 'flex', marginBottom: 8 }} wrap>
              <Form.Item
                name={[field.name, 'label']}
                label={`برچسب آیتم ${index + 1}`}
                rules={[{ required: true, message: 'برچسب الزامی است' }]}
              >
                <Input />
              </Form.Item>
              <Form.Item name={[field.name, 'fieldType']} label="نوع" initialValue="TextBox">
                <Select style={{ width: 140 }} options={fieldTypeOptions} />
              </Form.Item>
              <Form.Item
                noStyle
                shouldUpdate={(prev: FormValuesWithChecklist, cur: FormValuesWithChecklist) =>
                  prev.checklistFields?.[index]?.fieldType !== cur.checklistFields?.[index]?.fieldType
                }
              >
                {({ getFieldValue }) => {
                  const fieldType: ChecklistFieldType = getFieldValue(['checklistFields', field.name, 'fieldType'])
                  return choiceFieldTypes.includes(fieldType) ? (
                    <Form.Item name={[field.name, 'options']} label="گزینه‌ها (هر خط یک گزینه)">
                      <Input.TextArea rows={3} style={{ minWidth: 200 }} />
                    </Form.Item>
                  ) : null
                }}
              </Form.Item>
              <Button onClick={() => remove(field.name)}>حذف</Button>
            </Space>
          ))}
          <Button onClick={() => add({ fieldType: 'TextBox' })} block style={{ marginBottom: 16 }}>
            افزودن آیتم چک‌لیست
          </Button>
        </>
      )}
    </Form.List>
  )
}
