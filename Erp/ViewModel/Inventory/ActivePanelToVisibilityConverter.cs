﻿using Erp.Model.Inventory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Erp.ViewModel.Inventory
{
    public class ActivePanelToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var activePanel = (InvControlData.ActivePanel)value;
            var targetPanel = (InvControlData.ActivePanel)Enum.Parse(typeof(InvControlData.ActivePanel), (string)parameter);

            return activePanel == targetPanel ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
