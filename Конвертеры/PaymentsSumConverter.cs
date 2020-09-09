using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace LawCalculator_WPF
{
    class PaymentsSumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[1] == null) return (-(values[0] as Lawyer).Salary).ToString("N0", CultureInfo.InvariantCulture);
            if (parameter != null) return (values[0] as Lawyer).CountYearBalance((DateTime)values[1]).ToString("N0", CultureInfo.InvariantCulture);
            return (values[0] as Lawyer).CountBalance((DateTime)values[1]).ToString("N0", CultureInfo.InvariantCulture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}