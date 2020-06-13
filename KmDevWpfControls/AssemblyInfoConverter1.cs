using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace KmDevWpfControls
{
    public class AssemblyInfoConverter1 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Assembly a = (Assembly) value;
            if (a == null)
            {
                return null;
            }
            if ((String) parameter == "Name")

            {
                return a.GetName();
            }

            if ((string) parameter == "Location")
            {
                return a.IsDynamic ? null : a.Location;
            }

            if ((String) parameter == "Company")
            {
                return a.GetCustomAttribute < AssemblyCompanyAttribute>()?.Company;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}