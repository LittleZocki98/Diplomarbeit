using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

/// <summary>
/// 
/// </summary>
namespace Diplomarbeit {
  public static class CustomCommands {
    public static readonly RoutedUICommand Exit = new RoutedUICommand("Exit", "Exit", typeof(CustomCommands),
      new InputGestureCollection() {
        new KeyGesture(Key.Q, ModifierKeys.Control)
      }
    );

    public static readonly RoutedUICommand AddDevice = new RoutedUICommand("Add Device", "Add Device", typeof(CustomCommands),
      new InputGestureCollection() {
        new KeyGesture(Key.A, ModifierKeys.Control)
      }
    );

    public static readonly RoutedUICommand RemoveDevice = new RoutedUICommand("Remove Device", "Remove Device", typeof(CustomCommands),
      new InputGestureCollection() {
        new KeyGesture(Key.D, ModifierKeys.Control)
      }
    );
  }
}
