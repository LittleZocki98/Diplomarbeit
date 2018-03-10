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
    private List<Vector3D> zeroPoints;
    private Connection connection;

    private int legUp; // 1 = odd, 0 = even

    public List<HexaLeg> Legs { get { return this.legs; } }

    public Hexapod(Log log) {
      this.log = log;

      this.log.Show();
      this.displ.Show();

      this.legs = new List<HexaLeg>();
      this.zeroPoints = new List<Vector3D>();

      this.connection = new Connection();
    }

    /// <summary>
    /// Add a leg to the hexapod's trunk
    /// </summary>
    /// <param name="Leg"></param>
    public void AddLeg(HexaLeg Leg) {
      this.legs.Add(Leg);
      this.zeroPoints.Add(Leg.Point);
      this.log.WriteLog(
        "[INIT] Added new leg; Endpoint: " + Leg.Point.ToString() + "; " +
        "Lambda: " + Leg.Lambda.ToString() + "°; " +
        "Support: " + Leg.Support.ToString() + "; " +
        "Switch: " + Leg.SwitchLeg.ToString()
      );
    }

    public void INIT_Legs() {
      for (int i = 0; i < this.legs.Count; i++) {
        this.legs[i].CalculateAngles(this.zeroPoints[i]);
      }

      // raise legs 1, 3 and 5 10mm
      for (int i = 0; i < 3; i++) {
        this.legs[2 * i + 1].CalculateAngles(this.zeroPoints[2 * i + 1] + new Vector3D(0.0, 0.0, 10.0));
      }
      this.legUp = 1;
    }

    public void SwitchLegs() {
      switch(legUp) {
        case 0:

          break;
        case 1:

          break;
        default:
          break;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction">Movement-Vector</param>
    public void Move(Vector3D direction) {
      try {
        double alpha, beta, gamma;
        List<List<double>> data = new List<List<double>>();
        List<double> values = new List<double>();

        foreach(HexaLeg leg in this.legs) {
          // May be unneccessary, but who cares?
          alpha = double.NaN;
          beta = double.NaN;
          gamma = double.NaN;
          Vector3D nPoint = new Vector3D();

          try {
            nPoint = leg.Point - direction;
            leg.CalculateAngles(nPoint);
            alpha = leg.Alpha;
            beta = leg.Beta;
            gamma = leg.Gamma;
          }
          catch(OutOfBoundry ex) {
            // Should not occur, cuz' switch-legs fires first! (in case of proper usage)
            this.log.WriteLog("A fatal error has occured\n\tPoint " + nPoint.ToString() + "is out of reach!");
            throw new Exception("Fatal Error", ex);
          } 
          catch(Exception ex) { throw; } // And Cry y.y

          #region displayValues
          values.Clear();

          values.Add(leg.Point.X);
          values.Add(leg.Point.Y);
          values.Add(leg.Point.Z);

          values.Add(alpha);
          values.Add(beta);
          values.Add(gamma);

          data.Add(values);
          #endregion
        }
        displ.PrintData(data);
      } catch(Hexeption ex) {
        // Just please don't...
      } catch(Exception ex) { throw; } // Cry y.y

      bool switchL = false;
      bool support = false;

      for(int i = legUp; i < this.legs.Count; i += 2) {
        if (this.legs[i].NonSupportable()) {
          switchL = true;
          break;
        }
      }
      if(!switchL) {
        for(int i = legUp; i < this.legs.Count; i += 2) {
          if(this.legs[i].NeedSupport()) {
            support = true;
            break;
          }
        }
      }

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
        this.connection.Send(angles, 1, 2);
      } catch(ConnectionError ex) {

      } catch(Exception ex) { throw; }
    }

    private void printData(List<List<double>> data) {

    }
  }
}
