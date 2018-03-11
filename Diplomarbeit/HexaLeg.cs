using System;
using Diplomarbeit.Hexeptions;
using Diplomarbeit.Vector;

namespace Diplomarbeit.Hexaleg {
  public class Boundary {
    public double Alpha { get; set; }
    public double DistanceXY { get; set; }

    public override string ToString() {
      return "[Alpha: " + this.Alpha + ", DistanceXY: " + this.DistanceXY + "]";
    }
  }
  public class HexaLeg {
    /// <summary>
    ///   Offset -> Vector by which the whole leg is offset in reference to the main body
    ///   Hip    -> Vector which describes the first segment of the leg (length, internal offset between start and end)
    ///   Thigh  -> Vector which describes the second segment of the leg (length, internal offset between start and end)
    ///   Shank  -> Vector which describes the third segment of the leg (length, internal offset between start and end)
    ///   Lambda -> Angle between X-Axis and leg-axis (ccw positive <-> math. correct)
    /// </summary>
    private Vector3D offset, hip, thigh, shank;
    private double lambda;

    /// <summary>
    ///   Point on which the leg is pointing referenced to the main body
    /// </summary>
    private Vector3D endPoint;
    private Vector3D zeroPoint;

    private Boundary support;
    private Boundary switchLeg;

    /// <summary>
    ///   Alpha -> turning angle on the vertical axis
    ///   Beta  -> angle of the hip axis
    ///   Gamma -> angle of the knee axis
    /// </summary>
    private double alpha, beta, gamma;

    public double Alpha {
      get { return this.alpha * 180.0 / Math.PI; }
    }
    public double Beta {
      get { return this.beta * 180.0 / Math.PI; }
    }
    public double Gamma {
      get { return this.gamma * 180.0 / Math.PI; }
    }
    public double Lambda {
      get { return this.lambda * 180.0 / Math.PI; }
    }
    public Vector3D Offset {
      get { return this.offset; }
    }
    public Vector3D Hip {
      get { return this.hip; }
    }
    public Vector3D Thigh {
      get { return this.thigh; }
    }
    public Vector3D Shank {
      get { return this.shank; }
    }
    public Boundary Support {
      get { return this.support; }
    }
    public Boundary SwitchLeg {
      get { return this.switchLeg; }
    }
    public Vector3D ZeroPoint {
      get { return this.zeroPoint; }
    }

    public Vector3D Point {
      get { return this.endPoint; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Lambda">Thigh offset in degrees (messured ccw from pos. X-direction)</param>
    /// <param name="Offset">Vector between the hexapod's center and the connection point of the leg</param>
    /// <param name="Hip">Vector which describes the leg's hip</param>
    /// <param name="Thigh">Vector which describes the leg's thigh</param>
    /// <param name="Shank">Vector which describes the leg's shank</param>
    public HexaLeg(double Lambda, Vector3D Offset, Vector3D Hip, Vector3D Thigh, Vector3D Shank, Boundary Support, Boundary SwitchLeg) {
      this.lambda = Lambda * Math.PI / 180.0; // Convert to radians
      this.offset = Offset;
      this.hip = Hip;
      this.thigh = Thigh;
      this.shank = Shank;

      Vector3D temp = new Vector3D(this.hip + this.thigh + this.shank);
      double tempX = temp.X;
      temp.X = tempX * Math.Cos(this.lambda) - temp.Y * Math.Sin(this.lambda);
      temp.Y = tempX * Math.Sin(this.lambda) + temp.Y * Math.Cos(this.lambda);

      this.endPoint = new Vector3D(temp + this.offset);
      this.zeroPoint = this.endPoint;

      this.support = Support;
      this.switchLeg = SwitchLeg;

      CalculateAngles(this.endPoint);
    }

    /// <summary>
    ///   Method to calculate the necessary angles according to a given end-point
    /// </summary>
    /// <param name="Point">Point which should be approached by the leg</param>
    public void CalculateAngles(Vector3D Point) {
      this.endPoint = Point - this.offset;

      /*if (
        this.endPoint.Size > this.thigh.Size + this.shank.Size ||
        this.endPoint.Size < Math.Abs(this.shank.Size - this.thigh.Size)
      ) {
        throw new OutOfBoundry("Out of Boundry");
      }*/

      // Azimuth
      double d = this.endPoint.SizeXY;
      double e = this.hip.Y + this.thigh.Y + this.shank.Y;
      double zet = Math.Acos(e / d);
      double xi = Math.Atan2(this.endPoint.X, this.endPoint.Y);

      if(this.lambda > 4.712) // lambda > 270°?
        xi -= 2 * Math.PI;

      this.alpha = zet - xi - this.lambda;

      // Elevation
      Vector3D nP = new Vector3D(endPoint);
      nP.RotZ(-this.alpha - this.lambda);

      double dZ = nP.Z - this.hip.Z;
      double dF = nP.X - this.hip.X;
      double wSq = dZ * dZ + dF * dF;
      double w = Math.Sqrt(wSq);

      double bet0 = Math.Atan2(this.thigh.Z, this.thigh.X);
      double gam0 = Math.Atan2(this.shank.Z, this.shank.X);

      double omg = Math.Atan2(dZ, dF);

      double numer = w * w - this.thigh.SizeXZ_Sq - this.shank.SizeXZ_Sq;
      double denom = 2.0 * this.thigh.SizeXZ * this.shank.SizeXZ;

      double gam = Math.PI - Math.Acos(numer / denom);

      double eps = Math.Asin(Math.Sin(gam) * this.shank.SizeXZ / Math.Abs(w));
      double psi = eps + omg;

      this.beta = bet0 - psi;
      this.gamma = Math.PI + gam0 - gam - psi;
    }

    public void SetAngles(byte Alpha, byte Beta, byte Gamma) {
      this.alpha = Alpha;
      this.beta = Beta;
      this.gamma = Gamma;
    }

    public bool NeedSupport() {
      if(Math.Abs(this.alpha) > this.support.Alpha / 180.0 * Math.PI) // |Alpha| > 30°?
        return true;

      if(this.endPoint.SizeXY - this.zeroPoint.SizeXY > this.support.DistanceXY)
        return true;

      return false;
    }

    public bool Supportable() {
      if(Math.Abs(this.alpha) > this.switchLeg.Alpha / 180.0 * Math.PI) // |Alpha| > 45°?
        return true;

      if(this.endPoint.SizeXY - this.zeroPoint.SizeXY > this.switchLeg.DistanceXY)
        return true;

      return false;
    }
  }
}