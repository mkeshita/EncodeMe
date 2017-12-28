using System;
using MaterialDesignThemes.Wpf;

namespace NORSU.EncodeMe.Converters
{
    class IsOnlineToColorZone : ConverterBase
    {
        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (value as bool? ?? false)
                return ColorZoneMode.PrimaryLight;
            return ColorZoneMode.Dark;
            
            

        }
    }
}
