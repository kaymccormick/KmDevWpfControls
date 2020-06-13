using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for TraceView.xaml
    /// </summary>
    public partial class TraceView : UserControl
    {
        public static readonly RoutedEvent TraceListenerCreatedEvent = EventManager.RegisterRoutedEvent("TraceListenerCreated",
            RoutingStrategy.Bubble, typeof(TraceListenerCreatedHandler), typeof(TraceView));

        public delegate void TraceListenerCreatedHandler(object sender, TraceListenerCreatedEventArgs e);

        public TraceView()
        {
            PropertyDescriptorCollection = TypeDescriptor.GetProperties(typeof(XmlWriterTraceListener));

            InitializeComponent();
            Edit.DataContextChanged += Edit_DataContextChanged;
            Combo.SelectionChanged += Combo_SelectionChanged;
        }

        public event TraceListenerCreatedHandler TraceListenerCreated    
        {
            add => AddHandler(TraceListenerCreatedEvent, value);
            remove => RemoveHandler(TraceListenerCreatedEvent, value);
        }


        private void Edit_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var t = e.AddedItems.Cast<Type>().FirstOrDefault();
            Edit.Content = t;
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var b = e.Parameter == null;
            RefreshTraceSources(b);
        }

        public void RefreshTraceSources(bool b)
        {
            if (b)
                PresentationTraceSources.Refresh();
            view.Refresh();
        }

        private void CommandBinding_OnExecuted2(object sender, ExecutedRoutedEventArgs e)
        {
            TraceListener x = null;
            //XmlWriterTraceListener a = new XmlWriterTraceListener();
                        

        }

        public static readonly DependencyProperty PropertyDescriptorCollectionProperty = DependencyProperty.Register(
            "PropertyDescriptorCollection", typeof(PropertyDescriptorCollection), typeof(TraceView), new PropertyMetadata(default(PropertyDescriptorCollection), OnPropertyDescriptorCollectionChanged));

        public PropertyDescriptorCollection PropertyDescriptorCollection
        {
            get { return (PropertyDescriptorCollection) GetValue(PropertyDescriptorCollectionProperty); }
            set { SetValue(PropertyDescriptorCollectionProperty, value); }
        }

        private static void OnPropertyDescriptorCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TraceView) d).OnPropertyDescriptorCollectionChanged((PropertyDescriptorCollection) e.OldValue, (PropertyDescriptorCollection) e.NewValue);
        }



        protected virtual void OnPropertyDescriptorCollectionChanged(PropertyDescriptorCollection oldValue, PropertyDescriptorCollection newValue)
        {
        }

        public static readonly DependencyProperty TraceOptionsProperty = DependencyProperty.Register(
            "TraceOptions", typeof(TraceOptions), typeof(TraceView), new PropertyMetadata(default(TraceOptions), OnTraceOptionsChanged));

        public TraceOptions TraceOptions
        {
            get { return (TraceOptions) GetValue(TraceOptionsProperty); }
            set { SetValue(TraceOptionsProperty, value); }
        }

        private static void OnTraceOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TraceView) d).OnTraceOptionsChanged((TraceOptions) e.OldValue, (TraceOptions) e.NewValue);
        }

        public static readonly DependencyProperty ListenerProperty = DependencyProperty.Register(
            "Listener", typeof(TraceListener), typeof(TraceView), new PropertyMetadata(default(TraceListener), OnListenerChanged));

        public TraceListener Listener
        {
            get { return (TraceListener) GetValue(ListenerProperty); }
            set { SetValue(ListenerProperty, value); }
        }

        private static void OnListenerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TraceView) d).OnListenerChanged((TraceListener) e.OldValue, (TraceListener) e.NewValue);
        }


        public static readonly DependencyProperty ListenerTypesProperty = DependencyProperty.Register(
            "ListenerTypes", typeof(IEnumerable), typeof(TraceView), new PropertyMetadata(default(IEnumerable), OnListenerTypesChanged));

        public IEnumerable ListenerTypes
        {
            get { return (IEnumerable) GetValue(ListenerTypesProperty); }
            set { SetValue(ListenerTypesProperty, value); }
        }

        private static void OnListenerTypesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TraceView) d).OnListenerTypesChanged((IEnumerable<Type>) e.OldValue, (IEnumerable<Type>) e.NewValue);
        }



        protected virtual void OnListenerTypesChanged(IEnumerable<Type> oldValue, IEnumerable<Type> newValue)
        {
        }

        protected virtual void OnListenerChanged(TraceListener oldValue, TraceListener newValue)
        {
        }


        protected virtual void OnTraceOptionsChanged(TraceOptions oldValue, TraceOptions newValue)
        {
        }

        /// <inheritdoc />

        // public static IEnumerable TraceOptions { get; set; } = Enum.GetValues(typeof(TraceOptions)).Cast<TraceOptions>().Select(o=>new CheckableModelItem<TraceOptions>(o));
        private void CommandBinding_OnExecuted3(object sender, ExecutedRoutedEventArgs e)
        {
            CreateListener();
        }

        private void CreateListener()
        {
            var t =
                Combo.SelectedItem as Type;
            object x11 = null;
            try
            {
                x11 = Activator.CreateInstance(t);
            }
            catch
            {
            }

            var x = Edit.FindName("FileInputBox");
            var c = VisualTreeHelper.GetChild(Edit, 0);
            string file = null;
            int flags = 0;

            if (VisualTreeHelper.GetChildrenCount(c) > 0)
            {
                var c1 = VisualTreeHelper.GetChild(c, 0);
                var num = VisualTreeHelper.GetChildrenCount(c);
                for (int i = 0; i < num; i++)
                {
                    var c0 = VisualTreeHelper.GetChild(c, i);
                    if (c0 is FileInputBox xxx)
                    {
                        file = xxx.Filename;
                    }
                    else
                    {
                        if (c0 is EnumFlagsSelector f)
                        {
                            flags = f.Value;
                        }
                    }
                }
            }

            TraceListener xx;

            if (x11 == null)
            {
                if (t == typeof(XmlWriterTraceListener))
                {
                    Debug.WriteLine(file);

                    var xmlWriterTraceListener = new XmlWriterTraceListener(file);
                    Debug.WriteLine(xmlWriterTraceListener.Writer);
                    xx = xmlWriterTraceListener;
                }
                else
                {
                    xx = new ConsoleTraceListener();
                }
            }
            else
                xx = (TraceListener) x11;

            xx.TraceOutputOptions = (TraceOptions) flags;

            var ev = new TraceListenerCreatedEventArgs(TraceListenerCreatedEvent, this, xx);
            RaiseEvent(ev);

            view.SelectedTraceSource?.Listeners.Add(xx);
            view.Refresh();
            try
            {
                Debug.WriteLine(PresentationTraceSources.RoutedEventSource.Switch.Level);
                foreach (TraceListener listener in PresentationTraceSources.RoutedEventSource.Listeners)
                {
                    Debug.WriteLine(listener.GetType());
                }
            }
            catch
            {
            }

            //PresentationTraceSources.DataBindingSource.Listeners.Add(xx);
            Listener = xx;
        }
    }
}
