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

namespace paercebal.TuneSharp.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:paercebal.TuneSharp.CustomControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:paercebal.TuneSharp.CustomControls;assembly=paercebal.TuneSharp.CustomControls"
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
    ///     <MyNamespace:ClickableProgressBar/>
    ///
    /// </summary>
    public class ClickableProgressBar : ProgressBar
    {
        private Interfaces.IDebugOutputable debugOutputable = null;
        private bool isMouseDown = false;
        private string debugKey = "ClickableProgressBar";

        static ClickableProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClickableProgressBar), new FrameworkPropertyMetadata(typeof(ClickableProgressBar)));
        }

        public ClickableProgressBar()
        {
            this.MouseDown += this.OnProgressBarMouseDown;
            this.MouseUp += this.OnProgressBarMouseUp;
            this.MouseMove += this.OnProgressBarMouseMove;
        }

        public delegate void OnValueManuallyChangedDelegate(double oldValue, double newValue);
        public event OnValueManuallyChangedDelegate OnValueManuallyChanged;

        public void SetDebugOutputable(string debugKey, Interfaces.IDebugOutputable debugOutputable)
        {
            this.debugKey = debugKey;
            this.debugOutputable = debugOutputable;
        }

        private void SetProgressValue(Point mousePosition)
        {
            if (this.isMouseDown)
            {
                double oldValue = this.Value;
                double percent = mousePosition.X / this.ActualWidth;
                this.Value = (this.Maximum - this.Minimum) * percent;
                double newValue = this.Value;
                this.debugOutputable?.Log(this.debugKey, "mouse.X: {0} / W: {1} : {2}", mousePosition.X, this.ActualWidth, percent);

                if ((this.OnValueManuallyChanged != null) && (oldValue != newValue))
                {
                    this.OnValueManuallyChanged(oldValue, newValue);
                }
            }
        }

        private void OnProgressBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.isMouseDown = true;
            this.SetProgressValue(e.GetPosition(this));
        }

        private void OnProgressBarMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.isMouseDown = false;

        }

        private void OnProgressBarMouseMove(object sender, MouseEventArgs e)
        {
            this.SetProgressValue(e.GetPosition(this));
        }
    }
}
