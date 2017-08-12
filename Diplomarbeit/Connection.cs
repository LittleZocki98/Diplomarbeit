using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

namespace Diplomarbeit {
  class Connection {
    private SerialPort port;

    public Connection() { }

    /// <summary>
    ///   Get a list of possible ports
    /// </summary>
    /// <returns>List of possible ports</returns>
    public List<string> GetPorts() {
      List<string> possiblePorts = new List<string>();
      possiblePorts.AddRange(SerialPort.GetPortNames());
      return possiblePorts;
    }

    /// <summary>
    ///   Initializes the port
    /// </summary>
    /// <param name="portName">Name of the port (COM#)</param>
    /// <param name="boudRate">Boud Rate of the connection</param>
    /// <param name="parity">Parity configuration</param>
    /// <param name="dataBits">Amount of data bits sent</param>
    /// <param name="stopBits">Stop bit configuration</param>
    public void InitConnection(string portName, int boudRate, Parity parity, int dataBits, StopBits stopBits) {
      try {
        port = new SerialPort(portName, boudRate, parity, dataBits, stopBits);
        port.Open();
      } catch(Exception e) {
        // TO DO
      }
    }

    /// <summary>
    ///   Close the connection
    /// </summary>
    public void CloseConnection() {
      try {
        this.port.Close();
      }
      catch(Exception e) {
        // TO DO
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buffer">The byte array that contains the data to write to the port</param>
    /// <param name="offset">The zero-based byte offset in the <paramref name="buffer"/> parameter at which to begin copying bytes to the port</param>
    /// <param name="count">The number of bytes to write</param>
    public void Send(byte[] buffer, int offset, int count) {
      try {
        port.Write(buffer, offset, count);
      } catch(Exception e) {
        // TO DO
      }
    }
  }
}
