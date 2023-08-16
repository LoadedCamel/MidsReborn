using System;
using System.ComponentModel;

namespace Mids_Reborn.Controls.Converters
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
