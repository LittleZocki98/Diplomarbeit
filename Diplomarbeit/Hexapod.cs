using System;
using System.Collections.Generic;
using Diplomarbeit.Hexaleg;
using Diplomarbeit.Hexeptions;
using Diplomarbeit.Vector;

namespace Diplomarbeit.Hexapod {
  class Hexapod {
    private Log log;
    private Display displ = new Display();

    private List<HexaLeg> legs;
    private Connection connection;

    private int legUp; // 1 = odd, 0 = even
    private bool supporting; // r we supportin'?

    public List<HexaLeg> Legs { get { return legs; } }

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="log">Log window</param>
    public Hexapod(Log log) {
      this.log = log;

      this.log.Show();
      displ.Show();

      legs = new List<HexaLeg>();
      connection = new Connection();
      //this.supporting = false;
    }

    /// <summary>
    ///   Add a leg to the hexapod's trunk
    /// </summary>
    /// <param name="Leg"></param>
    public void AddLeg(HexaLeg Leg) {
      legs.Add(Leg);
      log.WriteLog(
        "[INIT]; Added new leg; Endpoint: " + Leg.Point.ToString() + "; " +
        "Lambda: " + Leg.Lambda.ToString() + "°; " +
        "Support-Criteria: " + Leg.Support.ToString() + "; " +
        "Switch-Criteria: " + Leg.SwitchLeg.ToString()
      );
    }

    /// <summary>
    ///   Initialize Legs
    /// </summary>
    public void INIT_Legs() {

      // Init legs
      foreach(HexaLeg leg in legs) {
        leg.CalculateAngles(leg.ZeroPoint);
      }

      // raise legs 1, 3 and 5 10mm
      for (int i = 1; i < 6; i+=2) {
        legs[i].CalculateAngles(legs[i].ZeroPoint + new Vector3D(0.0, 0.0, 10.0));
      }

      // Odd legs are up
      legUp = 1;
    }

    public void SwitchLegs() {
      legUp = 1 - legUp; // toggle legUp between 0 and 1
      //log.WriteLog("Switched Legs!");
    }

    /// <summary>
    ///   Move the hexapod (and it's legs accordingly)
    /// </summary>
    /// <param name="direction">Movement-Vector</param>
    public void Move(Vector3D direction) {
      try {
        for(int i = legUp; i < 6; i += 2) {
          Vector3D nPoint;

          try {
            nPoint = new Vector3D(legs[i].Point - direction);
            legs[i].CalculateAngles(nPoint);
          }
          catch(OutOfBoundry ex) {
            // Should not occur, cuz' switch-legs fires first! (in case of proper usage)
            log.WriteLog("A fatal error has occured\n\tPoint is out of reach!");
            throw new Exception("Fatal Error - OutOfBoundary", ex);
          }
          catch(Exception ex) { throw; } // Catch every other exception and throw it against what ever used this method.
        }

        // Check, if we need support of even should switch legs...
        bool switchL = legs[legUp].Supportable() || legs[legUp + 2].Supportable() || legs[legUp + 4].Supportable();
        bool support = legs[legUp].NeedSupport() || legs[legUp + 2].NeedSupport() || legs[legUp + 4].NeedSupport();

        // Did the support-value change?
        if(support != supporting) {
          // lower legs in front 
          if (support) {
            for(int i = (1 - legUp); i < 6; i += 2) {
              legs[i].CalculateAngles(legs[i].ZeroPoint + (direction * 10.0));
            }
          } else {
            for(int i = (1 - legUp); i < 6; i += 2) {
              legs[i].CalculateAngles(legs[i].ZeroPoint + new Vector3D(0.0, 0.0, 10.0));
            }
          }
          supporting = support;
          //log.WriteLog("Supporting: " + this.supporting.ToString());
        }

        // if we need support, we move the "raised" legs according to direction
        if(supporting) {
          for(int i = (1 - legUp); i < 6; i += 2) {
            legs[i].CalculateAngles(legs[i].Point - direction);
          }
        } else { // if we dont need support, move raised legs to raised zero pos
          for (int i = 1 - legUp; i < 6; i+=2) {
            legs[i].CalculateAngles(legs[i].ZeroPoint + new Vector3D(0.0, 0.0, 10.0));
          }
        }

        // If we need to switch legs -> switch it
        if(switchL) {
          SwitchLegs();
        }

        // Print leg-data (angles, position) to display-window
        PrintData();

      }
      catch(Hexeption ex) { throw; }
      catch(Exception ex) { throw; }
      
      // Send data to robot
      SendData();
    }

    /// <summary>
    ///   Connect to given port
    /// </summary>
    /// <param name="port">Port to which the connection should be established</param>
    public void Connect(string port) {
      try {
        connection.InitConnection(port, 19200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
      } catch(ConnectionError ex) { throw; }
    }

    /// <summary>
    ///   Disconnect connection
    /// </summary>
    public void Disconnect() {
      connection.CloseConnection();
    }

    /// <summary>
    ///   Send data to hexapod
    /// </summary>
    public void SendData() {
      List<byte> angles = new List<byte>();

      /*
      add angles from each leg to list (And add 90° to allow negative angles to be sent properly)
      0° in List = -90° Leg
      180° List = 90° Leg
      Servomotor is only capable to interpret Angles between 0° and 180°
      */
      foreach(HexaLeg l in legs) {
        angles.Add((Byte)(l.Alpha + 90.0));
        angles.Add((Byte)(l.Beta + 90.0));
        angles.Add((Byte)(l.Gamma + 90.0));
      }

      try {
        connection.Send(angles.ToArray(), 0, 18); // send all 18 angles (6 feet * 3 angles/foot)
      } catch(ConnectionError ex) {
        log.WriteLog("[Connection] Coult not send angles:\n" + ex.Message);
      } catch(Exception ex) { throw; }
    }

    /// <summary>
    ///   Show data on seperate data-window
    /// </summary>
    public void PrintData() {
      List<List<double>> data = new List<List<double>>();
      List<double> values;

      foreach(HexaLeg leg in legs) {
        values = new List<double>();

        values.Add(leg.Point.X);
        values.Add(leg.Point.Y);
        values.Add(leg.Point.Z);

        values.Add(leg.Alpha);
        values.Add(leg.Beta);
        values.Add(leg.Gamma);

        data.Add(values);
      }

      displ.PrintData(data);
    }
  }
}
