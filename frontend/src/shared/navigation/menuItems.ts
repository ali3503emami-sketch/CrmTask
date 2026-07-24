import type { MenuProps } from 'antd'

export type PageKey = 'dashboard' | 'staff' | 'positions' | 'customerCategories' | 'activityFields' | 'customers' | 'tasks'

export const menuItems: MenuProps['items'] = [
  {
    key: 'user-group',
    label: 'کاربر',
    children: [
      { key: 'dashboard', label: 'داشبورد' },
      {
        key: 'basic-info',
        label: 'اطلاعات پایه',
        children: [
          { key: 'staff', label: 'معرفی پرسنل' },
          { key: 'positions', label: 'معرفی سمت‌ها' },
          { key: 'customerCategories', label: 'معرفی دسته‌بندی مشتریان' },
          { key: 'activityFields', label: 'معرفی زمینه فعالیت' },
        ],
      },
      {
        key: 'customer-affairs',
        label: 'امور مشتریان',
        children: [{ key: 'customers', label: 'معرفی مشتریان' }],
      },
      { key: 'tasks', label: 'انجام کار' },
    ],
  },
]
