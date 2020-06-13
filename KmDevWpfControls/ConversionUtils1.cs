using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace KmDevWpfControls
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConversionUtils
    {
        public static StringBuilder TypeToText(Type tt, StringBuilder sb = null)
        {
            if(sb == null)
                sb = new StringBuilder(40);
            if(!String.IsNullOrEmpty(tt.Namespace)) {
                
                sb.Append(AbbreviateNamespace(tt.Namespace));
                sb.Append('.');
            }

            sb.Append(tt.Name);
            if (tt.IsGenericType)
            {
                if (!tt.IsConstructedGenericType)
                {
                    throw new InvalidOperationException(tt.ToString());
                }

                sb.Append('<');
                foreach (var ttarg in tt.GenericTypeArguments)
                {
                    TypeToText(ttarg, sb);
                }

                sb.Append('>');
            }

            return sb;

        }

        private static string AbbreviateNamespace(string ns)
        {
            ns = ns.Replace("System.Windows.Data", "SWD");
            ns = ns.Replace("System.Collections.ObjectModel", "SCOM");
            ns = ns.Replace("System.Collections.Generic", "SCG");
            return ns;
        }

        public static StringBuilder DoConvertToString(object value, StringBuilder sb, bool useTypeConverters = true,
            HashSet<object> hashSet = null)
        {
            if (value == null)
            {
                return sb.Append("null/converted");
            }

            switch (value)
            {
                case DateTime dt:
                    return sb.Append(dt.ToString());
                // case SyntaxNode sn:
                    // return sb.Append($"SyntaxNode{{{sn.RawKind};{sn.GetType().Name}}}");
                case Type tt:
                    sb.Append("Type{");
                    TypeToText(tt, sb);
                    return sb.Append('}');
                case string ss:
                    return sb.Append(ss);
            }

            if (hashSet != null && !value.GetType().IsValueType)
            {
                if (hashSet.Contains(value)) throw new InvalidOperationException(value.ToString());

                hashSet.Add(value);
            }

            switch (value)
            {
                case IEnumerable vv:
                {
                    var i = 0;
                    foreach (var o in vv)
                    {
                        sb.AppendFormat("[{0}]", i);
                        sb.Append(DoConvertToString(o, sb, useTypeConverters, hashSet));
                        sb.Append("n");
                        i++;
                    }

                    return sb;
                }
            }

            var t = value.GetType();
            if (t.IsGenericType && t.IsConstructedGenericType)
            {
                var args = t.GenericTypeArguments;
                if (args.Length == 2)
                {
                    var t2 = typeof(IDictionary<,>).MakeGenericType(args);
                    if (t2.IsAssignableFrom(t))
                    {
                        // var conv = typeof(DictConverter<,>).MakeGenericType(t2);
                        // var c = Activator.CreateInstance(conv);
                        // var method = conv.GetMethod("DumpInstance", BindingFlags.Instance | BindingFlags.Public);
                        // var strval = method.Invoke(c, new object[] {value})?.ToString() ?? "null";
                        // return sb.Append(strval);
                    }
                }
            }

            if ((bool) value?.GetType().IsPrimitive)
            {
                sb.Append(value).Append("{");
                TypeToText(value.GetType(), sb);
                return sb.Append("}");
            }

            var coll = TypeDescriptor.GetProperties(value.GetType());
            if (coll.Count != 0)
            {
                foreach (PropertyDescriptor property in coll)
                {
                    if (!property.IsBrowsable)
                    {
                        Debug.WriteLine($"{property.Name} unbrowsable");
                        continue;
                    }

                    sb.Append(property.Name);
                    sb.Append('(');
                    TypeToText(property.PropertyType, sb);
                    sb.Append(")\t = ");
                    var val = property.GetValue(value);
                    // DebugUtils.WriteLine($"{property.Name} = {val}");
                    string convval = null;
                    if (val is string sss)
                        convval = sss;
                    else if (val is byte[]) convval = "byte[]";

                    if (val == null)
                    {
                        convval = "null/converted";
                    }
                    else
                    {
                        if (useTypeConverters)
                        {
                            var conv = property.Converter;

                            if (conv != null && conv.CanConvertTo(typeof(string)))
                                convval = conv.ConvertTo(val, typeof(string))?.ToString() ?? "null";
                        }

                        if (convval == null && (!useTypeConverters || !(val is string) && val is IEnumerable))
                        {
                            DoConvertToString(val, sb, useTypeConverters, hashSet);
                            sb.Append("\n");
                            continue;
                        }

                        if (convval == null)
                        {
                            var conv = TypeDescriptor.GetConverter(val.GetType());
                            convval = conv.CanConvertTo(typeof(string))
                                ? conv.ConvertTo(val, typeof(string))?.ToString() ?? "null"
                                : val.ToString() + " {" + val.GetType().FullName + "}";
                        }

                    }

                    sb.Append(convval);
                    sb.Append("\n");
                }
            }
            else
            {
                return sb.Append(value);
            }

            return sb;
        }
    }
}