using MotorsportErp.Application.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorsportErp.Application.Common.Extensions;

public static class EnumExtensions
{
    public static Dictionary<string, int> GetEnumValues<TEnum>() 
        where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>()
            .Select(value => new
            {
                Name = value.ToString(),
                Value = Convert.ToInt32(value)
            })
            .ToDictionary(el => el.Name, el => el.Value);
    }
}
