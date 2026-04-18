import type { PropsWithChildren } from 'react';

type PillProps = PropsWithChildren<{
  tone?: 'accent' | 'success' | 'warning' | 'danger' | 'muted';
}>;

export const Pill = ({ children, tone = 'muted' }: PillProps) => {
  return <span className={`pill pill--${tone}`}>{children}</span>;
};
