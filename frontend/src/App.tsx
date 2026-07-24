import { useState } from 'react'
import { Button, Drawer, Layout, Typography } from 'antd'
import { MenuOutlined } from '@ant-design/icons'
import { CustomersPage } from './features/customers/CustomersPage'
import { TasksPage } from './features/tasks/TasksPage'
import { StaffPage } from './features/staff/StaffPage'
import { PositionsPage } from './features/positions/PositionsPage'
import { CustomerCategoriesPage } from './features/customerCategories/CustomerCategoriesPage'
import { ActivityFieldsPage } from './features/activityFields/ActivityFieldsPage'
import { DashboardPage } from './features/dashboard/DashboardPage'
import { CurrentUserProvider } from './shared/currentUser/CurrentUserContext'
import { CurrentUserPicker } from './shared/currentUser/CurrentUserPicker'
import { AppMenu } from './shared/navigation/AppMenu'
import type { PageKey } from './shared/navigation/menuItems'

const { Header, Content, Sider } = Layout
const { Title } = Typography

const pageComponents: Record<PageKey, () => React.JSX.Element> = {
  dashboard: DashboardPage,
  staff: StaffPage,
  positions: PositionsPage,
  customerCategories: CustomerCategoriesPage,
  activityFields: ActivityFieldsPage,
  customers: CustomersPage,
  tasks: TasksPage,
}

function App() {
  const [page, setPage] = useState<PageKey>('dashboard')
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false)
  const ActivePage = pageComponents[page]

  const handleSelectPage = (key: PageKey) => {
    setPage(key)
    setIsMobileMenuOpen(false)
  }

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
            gap: 12,
          }}
        >
          <Button
            className="mobile-menu-trigger"
            icon={<MenuOutlined />}
            onClick={() => setIsMobileMenuOpen(true)}
            aria-label="باز کردن منو"
          />
          <Title
            level={4}
            style={{ margin: 0, flex: 1, minWidth: 0, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}
          >
            سیستم مدیریت ارتباط با مشتری
          </Title>
          <CurrentUserPicker />
        </Header>
        <Layout>
          <Sider className="desktop-sider" width={240} style={{ background: '#fff', borderInlineEnd: '1px solid #e5e1d8' }}>
            <AppMenu selectedKey={page} onSelect={handleSelectPage} />
          </Sider>
          <Drawer
            title="منو"
            placement="right"
            open={isMobileMenuOpen}
            onClose={() => setIsMobileMenuOpen(false)}
            styles={{ body: { padding: 0 } }}
            size="default"
          >
            <AppMenu selectedKey={page} onSelect={handleSelectPage} />
          </Drawer>
          <Content style={{ padding: 20 }}>
            <ActivePage />
          </Content>
        </Layout>
      </Layout>
    </CurrentUserProvider>
  )
}

export default App
