﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Rnet.Monitor.Wpf
{
    class ResourceKeyToResourceConverter : IMultiValueConverter
    {
        // expects the target object as the first parameter, and the resource key as the second
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2)
            {
                return null;
            }

            var element = values[0] as FrameworkElement;
            var resourceKey = values[1];
            if (ResourceKeyConverter != null)
            {
                resourceKey = ResourceKeyConverter.Convert(resourceKey, targetType, ConverterParameter, culture);
            }
            else if (StringFormat != null && resourceKey is string)
            {
                resourceKey = string.Format(StringFormat, resourceKey);
            }

            var resource = element.TryFindResource(resourceKey);

            return resource;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public IValueConverter ResourceKeyConverter { get; set; }

        public object ConverterParameter { get; set; }

        public string StringFormat { get; set; }
    }
}
