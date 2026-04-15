using MotorsportErp.Application.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorsportErp.Application.Features.Cars.Contracts;

public class CarReferenceDataResponce
{
    public List<EnumValueResponse> CarClasses { get; set; } = [];
}
