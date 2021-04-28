using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace LawCalculator_WPF
{
    class LawyerPercentConverter : IMultiValueConverter
    {
        LawyersProject proj = new LawyersProject();
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (LawyersProject project in (values[0] as ObservableCollection<LawyersProject>)) if (project.Project == (values[1] as Project)) proj = project;
            return proj.Percent.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString())) proj.Percent = 0;
            else proj.Percent = float.Parse(value.ToString());
            object[] ret = new object[] { Binding.DoNothing, Binding.DoNothing };
            return ret;
        }
    }
}
