import PasswordField from './PasswordField'

export default function ContactCard({
  contact,
  loggedIn,
  editing,
  deleting,
  categoryName,
  subcategoryName,
  onEdit,
  onDelete,
}) {
  return (
    <li className="card">
      <div className="info">
        <strong>{contact.firstName} {contact.lastName}</strong>
        <span>{contact.email}</span>
        <span>{contact.phoneNumber}</span>
        {contact.dateOfBirth && (
          <span>{contact.dateOfBirth.slice(0, 10)}</span>
        )}
        <span className="meta">
          {categoryName(contact.categoryId)}
          {contact.subcategoryName ? ` · ${contact.subcategoryName}` : ''}
        </span>
      </div>
      {loggedIn && <PasswordField contactId={contact.id} />}
      {loggedIn && !editing && (
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
    </li>
  )
}
