using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KmDevWpfControls
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomTreeViewItem : TreeViewItem
    {
        // ReSharper d  isable once UnusedMember.Local
        TreeView ParentTreeView
        {
            get
            {
                for (ItemsControl itemsControl = this.ParentItemsControl; itemsControl != null; itemsControl = ItemsControl.ItemsControlFromItemContainer((DependencyObject)itemsControl))
                {
                    if (itemsControl is TreeView treeView)
                        return treeView;
                }
                return (TreeView)null;
            }
        }

        /// <inheritdoc />
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!e.Handled)
                if (e.LeftButton == MouseButtonState.Pressed)
                    DragDrop.DoDragDrop(this, ParentItemsControl.ItemContainerGenerator.ItemFromContainer(this),
                        DragDropEffects.Copy);
        }

        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public static readonly RoutedEvent ExpandingEvent = EventManager.RegisterRoutedEvent("Expanded",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomTreeViewItem));

        /// <summary>Identifies the <see cref="E:System.Windows.Controls.TreeViewItem.Collapsed" /> routed event. </summary>
        protected override void OnExpanded(RoutedEventArgs e)
        {
            base.OnExpanded(e);
        }

        protected override void OnCollapsed(RoutedEventArgs e)
        {
            base.OnCollapsed(e);
        }

        private static bool IsControlKeyDown
        {
            get { return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control; }
        }

        /// <inheritdoc />
        // ReSharper disable once UnusedMember.Global
#pragma warning disable 108,114
        protected override async void OnKeyDown(KeyEventArgs e)
#pragma warning restore 108,114
        {
            switch (e.Key)
            {
                case Key.Left when IsControlKeyDown || !CanExpandOnInput || !IsExpanded:
                    return;
                case Key.Left:
                {
                    if (IsFocused)
                        Collapse();
                    else
                        Focus();
                    e.Handled = true;
                    return;
                }
                case Key.Right when IsControlKeyDown || !CanExpandOnInput:
                    break;
                case Key.Right:
                    if (!IsExpanded)
                    {
                        e.Handled = true;
                        await ExpandAsync();
                    }

                    break;
                case Key.Add:
                    if (!CanExpandOnInput || IsExpanded)
                        break;
                    e.Handled = true;
                    await ExpandAsync();
                    break;
                case Key.Subtract:
                    if (!CanExpandOnInput || !IsExpanded)
                        break;
                    Collapse();
                    e.Handled = true;
                    break;
                default:
                    base.OnKeyDown(e);
                    return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled && IsEnabled)
                if (e.ClickCount % 2 == 0)
                {
                    e.Handled = true;
                    if (IsExpanded)
                        Collapse();
                    else
                        await ExpandAsync();
                    return;
                }

            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task ExpandAsync()
        {
            var item = ParentItemsControl.ItemContainerGenerator.ItemFromContainer(this);
            if (item is IAssemblyResourceNode d)
                await d.ExpandAsync();
            else if(item is IAsyncExpand i)
                await i.ExpandAsync();
            else
                Debug.WriteLine($"{item}");
        }

        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public bool CanExpandOnInput
        {
            get { return HasItems && IsEnabled; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Collapse()
        {
            var item = ParentItemsControl.ItemContainerGenerator.ItemFromContainer(this);
            if (item is IAssemblyResourceNode d)
                d.Collapse();
            else if (item is ITreeViewItemCollapse i)
                i.Collapse();
            else
                Debug.WriteLine($"{item}");
        }

        private ItemsControl ParentItemsControl
        {
            get { return ItemsControlFromItemContainer(this); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomTreeViewItem();
        }
    }

}