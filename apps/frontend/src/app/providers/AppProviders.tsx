import type { PropsWithChildren } from 'react';
import { AuthProvider } from './AuthProvider';

export const AppProviders = ({ children }: PropsWithChildren) => {
  return <AuthProvider>{children}</AuthProvider>;
};
