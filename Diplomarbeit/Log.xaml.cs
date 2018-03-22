using System;
using System.Windows;

namespace Diplomarbeit {
  /// <summary>
  ///   Interaktionslogik für Log.xaml
  /// </summary>
  public partial class Log : Window {

    // Remove close-button on window
    private const int GWL_STYLE = -16;
    private const int WS_SYSMENU = 0x80000;

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


    /// <summary>
    ///   Constructor
    /// </summary>
    public Log() {
      InitializeComponent();
      textBlock.Text = string.Empty;
    }

    /// <summary>
    ///   Write something to the log
    /// </summary>
    /// <param name="text">Log message</param>
    public void WriteLog(string text) {
      textBlock.Text += "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text + '\n';
      scrollViewer.ScrollToBottom();
    }

    /// <summary>
    ///   Loaded event
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e) {
      // Remove closing-button on window
      var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
      SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
    }
  }
}
