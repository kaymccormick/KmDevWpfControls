using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace KmDevWpfControls
{
    /// <summary>
    /// 
    /// </summary>
    public class AssemblyResourceTree1 : Control
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AssemblyProperty = DependencyProperty.Register(
            "Assembly", typeof(Assembly), typeof(AssemblyResourceTree1),
            new PropertyMetadata(default(Assembly), _OnAssemblyUpdated));

        private static void _OnAssemblyUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AssemblyResourceTree1) d).OnAssemblyUpdated((Assembly) e.OldValue,
                (Assembly) e.NewValue);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RootNodesProperty = DependencyProperty.Register(
            "RootNodes", typeof(ObservableCollection<AssemblyResourceNodeBase1>), typeof(AssemblyResourceTree1),
            new PropertyMetadata(default(ObservableCollection<AssemblyResourceNodeBase1>)));

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<AssemblyResourceNodeBase1> RootNodes
        {
            get { return (ObservableCollection<AssemblyResourceNodeBase1>) GetValue(RootNodesProperty); }
            set { SetValue(RootNodesProperty, (object) value); }
        }

        // ReSharper disable once UnusedParameter.Local
        private void OnAssemblyUpdated(Assembly old, Assembly newVal)
        {
            PopulateResourceTree(newVal);
        }

        private void PopulateResourceTree(Assembly assembly)
        {
            RootNodes.Clear();
            var resourceNames = assembly.GetManifestResourceNames();
            foreach (var resourceName in resourceNames)
            {
                var info = assembly.GetManifestResourceInfo(resourceName);
                if (info != null)
                {
                    var data = new RootNode1
                    {
                        Assembly = assembly,
                        FileName = info.FileName,
                        ResourceLocation = info.ResourceLocation,
                        ReferencedAssembly = info.ReferencedAssembly,
                        Name = resourceName
                    };
                    RootNodes.Add(data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public ObservableCollection<AssemblyResourceNodeBase1> RootItems { get; } = new ObservableCollection<AssemblyResourceNodeBase1>();

        /// <summary>
        /// 
        /// </summary>
        public Assembly Assembly
        {
            get { return (Assembly) GetValue(AssemblyProperty); }
            set { SetValue(AssemblyProperty, value); }
        }

        static AssemblyResourceTree1()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AssemblyResourceTree1),
                new FrameworkPropertyMetadata(typeof(AssemblyResourceTree1)));
        }

        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly RoutedEvent SelectedItemChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedItemChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<object>), typeof(AssemblyResourceTree1));

        private TreeView _treeView;

        /// <summary>
        /// 
        /// </summary>
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged
        {
            add { AddHandler(SelectedItemChangedEvent, value); }
            remove { RemoveHandler(SelectedItemChangedEvent, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public AssemblyResourceTree1()
        {
            AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(OnExpanded) );
            RootNodes = new ObservableCollection<AssemblyResourceNodeBase1>();
            CommandBindings.Add(new CommandBinding(CustomTreeView.ToggleNodeIsExpanded, OnExpandNodeExecutedAsync,
                OnToggleNodeCanExecute));
        }

        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            //ClearValue(CursorProperty);
        }

        private async void OnExpandNodeExecutedAsync(object sender, ExecutedRoutedEventArgs e)
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

        private void OnToggleNodeCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (!(e.Parameter is DependencyObject dependencyObject)) return;

                var itemFromContainer = _treeView.ItemContainerGenerator.ItemFromContainer(dependencyObject);
                if (!(itemFromContainer is IAssemblyResourceNode node)) return;
                //Debug.WriteLine("param is " + node);
                if (node.ItemsCount != 0 && node.ExpandedState != NodeExpandedState1.Expanded)
                    //  Debug.WriteLine("can execute");
                    e.CanExecute = true;
                else if (node.ExpandedState == NodeExpandedState1.Expanded) e.CanExecute = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // ignored
            }
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            _treeView = (TreeView) GetTemplateChild("TreeView");
            if (_treeView != null) _treeView.SelectedItemChanged += TreeViewOnSelectedItemChanged;
        }

        private void TreeViewOnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RaiseEvent(new RoutedPropertyChangedEventArgs<object>(e.OldValue, e.NewValue, SelectedItemChangedEvent));
            e.Handled = true;
        }
    }
}