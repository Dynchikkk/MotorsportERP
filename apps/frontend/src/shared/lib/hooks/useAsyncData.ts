import {
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
  type DependencyList,
  type Dispatch,
  type SetStateAction,
} from 'react';
import { getErrorMessage } from '@/shared/lib/errors';

type AsyncState<T> = {
  data: T | null;
  isLoading: boolean;
  error: string | null;
  reload: () => Promise<void>;
  setData: Dispatch<SetStateAction<T | null>>;
};

export const useAsyncData = <T,>(
  loader: () => Promise<T>,
  dependencies: DependencyList,
  options?: {
    immediate?: boolean;
    initialData?: T | null;
  },
): AsyncState<T> => {
  const [data, setData] = useState<T | null>(options?.initialData ?? null);
  const [isLoading, setIsLoading] = useState<boolean>(options?.immediate ?? true);
  const [error, setError] = useState<string | null>(null);
  const loaderRef = useRef(loader);
  const immediate = options?.immediate ?? true;

  useEffect(() => {
    loaderRef.current = loader;
  }, [loader]);

  const load = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const response = await loaderRef.current();
      setData(response);
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    if (!immediate) {
      setIsLoading(false);
      return;
    }

    void load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [immediate, load, ...dependencies]);

  return useMemo(
    () => ({
      data,
      isLoading,
      error,
      reload: load,
      setData,
    }),
    [data, error, isLoading, load, setData],
  );
};
