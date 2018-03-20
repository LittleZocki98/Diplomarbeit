using System;

/// <summary>
/// Namespace of various custom exceptions
/// </summary>
namespace Diplomarbeit.Hexeptions {
  class Hexeption : Exception {
    public Hexeption() { }
    public Hexeption(string message) : base(message) { }
    public Hexeption(string message, Exception inner) : base(message, inner) { }
  }
  class OutOfBoundry : Exception {
    public OutOfBoundry() { }
    public OutOfBoundry(string message) : base(message) { }
    public OutOfBoundry(string message, Exception inner) : base(message, inner) { }
  }
  class SwitchLegs : Exception {
    public SwitchLegs() { }
    public SwitchLegs(string message) : base(message) { }
    public SwitchLegs(string message, Exception inner) : base(message, inner) { }
  }
  class ConnectionError : Exception {
    public ConnectionError() { }
    public ConnectionError(string message) : base(message) { }
    public ConnectionError(string message, Exception inner) : base(message, inner) { }
  }
  class ConfigError : Exception {
    public ConfigError() { }
    public ConfigError(string message) : base(message) { }
    public ConfigError(string message, Exception inner) : base(message, inner) { }
  }
}
