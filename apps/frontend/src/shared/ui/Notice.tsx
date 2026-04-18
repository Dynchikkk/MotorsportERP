type NoticeProps = {
  tone?: 'success' | 'error';
  message: string;
};

export const Notice = ({ tone = 'success', message }: NoticeProps) => {
  return <div className={`notice notice--${tone}`}>{message}</div>;
};
