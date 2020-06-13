using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KmDevWpfControls
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class CustomTreeView : TreeView
    {
        public static RoutedUICommand ToggleNodeIsExpanded = new RoutedUICommand("Toggle node isexpanded",
            nameof(ToggleNodeIsExpanded), typeof(CustomTreeView));
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomTreeViewItem();
        }

        protected override bool ExpandSubtree(TreeViewItem container)
        {
            return base.ExpandSubtree(container);
        }

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            base.OnSelectedItemChanged(e);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return base.IsItemItsOwnContainerOverride(item);
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
        }

        protected override void OnItemContainerStyleChanged(Style oldItemContainerStyle, Style newItemContainerStyle)
        {
            base.OnItemContainerStyleChanged(oldItemContainerStyle, newItemContainerStyle);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
        }

        protected override bool ShouldApplyItemContainerStyle(DependencyObject container, object item)
        {
            return base.ShouldApplyItemContainerStyle(container, item);
        }
    }
}