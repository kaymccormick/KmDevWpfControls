using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace KmDevWpfControls
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomToggleButton1 : ToggleButton
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnToggle()
        {
            Debug.WriteLine("OnToggle");
        }
    }
}