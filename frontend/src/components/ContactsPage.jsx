import { useState, useEffect, useCallback } from 'react'
import { API_BASE, fetchWithAuth } from '../api/client'
import { translateCategory } from '../utils/categories'
import ContactForm from './ContactForm'
import ContactCard from './ContactCard'

export default function ContactsPage({ loggedIn, onLogout, onLoginClick }) {
  const [contacts, setContacts] = useState([])
  const [categories, setCategories] = useState([])
  const [subcategories, setSubcategories] = useState([])
  const [loading, setLoading] = useState(true)
  const [editing, setEditing] = useState(null)
  const [deleting, setDeleting] = useState(null)

  // Fetch contacts from API.
  const fetchContacts = useCallback(async () => {
    try {
      const res = await fetch(`${API_BASE}/Contacts`, { cache: 'no-store' })
      if (res.ok) setContacts(await res.json())
    } catch {  }
  }, [])

  // Fetch categories and subcategories
  useEffect(() => {
    const load = async () => {
      await fetchContacts()
      try {
        const [catRes, subRes] = await Promise.all([
          fetch(`${API_BASE}/Contacts/categories`),
          fetch(`${API_BASE}/Contacts/subcategories`),
        ])
        if (catRes.ok) setCategories(await catRes.json())
        if (subRes.ok) setSubcategories(await subRes.json())
      } catch { /* ignore */ }
      setLoading(false)
    }
    load()
  }, [fetchContacts])

  // Delete contact
  const handleDelete = async (id) => {
    setDeleting(id)
    try {
      await fetchWithAuth(`${API_BASE}/Contacts/${id}`, { method: 'DELETE' })
      await fetchContacts()
    } catch { }
    setDeleting(null)
  }

  // Save contact 
  const handleSave = async () => {
    setEditing(null)
    await fetchContacts()
  }

  // Logout user
  const handleLogout = async () => {
    const refreshToken = localStorage.getItem('refreshToken')
    const accessToken = localStorage.getItem('accessToken')
    try {
      await fetch(`${API_BASE}/Auth/logout`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${accessToken}`,
        },
        body: JSON.stringify({ refreshToken }),
      })
    } catch {  }
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
    onLogout()
  }

  const categoryName = (id) => {
    const cat = categories.find((c) => c.id === id)
    return cat ? translateCategory(cat.name) : ''
  }

  if (loading) return <section className="page"><p className="muted">Ładowanie...</p></section>

  return (
    <section className="page">
      <div className="page-head">
        <h1>Kontakty</h1>
        <div className="head-actions">
          {loggedIn ? (
            <>
              {!editing && <button onClick={() => setEditing('new')}>Dodaj</button>}
              <button className="ghost" onClick={handleLogout}>Wyloguj</button>
            </>
          ) : (
            <button onClick={onLoginClick}>Zaloguj się</button>
          )}
        </div>
      </div>

      {editing && (
        <div className="panel">
          <h2>{editing === 'new' ? 'Nowy kontakt' : 'Edytuj kontakt'}</h2>
          <ContactForm
            contact={editing === 'new' ? null : editing}
            categories={categories}
            subcategories={subcategories}
            onSave={handleSave}
            onCancel={() => setEditing(null)}
          />
        </div>
      )}

      {contacts.length === 0 ? (
        <p className="muted">Brak kontaktów</p>
      ) : (
        <ul className="list">
          {contacts.map((c) => (
            <ContactCard
              key={c.id}
              contact={c}
              loggedIn={loggedIn}
              editing={editing}
              deleting={deleting}
              categoryName={categoryName}
              onEdit={setEditing}
              onDelete={handleDelete}
            />
          ))}
        </ul>
      )}
    </section>
  )
}
