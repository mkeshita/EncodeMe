using System;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe;

namespace NORSU.EncodeMe.ViewModels
{
    class ScreenMenu
    {
        public ScreenMenu(string name, PackIconKind icon, Action<object> execute, Predicate<object> canExecute)
        : this(name, icon, new DelegateCommand(execute, canExecute)) { }
        public ScreenMenu(string name, PackIconKind icon, ICommand command = null)
        {
            Icon = icon;
            Name = name;
            Command = command;
        }

        public PackIconKind Icon { get; set; }
        public string Name { get; set; }
        public ICommand Command { get; set; }
    }
}
