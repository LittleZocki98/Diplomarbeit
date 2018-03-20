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
using System.Threading;
using Diplomarbeit.Configuration;
using Diplomarbeit.Hexaleg;
using Diplomarbeit.Hexeptions;
using Diplomarbeit.Vector;

namespace Diplomarbeit {
  /// <summary>
  /// Interaktionslogik für MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {

    private Config conf;
    private Log eLog;
    private Display displ;
    private Hexapod.Hexapod hexapod;

    private System.Windows.Threading.DispatcherTimer timer;
    private Vector3D p = new Vector3D(0.0, 0.0, 0.0);

    private double movingDistance = 1;
    private Key kMovF, kMovB, kMovL, kMovR, kMovU, kMovD, kRotL, kRotR;

    /// <summary>
    /// Constructor
    /// </summary>
    public MainWindow() {
      InitializeComponent();
    }

    /// <summary>
    /// Loaded-Event
    /// Initialize all instances and variables
    /// Load configurationfile
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e) {
      this.eLog = new Log();
      this.displ = new Display();
      
      this.hexapod = new Hexapod.Hexapod(this.eLog);

      this.timer = new System.Windows.Threading.DispatcherTimer();
      this.timer.Interval = new TimeSpan(0, 0, 0, 0, 50); // T = 50ms -> f = 20Hz
      this.timer.Tick += moving;

      this.kMovF = Key.W;
      this.kMovB = Key.S;
      this.kMovL = Key.A;
      this.kMovR = Key.D;
      this.kMovU = Key.E;
      this.kMovD = Key.Q;
      this.kRotL = Key.Y;
      this.kRotR = Key.X;

      this.btnConnect.IsEnabled = false;
      this.btnDisconnect.IsEnabled = false;
      
      if (loadConfig() != 0) {
        MessageBox.Show("Proper initialization failed.\nConfigurationfile corrupted!", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        this.Close();
      }

      this.Focus();
    }
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
      if(this.timer.IsEnabled) { this.timer.Stop(); }
      this.eLog.Close();
      this.displ.Close();
      Application.Current.Shutdown();
    }

    /// <summary>
    /// Load configurationfile
    /// </summary>
    /// <returns>load-status (0 = no error)</returns>
    private int loadConfig() {
      this.eLog.WriteLog("[Config] Loading");
      try {
        this.conf = new Config(@"Resources\Config.cfg");

        List<HexaLeg> confLegs = new List<HexaLeg>();

        confLegs = conf.Read();

        foreach(HexaLeg leg in confLegs) {
          this.hexapod.AddLeg(leg);
        }
        this.hexapod.INIT_Legs();
      } catch(ConfigError ex) {
        this.eLog.WriteLog("[Config] Failed!");
        this.eLog.WriteLog("[Config]" + ex.Message);
        return -1;
      }
      this.eLog.WriteLog("[Config] Loaded");
      return 0;
    }

    /// <summary>
    /// moving-method of timer
    /// </summary>
    private void moving(object sender, EventArgs e) {
      Vector3D dir = new Vector3D(0.0, 0.0, 0.0);
      
      if (Keyboard.IsKeyDown(kMovF)) { dir.Y += movingDistance; }
      if (Keyboard.IsKeyDown(kMovB)) { dir.Y -= movingDistance; }
      if (Keyboard.IsKeyDown(kMovL)) { dir.X -= movingDistance; }
      if (Keyboard.IsKeyDown(kMovR)) { dir.X += movingDistance; }
      //if (Keyboard.IsKeyDown(kMovU)) { dir.Z += movingDistance / 2.0; }
      //if (Keyboard.IsKeyDown(kMovD)) { dir.Z -= movingDistance / 2.0; }

      this.p.X += dir.X;
      this.p.Y += dir.Y;
      this.p.Z += dir.Z;

      this.txtXAbs.Text = String.Format("{0:+000.00;-000.00}", this.p.X);
      this.txtYAbs.Text = String.Format("{0:+000.00;-000.00}", this.p.Y);
      this.txtZAbs.Text = String.Format("{0:+000.00;-000.00}", this.p.Z);

      this.hexapod.Move(dir);
    }

    #region Buttons'n'stuff
    /// <summary>
    /// Search for COM-Ports
    /// </summary>
    private void btnPorts_Click(object sender, RoutedEventArgs e) {
      // clear list of potential outdated data
      this.comboPorts.Items.Clear();
      this.eLog.WriteLog("[Ports] Searching for ports...");

      // create List-object and fill it with the found COM ports
      List<string> port = new List<string>();
      port = Connection.GetPorts();
      this.eLog.WriteLog("[Ports] Found " + port.Count + " ports!");

      // insert found ports into the list
      foreach(string p in port)
        this.comboPorts.Items.Add(p);

      this.eLog.WriteLog("[Ports] Refreshed port list!");
    }

    /// <summary>
    /// Try to connect with the device
    /// </summary>
    private void btnConnect_Click(object sender, RoutedEventArgs e) {
      try {
        if(this.comboPorts.SelectedIndex != -1) {

          this.hexapod.Connect(this.comboPorts.SelectedItem.ToString());
          this.eLog.WriteLog("[Connection] Connected!");

          // start timer for sending data packages
          this.timer.Start();
          this.btnPorts.IsEnabled = false;
          this.comboPorts.IsEnabled = false;
          this.btnConnect.IsEnabled = false;
          this.btnDisconnect.IsEnabled = true;
        }
        else {
          this.eLog.WriteLog("[Connection] Please select a port!");
        }
    } catch(ConnectionError ex) {
        this.eLog.WriteLog("[ConnectionError] " + ex.Message);
    } catch(Exception ex) {
        this.eLog.WriteLog("[Connection] " + ex.Message);
      }

    }

    /// <summary>
    /// Try to disconnect the device from the application
    /// </summary>
    private void btnDisconnect_Click(object sender, RoutedEventArgs e) {
      try {
        // terminate the connection
        this.hexapod.Disconnect();
        this.eLog.WriteLog("[Connection] Disconnected!");

        // stop timer
        this.timer.Stop();

        // update buttons
        this.btnPorts.IsEnabled = true;
        this.comboPorts.IsEnabled = true;
        this.btnConnect.IsEnabled = true;
        this.btnDisconnect.IsEnabled = false;
      } catch(ConnectionError ex) {
        this.eLog.WriteLog("[ConnectionError] " + ex.Message);
      } catch(Exception ex) {
        this.eLog.WriteLog("[Connection] " + ex.Message);
      }
    }

    /// <summary>
    /// Check which item was selected in the combobox
    /// </summary>
    private void comboPorts_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      if(this.comboPorts.SelectedIndex != -1) {
        this.btnConnect.IsEnabled = true;
      }
    }

    #endregion

    #region  CustomMenu Command-Stuffz

    // # Exit
    private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }
    private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
      this.eLog.Close();
      Application.Current.Shutdown();
    }

    // # Add Device
    private void AddDevice_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }
    private void AddDevice_Executed(object sender, ExecutedRoutedEventArgs e) {
      try {
        Connection.AddDevice();
      } catch(ConnectionError ex) {

      } catch(Exception ex) { }
    }

    // # Remove Device
    private void RemoveDevice_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }
    private void RemoveDevice_Executed(object sender, ExecutedRoutedEventArgs e) {
      try {
        Connection.RemoveDevice();
      } catch(ConnectionError ex) {

      } catch(Exception ex) { }
    }

    #endregion End of custom command stuffz
  }
}
