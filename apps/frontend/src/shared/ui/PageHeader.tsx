import type { PropsWithChildren, ReactNode } from 'react';

type PageHeaderProps = PropsWithChildren<{
  eyebrow?: string;
  title: string;
  description?: string;
  actions?: ReactNode;
}>;

export const PageHeader = ({ eyebrow, title, description, actions, children }: PageHeaderProps) => {
  return (
    <section className="page-header">
      <div>
        {eyebrow ? <p className="page-header__eyebrow">{eyebrow}</p> : null}
        <h1>{title}</h1>
        {description ? <p>{description}</p> : null}
        {children}
      </div>
      {actions}
    </section>
  );
};
