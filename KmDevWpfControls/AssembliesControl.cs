using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KmDevWpfControls
{
    /// <summary>
    /// Generic "Assemblies list view" control.
    /// </summary>
    public class AssembliesControl : Control
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AssemblySourceProperty = DependencyProperty.Register(
            "AssemblySource", typeof(IEnumerable<Assembly>), typeof(AssembliesControl),
            new PropertyMetadata(default(IEnumerable<Assembly>)));

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Assembly> AssemblySource
        {
            get { return (IEnumerable<Assembly>) GetValue(AssemblySourceProperty); }
            set { SetValue(AssemblySourceProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedAssemblyProperty = DependencyProperty.Register(
            "SelectedAssembly", typeof(Assembly), typeof(AssembliesControl), new PropertyMetadata(default(Assembly)));

        /// <summary>
        /// 
        /// </summary>
        public Assembly SelectedAssembly
        {
            get { return (Assembly) GetValue(SelectedAssemblyProperty); }
            set { SetValue(SelectedAssemblyProperty, value); }
        }

        static AssembliesControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AssembliesControl),
                new FrameworkPropertyMetadata(typeof(AssembliesControl)));
        }

        public AssembliesControl()
        {
            AssemblySource = AppDomain.CurrentDomain.GetAssemblies();
        }
    }

    public class DropControl:Control
    {
        private IDataObject _data;

        static DropControl()
        {
            
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropControl),
                new FrameworkPropertyMetadata(typeof(DropControl)));
            
        }

        public static readonly DependencyProperty FormatsProperty = DependencyProperty.Register(
            "Formats", typeof(IEnumerable), typeof(DropControl), new PropertyMetadata(default(IEnumerable), OnFormatsChanged));

        public IEnumerable Formats
        {
            get { return (IEnumerable) GetValue(FormatsProperty); }
            set { SetValue(FormatsProperty, value); }
        }

        private static void OnFormatsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DropControl) d).OnFormatsChanged((IEnumerable) e.OldValue, (IEnumerable) e.NewValue);
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data", typeof(IEnumerable), typeof(DropControl), new PropertyMetadata(default(IEnumerable), OnDataChanged));

        private ObservableCollection<DataInfo> _list1;

        public IEnumerable Data
        {
            get { return (IEnumerable) GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DropControl) d).OnDataChanged((IEnumerable) e.OldValue, (IEnumerable) e.NewValue);
        }



        protected virtual void OnDataChanged(IEnumerable oldValue, IEnumerable newValue)
        {
        }



        protected virtual void OnFormatsChanged(IEnumerable oldValue, IEnumerable newValue)
        {
        }

        /// <inheritdoc />
        public DropControl()
        {
            _list1 = new ObservableCollection<DataInfo>();
            Data = _list1;
            EventManager.RegisterClassHandler(typeof(Window), DragDrop.PreviewDragOverEvent, new DragEventHandler(Target));
        }

        private void Target(object sender, DragEventArgs e)
        {
            if (_data != e.Data)
            {
                _data = e.Data;
                Formats = _data.GetFormats().ToList();
                _list1.Clear();
                foreach (string format in _data.GetFormats())
                {
                    try
                    {
                        var d = _data.GetData(format);
                        var di = new DataInfo() {Data = d, Format = format, Type = d.GetType()};
                        if (d is MemoryStream ms)
                        {
                            di.Length = ms.Length;
                            if(format == "DragImageBits")
                            try
                            {
                   
             MemoryStream imageStream = ms;
                                imageStream.Seek(0, SeekOrigin.Begin);
                                BinaryReader br = new BinaryReader(imageStream);
                                ShDragImage shDragImage;
                                shDragImage.sizeDragImage.cx = br.ReadInt32();
                                shDragImage.sizeDragImage.cy = br.ReadInt32();
                                shDragImage.ptOffset.x = br.ReadInt32();
                                shDragImage.ptOffset.y = br.ReadInt32();
                                shDragImage.hbmpDragImage = new IntPtr(br.ReadInt32()); // I do not know what this is for!
                                shDragImage.crColorKey = br.ReadInt32();
                                int stride = shDragImage.sizeDragImage.cx * 4;
                                var imageData = new byte[stride * shDragImage.sizeDragImage.cy];
                                // We must read the image data as a loop, so it's in a flipped format
                                for (int i = (shDragImage.sizeDragImage.cy - 1) * stride; i >= 0; i -= stride)
                                {
                                    br.Read(imageData, i, stride);
                                }
                                var bitmapSource = BitmapSource.Create(shDragImage.sizeDragImage.cx, shDragImage.sizeDragImage.cy,
                                    96, 96,
                                    PixelFormats.Bgra32,
                                    null,
                                    imageData,
                                    stride);
                                di.ImageSource = bitmapSource;
                            } catch{}
                            else
                            {
                                byte[] buffer = new byte[ms.Length];
                                ms.Read(buffer, 0, (int)ms.Length);
                                string html;
                                if (buffer[1] == (byte)0)  // Detecting unicode
                                {
                                    html = System.Text.Encoding.Unicode.GetString(buffer);
                                }
                                else
                                {
                                    html = System.Text.Encoding.ASCII.GetString(buffer);
                                }

                                di.Text = html;
                            }
                        }
                        _list1.Add(di);
                    }
                    catch
                    {

                    }
                }
            }
        }
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Win32Point
        {
            public int x;
            public int y;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Win32Size
        {
            public int cx;
            public int cy;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ShDragImage
        {
            public Win32Size sizeDragImage;
            public Win32Point ptOffset;
            public IntPtr hbmpDragImage;
            public int crColorKey;
        }
    }

    public class DataInfo
    {
        public string Format { get; set; }
        public object Data { get; set; }
        public Type Type { get; set; }
        public long Length { get; set; }
        public ImageSource ImageSource { get; set; }
        public string Text { get; set; }
    }
}