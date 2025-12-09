using Microsoft.DotNet.DesignTools.Designers;
using Mids_Reborn.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mids_Reborn.UI.Design.Designer
{
    public class ScrollPanelExDesigner : ParentControlDesigner
    {
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            if (component is ScrollPanelEx control)
            {
                // Tell the designer to use the inner panel
                EnableDesignMode(control.InnerPanel, "InnerPanel");
            }
        }
    }
}
