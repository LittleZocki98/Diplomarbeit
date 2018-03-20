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
    ///   Alpha -> turning angle on the vertical axis (hip)
    ///   Beta  -> angle of the hip axis (between hip and thigh)
    ///   Gamma -> angle of the knee axis (between thigh and shank)
    /// </summary>
    private double alpha, beta, gamma;

    // Public 'getter' of internal private variables
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
    /// Constructor
    /// </summary>
    /// <param name="Lambda">Thigh offset in degrees (messured ccw from pos. X-direction)</param>
    /// <param name="Offset">Vector between the hexapod's center and the connection point of the leg</param>
    /// <param name="Hip">Vector which describes the leg's hip</param>
    /// <param name="Thigh">Vector which describes the leg's thigh</param>
    /// <param name="Shank">Vector which describes the leg's shank</param>
    /// <param name="SupportCrit">Support-Criteria</param>
    /// <param name="SwitchLegCrit">Switch-Criteria</param>
    public HexaLeg(double Lambda, Vector3D Offset, Vector3D Hip, Vector3D Thigh, Vector3D Shank, Boundary SupportCrit, Boundary SwitchLegCrit) {
      this.lambda = Lambda * Math.PI / 180.0; // Convert to radians
      this.offset = Offset;
      this.hip = Hip;
      this.thigh = Thigh;
      this.shank = Shank;

      // Rotate leg to respect lambda (Rotate around Z-Axis)
      Vector3D temp = new Vector3D(this.hip + this.thigh + this.shank);
      temp.RotZ(this.lambda);

      // set zeropoint
      this.zeroPoint = new Vector3D(temp + this.offset);
      this.endPoint = this.zeroPoint;

      this.support = SupportCrit;
      this.switchLeg = SwitchLegCrit;

      // Calculate angles for zero-position. (should give 0° for every angle)
      CalculateAngles(this.endPoint);
    }

    /// <summary>
    /// Method to calculate the necessary angles according to a given end-point
    /// </summary>
    /// <param name="Point">Point which should be approached by the leg</param>
    public void CalculateAngles(Vector3D Point) {
      this.endPoint = Point - this.offset;

      
      // Not necessary, since it won't happen, when educated users are using this program
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

    /// <summary>
    /// Set the leg's angles manually
    /// </summary>
    /// <param name="Alpha"></param>
    /// <param name="Beta"></param>
    /// <param name="Gamma"></param>
    public void SetAngles(byte Alpha, byte Beta, byte Gamma) {
      this.alpha = Alpha;
      this.beta = Beta;
      this.gamma = Gamma;
    }

    /// <summary>
    /// Were any support-criteria reached?
    /// </summary>
    /// <returns></returns>
    public bool NeedSupport() {
      if(Math.Abs(this.alpha) > this.support.Alpha / 180.0 * Math.PI) // |Alpha| > 30°?
        return true;

      if(this.endPoint.SizeXY - this.zeroPoint.SizeXY > this.support.DistanceXY)
        return true;

      return false;
    }

    /// <summary>
    /// Were any switch-criteria reached?
    /// </summary>
    /// <returns></returns>
    public bool Supportable() {
      if(Math.Abs(this.alpha) > this.switchLeg.Alpha / 180.0 * Math.PI) // |Alpha| > 45°?
        return true;

      if(this.endPoint.SizeXY - this.zeroPoint.SizeXY > this.switchLeg.DistanceXY)
        return true;

      return false;
    }
  }
}