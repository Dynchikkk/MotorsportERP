using MotorsportErp.Application.Common.Contracts;

namespace MotorsportErp.Application.Features.Cars.Contracts;

public class CarReferenceDataResponse
{
    public List<EnumValueResponse> CarClasses { get; set; } = [];
}
