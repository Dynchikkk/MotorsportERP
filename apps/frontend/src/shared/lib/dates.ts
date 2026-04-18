export const toDateTimeInputValue = (value?: string | null) => {
  if (!value) {
    return '';
  }

  const date = new Date(value);
  const offset = date.getTimezoneOffset();
  const normalized = new Date(date.getTime() - offset * 60_000);

  return normalized.toISOString().slice(0, 16);
};

export const toIsoDateTime = (value: string) => new Date(value).toISOString();
