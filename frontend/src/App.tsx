import { useState } from 'react'
import { Layout, Tabs, Typography } from 'antd'
import { CustomersPage } from './features/customers/CustomersPage'
import { TasksPage } from './features/tasks/TasksPage'
import { StaffPage } from './features/staff/StaffPage'

const { Header, Content } = Layout
const { Title } = Typography

type Page = 'customers' | 'tasks' | 'staff'

const pageComponents: Record<Page, () => React.JSX.Element> = {
  customers: CustomersPage,
  tasks: TasksPage,
  staff: StaffPage,
}

function App() {
  const [page, setPage] = useState<Page>('customers')
  const ActivePage = pageComponents[page]

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Header style={{ background: '#fff', borderBottom: '1px solid #e5e1d8', paddingTop: 8 }}>
        <Title level={4} style={{ margin: '6px 0' }}>
          سیستم مدیریت ارتباط با مشتری
        </Title>
        <Tabs
          activeKey={page}
          onChange={(key) => setPage(key as Page)}
          items={[
            { key: 'customers', label: 'مشتریان' },
            { key: 'tasks', label: 'کارهای جاری' },
            { key: 'staff', label: 'پرسنل' },
          ]}
        />
      </Header>
      <Content style={{ padding: 20 }}>
        <ActivePage />
      </Content>
    </Layout>
  )
}

export default App
