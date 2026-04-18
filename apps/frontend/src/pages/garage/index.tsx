import { useState } from 'react';
import { CarCard } from '@/entities/car/ui/CarCard';
import { CarForm } from '@/features/cars/create-car/CarForm';
import { DeleteCarButton } from '@/features/cars/delete-car/DeleteCarButton';
import { carsApi } from '@/shared/api/cars';
import { useAsyncData } from '@/shared/lib/hooks/useAsyncData';
import { Button } from '@/shared/ui/Button';
import { EmptyState } from '@/shared/ui/EmptyState';
import { Loader } from '@/shared/ui/Loader';
import { Notice } from '@/shared/ui/Notice';
import { PageHeader } from '@/shared/ui/PageHeader';
import { PhotoUploader } from '@/shared/ui/PhotoUploader';

export const GaragePage = () => {
  const [editingCarId, setEditingCarId] = useState<string | null>(null);

  const garage = useAsyncData(
    async () => {
      const [cars, reference] = await Promise.all([carsApi.getMyCars(), carsApi.getReferenceData()]);
      return { cars, reference };
    },
    [],
  );

  const reloadGarage = async () => {
    await garage.reload();
    setEditingCarId(null);
  };

  if (garage.isLoading && !garage.data) {
    return <Loader label="Загружаем гараж" fullHeight />;
  }

  if (garage.error || !garage.data) {
    return <Notice tone="error" message={garage.error ?? 'Не удалось загрузить гараж'} />;
  }

  const cars = garage.data.cars.items;
  const editingCar = cars.find((car) => car.id === editingCarId);

  return (
    <div className="page">
      <PageHeader
        eyebrow="Гараж"
        title="Мои автомобили"
        description="Добавляйте машины, обновляйте их карточки и прикрепляйте изображения. Удаление ограничено сервером, если авто участвует в активных заявках."
      />

      <CarForm
        key={editingCar?.id ?? 'new-car'}
        carClasses={garage.data.reference.carClasses}
        initialValue={editingCar}
        onSuccess={reloadGarage}
        onCancel={() => setEditingCarId(null)}
      />

      {cars.length ? (
        <div className="cards-grid">
          {cars.map((car) => (
            <CarCard
              key={car.id}
              car={car}
              actions={
                <>
                  <Button variant="secondary" onClick={() => setEditingCarId(car.id)}>
                    Редактировать
                  </Button>
                  <PhotoUploader
                    label="Добавить фото"
                    onUploaded={async (file) => {
                      await carsApi.addPhoto(car.id, file.id);
                      await reloadGarage();
                    }}
                  />
                  <DeleteCarButton carId={car.id} onDeleted={reloadGarage} />
                </>
              }
            />
          ))}
        </div>
      ) : (
        <EmptyState
          title="Гараж пока пуст"
          description="Добавьте хотя бы одну машину, чтобы использовать её при подаче заявок на турниры."
        />
      )}
    </div>
  );
};
