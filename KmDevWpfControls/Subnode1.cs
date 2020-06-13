using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Baml2006;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KmDevWpfControls
{
    /// <summary>
    /// 
    /// </summary>
    public class Subnode1 : AssemblyResourceNodeBase1
    {
        private Task<IDataObject> _loadTask2;

        public Subnode1(bool addPlaceholder = true) : base(addPlaceholder)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object ResourceName { get; set; }

        /// <inheritdoc />
        public override bool CheckLoadItems(out NodeDataLoadState1 state)
        {
            state = DataState;
            switch (DataState)
            {
                case NodeDataLoadState1.NoAction:
                    return true;
                case NodeDataLoadState1.DataLoaded:
                    return true;
                case NodeDataLoadState1.RequiresAsync:
                    return false;
                case NodeDataLoadState1.LoadFailure:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public override DataLoadStrategy LoadStrategy { get; set; }

        /// <inheritdoc />
        // public override Task<IDataObject> CheckLoadItemsAsync()
        // {

        public override Task<IDataObject> CheckLoadItemsAsync()
        {
            if (_loadTask2 != null && _loadTask2.Status <= TaskStatus.Running)
                throw new InvalidOperationException("error");
            Items.Clear();
            _loadTask2 = Task.Run(LoadResourceData,
                CancellationToken.None);
            return _loadTask2;
        }

            // if (_loadTask2 != null && _loadTask2.Status <= TaskStatus.Running)
                // throw new InvalidOperationException("error");
            // Items.Clear();
            // _loadTask2 = Task.Factory.StartNew(LoadResourceData,
                // new TaskState<Subnode1>(this), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            // return _loadTask2;
        // }

        /// <inheritdoc />
        public override void LoadResult(IDataObject result)
        {
            Debug.WriteLine(result.ToString());
            foreach (var format in result.GetFormats())
            {
                switch (format)
                {
                    case "System.Windows.Media.Imaging.BitmapSource":
                        var v = new Border();
                        v.Child = new Image()
                        {
                            Source = result.GetData(format) as ImageSource,
                            HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center
                        };
                        Items.Add(new Subnode1(false) {Name = v});
                        break;
                    case nameof(IEnumerable):
                        var ee = result.GetData(format) as IEnumerable;
                        foreach (var o in ee)
                            if (o is SubnodeData sd)
                                Items.Add(new Subnode1() {Name = sd.Name, Value = sd.Value});
                        break;
                }
            }
        }

        private async Task<IDataObject>  LoadResourceData()
        {
            var subnode = this;
            Debug.WriteLine($"{nameof(TempLoadDat)}: {subnode}");
            if (subnode.
                ResourceName== null) throw new InvalidOperationException("ResourceName is null");
            if (subnode.Assembly == null) throw new InvalidOperationException("Assembly is null");
            var stream = subnode.Assembly.GetManifestResourceStream(subnode.ResourceName.ToString());

            if (stream == null) return null;
            using (var reader = new ResourceReader(stream))
            {
                reader.GetResourceData(subnode.Name.ToString(), out var dataType, out var data);
                switch (dataType)
                {
                    // Handle internally serialized string data (ResourceTypeCode members).
                    case "ResourceTypeCode.String":
                        var binaryReader = new BinaryReader(new MemoryStream(data));
                        var binData = binaryReader.ReadString();
                        Debug.WriteLine($"   Recreated Value: {binData}");
                        return MakeValue(binData);

                    case "ResourceTypeCode.Stream":
                        var offset = 4;

                        var size = BitConverter.ToInt32(data, 0);
                        var memoryStream = new MemoryStream(data, offset, size);
                        subnode.MemoryStream = memoryStream;
                        var ext = Path.GetExtension(subnode.Name.ToString()).ToLowerInvariant();
                        Debug.WriteLine(ext);
                        switch (ext)
                        {
                            case ".baml":
                                return await subnode.MakeBamlValue(subnode, memoryStream);
                            case ".png":
                                return await subnode.MakeBitmapValue();


                            case ".jpg":
                            case ".jpeg":
                            case ".gif":
                            case ".ico":

                                break;
                        }

                        return MakeStreamValue(size, memoryStream, data);
                }
            }

            return null;
        }

        public MemoryStream MemoryStream { get; set; }

        public delegate object InvocationDelegate();

        private async Task<IDataObject> MakeBamlValue(AssemblyResourceNodeBase1 subnode, MemoryStream memoryStream)
        {


            var ary = new object[] {memoryStream};
            var o = await subnode.Dispatcher.InvokeAsync(TempLoadDat, DispatcherPriority.Send);
            return (IDataObject) o;
        }

        private object HandleBamlResource(MemoryStream memoryStream, Subnode1 subnode)
        {
            object object1 = null;
            if (object1 is IDictionary rd)
            {
                try
                {
                    foreach (DictionaryEntry entry in rd)
                        try
                        {
                            var sb = new Subnode1() {Name = entry.Key, Value = entry.Value};
                            subnode.Items.Add(sb);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                            return new DataObject(typeof(Exception), ex);
                        }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return new DataObject(typeof(Exception).FullName, ex);
                }

                // ReSharper disable once AssignNullToNotNullAttribute
                return new DataObject(typeof(ResourceDictionary).FullName, object1);
            }

            return null;
        }

        private async Task<IDataObject> MakeBitmapValue()
        {
            var object3 = await Dispatcher.InvokeAsync(() =>
                {
                    Debug.WriteLine(".png");
                    
                    var png = new PngBitmapDecoder(MemoryStream, BitmapCreateOptions.None,
                        BitmapCacheOption.Default);
                    BitmapSource src = png.Frames[0];
                    var s = new SubnodeData();
                    s.Value = src;
                    s.Name = "Image";
                    return new DataObject(typeof(BitmapSource), src);
                },
            DispatcherPriority.Send);

            return object3;
        }

        private static IDataObject MakeStreamValue(int size, MemoryStream memoryStream, byte[] data)
        {
            return new DataObject("MemoryStream", memoryStream);
        }

        private static IDataObject MakeValue(string binData)
        {
            return new DataObject(nameof(String), binData);
        }


        private IDataObject TempLoadDat()
        {
            {
                object object1;
                using (Baml2006Reader reader1 = new Baml2006Reader(this.MemoryStream))
                {
                  try
                    {
                        object1 = XamlReader.Load(reader1);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        throw;
                    }
                }

                return new DataObject("BAML", object1);
            }
        }
    }
}