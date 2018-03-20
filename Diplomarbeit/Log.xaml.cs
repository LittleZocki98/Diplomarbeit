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
using System.Windows.Shapes;

namespace Diplomarbeit {
  /// <summary>
  /// Interaktionslogik für Log.xaml
  /// </summary>
  public partial class Log : Window {
    #region Remove closing button
    private const int GWL_STYLE = -16;
    private const int WS_SYSMENU = 0x80000;

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    public Log() {
      InitializeComponent();
      this.textBlock.Text = string.Empty;
    }

    /// <summary>
    /// Write something to the log
    /// </summary>
    /// <param name="text">Log message</param>
    public void WriteLog(string text) {
      this.textBlock.Text += "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text + '\n';
      this.scrollViewer.ScrollToBottom();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
      SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
    }
  }
}
