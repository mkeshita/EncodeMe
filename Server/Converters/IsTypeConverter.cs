using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NORSU.EncodeMe.Converters
{
    class IsTypeConverter : ConverterBase
    {
        public Type Type { get; set; }
        protected override object Convert(object value, Type targetType, object parameter)
        {
            return value?.GetType() == Type;
        }
    }
}
