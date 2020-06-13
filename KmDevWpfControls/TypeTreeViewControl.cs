using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;

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
    ///     <MyNamespace:TypeTreeViewControl/>
    ///
    /// </summary>
    public class TypeTreeViewControl : Control
    {
        private ObservableCollection<ITreeViewNode> _internalRootItems = new ObservableCollection<ITreeViewNode>();
        public static readonly DependencyProperty SelectedTypeProperty = DependencyProperty.Register(
            "SelectedType", typeof(Type), typeof(TypeTreeViewControl), new PropertyMetadata(default(Type)));

        public static readonly DependencyProperty AssembliesProperty = DependencyProperty.Register(
            "Assemblies", typeof(IEnumerable<Assembly>), typeof(TypeTreeViewControl), new PropertyMetadata(default(IEnumerable<Assembly>), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TypeTreeViewControl)d).OnAssembliesChanged((IEnumerable<Assembly>)e.OldValue, (IEnumerable<Assembly>)e.NewValue);
        }

        private void OnAssembliesChanged(IEnumerable<Assembly> oldVal, IEnumerable<Assembly> newVal)
        {
            if (oldVal is INotifyCollectionChanged col)
            {
                col.CollectionChanged -= ColOnCollectionChanged;
            }
            _internalRootItems.Clear();
            foreach (var assembly in newVal)
            {
                _internalRootItems.Add(new AssemblyNode() {Assembly = assembly});
            }

            if (newVal is INotifyCollectionChanged col2)
            {
                col2.CollectionChanged += ColOnCollectionChanged;
            }
        }

        private void ColOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var eNewItem in e.NewItems)
            {
                _internalRootItems.Add(new AssemblyNode(){Assembly = (Assembly) eNewItem});
            }
        }

        public IEnumerable<Assembly> Assemblies
        {
            get { return (IEnumerable<Assembly>) GetValue(AssembliesProperty); }
            set { SetValue(AssembliesProperty, value); }
        }
        public Type SelectedType
        {
            get { return (Type) GetValue(SelectedTypeProperty); }
            set { SetValue(SelectedTypeProperty, value); }
        }
        static TypeTreeViewControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TypeTreeViewControl), new FrameworkPropertyMetadata(typeof(TypeTreeViewControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _treeView = GetTemplateChild("TreeView") as TreeView;
            if (_treeView != null) _treeView.SelectedItemChanged += TreeViewOnSelectedItemChanged;
        }

        private void TreeViewOnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TypeNode2 n)
            {
                SelectedType = n.NamespaceType.Type;
            }
        }

        public TypeTreeViewControl()
        {
            CommandBindings.Add(new CommandBinding(CustomTreeView.ToggleNodeIsExpanded, Executed));
            RootItems = _internalRootItems;
        }

        private async void Executed(object sender, ExecutedRoutedEventArgs e)
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

        public static readonly DependencyProperty RootItemsProperty = DependencyProperty.Register(
            "RootItems", typeof(IEnumerable<ITreeViewNode>), typeof(TypeTreeViewControl), new PropertyMetadata(default(IEnumerable<ITreeViewNode>)));

        private TreeView _treeView;

        public IEnumerable<ITreeViewNode> RootItems
        {
            get { return (IEnumerable<ITreeViewNode>) GetValue(RootItemsProperty); }
            set { SetValue(RootItemsProperty, value); }
        }

        public static ITreeViewNode Placeholder { get; } = new AssemblyResourceNodesPlaceHolder();
    }
}
