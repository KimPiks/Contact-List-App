import { useState } from 'react'
import { API_BASE, fetchWithAuth } from '../api/client'

export default function PasswordField({ contactId }) {
  const [password, setPassword] = useState(null)
  const [visible, setVisible] = useState(false)
  const [loading, setLoading] = useState(false)

  const reveal = async () => {
    setLoading(true)
    try {
      const res = await fetchWithAuth(`${API_BASE}/Contacts/${contactId}/password`)
      if (res.ok) {
        const data = await res.json()
        setPassword(data.password)
        setVisible(true)
      }
    } catch { }
    setLoading(false)
  }

  const hide = () => {
    setVisible(false)
    setPassword(null)
  }

  return (
    <button
      type="button"
      className="password"
      onClick={visible ? hide : reveal}
      title={visible ? 'Ukryj hasło' : 'Pokaż hasło'}
    >
      <code className={visible ? 'revealed' : 'masked'}>
        {loading ? '...' : visible ? password : '********'}
      </code>
    </button>
  )
}
