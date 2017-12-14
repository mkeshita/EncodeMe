using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace NORSU.EncodeMe.Converters
{
    class EnumDescription : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (value != null)
            {
                var fi = value.GetType().GetField(value.ToString());
                if (fi != null)
                {
                    if (!fi.IsDefined(typeof(DescriptionAttribute), true)) return value.ToString();
                    
                    var attrib = (DescriptionAttribute) fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
                    return attrib?.Description;
                }
            }
            return Binding.DoNothing;
        }
    }
}
