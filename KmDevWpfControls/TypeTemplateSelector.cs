using System;
using System.Windows;
using System.Windows.Controls;

namespace KmDevWpfControls
{
    public class TypeTemplateSelector : DataTemplateSelector
    {
        /// <inheritdoc />
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item != null)
            return ((FrameworkElement) container).TryFindResource(new DataTemplateKey(item as Type)) as DataTemplate;
            return base.SelectTemplate(item, container);
        }
    }
}