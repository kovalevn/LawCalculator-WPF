using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace LawCalculator_WPF
{
    class PaymentsOnDateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<Payment> AllPaymentsOnDate = new ObservableCollection<Payment>();

            if (values[0] != null)
            {
                foreach (LawyersProject project in (ObservableCollection<LawyersProject>)values[1])
                {
                    foreach (Payment payment in project.Payments)
                    {
                        if (payment.Date.Year == ((DateTime)values[0]).Year && payment.Date.Month == ((DateTime)values[0]).Month) AllPaymentsOnDate.Add(payment);
                    }
                }
            }
            return AllPaymentsOnDate;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
