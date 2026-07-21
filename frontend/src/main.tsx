import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { ConfigProvider } from 'antd'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import faIR from 'antd/locale/fa_IR'
import '@fontsource-variable/vazirmatn'
import './index.css'
import App from './App.tsx'
import { crmTheme } from './theme.ts'

const queryClient = new QueryClient()

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <ConfigProvider direction="rtl" locale={faIR} theme={crmTheme}>
        <App />
      </ConfigProvider>
    </QueryClientProvider>
  </StrictMode>,
)
