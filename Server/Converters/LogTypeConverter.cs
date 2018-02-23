using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe.Models;

namespace NORSU.EncodeMe.Converters
{
    class LogTypeConverter : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (!(value is TerminalLog.Types type)) return Binding.DoNothing;
            switch (type)
            {
                case TerminalLog.Types.Information:
                    return PackIconKind.Information;
                    break;
                case TerminalLog.Types.Error:
                    return PackIconKind.CloseCircle;
                    break;
                case TerminalLog.Types.Warning:
                    return PackIconKind.Alert;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
