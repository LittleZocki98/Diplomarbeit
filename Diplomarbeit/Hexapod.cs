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
    private bool supporting;

    public List<HexaLeg> Legs { get { return this.legs; } }

    public Hexapod(Log log) {
      this.log = log;

      this.log.Show();
      this.displ.Show();

      this.legs = new List<HexaLeg>();
      this.connection = new Connection();
      this.supporting = false;
    }

    /// <summary>
    /// Add a leg to the hexapod's trunk
    /// </summary>
    /// <param name="Leg"></param>
    public void AddLeg(HexaLeg Leg) {
      this.legs.Add(Leg);
      this.log.WriteLog(
        "[INIT] Added new leg; Endpoint: " + Leg.Point.ToString() + "; " +
        "Lambda: " + Leg.Lambda.ToString() + "°; " +
        "Support: " + Leg.Support.ToString() + "; " +
        "Switch: " + Leg.SwitchLeg.ToString()
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

      bool switchL = legs[legUp].Supportable() || legs[legUp + 2].Supportable() || legs[legUp + 4].Supportable();
      bool support = legs[legUp].NeedSupport() || legs[legUp + 2].NeedSupport() || legs[legUp + 4].NeedSupport();

      if(switchL) {
        SwitchLegs();
      }
      if (support != this.supporting) {
        this.supporting = support;
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
        this.connection.Send(angles, 0, 18);
      } catch(ConnectionError ex) {

      } catch(Exception ex) { throw; }
    }

    private void printData(List<List<double>> data) {

    }
  }
}
