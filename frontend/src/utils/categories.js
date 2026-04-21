const CATEGORY_LABELS = {
  Business: 'Służbowy',
  Private: 'Prywatny',
  Custom: 'Inny',
}

export function translateCategory(name) {
  return CATEGORY_LABELS[name] || name
}

export function isBusinessCategory(categoryId, categories) {
  return categories.find((c) => c.id === Number(categoryId))?.name === 'Business'
}

export function isCustomCategory(categoryId, categories) {
  return categories.find((c) => c.id === Number(categoryId))?.name === 'Custom'
}
