﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplomarbeit {
  class Vector3D {
    private double x, y, z;

    public double X {
      get { return this.X; }
      set { this.x = value; }
    }
    public double Y {
      get { return this.Y; }
      set { this.y = value; }
    }
    public double Z {
      get { return this.Z; }
      set { this.z = value; }
    }

    public Vector3D(double X, double Y, double Z) {
      this.x = X;
      this.y = Y;
      this.z = Z;
    }
  }
}
