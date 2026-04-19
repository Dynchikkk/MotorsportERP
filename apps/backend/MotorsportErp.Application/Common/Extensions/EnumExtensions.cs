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
