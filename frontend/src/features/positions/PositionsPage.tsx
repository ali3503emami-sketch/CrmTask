import { ReferenceListPage } from '../../shared/referenceData/ReferenceListPage'

export function PositionsPage() {
  return (
    <ReferenceListPage
      queryKey="positions"
      basePath="/api/positions"
      pageTitle="سمت‌ها"
      countLabel="تعداد سمت‌ها"
      columnLabel="عنوان سمت"
      createButtonLabel="سمت جدید"
      modalTitle="سمت جدید"
      fieldLabel="عنوان"
    />
  )
}
