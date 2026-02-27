import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { api } from '@/services/api'
import type { User } from '@/types'

const TOKEN_KEY = 'token'
const USER_KEY = 'user'

function getStoredToken(): string | null {
  return localStorage.getItem(TOKEN_KEY)
}

function getStoredUser(): User | null {
  const raw = localStorage.getItem(USER_KEY)
  if (!raw) return null
  try {
    return JSON.parse(raw) as User
  } catch {
    return null
  }
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(getStoredToken())
  const user = ref<User | null>(getStoredUser())
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => !!token.value)
  const isAdmin = computed(() => user.value?.role === 'admin')

  function setAuth(newToken: string, newUser: User) {
    token.value = newToken
    user.value = newUser
    localStorage.setItem(TOKEN_KEY, newToken)
    localStorage.setItem(USER_KEY, JSON.stringify(newUser))
    error.value = null
  }

  function clearAuth() {
    token.value = null
    user.value = null
    error.value = null
    localStorage.removeItem(TOKEN_KEY)
    localStorage.removeItem(USER_KEY)
  }

  function clearError() {
    error.value = null
  }

  async function login(email: string, password: string) {
    error.value = null
    try {
      const { data } = await api.post<{ token: string; user: User }>('/api/auth/login', {
        email,
        password,
      })
      setAuth(data.token, data.user)
      return true
    } catch (err: unknown) {
      const msg = err && typeof err === 'object' && 'response' in err
        ? (err as { response?: { data?: { message?: string } } }).response?.data?.message
        : null
      error.value = msg || 'Error al iniciar sesión. Revisa el email y la contraseña.'
      return false
    }
  }

  async function register(email: string, password: string, name: string) {
    error.value = null
    try {
      const { data } = await api.post<{ token: string; user: User }>('/api/auth/register', {
        email,
        password,
        name,
      })
      setAuth(data.token, data.user)
      return true
    } catch (err: unknown) {
      const msg = err && typeof err === 'object' && 'response' in err
        ? (err as { response?: { data?: { message?: string } } }).response?.data?.message
        : null
      error.value = msg || 'Error al registrarse. El email puede estar ya en uso.'
      return false
    }
  }

  function logout() {
    clearAuth()
  }

  async function fetchMe() {
    if (!token.value) return
    try {
      const { data } = await api.get<User>('/api/auth/me')
      user.value = data
      localStorage.setItem(USER_KEY, JSON.stringify(data))
    } catch {
      clearAuth()
    }
  }

  return {
    token,
    user,
    error,
    isAuthenticated,
    isAdmin,
    login,
    register,
    logout,
    fetchMe,
    setAuth,
    clearAuth,
    clearError,
  }
})
