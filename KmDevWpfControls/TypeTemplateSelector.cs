using System;
using System.Diagnostics;
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
            {
                var frameworkElement = (FrameworkElement) container;
                DataTemplate findResource = frameworkElement.TryFindResource(new DataTemplateKey(item as Type)) as DataTemplate;
                if (findResource == null)
                {
                    findResource =
                        frameworkElement.TryFindResource(new DataTemplateKey(typeof(TraceListener))) as DataTemplate;
                }

                return findResource;
            }

            return base.SelectTemplate(item, container);
        }
    }
}