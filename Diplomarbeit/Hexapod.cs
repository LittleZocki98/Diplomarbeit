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

      this.log.WriteLog("[INIT] Added new leg with endpoint at " + Leg.Point.ToString() + " and Lambda = " + Leg.Lambda.ToString() + "°");
    }

    public void INIT_Legs() {
      for (int i = 0; i < this.legs.Count; i++) {
        this.legs[i].CalculateAngles(this.zeroPoints[i]);
      }
    }

    public void SwitchLegs() {
      // TODO!!!
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
          // Maybe unneccessary, but who cares?
          alpha = double.NaN;
          beta = double.NaN;
          gamma = double.NaN;

          try {
            Vector3D nPoint = leg.Point - direction;
            leg.CalculateAngles(nPoint);
            alpha = leg.Alpha;
            beta = leg.Beta;
            gamma = leg.Gamma;
          } catch(SwitchLegs ex) {
            SwitchLegs();
          }
          catch(OutOfBoundry ex) { } // Should not occur, cuz' switch-legs fires first! (in case of proper usage)
          catch(Hexeption ex) { }
          catch(Exception ex) { throw; } // And Cry y.y

          values.Clear();

          values.Add(leg.Point.X);
          values.Add(leg.Point.Y);
          values.Add(leg.Point.Z);

          values.Add(alpha);
          values.Add(beta);
          values.Add(gamma);

          data.Add(values);
        }
        displ.PrintData(data);
      } catch(Hexeption ex) {
        // Just please don't...
      } catch(Exception ex) {
        // Cry y.y
        System.Windows.MessageBox.Show("Exception while calculating angles!" + Environment.NewLine + ex.Message);
        throw;
      }

      //SendData();
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
