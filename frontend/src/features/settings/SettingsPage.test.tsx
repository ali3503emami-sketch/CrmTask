import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it } from 'vitest'
import { SettingsPage } from './SettingsPage'
import { sampleSettings } from '../../test/mocks/handlers'

function renderPage() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={queryClient}>
      <SettingsPage />
    </QueryClientProvider>,
  )
}

describe('SettingsPage', () => {
  it('shows the current settings values', async () => {
    renderPage()

    expect(await screen.findByDisplayValue('3')).toBeInTheDocument()
    expect(screen.getByDisplayValue('30')).toBeInTheDocument()
  })

  it('saves changed values', async () => {
    renderPage()
    const user = userEvent.setup()
    const taskWindowInput = await screen.findByLabelText(/کارهای/)

    await user.clear(taskWindowInput)
    await user.type(taskWindowInput, '5')
    await user.click(screen.getByRole('button', { name: 'ذخیره' }))

    await waitFor(() => {
      expect(sampleSettings.taskUpcomingWindowDays).toBe(5)
    })
  })
})
