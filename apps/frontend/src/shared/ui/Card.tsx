import type { HTMLAttributes, PropsWithChildren } from 'react';

type CardProps = PropsWithChildren<
  HTMLAttributes<HTMLDivElement> & {
    soft?: boolean;
  }
>;

export const Card = ({ children, className = '', soft = false, ...props }: CardProps) => {
  return (
    <div className={`card${soft ? ' card--soft' : ''}${className ? ` ${className}` : ''}`} {...props}>
      {children}
    </div>
  );
};
