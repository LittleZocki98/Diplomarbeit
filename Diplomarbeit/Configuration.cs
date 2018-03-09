using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Diplomarbeit.Hexeptions;
using Diplomarbeit.Vector;

namespace Diplomarbeit.Configuration {
  
  class Config {
    private class temp {
      public double Lambda { get; set; }
      public SimpleVector Offset { get; set; }
      public SimpleVector Hip { get; set; }
      public SimpleVector Thigh { get; set; }
      public SimpleVector Shank { get; set; }
    }

    private string filePath;

    public Config(string Path) {

      if (File.Exists(Path)) {
        this.filePath = Path;
      } else {
        this.filePath = string.Empty;
        throw new ConfigError("Could not find Configurationfile at \"" + Path + "\"");
      }
    }

    public List<Hexaleg.HexaLeg> Read() {
      Dictionary<string, Dictionary<string, List<temp>>> x = new Dictionary<string, Dictionary<string, List<temp>>>();
      Dictionary<string, List<temp>> y;
      List<temp> z;

      List<Hexaleg.HexaLeg> legs = new List<Hexaleg.HexaLeg>();

      string config = File.ReadAllText(this.filePath);

      try {
        x = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<temp>>>>(config);
      } catch(Exception ex) {
        throw new ConfigError("[Read]" + ex.Message);
      }

      try {
        foreach(string s1 in x.Keys) {
          y = x[s1];
          foreach(string s2 in y.Keys) {
            z = y[s2];
            foreach(temp t in z) {
              Hexaleg.HexaLeg hl = new Hexaleg.HexaLeg(t.Lambda, new Vector3D(t.Offset), new Vector3D(t.Hip), new Vector3D(t.Thigh), new Vector3D(t.Shank));
              legs.Add(hl);
            }
          }
        }
      } catch(Exception ex) {
        throw new ConfigError("[Mapping]" + ex.Message);
      }

      return legs;
    }

    /*public void Write(List<Hexaleg.HexaLeg> Legs) {
      List<temp> tempList = new List<temp>();
      temp tempLeg = new temp();
      

      foreach(Hexaleg.HexaLeg l in Legs) {
        tempLeg.Lambda = l.Lambda;
        tempLeg.Offset = new SimpleVector(l.Offset);
        tempLeg.Hip = new SimpleVector(l.Hip);
        tempLeg.Thigh = new SimpleVector(l.Thigh);
        tempLeg.Shank = new SimpleVector(l.Shank);
        tempList.Add(tempLeg);
      }

      Dictionary<string, List<temp>> DictLegs = new Dictionary<string, List<temp>>();
      DictLegs.Add("Legs", tempList);

      Dictionary<string, Dictionary<string, List<temp>>> DictHex = new Dictionary<string, Dictionary<string, List<temp>>>();
      DictHex.Add("Hexapod", DictLegs);
      try {
        string confString = JsonConvert.SerializeObject(DictHex, Formatting.Indented);
        File.WriteAllText(this.filePath, confString);
      } catch(Exception ex) {
        throw new ConfigError("[Writing]" + ex.Message);
      }
    }*/
  }
}
