using MotorsportErp.Application.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorsportErp.Application.Features.Users.Contracts;

public class UserReferenceDataResponce
{
    public List<EnumValueResponse> UserRoles { get; set; } = [];
}
