import { Layout, Typography } from 'antd'
import { CustomersPage } from './features/customers/CustomersPage'

const { Header, Content } = Layout
const { Title } = Typography

function App() {
  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Header style={{ background: '#fff', borderBottom: '1px solid #e5e1d8' }}>
        <Title level={4} style={{ margin: '14px 0' }}>
          سیستم مدیریت ارتباط با مشتری
        </Title>
      </Header>
      <Content style={{ padding: 20 }}>
        <CustomersPage />
      </Content>
    </Layout>
  )
}

export default App
