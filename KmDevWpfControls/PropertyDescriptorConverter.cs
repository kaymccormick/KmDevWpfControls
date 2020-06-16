using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Data;

namespace KmDevWpfControls
{
    public class PropertyDescriptorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            PropertyDescriptor x = (PropertyDescriptor) value;
            var p = (string) parameter;
            switch (p)
            {
                case "Editor":
                    // return x.GetEditor(typeof(UITypeEditor));
                    return null;
                case "InstanceCreationEditor":
                    var convert = x.GetEditor(typeof(InstanceCreationEditor));
                    if (convert != null)
                    {
                        Debug.WriteLine("Editor is " + convert);
                    }
                    return convert;
            }

            return null;
        }
    

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}