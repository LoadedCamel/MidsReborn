using Mids_Reborn.Core.Base.Data_Classes;
using System.ComponentModel;
using Mids_Reborn.Core;

namespace Mids_Reborn.UI.Controls;

[DesignerCategory("Code")]
public sealed class ArchetypeDropDownList : MidsDropDownList
{
    [Browsable(false)]
    public new Archetype? SelectedItem
    {
        get => base.SelectedItem as Archetype;
        set => base.SelectedItem = value;
    }
}

[DesignerCategory("Code")]
public sealed class OriginDropDownList : MidsDropDownList
{
    [Browsable(false)]
    public new string? SelectedItem => base.SelectedItem as string;
}

[DesignerCategory("Code")]
public sealed class PowersetDropDownList : MidsDropDownList
{
    [Browsable(false)]
    public new IPowerset? SelectedItem
    {
        get => base.SelectedItem as IPowerset;
        set => base.SelectedItem = value;
    }
}