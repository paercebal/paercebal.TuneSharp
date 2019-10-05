using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace paercebal.TuneSharp.Dialogs
{
    // See: https://stackoverflow.com/a/49802029/14089
    //
    // Usage:
    //
    // var winForm = new MyWinForm();
    // winForm.ShowDialog(new WpfWindowWrapper(wpfWindow));

    public class ParentWpfWrapperForChildWinForm : System.Windows.Forms.IWin32Window
    {
        public ParentWpfWrapperForChildWinForm(System.Windows.Window wpfWindow)
        {
            Handle = new System.Windows.Interop.WindowInteropHelper(wpfWindow).Handle;
        }

        public IntPtr Handle { get; private set; }
    }
}
