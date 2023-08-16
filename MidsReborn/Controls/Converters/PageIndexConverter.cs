using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using FormPagesDesigner = Mids_Reborn.Controls.Designer.FormPagesDesigner;

namespace MetaControls.Converters
{
    public class PageIndexConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
        {
            return true;
        }

        public override StandardValuesCollection? GetStandardValues(ITypeDescriptorContext? context)
        {
            if (context?.Instance is not FormPagesDesigner.FormPagesActionList actionList)
                return base.GetStandardValues(context);
            var pageIndexes = new List<int>();
            for (var i = 0; i < actionList.PageCollection.Count; i++)
            {
                pageIndexes.Add(i);
            }
            return new StandardValuesCollection(pageIndexes);

        }

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (context?.Instance is not FormPagesDesigner.FormPagesActionList actionList ||
                value is not string stringValue) return -1; // Default value, indicating no selection
            if (!int.TryParse(stringValue, out var index)) return -1; // Default value, indicating no selection
            if (index < 0 || index >= actionList.PageCollection.Count)
                return -1; // Default value, indicating no selection
            actionList.SelectedPageIndexDropDown = index;
            actionList.Refresh();
            return index;
        }
    }
}
