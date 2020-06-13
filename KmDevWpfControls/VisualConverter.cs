using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace KmDevWpfControls
{
    public class VisualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                Visual v = value as Visual;
                List<VisualTreeNode> vs = new List<VisualTreeNode>();
                if (v == null) return vs;
                var childrenCount = VisualTreeHelper.GetChildrenCount(v);
                for (int i = 0; i < childrenCount ; i++)
                {
                    vs.Add(new VisualTreeNode {Visual = (Visual) VisualTreeHelper.GetChild(v, i)});
                }

                return vs; 
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}