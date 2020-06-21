using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KmDevWpfControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:KmDevWpfControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:KmDevWpfControls;assembly=KmDevWpfControls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:TraceSourcesView/>
    ///
    /// </summary>
    public class TraceSourcesView : Control
    {
        public void Refresh()
        {
            //(_listView.View as GridView).
            CollectionViewSource.GetDefaultView(Sources)?.Refresh();
        }
        public static IEnumerable<TraceSource> Sources { get; } = new[]
        {
            PresentationTraceSources.AnimationSource,
            PresentationTraceSources.DataBindingSource,
            PresentationTraceSources.DependencyPropertySource,
            PresentationTraceSources.DocumentsSource,
            PresentationTraceSources.FreezableSource,
            PresentationTraceSources.HwndHostSource,
            PresentationTraceSources.MarkupSource,
            PresentationTraceSources.NameScopeSource,
            PresentationTraceSources.ResourceDictionarySource,
            PresentationTraceSources.RoutedEventSource,
            PresentationTraceSources.ShellSource
        };

        private ListView _listView;

        public static readonly DependencyProperty SelectedTraceSourceProperty = DependencyProperty.Register(
            "SelectedTraceSource", typeof(TraceSource), typeof(TraceSourcesView), new PropertyMetadata(default(TraceSource), OnSelectedTraceSourceChanged));

        public TraceSource SelectedTraceSource
        {
            get { return (TraceSource) GetValue(SelectedTraceSourceProperty); }
            set { SetValue(SelectedTraceSourceProperty, value); }
        }

        private static void OnSelectedTraceSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TraceSourcesView) d).OnSelectedTraceSourceChanged((TraceSource) e.OldValue, (TraceSource) e.NewValue);
        }



        protected virtual void OnSelectedTraceSourceChanged(TraceSource oldValue, TraceSource newValue)
        {
        }

        static TraceSourcesView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TraceSourcesView), new FrameworkPropertyMetadata(typeof(TraceSourcesView)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _listView = GetTemplateChild("ListView") as ListView;
        }

        public TraceSourcesView()
        {
            foreach (var traceSource in Sources)
            {
                
            }
        }
    }
}
