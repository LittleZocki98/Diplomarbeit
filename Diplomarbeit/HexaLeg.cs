using System;

namespace Diplomarbeit {
  class HexaLeg {
    /// <summary>
    ///   Offset -> Vector by which the whole leg is offset in reference to the main body
    ///   Thigh  -> Vector which describes the first segment of the leg (length, internal offset between start and end)
    ///   Shank  -> Vector which describes the second segment of the leg (length, internal offset between start and end)
    /// </summary>
    private Vector3D offset, thigh, shank;

    /// <summary>
    ///   Point on which the leg is pointing referenced to the main body
    /// </summary>
    private Vector3D point;

    /// <summary>
    ///   Alpha -> turning angle on the vertical axis
    ///   Beta  -> angle of the hip axis
    ///   Gamma -> angle of the knee axis
    /// </summary>
    private double alpha, beta, gamma;

    public double Alpha {
      get { return this.alpha; }
      set { this.alpha = value; }
    }
    public double Beta {
      get { return this.beta; }
      set { this.beta = value; }
    }
    public double Gamma {
      get { return this.gamma; }
      set { this.gamma = value; }
    }

    public Vector3D Point {
      get { return this.point; }
    }

    public HexaLeg(Vector3D Offset, Vector3D Thigh, Vector3D Shank) {
      this.offset = Offset;
      this.thigh = Thigh;
      this.shank = Shank;

      this.point = new Vector3D(0, 0, 0);

      this.alpha = 0.0;
      this.beta = 0.0;
      this.gamma = 0.0;
    }

    /// <summary>
    ///   Method to calculate the necessary angles from an given end-point
    /// </summary>
    /// <param name="Point">Point which should be approached with the leg</param>
    public void CalculateAngles(Vector3D Point) {
      this.point = Point;

      double a = Math.Sqrt(Point.X * Point.X + Point.Y * Point.Y);
      double b = (Point.Z - this.offset.Z) * (Point.Z - this.offset.Z);
      double c = (a - this.offset.X) * (a - this.offset.X);

      this.alpha = Math.Atan2(Point.Y, Point.X) - Math.Asin((this.offset.Y + this.thigh.Y) / a);
      this.gamma = Math.Acos(((b + c) - (this.thigh.Z * this.thigh.Z) - (this.shank.Z * this.shank.Z)) / (2.0 * this.thigh.Z * this.shank.Z));
      this.beta = (Math.PI / 2.0) - (Math.Atan2(Point.Z - this.offset.Z, a - this.offset.X) + Math.Asin(Math.Sin(gamma) * (this.shank.Z / Math.Sqrt(b + c))));
    }
  }
}