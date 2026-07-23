import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { CustomerCategoriesPage } from './CustomerCategoriesPage'

describe('CustomerCategoriesPage', () => {
  it('renders the customer categories list page', async () => {
    const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
    render(
      <QueryClientProvider client={queryClient}>
        <CustomerCategoriesPage />
      </QueryClientProvider>,
    )

    expect(await screen.findByText('تعداد دسته‌بندی‌ها')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'دسته‌بندی جدید' })).toBeInTheDocument()
  })
})
