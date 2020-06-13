using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Button = System.Windows.Controls.Button;
using Control = System.Windows.Controls.Control;
using ListView = System.Windows.Controls.ListView;

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
    ///     <MyNamespace:TypeProviderView/>
    ///
    /// </summary>
    public class TypeProviderView : Control, IContainer
    {
        public static RoutedUICommand LaunchEditor = new RoutedUICommand("LAunch Editor", nameof(LaunchEditor),
            typeof(TypeProviderView),
            new InputGestureCollection(new List<InputGesture>() {new KeyGesture(Key.E, ModifierKeys.Control)}));

        public static readonly DependencyProperty SelectedPropertyProperty = DependencyProperty.Register(
            "SelectedProperty", typeof(PropertyDescriptor), typeof(TypeProviderView), new PropertyMetadata(default(PropertyDescriptor)));

        public static readonly DependencyProperty TypeDescriptorProperty = DependencyProperty.Register(
            "TypeDescriptor", typeof(ICustomTypeDescriptor), typeof(TypeProviderView), new PropertyMetadata(default(ICustomTypeDescriptor)));

        public ICustomTypeDescriptor TypeDescriptor
        {
            get { return (ICustomTypeDescriptor) GetValue(TypeDescriptorProperty); }
            set { SetValue(TypeDescriptorProperty, value); }
        }
        public PropertyDescriptor SelectedProperty
        {
            get { return (PropertyDescriptor) GetValue(SelectedPropertyProperty); }
            set { SetValue(SelectedPropertyProperty, value); }
        }

        public TypeProviderView()
        {
            CommandBindings.Clear();
            CommandBindings.Add(new CommandBinding(LaunchEditor, Executed));

        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(Type), typeof(TypeProviderView), new PropertyMetadata(default(Type), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TypeProviderView) d).OnTypeChanged((Type) e.OldValue, (System.Type) e.NewValue);
        }

        public static readonly DependencyProperty ProviderProperty = DependencyProperty.Register(
            "Provider", typeof(TypeDescriptionProvider), typeof(TypeProviderView), new PropertyMetadata(default(TypeDescriptionProvider), OnProviderChanged));

        private static void OnProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TypeProviderView) d).TypeProviderChanged((TypeDescriptionProvider) e.OldValue,
                (TypeDescriptionProvider) e.NewValue);
        }

        private void TypeProviderChanged(TypeDescriptionProvider eOldValue, TypeDescriptionProvider eNewValue)
        {
            Debug.WriteLine(eNewValue.GetType());
            
        }

        private PropertyDescriptorCollection _properties;

        public TypeDescriptionProvider Provider
        {
            get { return (TypeDescriptionProvider) GetValue(ProviderProperty); }
            set { SetValue(ProviderProperty, value); }
        }

        private void OnTypeChanged(Type pldType, Type newType)
        {
            if (newType == null)
            {
                Provider = null;
                Properties = Enumerable.Empty<PropertyDescriptor>();
                return;
            }

            TypeDescriptionProvider typeDescriptionProvider = System.ComponentModel.TypeDescriptor.GetProvider(newType);
            Provider = typeDescriptionProvider;
            if (Provider != null)
            {
                TypeDescriptor = Provider.GetTypeDescriptor(newType);
                if (TypeDescriptor != null)
                {
                    InstanceCreationEditor = TypeDescriptor.GetEditor(typeof(InstanceCreationEditor));
                    Editor = TypeDescriptor.GetEditor(typeof(UITypeEditor));
                    if (Editor != null)
                    {
                        var s = ((UITypeEditor) Editor).GetEditStyle();
                        var p = ((UITypeEditor)Editor).GetPaintValueSupported();
                        Debug.WriteLine($"{s} {p}");
                    }

                    

                    Debug.WriteLine($"{TypeDescriptor}");
                    Properties = TypeDescriptor.GetProperties().Cast<PropertyDescriptor>();
                    
                    if (_listView != null) _listView.SelectedIndex = 0;
                    
                }
            }
        }

        public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register(
            "Properties", typeof(IEnumerable<PropertyDescriptor>), typeof(TypeProviderView), new PropertyMetadata(default(IEnumerable<PropertyDescriptor>)));

        public IEnumerable<PropertyDescriptor> Properties
        {
            get { return (IEnumerable<PropertyDescriptor>) GetValue(PropertiesProperty); }
            set { SetValue(PropertiesProperty, value); }
        }

        public static readonly DependencyProperty InstanceCreationEditorProperty = DependencyProperty.Register(
            "InstanceCreationEditor", typeof(object), typeof(TypeProviderView), new PropertyMetadata(default(object)));

        public object InstanceCreationEditor
        {
            get { return (object) GetValue(InstanceCreationEditorProperty); }
            set { SetValue(InstanceCreationEditorProperty, value); }
        }
        public static readonly DependencyProperty EditorProperty = DependencyProperty.Register(
            "Editor", typeof(object), typeof(TypeProviderView), new PropertyMetadata(default(object)));

        private ListView _listView;

        public object Editor
        {
            get { return (object) GetValue(EditorProperty); }
            set { SetValue(EditorProperty, value); }
        }

        public Type Type
        {
            get { return (Type) GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        static TypeProviderView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TypeProviderView), new FrameworkPropertyMetadata(typeof(TypeProviderView)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _edit = GetTemplateChild("Edit") as Button;
            _edit.Click += _edit_Click;
            _listView = GetTemplateChild("ListView") as ListView;
            _listView.SelectionChanged += ListViewOnSelectionChanged;
            _windowsFormsHost = (WindowsFormsHost) GetTemplateChild("WindowsFormsHost");
            _windowsFormsHost.ChildChanged += WindowsFormsHostOnChildChanged;

        }

        private void _edit_Click(object sender, RoutedEventArgs e)
        {
            DoEdit();
        }

        private void WindowsFormsHostOnChildChanged(object sender, ChildChangedEventArgs e)
        {
            if (e.PreviousChild != null) Debug.WriteLine(e.PreviousChild.ToString());
        }

        public static readonly DependencyProperty InstanceProperty = DependencyProperty.Register(
            "Instance", typeof(object), typeof(TypeProviderView), new PropertyMetadata(default(object)));

        private IContainer _containerImplementation = new Container();
        private WindowsFormsHost _windowsFormsHost;
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(TypeProviderView), new PropertyMetadata(default(bool)));
        private Button _edit;
        private ICustomTypeDescriptor _descriptor;

        public object Instance
        {
            get { return (object) GetValue(InstanceProperty); }
            set { SetValue(InstanceProperty, value); }
        }

        private void Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DoEdit();
        }

        private void DoEdit()
        {
            var provider = new Context(this, null, null, _windowsFormsHost, this,
                IsDropDownOpenProperty);
            if (Editor is InstanceCreationEditor ee)
            {
                var o11 = ee.CreateInstance(provider, Type);
            }
             if (Editor is UITypeEditor ui)
            {
                var serviceProvider = new X1();
                
                object o1 = null;
                try
                {
                    var typeConverter = _descriptor.GetConverter();
                    o1 = typeConverter?.CreateInstance(provider, new Dictionary<string, object>());
                    provider.Instance = o1;
                    //o1 = Provider.CreateInstance(serviceProvider, Type, Type.EmptyTypes, new object[] { });
                }
                catch
                {
                }

                try
                {
                    Instance = Activator.CreateInstance(Type);
                }
                catch
                {
                }

                Debug.WriteLine("Executing");
                try
                {
                    var o = ui.EditValue(provider, o1);
                }
                catch (Exception ex)
                {

                }
            }
        }


        private void ListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedProperty = e.AddedItems.Cast<PropertyDescriptor>().FirstOrDefault();
        }

        public void Dispose()
        {
            _containerImplementation.Dispose();
        }

        public void Add(IComponent component)
        {
            _containerImplementation.Add(component);
        }

        public void Add(IComponent component, string name)
        {
            _containerImplementation.Add(component, name);
        }

        public void Remove(IComponent component)
        {
            _containerImplementation.Remove(component);
        }

        public ComponentCollection Components
        {
            get { return _containerImplementation.Components; }
        }

        public bool IsDropDownOpen
        {
            get { return (bool) GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }
    }

    internal class X1 : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            Debug.WriteLine(serviceType.FullName);
            return null;
        }
    }


    internal class Context : ITypeDescriptorContext
    {
        public Context(IContainer container, PropertyDescriptor propertyDescriptor, object instance,
            WindowsFormsHost windowsFormsHost, DependencyObject d, DependencyProperty prop)
        {
            _container = container;
            _propertyDescriptor = propertyDescriptor;
            _instance = instance;
            _windowsFormsHost = windowsFormsHost;
            this.d = d;
            this.prop = prop;
        }

        private readonly IContainer _container;
        private readonly PropertyDescriptor _propertyDescriptor;
        private readonly object _instance;
        private readonly WindowsFormsHost _windowsFormsHost;
        private DependencyProperty prop;
        private DependencyObject d;

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IWindowsFormsEditorService))
            {
                return new S(_windowsFormsHost, d, prop);
            }
            Debug.WriteLine(serviceType.FullName);
            return null;
        }

        public bool OnComponentChanging()
        {
            return true;
        }

        public void OnComponentChanged()
        {
            
        }

        public IContainer Container
        {
            get { return _container; }
        }

        public object Instance
        {
            get { return _instance; }
            set { throw new NotImplementedException(); }
        }

        public PropertyDescriptor PropertyDescriptor
        {
            get { return _propertyDescriptor; }
        }
    }

    public class S : System.Windows.Forms.Design.IWindowsFormsEditorService
    {
        private readonly WindowsFormsHost _windowsFormsHost;
        private readonly DependencyObject _d;
        private readonly DependencyProperty _dropDownOpen;

        public S(WindowsFormsHost windowsFormsHost, DependencyObject d, DependencyProperty dropDownOpen)
        {
            _windowsFormsHost = windowsFormsHost;
            _d = d;
            _dropDownOpen = dropDownOpen;
        }

        public void CloseDropDown()
        {
            Debug.WriteLine("close");
            _d?.SetValue(_dropDownOpen, false);
        }

        public void DropDownControl(System.Windows.Forms.Control control)
        {

            Debug.WriteLine("opend rop down");
            _windowsFormsHost.Child = control;
            // try
            // {
                // Window w = new Window {Content = new WindowsFormsHost() {Width = 400, Height = 400, Child = control}};
                // Debug.WriteLine(w.ToString());
                // w.ShowDialog();
            // }
            // catch (Exception ex)
            // {
                // Debug.WriteLine(ex.ToString());
            // }

            _d?.SetValue(_dropDownOpen, true);
        

    }

        public DialogResult ShowDialog(Form dialog)
        {
            return dialog.ShowDialog();
        }
    }
}
