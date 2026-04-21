import PasswordField from './PasswordField'

export default function ContactCard({
  contact,
  loggedIn,
  editing,
  deleting,
  selected,
  categoryName,
  onSelect,
  onEdit,
  onDelete,
}) {
  const toggle = () => onSelect(selected ? null : contact.id)

  return (
    <li className={`card ${selected ? 'selected' : ''}`}>
      <button type="button" className="summary" onClick={toggle} aria-expanded={selected}>
        <span className="summary-name">
          <strong>{contact.firstName} {contact.lastName}</strong>
          <span className="meta">{categoryName(contact.categoryId)}</span>
        </span>
        <span className="chevron" aria-hidden="true">{selected ? '▾' : '▸'}</span>
      </button>

      {selected && (
        <div className="details">
          <div className="info">
            <span><strong>Email:</strong> {contact.email}</span>
            <span><strong>Telefon:</strong> {contact.phoneNumber}</span>
            {contact.dateOfBirth && (
              <span><strong>Data urodzenia:</strong> {contact.dateOfBirth.slice(0, 10)}</span>
            )}
            <span>
              <strong>Kategoria:</strong> {categoryName(contact.categoryId)}
              {contact.subcategoryName ? ` · ${contact.subcategoryName}` : ''}
            </span>
          </div>
          {loggedIn && (
            <div className="details-actions">
              <PasswordField contactId={contact.id} />
              {!editing && (
                <div className="row-actions">
                  <button className="small" onClick={() => onEdit(contact)}>Edytuj</button>
                  <button
                    className="small danger"
                    disabled={deleting === contact.id}
                    onClick={() => onDelete(contact.id)}
                  >
                    {deleting === contact.id ? '...' : 'Usuń'}
                  </button>
                </div>
              )}
            </div>
          )}
        </div>
      )}
    </li>
  )
}
