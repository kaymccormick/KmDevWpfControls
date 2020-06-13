using System;
using System.Collections.Generic;
using System.Linq;
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

namespace KmDevWpfControls
{
    /// <summary>
    /// Interaction logic for TypeProviderUserControl.xaml
    /// </summary>
    public partial class TypeProviderUserControl : UserControl
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type", typeof(Type), typeof(TypeProviderUserControl), new PropertyMetadata(default(Type)));

        public Type Type
        {
            get { return (Type) GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        public TypeProviderUserControl()
        {
            InitializeComponent();
        }
    }
}
