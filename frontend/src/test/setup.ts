import { afterAll, afterEach, beforeAll } from 'vitest'
import { cleanup } from '@testing-library/react'
import '@testing-library/jest-dom/vitest'
import { server } from './mocks/server'
import { resetMockCustomers } from './mocks/handlers'

// Without vitest's `globals: true`, Testing Library's own auto-cleanup can't
// find a global `afterEach` to hook into, so each render would otherwise
// stay mounted into the next test's document.
afterEach(() => cleanup())

beforeAll(() => server.listen({ onUnhandledRequest: 'error' }))
afterEach(() => {
  server.resetHandlers()
  resetMockCustomers()
})
afterAll(() => server.close())

// jsdom doesn't implement matchMedia; Ant Design's responsive grid hooks need it.
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: (query: string) => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: () => {},
    removeListener: () => {},
    addEventListener: () => {},
    removeEventListener: () => {},
    dispatchEvent: () => false,
  }),
})
