using MotorsportErp.Application.Common.Contracts;

namespace MotorsportErp.Application.Features.Users.Contracts;

public class UserReferenceDataResponse
{
    public List<EnumValueResponse> UserRoles { get; set; } = [];
}
