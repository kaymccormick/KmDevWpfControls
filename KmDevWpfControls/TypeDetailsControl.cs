using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KmDevWpfControls
{
    public class TypeDetailsControl : Control
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(Type), typeof(TypeDetailsControl), new PropertyMetadata(default(Type), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (
                (TypeDetailsControl) d).OnTypeChanged((Type) e.OldValue, (Type)e.NewValue);
        }

        protected virtual void OnTypeChanged(Type oldType, Type newType)
        {
            
        }

        public Type Type
        {
            get { return (Type) GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        static TypeDetailsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TypeDetailsControl),
                new FrameworkPropertyMetadata(typeof(TypeDetailsControl)));
        }
        public IEnumerable<Type> Ancestors
        {
            get
            {
                if (Type == null)
                    return Enumerable.Empty<Type>();
                var a = new List<Type>();
                var b = Type.IsGenericType? Type.GetGenericTypeDefinition() : Type;
                while (b != null)
                {
                    a.Add(b);
                    b = b.BaseType;
                }

                return ((IEnumerable<Type>)a).Reverse();
            }
        }

    }
}