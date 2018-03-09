using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using Diplomarbeit.Hexeptions;

namespace Diplomarbeit {
  class Connection {
    private SerialPort port;
    private bool connected;

    public bool Connected { get { return this.connected; } }

    /// <summary>
    ///   Get a list of possible ports
    /// </summary>
    /// <returns>List of possible ports</returns>
    public static List<string> GetPorts() {
      List<string> possiblePorts = new List<string>();
      possiblePorts.AddRange(SerialPort.GetPortNames());
      possiblePorts.Sort();
      return possiblePorts;
    }

    /// <summary>
    /// 
    /// </summary>
    public static void AddDevice() {
      try {
        Process p = Process.Start(@"C:\Windows\System32\DevicePairingWizard.exe");
      } catch(Exception ex) {
        throw new ConnectionError("Error while adding device", ex);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public static void RemoveDevice() {
      try {
        Process p = Process.Start(@"C:\Windows\System32\control.exe");
      } catch(Exception ex) {
        throw new ConnectionError("Error while removing device", ex);
      }
    }

    public Connection() {
      this.port = new SerialPort();

      this.connected = new bool();
      this.connected = false;
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
      if(!this.connected) {
        try {
          this.port = new SerialPort(portName, boudRate, parity, dataBits, stopBits);
          this.port.Open();
          this.connected = true;
        } catch(Exception ex) {
          throw new ConnectionError("Error while initializing connection", ex);
        }
      }
    }

    /// <summary>
    ///   Close the connection
    /// </summary>
    public void CloseConnection() {
      if(this.connected) {
        try {
          this.port.Close();
          this.connected = false;
        } catch(Exception ex) {
          throw;
        }
      }
    }

    /// <summary>
    ///   Send a buffer of bytes
    /// </summary>
    /// <param name="buffer">The byte array that contains the data to write to the port</param>
    /// <param name="offset">The zero-based byte offset in the <paramref name="buffer"/> parameter at which to begin copying bytes to the port</param>
    /// <param name="count">The number of bytes to write</param>
    public void Send(byte[] buffer, int offset, int count) {
      if(this.connected) {
        try {
          this.port.Write(buffer, offset, count);
        } catch(Exception ex) {
          throw new ConnectionError("Error while sending", ex);
        }
      }
    }
  }
}
