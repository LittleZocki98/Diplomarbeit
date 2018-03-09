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
  /// Interaktionslogik für Display.xaml
  /// </summary>
  public partial class Display : Window {
    #region Remove closing button
    private const int GWL_STYLE = -16;
    private const int WS_SYSMENU = 0x80000;

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    #endregion

    private List<List<TextBox>> boxes;

    public Display() {
      InitializeComponent();
      this.boxes = new List<List<TextBox>>();
      
      for (int i = 0; i < 6; i++) {
        #region Add Label
        Label lbl = new Label();
        lbl.Name = "lblLeg" + i.ToString();
        lbl.Content = "Leg " + (i + 1).ToString();
        lbl.HorizontalAlignment = HorizontalAlignment.Center;
        lbl.VerticalAlignment = VerticalAlignment.Center;
        lbl.Margin = new Thickness(2);

        Grid.SetRow(lbl, i + 1);
        Grid.SetColumn(lbl, 0);
        this.gr.Children.Add(lbl);
        #endregion

        this.boxes.Add(new List<TextBox>());
        for (int j = 0; j < 6; j++) {
          TextBox tb = new TextBox();

          tb.Name = "TextBox" + i.ToString() + j.ToString();
          tb.Text = "TextBox" + i.ToString() + j.ToString();

          tb.HorizontalAlignment = HorizontalAlignment.Center;
          tb.VerticalAlignment = VerticalAlignment.Center;
          tb.IsReadOnly = true;

          Grid.SetRow(tb, i + 1);
          Grid.SetColumn(tb, j + 1);
          this.gr.Children.Add(tb);
          this.boxes[i].Add(tb);
        }
      }
    }
    public void PrintData(List<List<double>> data) {

      for (int i = 0; i < data.Count; i++) {
        for (int j = 0; j < data[i].Count; j++) {
          this.boxes[i][j].Text = String.Format("{0:+000.00;-000.00}", data[i][j]);
        }
      }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
      SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
    }
  }
}
