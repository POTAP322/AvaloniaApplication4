// ReflectionApp/Converters/Converters.cs
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ReflectionApp.Converters
{
    public static class GreaterThanZeroConverter
    {
        public static bool Convert(int value) => value > 0;
    }

    public static class StringIsNotNullOrEmptyConverter
    {
        public static bool Convert(string? value) => !string.IsNullOrEmpty(value);
    }

    public static class ObjectIsNotNullConverter
    {
        public static bool Convert(object? value) => value != null;
    }

    // Реализация IValueConverter для использования в XAML
    public class GreaterThanZeroValueConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is int i && i > 0;

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    public class StringIsNotNullOrEmptyValueConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is string s && !string.IsNullOrEmpty(s);

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    public class ObjectIsNotNullValueConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value != null;

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}