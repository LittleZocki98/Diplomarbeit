using System;

namespace Diplomarbeit {
  class HexaLeg {

    private Vector3D offset, thigh, shank;
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

    public HexaLeg(Vector3D Offset, Vector3D Thigh, Vector3D Shank) {
      this.alpha = 0.0;
      this.beta = 0.0;
      this.gamma = 0.0;
    }

    public void CalculateAngles(Vector3D Point) {
      double a = Math.Sqrt(Point.X * Point.X + Point.Y * Point.Y);
      double b = (Point.Z - this.offset.Z) * (Point.Z - this.offset.Z);
      double c = (a - this.offset.X) * (a - this.offset.X);

      this.alpha = Math.Atan2(Point.Y, Point.X) - Math.Asin((this.offset.Y + this.thigh.Y) / a);
      this.gamma = Math.Acos(((b + c) - (this.thigh.Z * this.thigh.Z) - (this.shank.Z * this.shank.Z)) / (2.0 * this.thigh.Z * this.shank.Z));
      this.beta = (Math.PI / 2.0) - (Math.Atan2(Point.Z - this.offset.Z, a - this.offset.X) + Math.Asin(Math.Sin(gamma) * (this.shank.Z / Math.Sqrt(b + c))));
    }
  }
}