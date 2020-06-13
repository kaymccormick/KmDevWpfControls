using System.Diagnostics;
using System.Windows;

namespace KmDevWpfControls
{
    public class TraceListenerCreatedEventArgs : RoutedEventArgs
    {
        public TraceListener Instance { get; }

        /// <inheritdoc />
        public TraceListenerCreatedEventArgs(RoutedEvent routedEvent, object source, TraceListener instance) : base(routedEvent, source)
        {
            this.Instance = instance;
        }
    }
}