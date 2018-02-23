using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe.Models;
using Brushes = System.Drawing.Brushes;

namespace NORSU.EncodeMe.Converters
{
    class LogTypeToBrush:ConverterBase
    {
        private static SolidColorBrush _warningBrush;
        private SolidColorBrush WarningBrush
        {
            get
            {
                if (_warningBrush != null) return _warningBrush;
                _warningBrush = new SolidColorBrush(Colors.DarkOrange);
                _warningBrush.Freeze();
                return _warningBrush;
            }
        }

        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (!(value is TerminalLog.Types type))
                return Binding.DoNothing;
            
            switch (type)
            {
                case TerminalLog.Types.Information:
                    return Application.Current.FindResource("PrimaryHueDarkBrush");
                case TerminalLog.Types.Error:
                    return Application.Current.FindResource("ValidationErrorBrush");
                case TerminalLog.Types.Warning:
                    return WarningBrush;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
