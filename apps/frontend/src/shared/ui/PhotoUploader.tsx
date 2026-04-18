import { useRef, useState, type ChangeEvent } from 'react';
import { filesApi } from '@/shared/api/files';
import { getErrorMessage } from '@/shared/lib/errors';
import type { MediaFileResponse } from '@/shared/types/api';
import { Button } from './Button';

type PhotoUploaderProps = {
  label?: string;
  onUploaded: (file: MediaFileResponse) => Promise<void> | void;
};

export const PhotoUploader = ({ label = 'Добавить фото', onUploaded }: PhotoUploaderProps) => {
  const inputRef = useRef<HTMLInputElement | null>(null);
  const [isUploading, setIsUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleChange = async (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];

    if (!file) {
      return;
    }

    setIsUploading(true);
    setError(null);

    try {
      const uploaded = await filesApi.upload(file);
      await onUploaded(uploaded);
      event.target.value = '';
    } catch (uploadError) {
      setError(getErrorMessage(uploadError));
    } finally {
      setIsUploading(false);
    }
  };

  return (
    <div className="stack">
      <div className="upload">
        <input
          ref={inputRef}
          className="sr-only"
          type="file"
          accept="image/png,image/jpeg,image/webp"
          onChange={handleChange}
        />
        <Button onClick={() => inputRef.current?.click()} disabled={isUploading}>
          {isUploading ? 'Загрузка...' : label}
        </Button>
      </div>
      {error ? <div className="notice notice--error">{error}</div> : null}
    </div>
  );
};
