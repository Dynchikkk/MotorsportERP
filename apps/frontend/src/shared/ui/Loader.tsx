type LoaderProps = {
  label?: string;
  fullHeight?: boolean;
};

export const Loader = ({ label = 'Загружаем данные', fullHeight = false }: LoaderProps) => {
  return (
    <div
      className="loader"
      style={fullHeight ? { minHeight: '40vh', display: 'grid', placeItems: 'center' } : undefined}
    >
      {label}
    </div>
  );
};
