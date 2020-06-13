using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace KmDevWpfControls
{
    public class AssemblyNode : ITreeViewNode, INotifyPropertyChanged, IAsyncExpand
    {
        private bool _isExpanded;
        private IEnumerable _items;
        private bool _isSelected;

        public AssemblyNode()
        {
            Items = Enumerable.Repeat(TypeTreeViewControl.Placeholder, 1);
        }

        public Assembly Assembly
        {
            get { return _assembly; }
            set
            {
 _assembly = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Header));
            }
        }

        public void Collapse()
        {
            IsExpanded = false;

        }

        public object Header => Assembly.GetName().Name;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value == _isExpanded) return;
                _isExpanded=value;
                OnPropertyChanged();
            }
        }

        public IEnumerable Items
        {
            get { return _items; }
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Assembly _assembly;
        private int _depth;

        public int Depth    
        {
            get { return _depth; }
            set
            {
                if (value == _depth) return;
                _depth = value;
                OnPropertyChanged();
            }
        }

        public Task ExpandAsync()
        {
            Items = XX(Assembly?.GetExportedTypes().Select(t => new NamespaceType {Type = t, NamespaceParts = t.Namespace ?? ""}), Depth + 1);
            IsExpanded = true;
            return Task.CompletedTask;

        }

        public static IEnumerable<TypeNode> XX(IEnumerable<NamespaceType> @select, int depth)
        {
            return @select.SelectMany(Namesp).GroupBy(type => type.NamespacePart).Select(types => new TypeNode(depth, types));
        }

        [ItemCanBeNull]
        public static IEnumerable<NamespaceType> Namesp(NamespaceType arg)
        {
            var indexOf = arg.NamespaceParts.IndexOf('.');
            if (indexOf == -1)
            {
                //return Enumerable.Empty<NamespaceType>();
                return new[]
                {
                    new NamespaceType
                    {
                        Type = arg.Type, NamespaceParts = "", NamespacePart = ""
                    }
                };
            
            }
            return new[]{new NamespaceType
            {
                Type = arg.Type, NamespaceParts = arg.NamespaceParts.Substring(indexOf + 1),
                NamespacePart = arg.NamespaceParts.Substring(0, indexOf)
            }};
        }

    }

    public class TypeNode : ITreeViewNode,INotifyPropertyChanged,IAsyncExpand
    {
        private readonly int _depth;
        private bool _isSelected;
        private IEnumerable _items;
        private bool _isExpanded;
        private IGrouping<string, NamespaceType> _types;
        private object _header;

        public IGrouping<string, NamespaceType> Types   
        {
            get { return _types; }
            set
            {
                if (Equals(value, _types)) return;
                _types = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Header));
            }
        }

        public TypeNode(int depth, IGrouping<string, NamespaceType> types)
        {
            _depth = depth;
            var l = types.ToList();
            if (types.Key == "")
            {
                Header = "Types";
            }
            else
            {
                Header = types.Key;
            }
            Items = Enumerable.Repeat(TypeTreeViewControl.Placeholder, 1);
            Types = types;
        }

        public void Collapse()
        {
            IsExpanded = false;
        }

        public object Header
        {
            get { return _header; }
            set
            {
                if (Equals(value, _header)) return;
                _header = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value == _isExpanded) return;
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable Items
        {
            get { return _items; }
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Task ExpandAsync()
        {
            
            if (Types.Key == "")
            {
                Items = Types.Select(t => new TypeNode2(t));
            }
            else
            {
                Items = AssemblyNode.XX(Types, 1);
            }

            IsExpanded = true;
            return Task.CompletedTask;
        }
    }

    public class TypeNode2 : ITreeViewNode, INotifyPropertyChanged
    {
        public NamespaceType NamespaceType
        {
            get { return _namespaceType1; }
            set
            {
                if (Equals(value, _namespaceType1)) return;
                _namespaceType1 = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Header));
            }
        }

        private bool _isSelected;
        private bool _isExpanded;
        private NamespaceType _namespaceType1;

        public TypeNode2(NamespaceType namespaceType)
        {
            NamespaceType = namespaceType;
        }

        public void Collapse()
        {
            IsExpanded = false;
        }

        public object Header => NamespaceType.Type.Name;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value == _isExpanded) return;
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable Items => Enumerable.Empty<object>();

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
 _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class NamespaceType : INotifyPropertyChanged
    {
        private Type _type;
        private string _namespaceParts;
        private string _namespacePart;

        public Type Type    
        {
            get { return _type; }
            set
            {
                if (Equals(value, _type)) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        public string NamespaceParts    
        {
            get { return _namespaceParts; }
            set
            {
                if (value == _namespaceParts) return;
                _namespaceParts = value;
                OnPropertyChanged();
            }
        }

        public string NamespacePart
        {
            get { return _namespacePart; }
            set
            {
                if (value == _namespacePart) return;
                _namespacePart = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}