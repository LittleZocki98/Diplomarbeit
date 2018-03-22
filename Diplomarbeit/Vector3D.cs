using System;

namespace Diplomarbeit.Vector {

  /// <summary>
  ///   Simplified Vector for configuration
  /// </summary>
  public class SimpleVector {
    // Public 'getter' to prevent overwriting
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    /// <summary>
    ///   Constructor
    /// </summary>
    public SimpleVector() { }

    /// <summary>
    ///   Constructor
    ///   (Downgrade Vector3D to simplified)
    /// </summary>
    /// <param name="v"></param>
    public SimpleVector(Vector3D v) {
      X = v.X;
      Y = v.Y;
      Z = v.Z;
    }
  }

  public class Vector3D {
    /// <summary>
    ///   X, Y, Z -> vector dimensions
    /// </summary>
    private double x, y, z;

    //Public 'getter' for diverse parameters (to prevent overwriting and external calculations)
    public double X {
      get { return x; }
      set { x = value; }
    }
    public double Y {
      get { return y; }
      set { y = value; }
    }
    public double Z {
      get { return z; }
      set { z = value; }
    }
    public double SizeXY {
      get { return Math.Sqrt(x * x + y * y); }
    }
    public double SizeXY_Sq {
      get { return x * x + y * y; }
    }
    public double SizeXZ {
      get { return Math.Sqrt(x * x + z * z); }
    }
    public double SizeXZ_Sq {
      get { return x * x + z * z; }
    }
    public double SizeYZ {
      get { return Math.Sqrt(y * y + z * z); }
    }
    public double SizeYZ_Sq {
      get { return y * y + z * z; }
    }
    public double Size {
      get { return Math.Sqrt(x * x + y * y + z * z); }
    }
    public double Size_Sq {
      get { return x * x + y * y + z * z; }
    }

    // override some operators
    // Vector addition
    public static Vector3D operator +(Vector3D v1, Vector3D v2) => new Vector3D( v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z );
    // Vector subtraction
    public static Vector3D operator -(Vector3D v1, Vector3D v2) => new Vector3D( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );
    // Multiply a vector with a number
    public static Vector3D operator *(Vector3D v1, double d) => new Vector3D(v1.X * d, v1.Y * d, v1.Z * d);
    // Multiply a vector by a vactor (scalar product)
    public static double operator *(Vector3D v1, Vector3D v2) => v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
    // Multiply a vector by a vector (crossproduct)
    public static Vector3D operator ^(Vector3D v1, Vector3D v2) => new Vector3D(v1.y*v2.z - v1.z*v2.y, v1.z*v2.y - v1.x*v2.z, v1.x*v2.y - v1.y*v2.x);
    
    // override the ToString() method
    public override string ToString() {
      return "[" + X + ", " + Y + ", " + Z + "]";
    }

    /// <summary>
    ///   Constructor
    /// </summary>
    public Vector3D() {
      x = 0.0;
      y = 0.0;
      z = 0.0;
    }

    /// <summary>
    ///   Constructor
    ///   (And Init with X,Y,Z)
    /// </summary>
    /// <param name="X">Length in X</param>
    /// <param name="Y">Length in Y</param>
    /// <param name="Z">Length in Z</param>
    public Vector3D(double X, double Y, double Z) {
      x = X;
      y = Y;
      z = Z;
    }

    /// <summary>
    ///   Constructor
    ///   (Copy Vector)
    /// </summary>
    /// <param name="v"></param>
    public Vector3D(Vector3D v) {
      x = v.x;
      y = v.y;
      z = v.z;
    }

    /// <summary>
    ///   Constructor
    ///   (Upgrade SimpleVector to Vector3D)
    /// </summary>
    /// <param name="v"></param>
    public Vector3D(SimpleVector v) {
      x = v.X;
      y = v.Y;
      z = v.Z;
    }

    /// <summary>
    ///   Rotate this vector around the X-axis
    /// </summary>
    /// <param name="angle">Rotation angle</param>
    public void RotX(double angle) {
      double t = y;
      y = t * Math.Cos(angle) - z * Math.Sin(angle);
      z = z * Math.Cos(angle) + t * Math.Sin(angle);
    }

    /// <summary>
    ///   Rotate this vector around the Y-axis
    /// </summary>
    /// <param name="angle">Rotation angle</param>
    public void RotY(double angle) {
      double t = x;
      x = t * Math.Cos(angle) + z * Math.Sin(angle);
      z = z * Math.Cos(angle) - t * Math.Sin(angle);
    }

    /// <summary>
    ///   Rotate this vector around the Z-axis
    /// </summary>
    /// <param name="angle">Rotation angle</param>
    public void RotZ(double angle) {
      double t = x;
      x = t * Math.Cos(angle) - y * Math.Sin(angle);
      y = y * Math.Cos(angle) + t * Math.Sin(angle);
    }
  }
}
