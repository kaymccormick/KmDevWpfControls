using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace KmDevWpfControls
{
    public class ParameterInfoConverter1 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ParameterInfo p = (ParameterInfo) value;
            if ((string) parameter == "Tags")
            {
                List<string> tags = new List<string>();
                void a(string tag)
                {
                    tags.Add(tag);
                }
                if (p.IsIn)
                {
                    a("in");
                }
                
                if (p.IsLcid)
                {

                }
                if(p.IsOut)
                {
                    a("out");
                }

                return tags;
            } else if ((string) parameter == "CommaVisibility")
            {
                if (p.Position < (p.Member as MethodInfo)?.GetParameters().Length - 1)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}