using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace KmDevWpfControls
{
    
    public class VisualTreeView : Control
    {
        public static readonly DependencyProperty SelectedVisualTreeNodeProperty = DependencyProperty.Register(
            "SelectedVisualTreeNode", typeof(VisualTreeNode), typeof(VisualTreeView),
            new PropertyMetadata(default(Visual), new PropertyChangedCallback(Target)));

        private static void Target(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VisualTreeView) d).OnSelectedVisualTreeNodeChanged(e.OldValue as VisualTreeNode,
                e.NewValue as VisualTreeNode);
        }
    

    private void OnSelectedVisualTreeNodeChanged(VisualTreeNode old, VisualTreeNode @new)
        {
            var tvi = _treeView.ItemContainerGenerator.ContainerFromItem(@new)
                as TreeViewItem;

            if (tvi != null)
            {
                tvi.IsSelected = true;
            }
        }

        public VisualTreeNode SelectedVisualTreeNode
        {
            get { return (VisualTreeNode) GetValue(SelectedVisualTreeNodeProperty); }
            set { SetValue(SelectedVisualTreeNodeProperty, value); }
        }
        public ObservableCollection<VisualTreeNode> InternalRootItems { get; } = new ObservableCollection<VisualTreeNode>();

        public static readonly DependencyProperty RootItemsProperty = DependencyProperty.Register(
            "RootItems", typeof(IEnumerable), typeof(VisualTreeView), new PropertyMetadata(default(IEnumerable)));

        public IEnumerable RootItems
        {
            get { return (IEnumerable) GetValue(RootItemsProperty); }
            set { SetValue(RootItemsProperty, value); }
        }

        public static readonly DependencyProperty RootVisualProperty = DependencyProperty.Register(
            "RootVisual", typeof(Visual), typeof(VisualTreeView), new PropertyMetadata(default(Visual)));

        private VisualConverter _vc;
        private TreeView _treeView;
        private CustomTreeView _ctreeView;

        public Visual RootVisual
        {
            get { return (Visual) GetValue(RootVisualProperty); }
            set { SetValue(RootVisualProperty, value); }
        }
        public VisualTreeView()
        {
//            SetBinding(RootItemsProperty, new Binding("RootVisual") {Source = this,Converter = _vc});
//            RootItems.Add(new VisualTreeNode{Visual = Window.GetWindow(this)});
            CommandBindings.Add(new CommandBinding(CustomTreeView.ToggleNodeIsExpanded, OnToggleExecuted));
        }

        private async void OnToggleExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Debug.WriteLine("Received expand node command with param " + e.Parameter);
            try
            {
                if (!(e.Parameter is CustomTreeViewItem cc))
                {
                    Debug.WriteLine("PArameter is not CustomTreeViewItem");
                    return;
                }

                if (cc.IsExpanded)
                    cc.Collapse();
                else
                {
                    SetCurrentValue(Control.CursorProperty, Cursors.Wait);
                    Dispatcher.BeginInvoke(new Action(() => ClearValue(CursorProperty)), DispatcherPriority.ContextIdle, null);
                    await cc.ExpandAsync();
                    Debug.WriteLine("return from expand async");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // ignored
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _vc = TryFindResource("VisualConverter") as VisualConverter;
            _ctreeView = GetTemplateChild("TreeView") as CustomTreeView;
            _treeView = GetTemplateChild("TreeView") as TreeView;
            if (_treeView != null) _treeView.SelectedItemChanged += TreeViewOnSelectedItemChanged;
        }

        private void TreeViewOnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedVisualTreeNode = (VisualTreeNode) e.NewValue;
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            var source = DependencyPropertyHelper.GetValueSource(this, RootVisualProperty);
            if (source.IsExpression && source.BaseValueSource == BaseValueSource.Local)
                return;
            if (source.BaseValueSource != BaseValueSource.Default && source.BaseValueSource != BaseValueSource.Local &&
                source.BaseValueSource != BaseValueSource.Inherited)
            {
                return;

            }

            InternalRootItems.Clear();
            var window = Window.GetWindow(this);
            Visual v = window;
            if (window == null)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                for (v = (Visual) VisualParent; VisualTreeHelper.GetParent(v) != null; v = (Visual) VisualTreeHelper.GetParent(v)) ;
            }
            InternalRootItems.Add(new VisualTreeNode { Visual = v,
                TransformToSource = v});
        }


        static VisualTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VisualTreeView), new FrameworkPropertyMetadata(typeof(VisualTreeView)));
        }
    }
}