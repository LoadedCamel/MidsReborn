using System;
using System.ComponentModel;
using System.Windows.Forms;
using mrbBase.Utils;

namespace mrbBase.Base.Extensions
{
    public class FormExt : Form
    {
        private static GenResult _genResult;

        protected static GenResult GenResult
        {
            get => _genResult;
            set
            {
                if (IsEnumValid(value, (int)value, 0, 2))
                {
                    _genResult = value;
                }
                else
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(GenResult));
                }
            }
        }

        public GenResult ShowDialog(Control owner)
        {
            Show(owner);
            while (!IsDisposed)
            {
                Application.DoEvents();
            }
            return GenResult;
        }

        private static bool IsEnumValid(Enum enumValue, int value, int minValue, int maxValue)
        {
            var valid = (value >= minValue) && (value <= maxValue);
            return valid;
        }
    }
}
