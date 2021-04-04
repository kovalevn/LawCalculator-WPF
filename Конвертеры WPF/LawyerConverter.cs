using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace LawCalculator_WPF
{
    class LawyerConverter : IMultiValueConverter
    {
        //Заменён на GeneralPaymentConverter - удалить
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(values[0].ToString())) return "0";
            else return $"{double.Parse(values[0].ToString()) * double.Parse(values[1].ToString()) / 100}";
        }

        public object[] ConvertBack(object values, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
