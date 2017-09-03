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
        new KeyGesture(Key.F4, ModifierKeys.Alt)
      }
    );

    public static readonly RoutedUICommand AddDevice = new RoutedUICommand("Add Device", "Add Device", typeof(CustomCommands),
      new InputGestureCollection() {
        new KeyGesture(Key.A, ModifierKeys.Control)
      }
    );
  }
}
