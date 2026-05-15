using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WarsztatTuningowy.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var type = enumValue.GetType();
            bool isFlags = type.GetCustomAttribute<FlagsAttribute>() != null;

            if (isFlags)
            {
                var zeroValue = Enum.ToObject(type, 0);

                var names = Enum.GetValues(type)
                    .Cast<Enum>()
                    .Where(e => !e.Equals(zeroValue) && enumValue.HasFlag(e))
                    .Select(e =>
                     {
                         var field = type.GetField(e.ToString()!);
                         var attr = field?.GetCustomAttribute<DisplayAttribute>();
                         return attr?.Name ?? e.ToString();
                     });

                return string.Join(", ", names);
            }

            FieldInfo? field = type.GetField(enumValue.ToString()!);
            if (field == null) return enumValue.ToString();
            DisplayAttribute? attribute = field.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? enumValue.ToString();
        }
    }
}