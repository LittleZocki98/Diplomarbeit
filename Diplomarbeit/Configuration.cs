using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Diplomarbeit.Hexaleg;
using Diplomarbeit.Hexeptions;
using Diplomarbeit.Vector;

namespace Diplomarbeit.Configuration {
  
  class Config {
    private class tempLeg {
      public double Lambda { get; set; }
      public SimpleVector Offset { get; set; }
      public SimpleVector Hip { get; set; }
      public SimpleVector Thigh { get; set; }
      public SimpleVector Shank { get; set; }
      public Boundary Support { get; set; }
      public Boundary Switch { get; set; }
    }
    private class configFile {
      public List<tempLeg> Legs { get; set; }
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

    public List<HexaLeg> Read() {
      List<HexaLeg> legs = new List<HexaLeg>();
      Dictionary<string, configFile> x = new Dictionary<string, configFile>();
      configFile cF = new configFile();

      string config = File.ReadAllText(this.filePath);

      try {
        x = JsonConvert.DeserializeObject<Dictionary<string, configFile>>(config);
      } catch(Exception ex) {
        throw new ConfigError("[Read]" + ex.Message);
      }

      try {
        cF = x["Hexapod"];
        HexaLeg hl;

        foreach(tempLeg tL in cF.Legs) {
          hl = new HexaLeg(
            tL.Lambda,
            new Vector3D(tL.Offset),
            new Vector3D(tL.Hip),
            new Vector3D(tL.Thigh),
            new Vector3D(tL.Shank),
            tL.Support,
            tL.Switch
          );
          legs.Add(hl);
        }

      } catch(Exception ex) {
        throw new ConfigError("[Mapping]" + ex.Message);
      }

      return legs;
    }

    /*public void Write(List<Hexaleg.HexaLeg> Legs) {
      List<tempLeg> tempList = new List<tempLeg>();
      tempLeg tL = new tempLeg();
      

      foreach(Hexaleg.HexaLeg l in Legs) {
        tL.Lambda = l.Lambda;
        tL.Offset = new SimpleVector(l.Offset);
        tL.Hip = new SimpleVector(l.Hip);
        tL.Thigh = new SimpleVector(l.Thigh);
        tL.Shank = new SimpleVector(l.Shank);
        tempList.Add(tL);
      }

      Dictionary<string, List<tempLeg>> DictLegs = new Dictionary<string, List<tempLeg>>();
      DictLegs.Add("Legs", tempList);

      Dictionary<string, Dictionary<string, List<tempLeg>>> DictHex = new Dictionary<string, Dictionary<string, List<tempLeg>>>();
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
