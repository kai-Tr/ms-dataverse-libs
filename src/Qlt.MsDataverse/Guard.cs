using System;

namespace Qlt.MsDataverse
{
    public static class Guard
    {
        public static void ThrowIfNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void ThrowIfNull(string argumentValue, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argumentValue))
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}
