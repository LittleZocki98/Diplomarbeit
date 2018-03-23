using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Diplomarbeit.Configuration;
using Diplomarbeit.Hexaleg;
using Diplomarbeit.Hexeptions;
using Diplomarbeit.Vector;

namespace Diplomarbeit {
  /// <summary>
  ///   Interaktionslogik für MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {

    private Config conf;
    private Log eLog;
    private Display displ;
    private Hexapod.Hexapod hexapod;

    private System.Windows.Threading.DispatcherTimer timer;
    private Vector3D p = new Vector3D(0.0, 0.0, 0.0);

    private double movingDistance = 1;
    private Key kMovF, kMovB, kMovL, kMovR; //kMovU, kMovD, kRotL, kRotR;

    /// <summary>
    ///   Constructor
    /// </summary>
    public MainWindow() {
      InitializeComponent();
    }

    /// <summary>
    ///   Loaded-Event
    ///   ♦ Initialize all instances and variables
    ///   ♦ Load configurationfile
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e) {
      eLog = new Log();
      displ = new Display();
      
      hexapod = new Hexapod.Hexapod(eLog);

      timer = new System.Windows.Threading.DispatcherTimer();
      timer.Interval = new TimeSpan(0, 0, 0, 0, 50); // T = 50ms -> f = 20Hz
      timer.Tick += moving;

      kMovF = Key.W;
      kMovB = Key.S;
      kMovL = Key.A;
      kMovR = Key.D;

      //kMovU = Key.E;
      //kMovD = Key.Q;
      //kRotL = Key.Y;
      //kRotR = Key.X;

      btnConnect.IsEnabled = false;
      btnDisconnect.IsEnabled = false;
      
      if (loadConfig() != 0) {
        MessageBox.Show("Proper initialization failed.\nConfigurationfile corrupted!", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        Close();
      }

      Focus();

      JoystickHead.SetValue(Canvas.LeftProperty, (JoystickBase.Width - JoystickHead.Width) / 2.0);
      JoystickHead.SetValue(Canvas.TopProperty, (JoystickBase.Height - JoystickHead.Height) / 2.0);
      JoystickArm.SetValue(Canvas.LeftProperty, (JoystickBase.Width / 2.0));
      JoystickArm.SetValue(Canvas.TopProperty, (JoystickBase.Height - JoystickArm.Height) / 2.0);
      JoystickArm.Width = 0;
    }
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
      if(timer.IsEnabled) { timer.Stop(); }
      eLog.Close();
      displ.Close();
      Application.Current.Shutdown();
    }

    /// <summary>
    ///   Load configurationfile
    /// </summary>
    /// <returns>load-status (0 = no error)</returns>
    private int loadConfig() {
      eLog.WriteLog("[Config] Loading");
      try {
        conf = new Config(@"Resources\Config.cfg");

        List<HexaLeg> confLegs = new List<HexaLeg>();

        confLegs = conf.Read();

        foreach(HexaLeg leg in confLegs) {
          hexapod.AddLeg(leg);
        }
        hexapod.INIT_Legs();
      } catch(ConfigError ex) {
        eLog.WriteLog("[Config] Failed!");
        eLog.WriteLog("[Config]" + ex.Message);
        return -1;
      }
      eLog.WriteLog("[Config] Loaded");
      return 0;
    }

    /// <summary>
    ///   moving-method of timer
    /// </summary>
    private void moving(object sender, EventArgs e) {
      Vector3D dir = new Vector3D(0.0, 0.0, 0.0);
      
      if (Keyboard.IsKeyDown(kMovF)) { dir.Y += movingDistance; }
      if (Keyboard.IsKeyDown(kMovB)) { dir.Y -= movingDistance; }
      if (Keyboard.IsKeyDown(kMovL)) { dir.X -= movingDistance; }
      if (Keyboard.IsKeyDown(kMovR)) { dir.X += movingDistance; }

      /*
       * Moving up/down not implemented because it would
       * make everything much more complicated.
       */
      //if (Keyboard.IsKeyDown(kMovU)) { dir.Z += movingDistance / 2.0; }
      //if (Keyboard.IsKeyDown(kMovD)) { dir.Z -= movingDistance / 2.0; }

      double length = (JoystickBase.Width - JoystickHead.Width) / 2.0;
      double dX = 0.0;
      double dY = 0.0;
      double alpha = 0.0;
      if(dir.Size_Sq != 0) {
        alpha = Math.Atan2(dir.Y, dir.X);
        dX = Math.Cos(alpha) * (length - 5);
        dY = Math.Sin(-alpha) * (length - 5);
        JoystickArm.Width = length;
      } else {
        JoystickArm.Width = 0.0;
      }

      JoystickHead.SetValue(Canvas.LeftProperty, length + dX);
      JoystickHead.SetValue(Canvas.TopProperty, length + dY);
      JoystickArm.RenderTransform = new RotateTransform(alpha * -180.0 / Math.PI);

      // Move robot
      hexapod.Move(dir);
    }

    /*
     *  Following manage the window's Items (Buttons, ComboBoxes, ...)
     *  (Are they executable?; What do they do?)
     */

    /// <summary>
    ///   Search for COM-Ports
    /// </summary>
    private void btnPorts_Click(object sender, RoutedEventArgs e) {
      // clear list of potential outdated data
      comboPorts.Items.Clear();
      eLog.WriteLog("[Ports] Searching for ports...");

      // create List-object and fill it with the found COM ports
      List<string> port = new List<string>();
      port = Connection.GetPorts();
      eLog.WriteLog("[Ports] Found " + port.Count + " ports!");

      // insert found ports into the list
      foreach(string p in port)
        comboPorts.Items.Add(p);

      eLog.WriteLog("[Ports] Refreshed port list!");
    }

    /// <summary>
    ///   Try to connect with the device
    /// </summary>
    private void btnConnect_Click(object sender, RoutedEventArgs e) {
      try {
        if(comboPorts.SelectedIndex != -1) {

          hexapod.Connect(comboPorts.SelectedItem.ToString());
          eLog.WriteLog("[Connection] Connected!");

          // start timer for sending data packages
          timer.Start();
          btnPorts.IsEnabled = false;
          comboPorts.IsEnabled = false;
          btnConnect.IsEnabled = false;
          btnDisconnect.IsEnabled = true;
          connectedIndicator.Fill = new SolidColorBrush(Color.FromRgb(0x00, 0xCC, 0x00));
        }
        else {
          eLog.WriteLog("[Connection] Please select a port!");
        }
      } catch(ConnectionError ex) {
        eLog.WriteLog("[ConnectionError] " + ex.Message);
      } catch(Exception ex) {
        eLog.WriteLog("[Connection] " + ex.Message);
      }

    }

    /// <summary>
    ///   Try to disconnect the device from the application
    /// </summary>
    private void btnDisconnect_Click(object sender, RoutedEventArgs e) {
      try {
        // terminate the connection
        hexapod.Disconnect();
        eLog.WriteLog("[Connection] Disconnected!");

        // stop timer
        timer.Stop();

        // update buttons
        btnPorts.IsEnabled = true;
        comboPorts.IsEnabled = true;
        btnConnect.IsEnabled = true;
        btnDisconnect.IsEnabled = false;
        connectedIndicator.Fill = new SolidColorBrush(Color.FromRgb(0xCC, 0x00, 0x00));
      } catch(ConnectionError ex) {
        eLog.WriteLog("[ConnectionError] " + ex.Message);
      } catch(Exception ex) {
        eLog.WriteLog("[Connection] " + ex.Message);
      }
    }

    /// <summary>
    ///   Check which item was selected in the combobox
    /// </summary>
    private void comboPorts_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      if(comboPorts.SelectedIndex != -1) {
        btnConnect.IsEnabled = true;
        connectedIndicator.Fill = new SolidColorBrush(Color.FromRgb(0xCC, 0x00, 0x00));
      }
    }

    /*
     *  Following manage the menu's objects
     *  (Are they executable?; What do they do?)
     */

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
  }
}
