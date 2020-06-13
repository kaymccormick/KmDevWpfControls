using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace KmDevWpfControls
{
    /// <summary>Helper converter to get info on types.</summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public sealed class TypeInfoConverter1 : IValueConverter

    {
        #region Implementation of IValueConverter
        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert (
            object      value
            , Type        targetType
            , object      parameter
            , CultureInfo culture
        )
        {
            // Debug.WriteLine($"{value} {parameter}");

            if (value == null)
            {
                return null;
            }
            var source = value as Type;
            if (source == null)
                return null;

            
            if ( ( string ) parameter == "Interfaces" )
            {
                return source.GetInterfaces ( ) ;
            }

            if ((string) parameter == "GenericTypeDefinition" && source.IsGenericType)
            {
                var genericTypeDefinition = source.GetGenericTypeDefinition();
                return genericTypeDefinition;
            }

            if ((string) parameter == "GetInterfaces")
            {
                return source.GetInterfaces();
            }


            if ((string)parameter == "Methods")
            {
                return source.GetMethods().Where(m => m.IsSpecialName == false);
            }

            if ((string) parameter == "Properties")
            {
                return source.GetProperties();
            }

            if ((string) parameter == "Ancestors")
            {
                var a = new List<Type>();
                var b = source.IsGenericType ? source.GetGenericTypeDefinition() : source;
                while (b != null)
                {
                    a.Add(b);
                    b = b.BaseType;
                }

                return ((IEnumerable<Type>)a).Reverse();
            }
            

            
            return null ;
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack (
            object      value
            , Type        targetType
            , object      parameter
            , CultureInfo culture
        )
        {
            return null ;
        }
        #endregion
    }
}