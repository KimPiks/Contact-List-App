import { useState } from 'react'
import { API_BASE } from '../api/client'

export default function AuthForm({ onLogin }) {
  const [isRegister, setIsRegister] = useState(false)
  const [username, setUsername] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const switchMode = () => {
    setIsRegister(!isRegister)
    setError('')
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setLoading(true)

    try {
      const endpoint = isRegister ? 'register' : 'login'
      const body = isRegister
        ? { username, email, password }
        : { email, password }

      const res = await fetch(`${API_BASE}/Auth/${endpoint}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
      })

      const data = await res.json()

      if (!res.ok || !data.success) {
        setError(data.error || (isRegister ? 'Nie udało się zarejestrować' : 'Nieprawidłowy email lub hasło'))
        return
      }

      localStorage.setItem('accessToken', data.accessToken)
      localStorage.setItem('refreshToken', data.refreshToken)
      onLogin()
    } catch {
      setError('Nie udało się połączyć z serwerem')
    } finally {
      setLoading(false)
    }
  }

  return (
    <section className="auth">
      <h1>{isRegister ? 'Rejestracja' : 'Logowanie'}</h1>
      <form className="form" onSubmit={handleSubmit}>
        {isRegister && (
          <label>
            <span>Nazwa użytkownika</span>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              autoComplete="username"
            />
          </label>
        )}
        <label>
          <span>Email</span>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            autoComplete="email"
          />
        </label>
        <label>
          <span>Hasło</span>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            autoComplete={isRegister ? 'new-password' : 'current-password'}
          />
        </label>
        {error && <p className="error">{error}</p>}
        <button type="submit" disabled={loading}>
          {loading
            ? (isRegister ? 'Rejestracja...' : 'Logowanie...')
            : (isRegister ? 'Zarejestruj się' : 'Zaloguj się')}
        </button>
      </form>
      <p className="switch">
        {isRegister ? 'Masz już konto?' : 'Nie masz konta?'}{' '}
        <button type="button" className="link" onClick={switchMode}>
          {isRegister ? 'Zaloguj się' : 'Zarejestruj się'}
        </button>
      </p>
    </section>
  )
}
