using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace KmDevWpfControls
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AssemblyResourceNodeBase1 : INotifyPropertyChanged, IAssemblyResourceNode
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Task<TaskScheduler> GetScheduler(Dispatcher d)
        {
            var schedulerResult = new TaskCompletionSource<TaskScheduler>();
            d.BeginInvoke(new Action(() =>
                schedulerResult.SetResult(
                    TaskScheduler.FromCurrentSynchronizationContext())));
            return schedulerResult.Task;
        }

        private NodeExpandedState1 _expandedState = NodeExpandedState1.Indeterminate;
#pragma warning disable 169
        private Task<TempLoadData1> _loadTask;
#pragma warning restore 169
        private Dispatcher _dispatcher;
        private TaskScheduler _taskScheduler;

        private readonly ObservableCollection<IAssemblyResourceNode> _items =
            new ObservableCollection<IAssemblyResourceNode>();

        private NodeDataLoadState1 _dataState;

        /// <summary>
        /// 
        /// </summary>
        protected AssemblyResourceNodeBase1(bool addPlaceholder = true)
        {
            if(addPlaceholder)
            _items?.Add(new AssemblyResourceNodesPlaceHolder());
            Dispatcher = Dispatcher.CurrentDispatcher;
            _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Assembly Assembly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public ResourceLocation ResourceLocation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Assembly ReferencedAssembly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public virtual ObservableCollection<IAssemblyResourceNode> Items
        {
            get { return _items; }
        }

        public bool IsSelected { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual NodeExpandedState1 ExpandedState
        {
            get { return _expandedState; }
            protected set
            {
                if (value == _expandedState) return;
                _expandedState = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public abstract bool CheckLoadItems(out NodeDataLoadState1 state);

        public object Header => Name;

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsExpanded
        {
            get { return ExpandedState == NodeExpandedState1.Expanded; }
        }

        IEnumerable ITreeViewNode.Items
        {
            get { return Items; }
        }

        /// <inheritdoc />
        public virtual void SetIsExpanded(bool value)
        {
            Debug.WriteLine("Setting expanded.");

            ExpandedState = value ? NodeExpandedState1.Expanded : NodeExpandedState1.Collapsed;
        }

        /// <inheritdoc />
        public virtual async Task ExpandAsync()
        {
            Debug.WriteLine($"InternalItems has any {Items.Any()} and ExpandedState is {ExpandedState}");
            if (Items.Any() && ExpandedState != NodeExpandedState1.Expanded)
            {
                Debug.WriteLine("Data state is " + DataState);
                if (DataState != NodeDataLoadState1.DataLoaded)
                {
                    Debug.WriteLine("Attempting to load");
                    IDataObject result = null;
                    try
                    {
                        result = await CheckLoadItemsAsync();
                        Debug.WriteLine("return from async");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }

                    if (result != null) LoadResult(result);

                    if (!Items.Any()) return;

                    Debug.WriteLine("expanded is " + ExpandedState);
                }

                SetIsExpanded(true);
            }
        }

        /// <inheritdoc />
        public virtual void Collapse()
        {
            if (ExpandedState != NodeExpandedState1.Collapsed) SetIsExpanded(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        protected static void SetExpandedAction(Task<TempLoadData1> arg1, object arg2)
        {
            var state = (TaskState<AssemblyResourceNodeBase1>) arg2;
            Debug.WriteLine(nameof(SetExpandedAction));
            state.Node.Items.Clear();
            state.Node.ExpandedState = NodeExpandedState1.Expanded;
        }

        public int ItemsCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Task<IDataObject> CheckLoadItemsAsync();

        /// <inheritdoc />
        public abstract void LoadResult(IDataObject result);

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 
        /// </summary>
        public NodeDataLoadState1 DataState
        {
            get { return _dataState; }
            set
            {
                if (value == _dataState) return;
                _dataState = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Dispatcher Dispatcher
        {
            get { return _dispatcher; }
            set { _dispatcher = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract DataLoadStrategy LoadStrategy { get; set; }
    }

    public class AssemblyResourceNodesPlaceHolder : IAssemblyResourceNode
    {
        public void Collapse()
        {
            
        }

        public object Header { get; }
        public NodeExpandedState1 ExpandedState { get; }
        public bool IsExpanded { get; }
        public NodeDataLoadState1 DataState { get; set; }
        public int ItemsCount { get; set; }

        public Task<IDataObject> CheckLoadItemsAsync()
        {
            throw new NotImplementedException();
        }

        public void LoadResult(IDataObject result)
        {
            throw new NotImplementedException();
        }

        public void SetIsExpanded(bool value)
        {
            throw new NotImplementedException();
        }

        public Task ExpandAsync()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<IAssemblyResourceNode> Items { get; }
        public bool IsSelected { get; set; }
        public Assembly Assembly { get; set; }
        public object Name { get; set; }

        IEnumerable ITreeViewNode.Items
        {
            get { return Items; }
        }
    }
}