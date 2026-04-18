import axios from 'axios';

type ApiErrorPayload = {
  detail?: string;
  message?: string;
  title?: string;
  errors?: Record<string, string[]>;
};

export const getErrorMessage = (error: unknown, fallback = 'Что-то пошло не так') => {
  if (axios.isAxiosError<ApiErrorPayload>(error)) {
    const payload = error.response?.data;

    if (payload?.errors) {
      const firstError = Object.values(payload.errors)[0]?.[0];
      if (firstError) {
        return firstError;
      }
    }

    return payload?.detail ?? payload?.message ?? payload?.title ?? fallback;
  }

  if (error instanceof Error) {
    return error.message;
  }

  return fallback;
};
