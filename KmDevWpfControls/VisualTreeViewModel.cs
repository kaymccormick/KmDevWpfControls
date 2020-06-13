using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using JetBrains.Annotations;

namespace KmDevWpfControls
{
    public sealed class VisualTreeViewModel : INotifyPropertyChanged
    {
        private VisualTreeNode _currentVisual;
        private ObservableCollection<VisualTreeNode> _internalItems = new ObservableCollection<VisualTreeNode>();
        private Visual _rootVisual;

        public VisualTreeNode CurrentVisual
        {
            get { return _currentVisual; }
            set
            {
                if (Equals(value, _currentVisual)) return;
                _currentVisual = value;
                Debug.WriteLine("current visual updated to " + _currentVisual);
                OnPropertyChanged();
            }
        }

        public IEnumerable RootNodes
        {
            get { return InternalItems; }
        }

        public ObservableCollection<VisualTreeNode> InternalItems
        {
            get { return _internalItems; }
            set
            {
                if (Equals(value, _internalItems)) return;
                _internalItems = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RootNodes));
            }
        }

        public Visual RootVisual
        {
            get { return _rootVisual; }
            set
            {
                if (Equals(value, _rootVisual)) return;
                _rootVisual = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}