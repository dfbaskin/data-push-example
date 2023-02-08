const formatter = Intl.DateTimeFormat(undefined, {
  dateStyle: 'short',
  timeStyle: 'short',
});

export function formatDateText(dateTxt?: string) {
  if (!dateTxt) {
    return '';
  }

  const dt = new Date(dateTxt);
  return dt instanceof Date && !isNaN(dt.valueOf()) ? formatter.format(dt) : '';
}
