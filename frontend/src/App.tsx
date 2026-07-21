import { useState } from 'react'
import { Layout, Tabs, Typography } from 'antd'
import { CustomersPage } from './features/customers/CustomersPage'
import { TasksPage } from './features/tasks/TasksPage'

const { Header, Content } = Layout
const { Title } = Typography

type Page = 'customers' | 'tasks'

function App() {
  const [page, setPage] = useState<Page>('customers')

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
          ]}
        />
      </Header>
      <Content style={{ padding: 20 }}>
        {page === 'customers' ? <CustomersPage /> : <TasksPage />}
      </Content>
    </Layout>
  )
}

export default App
