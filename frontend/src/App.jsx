import { useState, useEffect } from 'react'
import { setSessionExpiredHandler } from './api/client'
import AuthForm from './components/AuthForm'
import ContactsPage from './components/ContactsPage'
import './App.css'

export default function App() {
  const [loggedIn, setLoggedIn] = useState(() => !!localStorage.getItem('accessToken'))
  const [showAuth, setShowAuth] = useState(false)

  useEffect(() => {
    setSessionExpiredHandler(() => setLoggedIn(false))
    return () => setSessionExpiredHandler(() => {})
  }, [])

  if (showAuth && !loggedIn) {
    return (
      <main className="app">
        <AuthForm onLogin={() => { setLoggedIn(true); setShowAuth(false) }} />
      </main>
    )
  }

  return (
    <main className="app">
      <ContactsPage
        loggedIn={loggedIn}
        onLogout={() => setLoggedIn(false)}
        onLoginClick={() => setShowAuth(true)}
      />
    </main>
  )
}
