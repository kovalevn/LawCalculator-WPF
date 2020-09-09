using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace LawCalculator_WPF
{
    class PartnerPaymentsSumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[1] == null) return 0.ToString("N0", CultureInfo.InvariantCulture);
            if (parameter != null) return (values[0] as Partner).CountYearBalance((DateTime)values[1]).ToString("N0", CultureInfo.InvariantCulture);
            if (values.Length < 3) return (values[0] as Partner).CountBalance((DateTime)values[1]).ToString("N0", CultureInfo.InvariantCulture);
            return (values[0] as Partner).CountBalance((DateTime)values[1], (CurrencyType)values[2]).ToString("N0", CultureInfo.InvariantCulture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
