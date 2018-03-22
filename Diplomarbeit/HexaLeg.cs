using System;
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
    ///   Hip    -> Vector which describes the first segment of the leg
    ///   Thigh  -> Vector which describes the second segment of the leg
    ///   Shank  -> Vector which describes the third segment of the leg
    ///   Lambda -> Angle between X-Axis and leg-axis (ccw positive <-> math. correct)
    /// </summary>
    private Vector3D offset, hip, thigh, shank;
    private double lambda;

    /// <summary>
    ///   endPoint -> Point on which the leg is pointing referenced to the main body
    ///   zeroPoint -> Point which refers to alpha/beta/gamma = 0°
    /// </summary>
    private Vector3D endPoint;
    private Vector3D zeroPoint;

    /// <summary>
    ///   support -> Support criteria
    ///   switchLeg -> Switch criteria
    /// </summary>
    private Boundary support;
    private Boundary switchLeg;

    /// <summary>
    ///   Alpha -> turning angle on the vertical axis (hip)
    ///   Beta  -> angle of the thigh axis (between hip and thigh)
    ///   Gamma -> angle of the shank axis (between thigh and shank)
    /// </summary>
    private double alpha, beta, gamma;

    // Public 'getter' of internal private variables (to prevent overwriting)
    public double Alpha {
      get { return alpha * 180.0 / Math.PI; }
    }
    public double Beta {
      get { return beta * 180.0 / Math.PI; }
    }
    public double Gamma {
      get { return gamma * 180.0 / Math.PI; }
    }
    public double Lambda {
      get { return lambda * 180.0 / Math.PI; }
    }
    public Vector3D Offset {
      get { return offset; }
    }
    public Vector3D Hip {
      get { return hip; }
    }
    public Vector3D Thigh {
      get { return thigh; }
    }
    public Vector3D Shank {
      get { return shank; }
    }
    public Boundary Support {
      get { return support; }
    }
    public Boundary SwitchLeg {
      get { return switchLeg; }
    }
    public Vector3D ZeroPoint {
      get { return zeroPoint; }
    }
    public Vector3D Point {
      get { return endPoint; }
    }

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="Lambda">Thigh offset in degrees (messured ccw from pos. X-direction)</param>
    /// <param name="Offset">Vector between the hexapod's center and the connection point of the leg</param>
    /// <param name="Hip">Vector which describes the leg's hip</param>
    /// <param name="Thigh">Vector which describes the leg's thigh</param>
    /// <param name="Shank">Vector which describes the leg's shank</param>
    /// <param name="SupportCrit">Support-Criteria</param>
    /// <param name="SwitchLegCrit">Switch-Criteria</param>
    public HexaLeg(double Lambda, Vector3D Offset, Vector3D Hip, Vector3D Thigh, Vector3D Shank, Boundary SupportCrit, Boundary SwitchLegCrit) {
      lambda = Lambda * Math.PI / 180.0; // Convert to radians
      offset = Offset;
      hip = Hip;
      thigh = Thigh;
      shank = Shank;

      // Rotate leg to respect lambda (Rotate around Z-Axis)
      Vector3D temp = new Vector3D(hip + thigh + shank);
      temp.RotZ(lambda);

      // set zeropoint
      zeroPoint = new Vector3D(temp + offset);
      endPoint = zeroPoint;

      support = SupportCrit;
      switchLeg = SwitchLegCrit;

      // Calculate angles for zero-position. (should give 0° for every angle)
      CalculateAngles(endPoint);
    }

    /// <summary>
    ///   Method to calculate the necessary angles according to a given end-point
    /// </summary>
    /// <param name="Point">Point which should be approached by the leg</param>
    public void CalculateAngles(Vector3D Point) {
      endPoint = Point - offset;

      
      // Not necessary, since it won't happen, when educated users are using this program
      /*if (
        this.endPoint.Size > this.thigh.Size + this.shank.Size ||
        this.endPoint.Size < Math.Abs(this.shank.Size - this.thigh.Size)
      ) {
        throw new OutOfBoundry("Out of Boundry");
      }*/

      // Azimuth
      double d = endPoint.SizeXY;
      double e = hip.Y + thigh.Y + shank.Y;
      double zet = Math.Acos(e / d);
      double xi = Math.Atan2(endPoint.X, endPoint.Y);

      if(lambda > 4.71238898) // lambda > 270°?
        xi -= 2 * Math.PI;

      alpha = zet - xi - lambda;

      // Elevation
      Vector3D nP = new Vector3D(endPoint);
      nP.RotZ(-alpha - lambda);

      double dZ = nP.Z - hip.Z;
      double dF = nP.X - hip.X;
      double wSq = dZ * dZ + dF * dF;
      double w = Math.Sqrt(wSq);

      double bet0 = Math.Atan2(thigh.Z, thigh.X);
      double gam0 = Math.Atan2(shank.Z, shank.X);

      double omg = Math.Atan2(dZ, dF);

      double numer = wSq - thigh.SizeXZ_Sq - shank.SizeXZ_Sq;
      double denom = 2.0 * thigh.SizeXZ * shank.SizeXZ;

      double gam = Math.PI - Math.Acos(numer / denom);

      double eps = Math.Asin(Math.Sin(gam) * shank.SizeXZ / Math.Abs(w));
      double psi = eps + omg;

      beta = bet0 - psi;
      gamma = Math.PI + gam0 - gam - psi;
    }

    /// <summary>
    ///   Set the leg's angles manually
    /// </summary>
    /// <param name="Alpha">Angle Alpha</param>
    /// <param name="Beta">Angle Beta</param>
    /// <param name="Gamma">Angle Gamma</param>
    public void SetAngles(byte Alpha, byte Beta, byte Gamma) {
      alpha = Alpha;
      beta = Beta;
      gamma = Gamma;
    }

    /// <summary>
    ///   Were any support-criteria reached?
    /// </summary>
    /// <returns></returns>
    public bool NeedSupport() {
      if(Math.Abs(alpha) > support.Alpha / 180.0 * Math.PI)
        return true;

      if(endPoint.SizeXY - zeroPoint.SizeXY > support.DistanceXY) 
        return true;

      return false;
    }

    /// <summary>
    ///   Were any switch-criteria reached?
    /// </summary>
    /// <returns></returns>
    public bool Supportable() {
      if(Math.Abs(alpha) > switchLeg.Alpha / 180.0 * Math.PI)
        return true;

      if(endPoint.SizeXY - zeroPoint.SizeXY > switchLeg.DistanceXY)
        return true;

      return false;
    }
  }
}