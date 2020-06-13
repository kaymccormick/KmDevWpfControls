using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace KmDevWpfControls
{
    /// <summary>
    /// 
    /// </summary>
    public class RootNode1 : AssemblyResourceNodeBase1
    {
        private Task<IDataObject> _loadTask2;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool CheckLoadItems(out NodeDataLoadState1 state)
        {
            state = NodeDataLoadState1.RequiresAsync;
            return false;
        }

        /// <inheritdoc />
        public override DataLoadStrategy LoadStrategy { get; set; }

        /// <inheritdoc />
        public override Task<IDataObject> CheckLoadItemsAsync()
        {
            if (_loadTask2 != null && _loadTask2.Status <= TaskStatus.Running)
                throw new InvalidOperationException("error");
            Items.Clear();
            _loadTask2 = Task.Factory.StartNew(LoadResources,
                new TaskState<RootNode1>
                    (this), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            return _loadTask2;
        }

        private static IDataObject LoadResources(object state1)
        {
            var st = (TaskState<RootNode1>) state1;
            var node = st.Node;
            try
            {
                var dta = new List<SubnodeData>();
                using (var stream = node.Assembly.GetManifestResourceStream(node.Name.ToString()))
                {
                    if (stream != null && node.Name.ToString().EndsWith(".resources"))
                        using (var reader = new ResourceReader(stream))
                        {
                            foreach (var dictionaryEntry in reader.Cast<DictionaryEntry>())
                            {
                                var value = dictionaryEntry.Value;
                                if (((string) dictionaryEntry.Key).EndsWith(".png"))
                                    Debug.WriteLine(dictionaryEntry.Value.ToString());
                                try
                                {
                                    var data = new SubnodeData
                                    {
                                        Name = dictionaryEntry.Key as string,
                                        Value = value,
                                        ResourceName = node.Name,
                                        Assembly = node.Assembly
                                    };
                                    dta.Add(data);
                                }
                                catch (Exception ex)
                                {
                                    // DebugUtils.WriteLine(ex.ToString());
                                }
                            }
                        }
                }

                return new DataObject(typeof(IEnumerable), dta);
            }
            catch (Exception ex)
            {
                // DebugUtils.WriteLine(ex.ToString());
                throw;
            }
        }

        /// <inheritdoc />
        public override void LoadResult(IDataObject result)
        {
            // DebugUtils.WriteLine("Loading data");
            Items.Clear();
            var x = (IEnumerable) result.GetData(typeof(IEnumerable));
            if (x != null)
                foreach (var o in x)
                {
                    var subnodeData = (SubnodeData) o;
                    //DebugUtils.WriteLine("Adding " + subnodeData.Name);
                    Items.Add(new Subnode1()
                    {
                        Assembly = subnodeData.Assembly, Name = subnodeData.Name,
                        ResourceName = subnodeData.ResourceName
                    });
                }

            //DebugUtils.WriteLine("Setting data loaded");
            DataState = NodeDataLoadState1.DataLoaded;
        }
    }

    public enum DataLoadStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        None,

        /// <summary>
        /// 
        /// </summary>
        LoadAsync,

        /// <summary>
        /// 
        /// </summary>
        LoadSync
    }

    public class SubnodeData

    {
        /// <summary>
        /// 
        /// </summary>
        public object Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public object ResourceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Assembly Assembly { get; set; }

        public object Value { get; set; }
    }

    public class TaskState<T> where T : IAssemblyResourceNode
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subnodeData"></param>
        public TaskState(T subnodeData)
        {
            Node = subnodeData;
        }

        /// <summary>
        /// 
        /// </summary>
        public T Node { get; set; }
    }
}