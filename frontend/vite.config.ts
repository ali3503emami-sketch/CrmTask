/// <reference types="vitest/config" />
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  test: {
    environment: 'jsdom',
    setupFiles: ['./src/test/setup.ts'],
    // Multi-step form interactions (type several fields, open a dropdown, wait
    // for a mutation round-trip) can exceed the 5s default under load when the
    // full suite runs together — bump it rather than let CI be flaky.
    testTimeout: 15000,
  },
})
