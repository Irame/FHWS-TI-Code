using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Graphs.Utils
{
    class ValueConverter<TIn, TOut> : IValueConverter
    {
        private Func<TIn, object, CultureInfo, TOut> _converter;
        private Func<TOut, object, CultureInfo, TIn> _backConverter;

        public ValueConverter(Func<TIn, object, CultureInfo, TOut> converter,
            Func<TOut, object, CultureInfo, TIn> backConverter = null)
        {
            _converter = converter;
            _backConverter = backConverter;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TIn && targetType == typeof(TOut))
                return _converter((TIn)value, parameter, culture);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (_backConverter == null)
                throw new NotImplementedException();

            if (value is TOut && targetType == typeof(TIn))
                return _backConverter((TOut)value, parameter, culture);
            return null;
        }
    }
}
