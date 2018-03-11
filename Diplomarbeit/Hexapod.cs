using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public List<HexaLeg> Legs { get { return this.legs; } }

    public Hexapod(Log log) {
      this.log = log;

      this.log.Show();
      this.displ.Show();

      this.legs = new List<HexaLeg>();
      this.connection = new Connection();
      //this.supporting = false;
    }

    /// <summary>
    /// Add a leg to the hexapod's trunk
    /// </summary>
    /// <param name="Leg"></param>
    public void AddLeg(HexaLeg Leg) {
      this.legs.Add(Leg);
      this.log.WriteLog(
        "[INIT]; Added new leg; Endpoint: " + Leg.Point.ToString() + "; " +
        "Lambda: " + Leg.Lambda.ToString() + "°; " +
        "Support-Criteria: " + Leg.Support.ToString() + "; " +
        "Switch-Criteria: " + Leg.SwitchLeg.ToString()
      );
    }

    public void INIT_Legs() {

      // Init legs
      foreach(HexaLeg leg in this.legs) {
        leg.CalculateAngles(leg.ZeroPoint);
      }

      // raise legs 1, 3 and 5 10mm
      for (int i = 1; i < 6; i+=2) {
        this.legs[i].CalculateAngles(this.legs[i].ZeroPoint + new Vector3D(0.0, 0.0, 10.0));
      }

      // Odd legs are up
      this.legUp = 1;
    }

    // TODO!!
    public void SwitchLegs() {
      legUp = 1 - legUp; // toggle legUp between 0 and 1
      log.WriteLog("Switched Legs!");
    }

    /// <summary>
    /// 
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
            this.log.WriteLog("A fatal error has occured\n\tPoint is out of reach!");
            throw new Exception("Fatal Error - OutOfBoundary", ex);
          }
          catch(Exception ex) { throw; }  // And Cry y.y
        }

        bool switchL = legs[legUp].Supportable() || legs[legUp + 2].Supportable() || legs[legUp + 4].Supportable();
        bool support = legs[legUp].NeedSupport() || legs[legUp + 2].NeedSupport() || legs[legUp + 4].NeedSupport();

        if(support != this.supporting) {
          // lower legs in front 
          if (support) {
            for(int i = (1 - legUp); i < 6; i += 2) {
              legs[i].CalculateAngles(legs[i].ZeroPoint + (direction * 10.0));
            }
          }
          else {
            for(int i = (1 - legUp); i < 6; i += 2) {
              legs[i].CalculateAngles(legs[i].ZeroPoint + new Vector3D(0.0, 0.0, 10.0));
            }
          }
          this.supporting = support;
          log.WriteLog("Supporting: " + this.supporting.ToString());
        }

        // if we need support, we move the "raised" legs according to direction
        if(supporting) {
          for(int i = (1 - legUp); i < 6; i += 2) {
            legs[i].CalculateAngles(legs[i].Point - direction);
          }
        }
        else { // if we dont need support, move raised legs to raised zero pos
          for (int i = 1 - legUp; i < 6; i+=2) {
            legs[i].CalculateAngles(legs[i].ZeroPoint + new Vector3D(0.0, 0.0, 10.0));
          }
        }

        if(switchL)
          SwitchLegs();

        PrintData();

      } catch(Hexeption ex) {
        // Just please don't...
      } catch(Exception ex) { throw; } // Cry y.y
      

      SendData();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="port"></param>
    public void Connect(string port) {
      try {
        this.connection.InitConnection(port, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
      } catch(ConnectionError ex) { throw; }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Disconnect() {
      this.connection.CloseConnection();
    }

    /// <summary>
    /// 
    /// </summary>
    public void SendData() {
      List<byte> temp = new List<byte>();
      byte[] angles = new byte[18];
      foreach(HexaLeg l in this.legs) {
        temp.Add((Byte)(l.Alpha + 90.0));
        temp.Add((Byte)(l.Beta + 90.0));
        temp.Add((Byte)(l.Gamma + 90.0));
      }

      for (int i = 0; i < temp.Count; ++i) {
        angles[i] = temp[i];
      }

      try {
        this.connection.Send(angles, 0, 18); // send all 18 angles (6 feet * 3 angles/foot)
      } catch(ConnectionError ex) {
        this.log.WriteLog("[Connection] Coult not send angles:\n" + ex.Message);
      } catch(Exception ex) { throw; }
    }

    public void PrintData() {
      List<List<double>> data = new List<List<double>>();
      List<double> values;

      foreach(HexaLeg leg in this.legs) {
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
