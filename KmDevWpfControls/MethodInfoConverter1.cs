using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using JetBrains.Annotations;

namespace KmDevWpfControls
{
    /// <summary>
    /// </summary>
    public sealed class MethodInfoConverter1 : IValueConverter
    {
        #region Implementation of IValueConverter
        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        [ NotNull ]
        public object Convert (
            object      value
            , Type        targetType
            , object      parameter
            , CultureInfo culture
        )
        {
            var methodInfo = ( MethodInfo ) value ;
            if ( value == null )
            {
                return "(null)" ;
            }

            if ( ( string ) parameter == "Parameters" )
            {
                var parameterInfos = methodInfo.GetParameters ( );
                return parameterInfos ;
            }

            var bytes = methodInfo.GetMethodBody().GetILAsByteArray();
            
            return "(undefined)" ;
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