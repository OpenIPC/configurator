using System;
using System.Globalization;
using Avalonia.Data.Converters;
using OpenIPCConfigurator.Shared;

namespace OpenIPCConfigurator.Avalonia.Converters;

public class DeviceTypeToYamlEnabledConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DeviceType deviceType)
        {
            return deviceType == DeviceType.OpenIPC;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
