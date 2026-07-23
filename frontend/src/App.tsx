import { useState } from 'react'
import { Layout, Menu, Typography } from 'antd'
import type { MenuProps } from 'antd'
import { CustomersPage } from './features/customers/CustomersPage'
import { TasksPage } from './features/tasks/TasksPage'
import { StaffPage } from './features/staff/StaffPage'
import { PositionsPage } from './features/positions/PositionsPage'
import { CustomerCategoriesPage } from './features/customerCategories/CustomerCategoriesPage'
import { ActivityFieldsPage } from './features/activityFields/ActivityFieldsPage'
import { DashboardPage } from './features/dashboard/DashboardPage'
import { CurrentUserProvider } from './shared/currentUser/CurrentUserContext'
import { CurrentUserPicker } from './shared/currentUser/CurrentUserPicker'

const { Header, Content, Sider } = Layout
const { Title } = Typography

type PageKey = 'dashboard' | 'staff' | 'positions' | 'customerCategories' | 'activityFields' | 'customers' | 'tasks'

const pageComponents: Record<PageKey, () => React.JSX.Element> = {
  dashboard: DashboardPage,
  staff: StaffPage,
  positions: PositionsPage,
  customerCategories: CustomerCategoriesPage,
  activityFields: ActivityFieldsPage,
  customers: CustomersPage,
  tasks: TasksPage,
}

const menuItems: MenuProps['items'] = [
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

function App() {
  const [page, setPage] = useState<PageKey>('dashboard')
  const ActivePage = pageComponents[page]

  return (
    <CurrentUserProvider>
      <Layout style={{ minHeight: '100vh' }}>
        <Header
          style={{
            background: '#fff',
            borderBottom: '1px solid #e5e1d8',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
          }}
        >
          <Title level={4} style={{ margin: 0 }}>
            سیستم مدیریت ارتباط با مشتری
          </Title>
          <CurrentUserPicker />
        </Header>
        <Layout>
          <Sider width={240} style={{ background: '#fff' }}>
            <Menu
              mode="inline"
              style={{ height: '100%', borderInlineEnd: '1px solid #e5e1d8' }}
              defaultOpenKeys={['user-group', 'basic-info', 'customer-affairs']}
              selectedKeys={[page]}
              items={menuItems}
              onClick={({ key }) => setPage(key as PageKey)}
            />
          </Sider>
          <Content style={{ padding: 20 }}>
            <ActivePage />
          </Content>
        </Layout>
      </Layout>
    </CurrentUserProvider>
  )
}

export default App
