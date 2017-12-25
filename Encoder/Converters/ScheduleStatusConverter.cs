using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using NORSU.EncodeMe.Network;

namespace NORSU.EncodeMe.Converters
{
    class ScheduleStatusToBrushConverter : ConverterBase
    {
        private Brush Accepted, Closed, Normal;
        public ScheduleStatusToBrushConverter(Brush accepted, Brush closed, Brush normal)
        {
            Accepted = accepted;
            Closed = closed;
            Normal = normal;
        }
        
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var status = (ScheduleStatuses) value;
            switch (status)
            {
                case ScheduleStatuses.Pending:
                    return Normal;
                case ScheduleStatuses.Accepted:
                    return Accepted;
                case ScheduleStatuses.Conflict:
                    return Normal;
                case ScheduleStatuses.Closed:
                    return Closed;
                default:
                    return Normal;
            }
        }
    }
    
    class ScheduleStatusConverter : ConverterBase
    {
        
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var status =(ScheduleStatuses) value;
            switch (status)
            {
                case ScheduleStatuses.Pending:
                    return PackIconKind.HelpCircle;
                case ScheduleStatuses.Accepted:
                    return PackIconKind.CheckCircle;
                case ScheduleStatuses.Conflict:
                    return PackIconKind.AlertCircle;
                case ScheduleStatuses.Closed:
                    return PackIconKind.CloseCircle;
                default:
                    return Binding.DoNothing;
            }
        }
    }
}
