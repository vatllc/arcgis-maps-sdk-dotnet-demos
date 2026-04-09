using System;
#if MAUI
using System.Globalization;
#elif WINUI
using Microsoft.UI.Xaml.Data;
#else
using System.Globalization;
using System.Windows.Data;
#endif

namespace RoutingSample.Converters
{
	/// <summary>
	/// Base converter class for handling converter differences between .NET and Windows Runtime
	/// </summary>
    public abstract class ValueConverter : IValueConverter
    {
#if WINUI
		object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
#else
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
		{
#if !WINUI
			string language = culture.TwoLetterISOLanguageName;
#endif
			return Convert(value, targetType, parameter, language);
		}

		protected abstract object Convert(object value, Type targetType, object parameter, string language);

#if WINUI
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
#else
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
		{
#if !WINUI
			string language = culture.TwoLetterISOLanguageName;
#endif
			return ConvertBack(value, targetType, parameter, language);
		}

		protected abstract object ConvertBack(object value, Type targetType, object parameter, string language);
	}

	public abstract class StringFormatter : IValueConverter
	{
		protected abstract string Format(object value, object parameter, string language);

#if WINUI
		object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
#else
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
		{
#if !WINUI
			string language = culture.TwoLetterISOLanguageName;
#endif

            return Format(value, parameter, language);
		}

#if WINUI
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
#else
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
		{
			throw new NotSupportedException();
		}
	}
}
