using System;
using System.Windows;
using System.Windows.Controls;

namespace KmDevWpfControls
{
    public class TypeControl : Control
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(Type), typeof(TypeControl), new PropertyMetadata(default(Type)));

        public Type Type
        {
            get { return (Type) GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        static TypeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TypeControl),
                new FrameworkPropertyMetadata(typeof(TypeControl)));

        }
    }
}