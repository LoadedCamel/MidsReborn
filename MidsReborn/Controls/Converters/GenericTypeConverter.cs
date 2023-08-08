using System;
using System.ComponentModel;
using System.Globalization;

namespace MetaControls.Converters
{
    public class GenericTypeConverter<T> : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext? context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
        {
            return TypeDescriptor.GetProperties(typeof(T));
        }
    }
}
