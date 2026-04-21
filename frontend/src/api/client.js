export const API_BASE = '/api'

// Helper to get auth headers for API requests
export function authHeaders() {
  const token = localStorage.getItem('accessToken')
  return token ? { Authorization: `Bearer ${token}` } : {}
}

let refreshPromise = null
let sessionExpiredHandler = () => {}

export function setSessionExpiredHandler(fn) {
  sessionExpiredHandler = fn
}

// API Call for refreshing tokens. Returns true if successful, false otherwise.
async function refreshTokens() {
  const refreshToken = localStorage.getItem('refreshToken')
  if (!refreshToken) return false

  const res = await fetch(`${API_BASE}/Auth/refresh`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken }),
  })

  if (!res.ok) {
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
    sessionExpiredHandler()
    return false
  }

  const data = await res.json()
  localStorage.setItem('accessToken', data.accessToken)
  localStorage.setItem('refreshToken', data.refreshToken)
  return true
}

// API call for fetching data with token
export async function fetchWithAuth(url, options = {}) {
  const res = await fetch(url, {
    ...options,
    headers: { ...options.headers, ...authHeaders() },
  })

  if (res.status !== 401) return res

  if (!refreshPromise) {
    refreshPromise = refreshTokens().finally(() => { refreshPromise = null })
  }

  const refreshed = await refreshPromise
  if (!refreshed) return res

  return fetch(url, {
    ...options,
    headers: { ...options.headers, ...authHeaders() },
  })
}
