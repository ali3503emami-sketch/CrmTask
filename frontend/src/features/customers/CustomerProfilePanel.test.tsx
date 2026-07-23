import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { CustomerProfilePanel } from './CustomerProfilePanel'
import type { Customer } from './types'
import { sampleCustomers, samplePositions } from '../../test/mocks/handlers'

const customer: Customer = {
  id: '11111111-1111-1111-1111-111111111111',
  name: 'شرکت فناوران البرز',
  category: 'Legal',
  phone: '02112345678',
  createdAt: '2026-07-01T00:00:00Z',
  createdAtShamsi: '1405/04/10',
  managerName: null,
  managerBirthDate: null,
  managerBirthDateShamsi: null,
  address: null,
  fax: null,
  notes: null,
  nationalId: null,
  categoryTitle: null,
  activityField: null,
  personnel: [],
}

function renderPanel(customerOverride: Customer = customer) {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <CustomerProfilePanel customer={customerOverride} open onClose={() => {}} />
    </QueryClientProvider>,
  )
}

describe('CustomerProfilePanel', () => {
  it('pre-fills the form with the customer core fields', () => {
    renderPanel()

    expect(screen.getByLabelText('نام مشتری')).toHaveValue('شرکت فناوران البرز')
    expect(screen.getByLabelText('تلفن')).toHaveValue('02112345678')
  })

  it('saves profile fields and a new personnel entry', async () => {
    samplePositions.push({ id: 'p1', title: 'مسئول دفتر' })
    renderPanel()
    const user = userEvent.setup()

    await user.type(screen.getByLabelText('نام مدیرعامل'), 'مهندس رضا کیانی')
    await user.type(screen.getByLabelText('شماره ملی / شناسه ملی'), '1234567890')
    await user.click(screen.getByRole('button', { name: 'افزودن پرسنل' }))
    await user.type(screen.getByLabelText('نام پرسنل 1'), 'سارا محمدی')
    await user.click(screen.getByLabelText('سمت'))
    const positionOptions = await screen.findAllByText('مسئول دفتر')
    await user.click(positionOptions[positionOptions.length - 1])

    await user.click(screen.getByRole('button', { name: 'ذخیره تغییرات' }))

    await waitFor(() => {
      expect(sampleCustomers[0].managerName).toBe('مهندس رضا کیانی')
    })
    expect(sampleCustomers[0].nationalId).toBe('1234567890')
    expect(sampleCustomers[0].personnel).toHaveLength(1)
    expect(sampleCustomers[0].personnel[0]).toMatchObject({ fullName: 'سارا محمدی', position: 'مسئول دفتر' })
  })
})
