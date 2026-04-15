using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorsportErp.Application.Common.Interfaces.Services;

public interface IReferenceDataServices<T>
{
    T GetReferenceData();
}
