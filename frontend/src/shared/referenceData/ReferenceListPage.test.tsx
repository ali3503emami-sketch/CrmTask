import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen, waitFor, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { ReferenceListPage } from './ReferenceListPage'
import { samplePositions } from '../../test/mocks/handlers'

function renderPage() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <ReferenceListPage
        queryKey="positions"
        basePath="/api/positions"
        pageTitle="سمت‌ها"
        countLabel="تعداد سمت‌ها"
        columnLabel="عنوان سمت"
        createButtonLabel="سمت جدید"
        modalTitle="سمت جدید"
        fieldLabel="عنوان"
      />
    </QueryClientProvider>,
  )
}

describe('ReferenceListPage', () => {
  it('shows the empty count before any item exists', async () => {
    renderPage()

    expect(await screen.findByText('تعداد سمت‌ها')).toBeInTheDocument()
    expect(screen.getByText('0')).toBeInTheDocument()
  })

  it('creates a new item through the form and shows it in the list', async () => {
    renderPage()
    await screen.findByText('تعداد سمت‌ها')
    const user = userEvent.setup()

    await user.click(screen.getByRole('button', { name: 'سمت جدید' }))
    const dialog = await screen.findByRole('dialog')

    await user.type(within(dialog).getByLabelText('عنوان'), 'مسئول دفتر')
    await user.click(within(dialog).getByRole('button', { name: 'ثبت' }))

    await waitFor(() => {
      expect(samplePositions).toHaveLength(1)
    })
    expect(await screen.findAllByText('مسئول دفتر')).not.toHaveLength(0)
  })
})
