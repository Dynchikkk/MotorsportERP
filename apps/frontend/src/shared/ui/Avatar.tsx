import { getAvatarUrl, getInitials } from '@/shared/lib/format';
import type { MediaFileResponse } from '@/shared/types/api';

type AvatarProps = {
  name: string;
  avatar?: MediaFileResponse | null;
  size?: number;
};

export const Avatar = ({ name, avatar, size = 44 }: AvatarProps) => {
  const url = getAvatarUrl(avatar);

  return (
    <span className="avatar" style={{ width: size, height: size }}>
      {url ? <img src={url} alt={name} /> : getInitials(name)}
    </span>
  );
};
