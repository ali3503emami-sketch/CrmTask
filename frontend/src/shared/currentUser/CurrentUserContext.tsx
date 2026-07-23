import { createContext, useContext, useState, type ReactNode } from 'react'

const STORAGE_KEY = 'crmtask.currentStaffId'

interface CurrentUserContextValue {
  currentStaffId: string | null
  setCurrentStaffId: (staffId: string | null) => void
}

const CurrentUserContext = createContext<CurrentUserContextValue | null>(null)

/**
 * There is deliberately no real login yet (see CLAUDE.md) — this is a
 * temporary "who are you?" simulation so the Dashboard can filter tasks by
 * assignee/creator. Kept in sessionStorage (not localStorage) so it resets
 * when the browser tab/session ends, not persisted like a real session would be.
 */
export function CurrentUserProvider({ children }: { children: ReactNode }) {
  const [currentStaffId, setCurrentStaffIdState] = useState<string | null>(() =>
    sessionStorage.getItem(STORAGE_KEY),
  )

  const setCurrentStaffId = (staffId: string | null) => {
    setCurrentStaffIdState(staffId)
    if (staffId) {
      sessionStorage.setItem(STORAGE_KEY, staffId)
    } else {
      sessionStorage.removeItem(STORAGE_KEY)
    }
  }

  return (
    <CurrentUserContext.Provider value={{ currentStaffId, setCurrentStaffId }}>
      {children}
    </CurrentUserContext.Provider>
  )
}

export function useCurrentUser(): CurrentUserContextValue {
  const context = useContext(CurrentUserContext)
  if (!context) {
    throw new Error('useCurrentUser must be used within a CurrentUserProvider')
  }
  return context
}
