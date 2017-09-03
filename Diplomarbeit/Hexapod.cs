using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplomarbeit {
  class Hexapod {
    private List<HexaLeg> legs;

    public Hexapod() {
      this.legs = new List<HexaLeg>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Leg"></param>
    public void AddLeg(HexaLeg Leg) {
      this.legs.Add(Leg);
    }
  }
}
