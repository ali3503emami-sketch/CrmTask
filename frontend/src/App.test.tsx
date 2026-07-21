import { describe, expect, it } from 'vitest'
import { render, screen } from '@testing-library/react'
import App from './App'

describe('App', () => {
  it('shows the CRM page title', () => {
    render(<App />)

    expect(
      screen.getByRole('heading', { name: 'سیستم مدیریت ارتباط با مشتری' }),
    ).toBeInTheDocument()
  })

  it('lists customers with their contract status', () => {
    render(<App />)

    expect(screen.getAllByText('شرکت فناوران البرز').length).toBeGreaterThan(0)
    expect(screen.getAllByText('در حال اتمام').length).toBeGreaterThan(0)
  })
})
