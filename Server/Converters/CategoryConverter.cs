using System;
using System.Linq;
using NORSU.EncodeMe.Models;

namespace Server.Converters
{
    class CategoryConverter : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var id = (long)value;
            return Category.Cache.FirstOrDefault(x => x.Id == id)?.ShortName;
        }
    }
}
