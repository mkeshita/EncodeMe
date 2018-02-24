using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NORSU.EncodeMe.Converters
{
    class StudentStatusConverter  :ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var stat = (int) value;
            if (stat == 0) return "OnGoing";
            if (stat == 1) return "Returnee";
            if (stat == 2) return "Shiftee";
            if (stat == 3) return "Transferee";
            
            return "N/A";
        }
    }
}
