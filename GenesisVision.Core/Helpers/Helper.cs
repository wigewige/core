using System;
using System.ComponentModel;

namespace GenesisVision.Core.Helpers
{
    public static class Helper
    {
        public static string GetDescription(Type type)
        {
            var descriptions = (DescriptionAttribute[])type.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return descriptions.Length == 0 ? null : descriptions[0].Description;
        }
    }
}
