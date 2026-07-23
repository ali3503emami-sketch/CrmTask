import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { ActivityFieldsPage } from './ActivityFieldsPage'

describe('ActivityFieldsPage', () => {
  it('renders the activity fields list page', async () => {
    const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
    render(
      <QueryClientProvider client={queryClient}>
        <ActivityFieldsPage />
      </QueryClientProvider>,
    )

    expect(await screen.findByText('تعداد زمینه‌های فعالیت')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'زمینه فعالیت جدید' })).toBeInTheDocument()
  })
})
