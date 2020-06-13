using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using JetBrains.Annotations;
using Microsoft.Win32;
using Path = System.IO.Path;

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
    ///     <MyNamespace:FileInputBox/>
    ///
    /// </summary>
    public class FileInputBox : Control
    {
        public static readonly DependencyProperty FilenameProperty = DependencyProperty.Register(
            "Filename", typeof(string), typeof(FileInputBox), new PropertyMetadata(default(string), OnFilenameChanged));

        public string Filename
        {
            get { return (string) GetValue(FilenameProperty); }
            set { SetValue(FilenameProperty, value); }
        }

        private static void OnFilenameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FileInputBox) d).OnFilenameChanged((string) e.OldValue, (string) e.NewValue);
        }



        protected virtual void OnFilenameChanged(string oldValue, string newValue)
        {
        }

        private TextBox _te;

        public static readonly DependencyProperty FileSystemInfoProperty = DependencyProperty.Register(
            "FileSystemInfo", typeof(FileSystemInfo), typeof(FileInputBox), new PropertyMetadata(default(FileSystemInfo)));

        public FileSystemInfo FileSystemInfo
        {
            get { return (FileSystemInfo) GetValue(FileSystemInfoProperty); }
            set { SetValue(FileSystemInfoProperty, value); }
        }

        public static readonly DependencyProperty IsFileSelectedProperty = DependencyProperty.Register(
            "IsFileSelected", typeof(bool), typeof(FileInputBox), new PropertyMetadata(default(bool)));

        public bool IsFileSelected
        {
            get { return (bool) GetValue(IsFileSelectedProperty); }
            set { SetValue(IsFileSelectedProperty, value); }
        }
        static FileInputBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileInputBox),
                new FrameworkPropertyMetadata(typeof(FileInputBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Executed));
            _te = (TextBox) GetTemplateChild("TextBox");
        }

        private void Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var openFileDialog = new SaveFileDialog();
            
            var r = openFileDialog.ShowDialog().GetValueOrDefault();

            if (!r)
                return;
            var fileName = openFileDialog.FileName;
            _te.Text = fileName;
            Filename = fileName;
            IsFileSelected = true;
            // var fileAttributes = File.GetAttributes(fileName);
            // FileSystemInfo = ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory) ? (FileSystemInfo) new DirectoryInfo(fileName) : new FileInfo(fileName);
            
        } 

    }

    public class EnumFlagsSelector :Control
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(int), typeof(EnumFlagsSelector), new PropertyMetadata(default(int), OnValueChanged));

        public static readonly DependencyProperty EnumValueProperty = DependencyProperty.Register(
            "EnumValue", typeof(object), typeof(EnumFlagsSelector), new PropertyMetadata(default(Enum), OnEnumValueChanged));

        [CanBeNull]
        public object EnumValue
        {
            get { return  GetValue(EnumValueProperty); }
            set {  SetValue(EnumValueProperty, value); }
        }

        private static void OnEnumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EnumFlagsSelector) d).OnEnumValueChanged(e.OldValue, e.NewValue);
        }



        protected virtual void OnEnumValueChanged(object oldValue, object newValue)
        {
            Value = Convert.ToInt32(newValue);
        }

        public int Value
        {
            get { return (int) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EnumFlagsSelector) d).OnValueChanged((int) e.OldValue, (int) e.NewValue);
        }

        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register(
            "EnumType", typeof(Type), typeof(EnumFlagsSelector), new PropertyMetadata(default(Type), OnEnumTypeChanged));

        public Type EnumType
        {
            get { return (Type) GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); }
        }

        private static void OnEnumTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EnumFlagsSelector) d).OnEnumTypeChanged((Type) e.OldValue, (Type) e.NewValue);
        }

        private ObservableCollection<CheckableModelItem<object>> _internalItems = new ObservableCollection<CheckableModelItem<object>>();

        public static readonly DependencyProperty EditStyleProperty = DependencyProperty.Register(
            "EditStyle", typeof(EnumEditStyle), typeof(EnumFlagsSelector), new PropertyMetadata(default(EnumEditStyle), OnEditStyleChanged));

        public EnumEditStyle EditStyle
        {
            get { return (EnumEditStyle) GetValue(EditStyleProperty); }
            set { SetValue(EditStyleProperty, value); }
        }

        private static void OnEditStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EnumFlagsSelector) d).OnEditStyleChanged((EnumEditStyle) e.OldValue, (EnumEditStyle) e.NewValue);
        }



        protected virtual void OnEditStyleChanged(EnumEditStyle oldValue, EnumEditStyle newValue)
        {
        }

        protected virtual void OnEnumTypeChanged(Type oldValue, Type newValue)
        {
            _internalItems.Clear();
            foreach (var value in Enum.GetValues(newValue))
            {
                var checkableModelItem = new CheckableModelItem<object>( value);
                checkableModelItem.PropertyChanged += CheckableModelItemOnPropertyChanged;
                _internalItems.Add(checkableModelItem);
            }
            UpdateItemsChecked(Value);
        }

        private void CheckableModelItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (EnumType == null)
                return;
	Debug.WriteLine(nameof(CheckableModelItemOnPropertyChanged) + "." + e.PropertyName);	
            if (e.PropertyName == nameof(CheckableModelItem<object>.IsChecked))
            {
                var modelItem = (CheckableModelItem<object>)sender;
	Debug.WriteLine($"modelItem = {modelItem}");
                var item = modelItem.Item;
                var int32 = Convert.ToInt32(item);
		Debug.WriteLine("int val for "  + item + " is " + int32);
                if (modelItem.IsChecked) {
                
		Debug.WriteLine($"Value = {Value:X} | {int32:X} = {Value | int32:X}");
                    Value = int32 != 0 ? (Value | int32) : 0;
                }
                else
                {
		Debug.WriteLine($"Value = {Value:X} & {~((int)item):X} = {Value & ~((int)item):X}");
                    Value = Value & ~((int)item);
                }
            }
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items", typeof(IEnumerable), typeof(EnumFlagsSelector), new PropertyMetadata(default(IEnumerable), OnItemsChanged));

        public IEnumerable Items
        {
            get { return (IEnumerable) GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EnumFlagsSelector) d).OnItemsChanged((IEnumerable) e.OldValue, (IEnumerable) e.NewValue);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(EnumFlagsSelector), new PropertyMetadata(default(string), OnTextChanged));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EnumFlagsSelector) d).OnTextChanged((string) e.OldValue, (string) e.NewValue);
        }



        protected virtual void OnTextChanged(string oldValue, string newValue)
        {
        }


        protected virtual void OnItemsChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            // if (Value == 0)
            // {
                UpdateItemsChecked(Value);
            // }
            // Value = 0;

        }



        protected virtual void OnValueChanged(int oldValue, int newValue)
        {
            if (EnumType != null)
            {
                UpdateItemsChecked(newValue);
                EnumValue = (Enum) Enum.Parse(EnumType, newValue.ToString());
                Text = EnumValue.ToString();
            }
        }

        private void UpdateItemsChecked(int newValue)
        {
            if (EnumType == null)
                return;
            if (newValue == 0)
            {
                foreach (CheckableModelItem<object> item in Items)
                {
                    var vv = Enum.Parse(EnumType, item.Item.ToString());
                    item.IsChecked = (int) vv == 0;
                }

                return;
            }

            var v = (Enum) Enum.Parse(EnumType, newValue.ToString());
            Debug.WriteLine(v);
            foreach (var value in Enum.GetValues(EnumType))
            {
                var @enum = value as Enum;
            Debug.WriteLine(@enum
	    );
            // if(((int)value == 0)) { 
                    // continue;
                var checkableModelItem = Items.Cast<CheckableModelItem<object>>().FirstOrDefault(x=>x.Item.Equals(@enum));
            Debug.WriteLine(checkableModelItem);
                if ((int)value != 0 && v.HasFlag(@enum))
                {
                    if (checkableModelItem != null) checkableModelItem.IsChecked = true;
                }
                else
                {
                    if (checkableModelItem != null) {
		                Debug.WriteLine("Setting ischecked to false for " + @enum);
checkableModelItem.IsChecked = false;
}
                }
            }
        }


        static EnumFlagsSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumFlagsSelector),
                new FrameworkPropertyMetadata(typeof(EnumFlagsSelector)));
        }

        /// <inheritdoc />
        public EnumFlagsSelector()

        {
            Items = _internalItems;
        }
    }

    public enum EnumEditStyle
    {
        ComboBox,
        ItemsControl
    }
}
