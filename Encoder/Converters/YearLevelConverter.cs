using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace NORSU.EncodeMe.Converters
{
    class YearLevelConverter : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            switch((int)value)
            {
                case 0:
                    return "FIRST YEAR";
                case 1:
                    return "SECOND YEAR";
                case 2:
                    return "THIRD YEAR";
                case 3:
                    return "FOURTH YEAR";
                case 4:
                    return "FIFTH YEAR";
            }
            return "N/A";
        }
    }
}
