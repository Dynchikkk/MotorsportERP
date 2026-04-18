import type { InputHTMLAttributes, ReactNode, SelectHTMLAttributes, TextareaHTMLAttributes } from 'react';

type BaseFieldProps = {
  label: string;
  hint?: string;
  error?: string | null;
  actions?: ReactNode;
};

type InputFieldProps = BaseFieldProps & InputHTMLAttributes<HTMLInputElement>;
type SelectFieldProps = BaseFieldProps &
  SelectHTMLAttributes<HTMLSelectElement> & {
    children: ReactNode;
  };
type TextareaFieldProps = BaseFieldProps & TextareaHTMLAttributes<HTMLTextAreaElement>;

const FieldShell = ({
  label,
  hint,
  error,
  actions,
  children,
}: BaseFieldProps & { children: ReactNode }) => {
  return (
    <label className="field">
      <span className="field__label">
        <span>{label}</span>
        {actions}
      </span>
      {children}
      {error ? <span className="field__error">{error}</span> : hint ? <span className="field__hint">{hint}</span> : null}
    </label>
  );
};

export const InputField = ({ label, hint, error, actions, className = '', ...props }: InputFieldProps) => {
  return (
    <FieldShell label={label} hint={hint} error={error} actions={actions}>
      <input className={`input${className ? ` ${className}` : ''}`} {...props} />
    </FieldShell>
  );
};

export const SelectField = ({
  label,
  hint,
  error,
  actions,
  children,
  className = '',
  ...props
}: SelectFieldProps) => {
  return (
    <FieldShell label={label} hint={hint} error={error} actions={actions}>
      <select className={`select${className ? ` ${className}` : ''}`} {...props}>
        {children}
      </select>
    </FieldShell>
  );
};

export const TextAreaField = ({
  label,
  hint,
  error,
  actions,
  className = '',
  ...props
}: TextareaFieldProps) => {
  return (
    <FieldShell label={label} hint={hint} error={error} actions={actions}>
      <textarea className={`textarea${className ? ` ${className}` : ''}`} {...props} />
    </FieldShell>
  );
};
