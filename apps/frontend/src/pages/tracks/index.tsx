import { useDeferredValue, useState } from 'react';
import { useAuth } from '@/app/providers/AuthProvider';
import { TrackForm } from '@/features/tracks/create-track/TrackForm';
import { tracksApi } from '@/shared/api/tracks';
import { useAsyncData } from '@/shared/lib/hooks/useAsyncData';
import { isModeratorOrAbove } from '@/shared/lib/roles';
import { trackStatusLabels } from '@/shared/lib/format';
import { Card } from '@/shared/ui/Card';
import { EmptyState } from '@/shared/ui/EmptyState';
import { InputField, SelectField } from '@/shared/ui/Field';
import { Loader } from '@/shared/ui/Loader';
import { Notice } from '@/shared/ui/Notice';
import { PageHeader } from '@/shared/ui/PageHeader';
import { TrackCardList } from '@/widgets/track-card-list/TrackCardList';
import type { TrackStatus } from '@/shared/types/api';

export const TracksPage = () => {
  const { currentUser } = useAuth();
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState('');
  const deferredSearch = useDeferredValue(search);

  const data = useAsyncData(
    async () => {
      const [tracks, reference] = await Promise.all([
        tracksApi.getAll({
          search: deferredSearch || undefined,
          status: status ? Number(status) : undefined,
        }),
        tracksApi.getReferenceData(),
      ]);

      return { tracks, reference };
    },
    [deferredSearch, status],
  );

  if (data.isLoading && !data.data) {
    return <Loader label="Загружаем трассы" fullHeight />;
  }

  if (data.error || !data.data) {
    return <Notice tone="error" message={data.error ?? 'Не удалось загрузить трассы'} />;
  }

  const canModerate = isModeratorOrAbove(currentUser?.roles);
  const canCreateTrack =
    Boolean(currentUser) && (canModerate || (currentUser?.raceCount ?? 0) >= data.data.reference.minRacesToCreateTrack);

  return (
    <div className="page">
      <PageHeader
        eyebrow="Трассы"
        title="Каталог трасс"
        description="Здесь собраны площадки, на которых можно проводить турниры: видно статус трассы, можно отфильтровать подтверждённые варианты и при нужном стаже предложить новую."
      />

      <Card className="page__section">
        <div className="form__row">
          <InputField
            label="Поиск"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Название или местоположение"
          />
          <SelectField label="Статус" value={status} onChange={(event) => setStatus(event.target.value)}>
            <option value="">Все статусы</option>
            {data.data.reference.trackStatuses.map((item) => (
              <option key={item.value} value={item.value}>
                {trackStatusLabels[item.value as TrackStatus] ?? item.name}
              </option>
            ))}
          </SelectField>
        </div>
      </Card>

      {currentUser ? (
        canCreateTrack ? (
          <TrackForm
            statusOptions={data.data.reference.trackStatuses}
            allowStatusSelection={canModerate}
            onSuccess={data.reload}
          />
        ) : (
          <Notice
            tone="error"
            message={`Сейчас добавить трассу нельзя: нужно минимум ${data.data.reference.minRacesToCreateTrack} завершённых гонок или права модератора.`}
          />
        )
      ) : null}

      {data.data.tracks.items.length ? (
        <TrackCardList tracks={data.data.tracks.items} />
      ) : (
        <EmptyState title="По этим параметрам трасс не нашлось" description="Попробуйте убрать фильтр по статусу или изменить поисковый запрос." />
      )}
    </div>
  );
};
