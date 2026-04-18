import type { ButtonHTMLAttributes, PropsWithChildren } from 'react';

type ButtonProps = PropsWithChildren<
  ButtonHTMLAttributes<HTMLButtonElement> & {
    variant?: 'primary' | 'secondary' | 'ghost' | 'danger';
  }
>;

export const Button = ({
  children,
  className = '',
  type = 'button',
  variant = 'secondary',
  ...props
}: ButtonProps) => {
  return (
    <button
      type={type}
      className={`button button--${variant}${className ? ` ${className}` : ''}`}
      {...props}
    >
      {children}
    </button>
  );
};
