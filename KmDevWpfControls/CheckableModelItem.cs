using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace KmDevWpfControls
{
    public class CheckableModelItem<T> : INotifyPropertyChanged
    {
        public override string ToString() { return $"{Item} (Checked={IsChecked})";}
        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public T Item { get; }

        public CheckableModelItem(T item)
        {
            Item = item;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}