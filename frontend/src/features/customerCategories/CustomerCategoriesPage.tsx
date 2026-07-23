import { ReferenceListPage } from '../../shared/referenceData/ReferenceListPage'

export function CustomerCategoriesPage() {
  return (
    <ReferenceListPage
      queryKey="customerCategories"
      basePath="/api/customer-categories"
      pageTitle="دسته‌بندی مشتریان"
      countLabel="تعداد دسته‌بندی‌ها"
      columnLabel="عنوان دسته‌بندی"
      createButtonLabel="دسته‌بندی جدید"
      modalTitle="دسته‌بندی جدید"
      fieldLabel="عنوان"
    />
  )
}
