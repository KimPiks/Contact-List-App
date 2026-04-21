import { useState } from 'react'
import { API_BASE, fetchWithAuth } from '../api/client'
import { translateCategory, isBusinessCategory, isCustomCategory } from '../utils/categories'

export default function ContactForm({ contact, categories, subcategories, onSave, onCancel }) {
  const [form, setForm] = useState({
    firstName: contact?.firstName || '',
    lastName: contact?.lastName || '',
    email: contact?.email || '',
    password: '',
    phoneNumber: contact?.phoneNumber || '',
    dateOfBirth: contact?.dateOfBirth ? contact.dateOfBirth.slice(0, 10) : '',
    categoryId: contact?.categoryId || '',
    subcategoryId: contact?.subcategoryId || '',
    customSubcategory: '',
  })
  const [error, setError] = useState('')
  const [saving, setSaving] = useState(false)

  const filteredSubs = subcategories.filter(
    (s) => s.categoryId === Number(form.categoryId)
  )
  const businessSelected = isBusinessCategory(form.categoryId, categories)
  const customSelected = isCustomCategory(form.categoryId, categories)

  const handleChange = (e) => {
    const { name, value } = e.target
    setForm((prev) => {
      const next = { ...prev, [name]: value }
      if (name === 'categoryId') {
        next.subcategoryId = ''
        next.customSubcategory = ''
      }
      return next
    })
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setSaving(true)

    const body = {
      firstName: form.firstName,
      lastName: form.lastName,
      email: form.email,
      password: form.password,
      phoneNumber: form.phoneNumber,
      dateOfBirth: form.dateOfBirth,
      categoryId: Number(form.categoryId),
      subcategoryId: form.subcategoryId ? Number(form.subcategoryId) : null,
      customSubcategory: customSelected ? form.customSubcategory : null,
    }

    try {
      const isEdit = !!contact
      const url = isEdit
        ? `${API_BASE}/Contacts/${contact.id}`
        : `${API_BASE}/Contacts`

      const res = await fetchWithAuth(url, {
        method: isEdit ? 'PUT' : 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
      })

      if (!res.ok) {
        const text = await res.text()
        setError(text || 'Nie udało się zapisać kontaktu')
        return
      }

      onSave()
    } catch {
      setError('Nie udało się połączyć z serwerem')
    } finally {
      setSaving(false)
    }
  }

  return (
    <form className="form" onSubmit={handleSubmit}>
      <label>
        <span>Imię</span>
        <input name="firstName" value={form.firstName} onChange={handleChange} required />
      </label>
      <label>
        <span>Nazwisko</span>
        <input name="lastName" value={form.lastName} onChange={handleChange} required />
      </label>
      <label>
        <span>Email</span>
        <input name="email" type="email" value={form.email} onChange={handleChange} required />
      </label>
      <label>
        <span>Hasło</span>
        <input name="password" type="password" value={form.password} onChange={handleChange} required autoComplete="new-password" />
        <small>Min. 8 znaków, wielka i mała litera, cyfra, znak specjalny</small>
      </label>
      <label>
        <span>Telefon</span>
        <input name="phoneNumber" value={form.phoneNumber} onChange={handleChange} required />
      </label>
      <label>
        <span>Data urodzenia</span>
        <input name="dateOfBirth" type="date" value={form.dateOfBirth} onChange={handleChange} required />
      </label>
      <label>
        <span>Kategoria</span>
        <select name="categoryId" value={form.categoryId} onChange={handleChange} required>
          <option value="">wybierz</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>{translateCategory(c.name)}</option>
          ))}
        </select>
      </label>
      {!customSelected && filteredSubs.length > 0 && (
        <label>
          <span>Podkategoria{businessSelected ? ' *' : ''}</span>
          <select name="subcategoryId" value={form.subcategoryId} onChange={handleChange} required={businessSelected}>
            <option value="">{businessSelected ? 'wybierz' : 'brak'}</option>
            {filteredSubs.map((s) => (
              <option key={s.id} value={s.id}>{s.name}</option>
            ))}
          </select>
        </label>
      )}
      {customSelected && (
        <label>
          <span>Własna podkategoria</span>
          <input
            name="customSubcategory"
            value={form.customSubcategory}
            onChange={handleChange}
            placeholder="Wpisz nazwę podkategorii"
          />
        </label>
      )}
      {error && <p className="error">{error}</p>}
      <div className="actions">
        <button type="submit" disabled={saving}>
          {saving ? 'Zapisywanie...' : (contact ? 'Zapisz' : 'Dodaj')}
        </button>
        <button type="button" className="ghost" onClick={onCancel}>Anuluj</button>
      </div>
    </form>
  )
}
