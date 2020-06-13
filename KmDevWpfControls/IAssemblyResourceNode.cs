using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace KmDevWpfControls
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAssemblyResourceNode : ITreeViewNode
    {
        /// <summary>
        /// 
        /// </summary>
        Assembly Assembly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        object Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        //ObservableCollection<IAssemblyResourceNode> Items { get; }

        /// <summary>
        /// 
        /// </summary>
        NodeExpandedState1 ExpandedState { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsExpanded { get; }

        /// <summary>
        /// 
        /// </summary>
        NodeDataLoadState1 DataState { get; set; }

        int ItemsCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IDataObject> CheckLoadItemsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        void LoadResult(IDataObject result);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void SetIsExpanded(bool value);

        /// <summary>
        /// 
        /// </summary>
        Task ExpandAsync();

    }
}