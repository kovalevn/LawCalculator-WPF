using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace LawCalculator_WPF
{
    class GeneralPaymentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double amount = Project.CountMoneyOfCurrency(values[0] as ObservableCollection<Payment>, (CurrencyType)values[1]);
            if (!string.IsNullOrEmpty(values[2] as string)) return (amount * int.Parse(values[2] as string) / 100).ToString("N0", CultureInfo.InvariantCulture);
            else return "0";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
