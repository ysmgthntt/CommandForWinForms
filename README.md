# RoutedCommands for Windows Forms

**RoutedCommandsForWinForms** is a command routing library for Windows Forms applications.

It provides routed command processing with `CommandBinding`, including tunneling and bubbling event routing, allowing command handling to be separated from the controls that initiate commands.

Although Windows Forms introduced built-in Command support in .NET 7, this library provides routed command functionality that is not available through the standard WinForms Command API. It also enables Command functionality in applications targeting .NET Framework.

## Features

- **Routed command processing** – Commands are routed through the control hierarchy, allowing command handlers to be defined at different levels.
- **Tunneling and bubbling** – Supports `PreviewCanExecute`, `CanExecute`, and `Executed` event routing.
- **CommandBinding** – Associates commands with handlers for execution and execution availability.
- **Input gestures** – Supports keyboard and mouse gestures through `KeyBinding`, `MouseBinding`, and `InputBinding`.
- **Command sources** – Commands can be associated with buttons and tool strip items.
- **Command availability** – Automatically updates command availability based on `CanExecute`.
- **.NET Framework support** – Provides Command functionality for applications targeting .NET Framework, including a compatible `ICommand` implementation for .NET Framework 3.5.

## Installation

Install the NuGet package.

Using the .NET CLI:

```bash
dotnet add package RoutedCommandsForWinForms
```

Using the Package Manager Console:

```powershell
Install-Package RoutedCommandsForWinForms
```

## Getting Started

### 1. Initialize CommandManager

Call `CommandManager.Initialize` with the form to enable command processing and other command-related functionality within that form.

```csharp
using System.Windows.Forms;
using CommandForWinForms;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();

        CommandManager.Initialize(this);
    }
}
```

### 2. Define a command and its handler

Create a command and associate it with a `CommandBinding`.

```csharp
private static readonly UICommand SaveCommand = new UICommand("Save", "Save");

public MainForm()
{
    InitializeComponent();

    CommandManager.Initialize(this);

    this.GetCommandBindings().Add(
        new CommandBinding(
            SaveCommand,
            executed: Save_Executed,
            canExecute: Save_CanExecute));
}

private void Save_Executed(object? sender, ExecutedEventArgs e)
{
    // Save the document.
}

private void Save_CanExecute(object? sender, CanExecuteEventArgs e)
{
    e.CanExecute = IsDocumentModified;
}
```

### 3. Associate a command with a control

Commands can be associated with buttons and other supported command sources.

For applications targeting .NET 7 or later, you can assign a command directly to the `Command` property.

```csharp
saveButton.Command = SaveCommand;
```

For applications targeting earlier versions of .NET, or when using command sources that do not provide a `Command` property, use the `SetCommand` extension method.

```csharp
saveButton.SetCommand(SaveCommand);
```

The command is executed when the command source is activated, provided that the command can be executed.

### 4. Add an input binding

Commands can also be invoked using keyboard or mouse gestures.

```csharp
this.GetInputBindings().Add(
    new KeyBinding(
        SaveCommand,
        Keys.S,
        KeyModifiers.Control));
```

This example associates Ctrl+S with the Save command.

## Command Routing

Commands are routed through the control hierarchy, starting from the command target.

The routing behavior is as follows:

| Event | Routing direction |
|---|---|
| `PreviewCanExecute` | From the root toward the command target (tunneling) |
| `CanExecute` | From the command target toward the root (bubbling) |
| `Executed` | From the command target toward the root (bubbling) |

A `CommandBinding` can handle an event to stop further routing.

This allows commands to be handled by the control that initiated them, a parent control, or the form itself, depending on where the command bindings are defined.

## Supported Frameworks

RoutedCommandsForWinForms targets the following frameworks:

- .NET 5 or later
- .NET Framework 3.5 or later

## Documentation

This documentation was prepared with assistance from AI.

## License

This project is licensed under the [MIT License](LICENSE.txt).
