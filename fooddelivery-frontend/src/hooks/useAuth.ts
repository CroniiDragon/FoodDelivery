import { useState, useEffect } from 'react'
import type { UserResponse } from '../types'

const STORAGE_KEY = 'fd_user'

export function useAuth() {
  const [user, setUser] = useState<UserResponse | null>(() => {
    try {
      const stored = localStorage.getItem(STORAGE_KEY)
      return stored ? JSON.parse(stored) : null
    } catch { return null }
  })

  const login = (u: UserResponse) => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(u))
    setUser(u)
  }

  const logout = () => {
    localStorage.removeItem(STORAGE_KEY)
    setUser(null)
  }

  return { user, login, logout, isLoggedIn: !!user }
}
