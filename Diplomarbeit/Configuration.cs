using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Diplomarbeit.Hexaleg;
using Diplomarbeit.Hexeptions;
using Diplomarbeit.Vector;

namespace Diplomarbeit.Configuration {
  
  class Config {

    /// <summary>
    ///   private internal class to define the leg's structure inside the configuration structure
    /// </summary>
    private class tempLeg {
      public double Lambda { get; set; }
      public SimpleVector Offset { get; set; }
      public SimpleVector Hip { get; set; }
      public SimpleVector Thigh { get; set; }
      public SimpleVector Shank { get; set; }
      public Boundary Support { get; set; }
      public Boundary Switch { get; set; }
    }

    /// <summary>
    ///   private internal class to define the configuration structure
    /// </summary>
    private class configFile {
      public List<tempLeg> Legs { get; set; }
    }

    // Path to the configuration file
    private string filePath;

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="Path">Path to the configuration file</param>
    public Config(string Path) {

      if (File.Exists(Path)) {
        filePath = Path;
      } else {
        filePath = string.Empty;
        throw new ConfigError("Could not find Configurationfile at \"" + Path + "\"");
      }
    }

    /// <summary>
    ///   Read configuration file
    /// </summary>
    /// <returns>Read configuration in form of a list of finished legs</returns>
    public List<HexaLeg> Read() {
      List<HexaLeg> legs = new List<HexaLeg>();
      Dictionary<string, configFile> x = new Dictionary<string, configFile>();
      configFile cF = new configFile();

      string config = File.ReadAllText(this.filePath);

      // Try to convert read text to structure
      try {
        x = JsonConvert.DeserializeObject<Dictionary<string, configFile>>(config);
      } catch(Exception ex) {
        throw new ConfigError("[Read]" + ex.Message);
      }

      // Try to break down structure to individually legs
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
  }
}
