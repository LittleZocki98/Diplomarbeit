using System;

namespace Diplomarbeit.Vector {
  public class VectorException : Exception {
    public VectorException() { }
    public VectorException(string message) : base(message) { }
    public VectorException(string message, Exception inner) : base(message, inner) { }
  }

  public class SimpleVector {
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public SimpleVector() { }
    public SimpleVector(Vector3D v) {
      this.X = v.X;
      this.Y = v.Y;
      this.Z = v.Z;
    }
  }

  public class Vector3D {
    /// <summary>
    ///   X, Y, Z -> vector dimensions
    /// </summary>
    private double x, y, z;

    public double X {
      get { return this.x; }
      set { this.x = value; }
    }
    public double Y {
      get { return this.y; }
      set { this.y = value; }
    }
    public double Z {
      get { return this.z; }
      set { this.z = value; }
    }

    public double SizeXY {
      get { return Math.Sqrt(this.x * this.x + this.y * this.y); }
    }
    public double SizeXY_Sq {
      get { return this.x * this.x + this.y * this.y; }
    }
    public double SizeXZ {
      get { return Math.Sqrt(this.x * this.x + this.z * this.z); }
    }
    public double SizeXZ_Sq {
      get { return this.x * this.x + this.z * this.z; }
    }
    public double SizeYZ {
      get { return Math.Sqrt(this.y * this.y + this.z * this.z); }
    }
    public double SizeYZ_Sq {
      get { return this.y * this.y + this.z * this.z; }
    }
    public double Size {
      get { return Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z); }
    }
    public double Size_Sq {
      get { return this.x * this.x + this.y * this.y + this.z * this.z; }
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
      return "[" + this.X + ", " + this.Y + ", " + this.Z + "]";
    }

    public Vector3D() {
      this.x = 0.0;
      this.y = 0.0;
      this.z = 0.0;
    }
    public Vector3D(double X, double Y, double Z) {
      this.x = X;
      this.y = Y;
      this.z = Z;
    }
    public Vector3D(Vector3D v) {
      this.x = v.x;
      this.y = v.y;
      this.z = v.z;
    }
    public Vector3D(SimpleVector v) {
      this.x = v.X;
      this.y = v.Y;
      this.z = v.Z;
    }

    public void RotX(double angle) {
      double t = this.y;
      this.y = t * Math.Cos(angle) - this.z * Math.Sin(angle);
      this.z = this.z * Math.Cos(angle) + t * Math.Sin(angle);
    }
    public void RotY(double angle) {
      double t = this.x;
      this.x = t * Math.Cos(angle) + this.z * Math.Sin(angle);
      this.z = this.z * Math.Cos(angle) - t * Math.Sin(angle);
    }
    public void RotZ(double angle) {
      double t = this.x;
      this.x = t * Math.Cos(angle) - this.y * Math.Sin(angle);
      this.y = this.y * Math.Cos(angle) + t * Math.Sin(angle);
    }
  }
}
