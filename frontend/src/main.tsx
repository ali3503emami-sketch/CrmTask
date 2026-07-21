import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { ConfigProvider } from 'antd'
import faIR from 'antd/locale/fa_IR'
import '@fontsource-variable/vazirmatn'
import './index.css'
import App from './App.tsx'
import { crmTheme } from './theme.ts'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ConfigProvider direction="rtl" locale={faIR} theme={crmTheme}>
      <App />
    </ConfigProvider>
  </StrictMode>,
)
