using System.Windows.Input;

/// <summary>
///   This class manages the customized commands implemented in this program.
/// </summary>
namespace Diplomarbeit {
  public static class CustomCommands {
    // Strg + Q -> Exit program
    public static readonly RoutedUICommand Exit = new RoutedUICommand("Exit", "Exit", typeof(CustomCommands),
      new InputGestureCollection() {
        new KeyGesture(Key.Q, ModifierKeys.Control)
      }
    );

    // Strg + A -> Add device
    public static readonly RoutedUICommand AddDevice = new RoutedUICommand("Add Device", "Add Device", typeof(CustomCommands),
      new InputGestureCollection() {
        new KeyGesture(Key.A, ModifierKeys.Control)
      }
    );

    // Strg + D -> Remove Device
    public static readonly RoutedUICommand RemoveDevice = new RoutedUICommand("Remove Device", "Remove Device", typeof(CustomCommands),
      new InputGestureCollection() {
        new KeyGesture(Key.D, ModifierKeys.Control)
      }
    );
  }
}
