using System;
using System.Windows;
using System.Windows.Data;

namespace Graphs.Utils
{
    class BoolToVisibilityConverter : ConverterBase
    {
        public BoolToVisibilityConverter()
        {}

        public bool Inverted { get; set; }

        public Visibility NotVisibileState { get; set; } = Visibility.Collapsed;

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? boolVar = value as bool?;
            if (boolVar == null) return null;
            if (Inverted)
                return !boolVar.Value ? System.Windows.Visibility.Visible : NotVisibileState;
            else
                return boolVar.Value ? System.Windows.Visibility.Visible : NotVisibileState;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    class NullToVisibilityConverter : ConverterBase
    {
        public NullToVisibilityConverter()
        { }

        public bool Inverted { get; set; }

        public Visibility NotVisibileState { get; set; } = Visibility.Collapsed;

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Inverted)
                return value == null ? System.Windows.Visibility.Visible : NotVisibileState;
            else
                return value != null ? System.Windows.Visibility.Visible : NotVisibileState;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    [System.Windows.Markup.MarkupExtensionReturnType(typeof(ConverterBase))]
    abstract class ConverterBase : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public ConverterBase()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public abstract object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);
    }
}
