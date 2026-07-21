import { Card, Col, Layout, Row, Statistic, Table, Tag, Typography } from 'antd'
import type { ColumnsType } from 'antd/es/table'

const { Header, Content } = Layout
const { Title } = Typography

interface CustomerRow {
  key: string
  name: string
  category: string
  lastContact: string
  contractStatus: 'active' | 'expiringSoon' | 'ended'
}

const sampleCustomers: CustomerRow[] = [
  {
    key: '1',
    name: 'شرکت فناوران البرز',
    category: 'حقوقی',
    lastContact: '۱۴۰۵/۰۴/۲۸',
    contractStatus: 'active',
  },
  {
    key: '2',
    name: 'گروه صنعتی پارسیان',
    category: 'حقوقی',
    lastContact: '۱۴۰۵/۰۴/۲۵',
    contractStatus: 'expiringSoon',
  },
  {
    key: '3',
    name: 'فروشگاه‌های زنجیره‌ای رفاه‌گستر',
    category: 'حقوقی',
    lastContact: '۱۴۰۵/۰۴/۲۰',
    contractStatus: 'active',
  },
  {
    key: '4',
    name: 'مهندس رضا کیانی',
    category: 'حقیقی',
    lastContact: '۱۴۰۵/۰۴/۱۸',
    contractStatus: 'ended',
  },
]

const statusLabel: Record<CustomerRow['contractStatus'], { text: string; color: string }> = {
  active: { text: 'فعال', color: 'success' },
  expiringSoon: { text: 'در حال اتمام', color: 'warning' },
  ended: { text: 'خاتمه‌یافته', color: 'default' },
}

const columns: ColumnsType<CustomerRow> = [
  { title: 'نام مشتری', dataIndex: 'name', key: 'name' },
  { title: 'دسته‌بندی', dataIndex: 'category', key: 'category', width: 110 },
  { title: 'آخرین تماس', dataIndex: 'lastContact', key: 'lastContact', width: 130 },
  {
    title: 'وضعیت قرارداد',
    dataIndex: 'contractStatus',
    key: 'contractStatus',
    width: 140,
    render: (status: CustomerRow['contractStatus']) => (
      <Tag color={statusLabel[status].color}>{statusLabel[status].text}</Tag>
    ),
  },
]

function App() {
  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Header style={{ background: '#fff', borderBottom: '1px solid #e5e1d8' }}>
        <Title level={4} style={{ margin: '14px 0' }}>
          سیستم مدیریت ارتباط با مشتری
        </Title>
      </Header>
      <Content style={{ padding: 20 }}>
        <Row gutter={12} style={{ marginBottom: 16 }}>
          <Col span={6}>
            <Card size="small">
              <Statistic title="مشتریان فعال" value={128} />
            </Card>
          </Col>
          <Col span={6}>
            <Card size="small">
              <Statistic
                title="قراردادهای در حال اتمام"
                value={5}
                styles={{ content: { color: '#b6742a' } }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card size="small">
              <Statistic title="کارهای امروز" value={12} />
            </Card>
          </Col>
        </Row>

        <Card title="مشتریان" size="small">
          <Table
            size="small"
            columns={columns}
            dataSource={sampleCustomers}
            pagination={false}
          />
        </Card>
      </Content>
    </Layout>
  )
}

export default App
