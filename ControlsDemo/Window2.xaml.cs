using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AvalonDock.Layout;
using ControlsDemo.Annotations;
using KmDevWpfControls;
using Control = System.Windows.Forms.Control;
using Cursor = System.Windows.Input.Cursor;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace ControlsDemo
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            var b = new Bitmap(@"C:\temp\example.bmp");
            InitializeComponent();
            Anchorables.Add("test");
            Debug.WriteLine(DM.LayoutUpdateStrategy?.ToString());
        }


        public ObservableCollection<object> DockItems { get; set; } = new ObservableCollection<object>();

        public ObservableCollection<object> Anchorables { get; set; } = new ObservableCollection<object>();

        public Window2ViewModel ViewModel { get; set; } = new Window2ViewModel();

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var open = new OpenFileDialog();
            var result = open.ShowDialog();
            if ((bool) result)
            {
                var ext = System.IO.Path.GetExtension(open.FileName);
                if (ext == ".dll")
                    try
                    {
                        var assembly = Assembly.LoadFile(open.FileName);
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.ToString());
                    }
            }
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var xxx = new Form();
            var creatable = new HashSet<Type>();
            var values = new HashSet<Type>();
            var editorTypes = new HashSet<Type>();
            var paints = new HashSet<PropertyDescriptor>();
            var comp = new List<Tuple<PropertyDescriptor, object>>();
            var a = ViewModel.Assemblies.ToList();
            foreach (var viewModelAssembly in a)
            {
                Debug.WriteLine(a);
                try
                {
                    foreach (var exportedType in viewModelAssembly.ExportedTypes)
                        //                        Debug.WriteLine(exportedType);
                        try
                        {
                            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(exportedType))
                            {
                                if (property.Converter.CanConvertTo(typeof(InstanceDescriptor)))
                                    Debug.WriteLine("!!!!" + property.PropertyType);
                                var ch = property.GetChildProperties();
                                if (false && ch.Count > 0)
                                    foreach (PropertyDescriptor o in ch)
                                        Debug.WriteLine(
                                            $"XXX {property.PropertyType} {o.PropertyType} {o.ComponentType}");
                                if (property.Converter.GetCreateInstanceSupported(new DD(property)))
                                    if (!creatable.Contains(property.PropertyType))
                                    {
                                        creatable.Add(property.PropertyType);
                                        Debug.WriteLine(property.PropertyType);
                                        var x = new D();
                                        var o = property.Converter.CreateInstance(x);
                                    }

                                var t = property.PropertyType;
                                Type at = null;
                                if (t.IsGenericType)
                                {
                                    var d = t.GetGenericTypeDefinition();
                                    if (d == typeof(Nullable<>)) at = t.GenericTypeArguments[0];
                                }

                                if (at != typeof(bool) && !(at?.IsEnum ?? false) &&
                                    property.PropertyType != typeof(bool) && property.PropertyType != typeof(Cursor) &&
                                    property.PropertyType != typeof(System.Globalization.CultureInfo) &&
                                    property.PropertyType.IsEnum == false &&
                                    property.Converter.GetStandardValuesSupported())
                                    if (!values.Contains(t))
                                    {
                                        var standardValues = property.Converter.GetStandardValues();

                                        if (standardValues.Count > 0)
                                        {
                                            Debug.WriteLine(property.ComponentType + " " + property.Name + " " +
                                                            property.PropertyType);
                                            values.Add(t);
                                            foreach (var standardValue in standardValues)
                                                Debug.WriteLine(standardValue);
                                        }
                                    }

                                {
                                    var e3 = property.GetEditor(typeof(UITypeEditor));
                                    if (e3 != null)
                                        editorTypes.Add(e3.GetType());
                                    if (e3 != null && e3.GetType() != typeof(CollectionEditor))
                                    {
                                        var paint = ((UITypeEditor) e3).GetPaintValueSupported();
                                        var kind = ((UITypeEditor) e3).GetEditStyle();
                                        Debug.WriteLine(
                                            $"{exportedType?.FullName} {paint} {kind} {property.Name} {property.Category} {e3}");
                                        if (paint) paints.Add(property);
                                    }
                                }
                                {
                                    var e3 = property.GetEditor(typeof(ComponentEditor));
                                    if (e3 != null)
                                        editorTypes.Add(e3.GetType());
                                    if (e3 != null && e3.GetType() != typeof(
                                        CollectionEditor))
                                    {
                                        comp.Add(Tuple.Create(property, e3));
                                        // var paint = ((UITypeEditor)e3).GetPaintValueSupported();
                                        // var kind = ((UITypeEditor)e3).GetEditStyle();
                                        Debug.WriteLine(
                                            $"!!!! {exportedType?.FullName}  {property.Name} {property.Category} {e3}");
                                    }
                                }
                                {
                                    var e3 = property.GetEditor(typeof(InstanceCreationEditor));
                                    if (e3 != null)
                                        editorTypes.Add(e3.GetType());
                                    if (e3 != null && e3.GetType() != typeof(CollectionEditor))
                                        // var paint = ((UITypeEditor)e3).GetPaintValueSupported();
                                        // var kind = ((UITypeEditor)e3).GetEditStyle();
                                        Debug.WriteLine(
                                            $"!!!! {exportedType?.FullName}  {property.Name} {property.Category} {e3}");
                                }
                            }
                            // var e2 = TypeDescriptor.GetEditor(exportedType, typeof(UITypeEditor));
                            // if (e2 != null)
                            // {
                            // Debug.WriteLine($"{exportedType} {e2}");
                            // }

                            var e1 = TypeDescriptor.GetEditor(exportedType, typeof(InstanceCreationEditor));
                            if (e1 != null)
                            {
                                editorTypes.Add(e1.GetType());

                                Debug.WriteLine("!!!" + e1);
                            }
                        }
                        catch
                        {
                        }
                }
                catch
                {
                }
            }

            var ll = paints.GroupBy(descriptor => descriptor.ComponentType.FullName).ToDictionary(
                grouping => grouping.Key,
                grouping => grouping.Select(pp => new
                    {CType = pp.ComponentType.FullName, pp.Name, PType = pp.PropertyType.FullName}));
            File.WriteAllText(@"c:\temp\paint.json", JsonSerializer.Serialize(ll));
            foreach (var editorType in editorTypes) Debug.WriteLine(editorType);
        }
    }

    internal class DD : ITypeDescriptorContext
    {
        public List<Type> req = new List<Type>();

        public DD(PropertyDescriptor propertyDescriptor)
        {
            PropertyDescriptor = propertyDescriptor;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IWindowsFormsEditorService)) return new Sz();
            Debug.WriteLine(serviceType.FullName);

            req.Add(serviceType);
            return null;
        }

        /// <inheritdoc />
        public bool OnComponentChanging()
        {
            return true;
        }

        /// <inheritdoc />
        public void OnComponentChanged()
        {
        }

        /// <inheritdoc />
        public IContainer Container { get; }

        /// <inheritdoc />
        public object Instance { get; }

        /// <inheritdoc />
        public PropertyDescriptor PropertyDescriptor { get; }
    }

    internal class Sz : IWindowsFormsEditorService
    {
        /// <inheritdoc />
        public void CloseDropDown()
        {
        }

        /// <inheritdoc />
        public void DropDownControl(Control control)
        {
        }

        /// <inheritdoc />
        public DialogResult ShowDialog(Form dialog)
        {
            return DialogResult.None;
        }
    }

    internal class D : IDictionary
    {
        private IDictionary _dictionaryImplementation = new Hashtable();

        /// <inheritdoc />
        public bool Contains(object key)
        {
            return _dictionaryImplementation.Contains(key);
        }

        /// <inheritdoc />
        public void Add(object key, object value)
        {
            _dictionaryImplementation.Add(key, value);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _dictionaryImplementation.Clear();
        }

        /// <inheritdoc />
        public IDictionaryEnumerator GetEnumerator()
        {
            return _dictionaryImplementation.GetEnumerator();
        }

        /// <inheritdoc />
        public void Remove(object key)
        {
            _dictionaryImplementation.Remove(key);
        }

        /// <inheritdoc />
        public object this[object key]
        {
            get
            {
                Debug.WriteLine($"{key}");
                return _dictionaryImplementation[key];
            }
            set { _dictionaryImplementation[key] = value; }
        }

        /// <inheritdoc />
        public ICollection Keys
        {
            get { return _dictionaryImplementation.Keys; }
        }

        /// <inheritdoc />
        public ICollection Values
        {
            get { return _dictionaryImplementation.Values; }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return _dictionaryImplementation.IsReadOnly; }
        }

        /// <inheritdoc />
        public bool IsFixedSize
        {
            get { return _dictionaryImplementation.IsFixedSize; }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _dictionaryImplementation).GetEnumerator();
        }

        /// <inheritdoc />
        public void CopyTo(Array array, int index)
        {
            _dictionaryImplementation.CopyTo(array, index);
        }

        /// <inheritdoc />
        public int Count
        {
            get { return _dictionaryImplementation.Count; }
        }

        /// <inheritdoc />
        public object SyncRoot
        {
            get { return _dictionaryImplementation.SyncRoot; }
        }

        /// <inheritdoc />
        public bool IsSynchronized
        {
            get { return _dictionaryImplementation.IsSynchronized; }
        }
    }

    public class Window2ViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Assembly> Assemblies { get; set; } = new ObservableCollection<Assembly>();
        private Type _selectedType;

        public Window2ViewModel()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomainOnAssemblyLoad;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) Assemblies.Add(assembly);
        }

        private void CurrentDomainOnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Debug.Write("Loaded " + args.LoadedAssembly.FullName);
            Assemblies.Add(args.LoadedAssembly);
        }

        public Type SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (Equals(value, _selectedType)) return;
                _selectedType = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}