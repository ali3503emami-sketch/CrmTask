import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { PositionsPage } from './PositionsPage'

describe('PositionsPage', () => {
  it('renders the positions list page', async () => {
    const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
    render(
      <QueryClientProvider client={queryClient}>
        <PositionsPage />
      </QueryClientProvider>,
    )

    expect(await screen.findByText('تعداد سمت‌ها')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'سمت جدید' })).toBeInTheDocument()
  })
})
