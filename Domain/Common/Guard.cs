using System;

namespace Domain.Common
{
    /// <summary>
    /// Centralized guard clauses used by entity constructors / mutator methods
    /// to enforce domain invariants at the point of object creation or change,
    /// instead of relying on callers (services, controllers) to remember to validate.
    /// Throwing here means an invalid entity can never exist in memory.
    /// </summary>
    public static class Guard
    {
        public static string NotNullOrWhiteSpace(string? value, string paramName, int maxLength = 200)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} is required.", paramName);

            value = value.Trim();

            if (value.Length > maxLength)
                throw new ArgumentException($"{paramName} cannot exceed {maxLength} characters.", paramName);

            return value;
        }

        public static int Positive(int value, string paramName)
        {
            if (value <= 0)
                throw new ArgumentException($"{paramName} must be a positive number.", paramName);

            return value;
        }

        public static float PositiveOrZero(float value, string paramName)
        {
            if (value < 0)
                throw new ArgumentException($"{paramName} cannot be negative.", paramName);

            return value;
        }

        public static double PositiveOrZero(double value, string paramName)
        {
            if (value < 0)
                throw new ArgumentException($"{paramName} cannot be negative.", paramName);

            return value;
        }

        public static DateTime NotDefault(DateTime value, string paramName)
        {
            if (value == default)
                throw new ArgumentException($"{paramName} must be a valid date.", paramName);

            return value;
        }

        public static DateTime NotInFuture(DateTime value, string paramName)
        {
            if (value > DateTime.UtcNow)
                throw new ArgumentException($"{paramName} cannot be in the future.", paramName);

            return value;
        }

        public static void Range(float min, float max, string minParamName, string maxParamName)
        {
            if (min >= max)
                throw new ArgumentException($"{minParamName} must be less than {maxParamName}.");
        }
    }
}
