export function upsertById<T extends { id: string }>(items: T[], next: T): T[] {
  const exists = items.some((item) => item.id === next.id);
  if (!exists) {
    return [next, ...items];
  }

  return items.map((item) => (item.id === next.id ? next : item));
}
