using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace KmDevWpfControls

{
    public class TypeConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(string))
            {
                return (value as Type)?.FullName ?? "";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(typeof(Type).IsAssignableFrom(targetType))
            {
                var s = value.ToString();
                var type = Type.GetType(s);
                var t = type ??
                        AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).SelectMany(a => a.GetExportedTypes()).FirstOrDefault(tt => tt.FullName == s);
                return t;
            }

            return null;
        }
    }
    public class TypeConverter1 : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

        return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context
            , CultureInfo culture, object value)
        {
            if(value is string s)
            {
                var t = Type.GetType(s) ??
                        AppDomain.CurrentDomain.GetAssemblies().SelectMany(a=>a.GetExportedTypes()).FirstOrDefault(tt => tt.FullName == s);
                if (t != null)
                    return t;
                return Type.Missing;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if(value is Type t)
            {
                if (destinationType == typeof(string))
                    return t.FullName;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return base.GetStandardValues(context);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return base.GetStandardValuesSupported(context);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return base.IsValid(context, value);
        }
    }
}