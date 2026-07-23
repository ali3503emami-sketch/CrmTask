import { ReferenceListPage } from '../../shared/referenceData/ReferenceListPage'

export function ActivityFieldsPage() {
  return (
    <ReferenceListPage
      queryKey="activityFields"
      basePath="/api/activity-fields"
      pageTitle="زمینه فعالیت"
      countLabel="تعداد زمینه‌های فعالیت"
      columnLabel="عنوان زمینه فعالیت"
      createButtonLabel="زمینه فعالیت جدید"
      modalTitle="زمینه فعالیت جدید"
      fieldLabel="عنوان"
    />
  )
}
