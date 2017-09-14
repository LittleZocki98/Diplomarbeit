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

namespace Diplomarbeit {
  /// <summary>
  /// Interaktionslogik für MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {

    private Connection connection;

    public MainWindow() {
      InitializeComponent();
      this.connection = new Connection();
    }


    // CustomMenu Command-Stuffz
    // # Exit
    private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }
    private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
      Application.Current.Shutdown();
    }

    // # Add Device
    private void AddDevice_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }
    private void AddDevice_Executed(object sender, ExecutedRoutedEventArgs e) {
      this.connection.AddDevice();
    }

    // # Remove Device
    private void RemoveDevice_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }
    private void RemoveDevice_Executed(object sender, ExecutedRoutedEventArgs e) {
      this.connection.RemoveDevice();
    }
  }
}
