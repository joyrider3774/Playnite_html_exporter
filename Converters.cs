using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace HtmlExporterPlugin
{
    public class BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolvalue)
            {
                return boolvalue ? Constants.HTMLExporterTrue : Constants.HTMLExporterFalse;
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringvalue)
            {
                if (stringvalue == Constants.HTMLExporterTrue)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return null;
        }
    }

    public class FieldToFieldTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringvalue)
            {
                return Constants.GetNameFromField(stringvalue, false);
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringvalue)
            {
                return Constants.GetFieldFromName(stringvalue, false);
            }
            return "";
        }
    }

    public class RowNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataGridRow row = (DataGridRow)value;
            if (row == null)
                throw new InvalidOperationException("This converter class can only be used with DataGridRow elements.");

            return row.GetIndex() + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
