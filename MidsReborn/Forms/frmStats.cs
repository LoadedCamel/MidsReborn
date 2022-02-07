using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Master_Classes;
using mrbControls;

namespace Mids_Reborn.Forms
{
    public partial class frmStats : Form
    {
        private readonly frmMain myParent;

        private IPower[] BaseArray;
        private bool BaseOverride;
        private ImageButton btnClose;

        private ComboBox cbSet;

        private ComboBox cbStyle;

        private ComboBox cbValues;

        private ImageButton chkOnTop;

        private IPower[] EnhArray;
        private ctlMultiGraph Graph;
        private float GraphMax;
        private Label lblKey1;
        private Label lblKey2;
        private Label lblKeyColor1;
        private Label lblKeyColor2;
        private Label lblScale;
        private bool Loaded;

        private TrackBar tbScaleX;
        private ToolTip tTip;

        public frmStats(ref frmMain iParent)
        {
            FormClosed += frmStats_FormClosed;
            Load += frmStats_Load;
            Move += frmStats_Move;
            Resize += frmStats_Resize;
            VisibleChanged += frmStats_VisibleChanged;
            BaseArray = new IPower[0];
            EnhArray = new IPower[0];
            GraphMax = 1f;
            BaseOverride = false;
            Loaded = false;
            InitializeComponent();
            btnClose.ButtonClicked += btnClose_Click;
            chkOnTop.ButtonClicked += chkOnTop_CheckedChanged;
            Name = nameof(frmStats);
            var componentResourceManager = new ComponentResourceManager(typeof(frmStats));
            Icon = Resources.reborn;
            myParent = iParent;
        }

        private void btnClose_Click()

        {
            Close();
        }

        private void cbSet_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (!Loaded)
                return;
            GetPowerArray();
            DisplayGraph();
        }

        private void cbStyle_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (!Loaded)
                return;
            SetGraphType();
            DisplayGraph();
        }

        private void cbValues_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (!Loaded)
                return;
            DisplayGraph();
        }

        private void chkOnTop_CheckedChanged()

        {
            TopMost = chkOnTop.Checked;
        }

        private void DisplayGraph()
        {
            if ((MainModule.MidsController.Toon == null) | !MainModule.MidsController.IsAppInitialized)
                return;
            Graph.BeginUpdate();
            Graph.Clear();
            if (cbValues.SelectedIndex > -1)
                switch (cbValues.SelectedIndex)
                {
                    case 0:
                        Graph.ColorFadeEnd = Color.FromArgb(byte.MaxValue, byte.MaxValue, 0);
                        Graph_Acc();
                        break;
                    case 1:
                        Graph.ColorFadeEnd = Color.Red;
                        Graph_Damage();
                        break;
                    case 2:
                        Graph.ColorFadeEnd = Color.Red;
                        Graph_DPA();
                        break;
                    case 3:
                        Graph.ColorFadeEnd = Color.Red;
                        Graph_DPS();
                        break;
                    case 4:
                        Graph.ColorFadeEnd = Color.Red;
                        Graph_DPE();
                        break;
                    case 5:
                        Graph.ColorFadeEnd = Color.FromArgb(192, 192, byte.MaxValue);
                        Graph_End();
                        break;
                    case 6:
                        Graph.ColorFadeEnd = Color.FromArgb(192, 192, byte.MaxValue);
                        Graph_EPS();
                        break;
                    case 7:
                        Graph.ColorFadeEnd = Color.FromArgb(96, byte.MaxValue, 96);
                        Graph_Heal();
                        break;
                    case 8:
                        Graph.ColorFadeEnd = Color.FromArgb(96, byte.MaxValue, 96);
                        Graph_HealPS();
                        break;
                    case 9:
                        Graph.ColorFadeEnd = Color.FromArgb(96, byte.MaxValue, 96);
                        Graph_HealPE();
                        break;
                    case 10:
                        Graph.ColorFadeEnd = Color.FromArgb(128, 0, byte.MaxValue);
                        Graph_Duration();
                        break;
                    case 11:
                        Graph.ColorFadeEnd = Color.FromArgb(64, 128, 96);
                        Graph_Range();
                        break;
                    case 12:
                        Graph.ColorFadeEnd = Color.FromArgb(byte.MaxValue, 192, 128);
                        Graph_Recharge();
                        break;
                    case 13:
                        Graph.ColorFadeEnd = Color.FromArgb(96, 192, 96);
                        Graph_Regen();
                        break;
                }

            Graph.Max = GraphMax;
            tbScaleX.Value = Graph.ScaleIndex;
            SetScaleLabel();
            SetGraphMetrics();
            Graph.EndUpdate();
        }

        private void FillComboBoxes()

        {
            NewSets();
            cbValues.BeginUpdate();
            var items1 = cbValues.Items;
            items1.Clear();
            items1.Add("Accuracy");
            items1.Add("Damage");
            items1.Add("Damage / Anim");
            items1.Add("Damage / Sec");
            items1.Add("Damage / End");
            items1.Add("End Use");
            items1.Add("End / Sec");
            items1.Add("Healing");
            items1.Add("Heal / Sec");
            items1.Add("Heal / End");
            items1.Add("Effect Duration");
            items1.Add("Range");
            items1.Add("Recharge Time");
            items1.Add("Regeneration");
            cbValues.SelectedIndex = 1;
            cbValues.EndUpdate();
            cbStyle.BeginUpdate();
            var items2 = cbStyle.Items;
            items2.Clear();
            items2.Add("Base & Enhanced");
            items2.Add("Stacked Base + Enhanced");
            items2.Add("Base Only");
            items2.Add("Enhanced Only");
            items2.Add("Active & Alternate");
            items2.Add("Stacked Active + Alt");
            if (MidsContext.Config.StatGraphStyle > (Enums.GraphStyle) (cbStyle.Items.Count - 1))
                MidsContext.Config.StatGraphStyle = Enums.GraphStyle.Stacked;
            cbStyle.SelectedIndex = (int) MidsContext.Config.StatGraphStyle;
            cbStyle.EndUpdate();
        }

        private void frmStats_FormClosed(object sender, FormClosedEventArgs e)

        {
            myParent.FloatStatGraph(false);
        }

        private void frmStats_Load(object sender, EventArgs e)

        {
            FillComboBoxes();
            Loaded = true;
            tbScaleX.Minimum = 0;
            tbScaleX.Maximum = Graph.ScaleCount - 1;
            UpdateData(false);
        }

        private void frmStats_Move(object sender, EventArgs e)

        {
            StoreLocation();
        }

        private void frmStats_Resize(object sender, EventArgs e)

        {
            if (Graph != null)
            {
                Graph.Width = ClientSize.Width - (Graph.Left + 4);
                var graph = Graph;
                var height = ClientSize.Height;
                var top = Graph.Top;
                var clientSize1 = ClientSize;
                var num1 = clientSize1.Height - this.tbScaleX.Top;
                var num2 = top + num1 + 4;
                var num3 = height - num2;
                graph.Height = num3;
                var tbScaleX = this.tbScaleX;
                clientSize1 = ClientSize;
                var num4 = clientSize1.Width - (this.tbScaleX.Left + (ClientSize.Width - chkOnTop.Left) + 4);
                tbScaleX.Width = num4;
                lblScale.Left = (int) Math.Round(this.tbScaleX.Left + (this.tbScaleX.Width - lblScale.Width) / 2.0);
                var num5 = this.cbStyle.Left + 154;
                var clientSize2 = ClientSize;
                var num6 = clientSize2.Width - 3;
                if (num5 > num6)
                {
                    var cbStyle = this.cbStyle;
                    clientSize2 = ClientSize;
                    var num7 = clientSize2.Width - this.cbStyle.Left - 4;
                    cbStyle.Width = num7;
                }
                else
                {
                    cbStyle.Width = 154;
                }
            }

            StoreLocation();
        }

        private void frmStats_VisibleChanged(object sender, EventArgs e)

        {
        }

        private void GetPowerArray()
        {
            if ((MainModule.MidsController.Toon == null) | !MainModule.MidsController.IsAppInitialized)
                return;
            BaseArray = Array.Empty<IPower>();
            EnhArray = Array.Empty<IPower>();
            MainModule.MidsController.Toon.GenerateBuffedPowerArray();
            if (cbSet.SelectedIndex <= -1)
                return;
            switch (cbSet.SelectedIndex)
            {
                case 0:
                    GetSetArray(Enums.PowersetType.Primary, Enums.ePowerType.Auto_);
                    GetSetArray(Enums.PowersetType.Secondary, Enums.ePowerType.Auto_);
                    GetSetArray(Enums.PowersetType.Ancillary, Enums.ePowerType.Auto_);
                    GetSetArray(Enums.PowersetType.Pool0, Enums.ePowerType.Auto_);
                    GetSetArray(Enums.PowersetType.Pool1, Enums.ePowerType.Auto_);
                    GetSetArray(Enums.PowersetType.Pool2, Enums.ePowerType.Auto_);
                    GetSetArray(Enums.PowersetType.Pool3, Enums.ePowerType.Auto_);
                    break;
                case 1:
                    GetSetArray(Enums.PowersetType.Primary, Enums.ePowerType.Auto_);
                    GetSetArray(Enums.PowersetType.Secondary, Enums.ePowerType.Auto_);
                    break;
                case 2:
                    GetSetArray(Enums.PowersetType.Primary, Enums.ePowerType.Auto_);
                    break;
                case 3:
                    GetSetArray(Enums.PowersetType.Secondary, Enums.ePowerType.Auto_);
                    break;
                case 4:
                    GetSetArray(Enums.PowersetType.Ancillary, Enums.ePowerType.Auto_);
                    break;
                case 5:
                    GetSetArray(Enums.PowersetType.Pool0, Enums.ePowerType.Auto_);
                    GetSetArray(Enums.PowersetType.Pool1, Enums.ePowerType.Auto_);
                    GetSetArray(Enums.PowersetType.Pool2, Enums.ePowerType.Auto_);
                    GetSetArray(Enums.PowersetType.Pool3, Enums.ePowerType.Auto_);
                    break;
                case 6:
                    if (!BaseOverride)
                    {
                        var num = MidsContext.Character.CurrentBuild.Powers.Count - 1;
                        for (var iPower = 0; iPower <= num; ++iPower)
                        {
                            if (!((MidsContext.Character.CurrentBuild.Powers[iPower].NIDPowerset > -1) & (MidsContext.Character.CurrentBuild.Powers[iPower].IDXPower > -1)) || !DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[iPower].NIDPowerset].Powers[MidsContext.Character.CurrentBuild.Powers[iPower].IDXPower].Slottable) continue;
                            Array.Resize(ref BaseArray, BaseArray.Length + 1);
                            Array.Resize(ref EnhArray, EnhArray.Length + 1);
                            var index = BaseArray.Length - 1;
                            BaseArray[index] = new Power(DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[iPower].NIDPowerset].Powers[MainModule.MidsController.Toon.CurrentBuild.Powers[iPower].IDXPower]);
                            EnhArray[index] = MainModule.MidsController.Toon.GetEnhancedPower(iPower);
                        }

                        break;
                    }

                    var num1 = MidsContext.Character.CurrentBuild.Powers.Count - 1;
                    for (var iPower = 0; iPower <= num1; ++iPower)
                    {
                        if (!((MidsContext.Character.CurrentBuild.Powers[iPower].NIDPowerset > -1) & (MidsContext.Character.CurrentBuild.Powers[iPower].IDXPower > -1)) || !DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[iPower].NIDPowerset].Powers[MidsContext.Character.CurrentBuild.Powers[iPower].IDXPower].Slottable) continue;
                        Array.Resize(ref BaseArray, BaseArray.Length + 1);
                        //BaseArray = (IPower[]) Utils.CopyArray(BaseArray, new IPower[BaseArray.Length + 1]);
                        BaseArray[BaseArray.Length - 1] = MainModule.MidsController.Toon.GetEnhancedPower(iPower);
                    }

                    MainModule.MidsController.Toon.FlipAllSlots();
                    var num2 = MidsContext.Character.CurrentBuild.Powers.Count - 1;
                    for (var iPower = 0; iPower <= num2; ++iPower)
                    {
                        if (!((MidsContext.Character.CurrentBuild.Powers[iPower].NIDPowerset > -1) &
                              (MidsContext.Character.CurrentBuild.Powers[iPower].IDXPower > -1)) || !DatabaseAPI
                            .Database
                            .Powersets[MidsContext.Character.CurrentBuild.Powers[iPower].NIDPowerset]
                            .Powers[MidsContext.Character.CurrentBuild.Powers[iPower].IDXPower].Slottable)
                            continue;
                        Array.Resize(ref EnhArray, EnhArray.Length + 1);
                        //EnhArray = (IPower[]) Utils.CopyArray(EnhArray, new IPower[EnhArray.Length + 1]);
                        EnhArray[EnhArray.Length - 1] = MainModule.MidsController.Toon.GetEnhancedPower(iPower);
                    }

                    MainModule.MidsController.Toon.FlipAllSlots();
                    break;
                case 7:
                    GetSetArray(Enums.PowersetType.Primary, Enums.ePowerType.Toggle);
                    GetSetArray(Enums.PowersetType.Secondary, Enums.ePowerType.Toggle);
                    GetSetArray(Enums.PowersetType.Ancillary, Enums.ePowerType.Toggle);
                    GetSetArray(Enums.PowersetType.Pool0, Enums.ePowerType.Toggle);
                    GetSetArray(Enums.PowersetType.Pool1, Enums.ePowerType.Toggle);
                    GetSetArray(Enums.PowersetType.Pool2, Enums.ePowerType.Toggle);
                    GetSetArray(Enums.PowersetType.Pool3, Enums.ePowerType.Toggle);
                    break;
                case 8:
                    GetSetArray(Enums.PowersetType.Primary, Enums.ePowerType.Click);
                    GetSetArray(Enums.PowersetType.Secondary, Enums.ePowerType.Click);
                    GetSetArray(Enums.PowersetType.Ancillary, Enums.ePowerType.Click);
                    GetSetArray(Enums.PowersetType.Pool0, Enums.ePowerType.Click);
                    GetSetArray(Enums.PowersetType.Pool1, Enums.ePowerType.Click);
                    GetSetArray(Enums.PowersetType.Pool2, Enums.ePowerType.Click);
                    GetSetArray(Enums.PowersetType.Pool3, Enums.ePowerType.Click);
                    break;
            }
        }

        private void GetSetArray(Enums.PowersetType SetType, Enums.ePowerType iType)
        {
            if (MidsContext.Character.Powersets[(int) SetType] == null)
                return;
            if (cbStyle.SelectedIndex >= cbStyle.Items.Count - 2)
            {
                var num1 = MidsContext.Character.Powersets[(int) SetType].Powers.Length - 1;
                for (var iPower = 0; iPower <= num1; ++iPower)
                {
                    if (!((iType == Enums.ePowerType.Auto_) |
                          (MidsContext.Character.Powersets[(int) SetType].Powers[iPower].PowerType == iType)))
                        continue;
                    Array.Resize(ref BaseArray, BaseArray.Length + 1);
                    //BaseArray = (IPower[]) Utils.CopyArray(BaseArray, new IPower[BaseArray.Length + 1]);
                    var index = BaseArray.Length - 1;
                    BaseArray[index] =
                        MainModule.MidsController.Toon.GetEnhancedPower(GrabPlaced(
                            MidsContext.Character.Powersets[(int) SetType].nID,
                            iPower));
                    if (BaseArray[index] != null)
                        continue;
                    BaseArray[index] = new Power(MidsContext.Character.Powersets[(int) SetType].Powers[iPower]);
                    BaseArray[index].Accuracy *= MidsContext.Config.BaseAcc;
                }

                MainModule.MidsController.Toon.FlipAllSlots();
                var num2 = MidsContext.Character.Powersets[(int) SetType].Powers.Length - 1;
                for (var iPower = 0; iPower <= num2; ++iPower)
                {
                    if (!((iType == Enums.ePowerType.Auto_) |
                          (MidsContext.Character.Powersets[(int) SetType].Powers[iPower].PowerType == iType)))
                        continue;
                    Array.Resize(ref EnhArray, EnhArray.Length + 1);
                    //EnhArray = (IPower[]) Utils.CopyArray(EnhArray, new IPower[EnhArray.Length + 1]);
                    var index = EnhArray.Length - 1;
                    EnhArray[index] =
                        MainModule.MidsController.Toon.GetEnhancedPower(GrabPlaced(
                            MidsContext.Character.Powersets[(int) SetType].nID,
                            iPower));
                    if (EnhArray[index] != null)
                        continue;
                    EnhArray[index] = new Power(MidsContext.Character.Powersets[(int) SetType].Powers[iPower]);
                    EnhArray[index].Accuracy *= MidsContext.Config.BaseAcc;
                }

                MainModule.MidsController.Toon.FlipAllSlots();
            }
            else
            {
                var num = MidsContext.Character.Powersets[(int) SetType].Powers.Length - 1;
                for (var iPower = 0; iPower <= num; ++iPower)
                {
                    if (!((iType == Enums.ePowerType.Auto_) |
                          (MidsContext.Character.Powersets[(int) SetType].Powers[iPower].PowerType == iType)))
                        continue;
                    Array.Resize(ref BaseArray, BaseArray.Length + 1);
                    Array.Resize(ref EnhArray, EnhArray.Length + 1);
                    //BaseArray = (IPower[]) Utils.CopyArray(BaseArray, new IPower[BaseArray.Length + 1]);
                    //EnhArray = (IPower[]) Utils.CopyArray(EnhArray, new IPower[EnhArray.Length + 1]);
                    var index = BaseArray.Length - 1;
                    BaseArray[index] = new Power(MidsContext.Character.Powersets[(int) SetType].Powers[iPower]);
                    EnhArray[index] =
                        MainModule.MidsController.Toon.GetEnhancedPower(GrabPlaced(
                            MidsContext.Character.Powersets[(int) SetType].nID,
                            iPower)) ??
                        new Power(BaseArray[index]);
                }
            }
        }

        private int GrabPlaced(int iSet, int iPower)
        {
            if (!MainModule.MidsController.Toon.Locked)
                return -1;
            var num = MidsContext.Character.CurrentBuild.Powers.Count - 1;
            for (var index = 0; index <= num; ++index)
                if ((MidsContext.Character.CurrentBuild.Powers[index].NIDPowerset == iSet) &
                    (MidsContext.Character.CurrentBuild.Powers[index].IDXPower == iPower))
                    return index;

            return -1;
        }

        private void Graph_Acc()
        {
            var num1 = 1f;
            var num2 = BaseArray.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                if (!((BaseArray[index].EntitiesAutoHit == Enums.eEntity.None) | ((BaseArray[index].Range > 20.0) &
                    BaseArray[index].I9FXPresentP(Enums.eEffectType.Mez, Enums.eMez.Taunt))))
                    continue;
                var accuracy = BaseArray[index].Accuracy;
                if (Math.Abs(accuracy) < float.Epsilon)
                    continue;
                var num3 = EnhArray[index].Accuracy;
                float nEnh;
                float nBase;
                if (BaseOverride)
                {
                    nEnh = num3 * 100f;
                    nBase = accuracy * 100f;
                }
                else
                {
                    if (Math.Abs(EnhArray[index].Accuracy - (double) accuracy) < float.Epsilon)
                        num3 = MidsContext.Config.BaseAcc * num3;
                    nEnh = num3 * 100f;
                    nBase = (float) (MidsContext.Config.BaseAcc * (double) accuracy * 100.0);
                }

                var displayName = BaseArray[index].DisplayName;
                if (num1 < (double) nEnh)
                    num1 = nEnh;
                if (num1 < (double) nBase)
                    num1 = nBase;
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                string iTip;
                if (Graph.Style != Enums.GraphStyle.baseOnly)
                {
                    iTip = $"{displayName}: {Convert.ToDecimal(nEnh):##0.#)}%";
                }
                else
                {
                    iTip = $"{displayName}: {Convert.ToDecimal(nBase):##0.#)}%";
                }

                if (Math.Abs(nBase - (double) nEnh) > float.Epsilon)
                {
                    iTip = $"{iTip} ({Convert.ToDecimal(nBase):##0.#)}%)";
                }

                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                Graph.AddItem(BaseArray[index].DisplayName, nBase, nEnh, iTip);
            }

            GraphMax = num1 * 1.025f;
        }

        private void Graph_Damage()
        {
            var num1 = 1f;
            var returnValue = MidsContext.Config.DamageMath.ReturnValue;
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.Numeric;
            var num2 = BaseArray.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var nBase = BaseArray[index].FXGetDamageValue();
                if (Math.Abs(nBase) < float.Epsilon)
                    continue;
                var nEnh = EnhArray[index].FXGetDamageValue();
                var iTip = BaseArray[index].DisplayName;
                if (num1 < (double) nEnh)
                    num1 = nEnh;
                if (num1 < (double) nBase)
                    num1 = nBase;
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                if (Graph.Style == Enums.GraphStyle.baseOnly)
                {
                    iTip = iTip + "\r\n" + BaseArray[index].FXGetDamageString();
                }
                else
                {
                    if (!BaseOverride)
                        iTip = iTip + "\r\n" + EnhArray[index].FXGetDamageString();
                    if (BaseOverride)
                        iTip = iTip + "\r\n" + BaseArray[index].FXGetDamageString();
                }

                if (Math.Abs(nBase - (double) nEnh) > float.Epsilon)
                {
                    iTip = $"{iTip} ({Convert.ToDecimal(nBase):##0.##)}%)";
                }

                if (BaseArray[index].PowerType == Enums.ePowerType.Toggle)
                    iTip = iTip + "\r\n(Applied every " +
                           Convert.ToString(BaseArray[index].ActivatePeriod, CultureInfo.InvariantCulture) + "s)";
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                Graph.AddItem(BaseArray[index].DisplayName, nBase, nEnh, iTip);
            }

            GraphMax = num1 * 1.025f;
            MidsContext.Config.DamageMath.ReturnValue = returnValue;
        }

        private void Graph_DPA()
        {
            var num1 = 1f;
            var returnValue = MidsContext.Config.DamageMath.ReturnValue;
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPA;
            var num2 = BaseArray.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var nBase = BaseArray[index].FXGetDamageValue();
                if (Math.Abs(nBase) < float.Epsilon)
                    continue;
                var nEnh = EnhArray[index].FXGetDamageValue();
                var str = BaseArray[index].DisplayName;
                if (num1 < (double) nEnh)
                    num1 = nEnh;
                if (num1 < (double) nBase)
                    num1 = nBase;
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                if (Graph.Style == Enums.GraphStyle.baseOnly)
                {
                    str = str + "\r\n" + BaseArray[index].FXGetDamageString();
                }
                else
                {
                    if (!BaseOverride)
                        str = str + "\r\n" + EnhArray[index].FXGetDamageString();
                    if (BaseOverride)
                        str = str + "\r\n" + BaseArray[index].FXGetDamageString();
                }

                var iTip = str + "/s";
                if (Math.Abs(nBase - (double) nEnh) > float.Epsilon)
                {
                    iTip = $"{iTip} ({Convert.ToDecimal(nBase):##0.##)})";
                }

                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                Graph.AddItem(BaseArray[index].DisplayName, nBase, nEnh, iTip);
            }

            GraphMax = num1 * 1.025f;
            MidsContext.Config.DamageMath.ReturnValue = returnValue;
        }

        private void Graph_DPE()
        {
            var num1 = 1f;
            var returnValue = MidsContext.Config.DamageMath.ReturnValue;
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.Numeric;
            var num2 = BaseArray.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var nBase = BaseArray[index].FXGetDamageValue();
                if (Math.Abs(nBase) < float.Epsilon)
                    continue;
                var nEnh = EnhArray[index].FXGetDamageValue();
                if (EnhArray[index].EndCost > 0.0)
                {
                    nBase /= BaseArray[index].EndCost;
                    nEnh /= EnhArray[index].EndCost;
                }

                var str = BaseArray[index].DisplayName;
                if (num1 < (double) nEnh)
                    num1 = nEnh;
                if (num1 < (double) nBase)
                    num1 = nBase;
                if (Graph.Style == Enums.GraphStyle.baseOnly)
                {
                    str = str + "\r\n" + BaseArray[index].FXGetDamageString();
                }
                else
                {
                    if (!BaseOverride)
                        str = str + "\r\n" + EnhArray[index].FXGetDamageString();
                    if (BaseOverride)
                        str = str + "\r\n" + BaseArray[index].FXGetDamageString();
                }

                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                string iTip;
                if (Graph.Style == Enums.GraphStyle.baseOnly)
                {
                    iTip = $"{str}\r\nDamage per unit of End: {Convert.ToDecimal(nBase):##0.##}";
                }
                else
                {
                    iTip = $"{str}\r\nDamage per unit of End: {Convert.ToDecimal(nEnh):##0.##}";
                    if (Math.Abs(nBase - (double) nEnh) > float.Epsilon)
                    {
                        iTip = $"{iTip} ({Convert.ToDecimal(nBase):##0.##)})";
                    }
                }

                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                Graph.AddItem(BaseArray[index].DisplayName, nBase, nEnh, iTip);
            }

            GraphMax = num1 * 1.025f;
            MidsContext.Config.DamageMath.ReturnValue = returnValue;
        }

        private void Graph_DPS()
        {
            var num1 = 1f;
            var returnValue = MidsContext.Config.DamageMath.ReturnValue;
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPS;
            var num2 = BaseArray.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var nBase = BaseArray[index].FXGetDamageValue();
                if (Math.Abs(nBase) < float.Epsilon)
                    continue;
                var nEnh = EnhArray[index].FXGetDamageValue();
                var str = BaseArray[index].DisplayName;
                if (num1 < (double) nEnh)
                    num1 = nEnh;
                if (num1 < (double) nBase)
                    num1 = nBase;
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                if (Graph.Style == Enums.GraphStyle.baseOnly)
                {
                    str = str + "\r\n" + BaseArray[index].FXGetDamageString();
                }
                else
                {
                    if (!BaseOverride)
                        str = str + "\r\n" + EnhArray[index].FXGetDamageString();
                    if (BaseOverride)
                        str = str + "\r\n" + BaseArray[index].FXGetDamageString();
                }

                var iTip = str + "/s";
                if (Math.Abs(nBase - (double) nEnh) > float.Epsilon)
                {
                    iTip = $"{iTip} ({Convert.ToDecimal(nBase):##0.##)})";
                }

                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                Graph.AddItem(BaseArray[index].DisplayName, nBase, nEnh, iTip);
            }

            GraphMax = num1 * 1.025f;
            MidsContext.Config.DamageMath.ReturnValue = returnValue;
        }

        private void Graph_Duration()
        {
            var num1 = 1f;
            var num2 = BaseArray.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var durationEffectId = BaseArray[index].GetDurationEffectID();
                if (durationEffectId <= -1)
                    continue;
                var nBase = BaseArray[index].Effects[durationEffectId].Duration;
                var nEnh = EnhArray[index].Effects[durationEffectId].Duration;
                if (!((Math.Abs(nBase) > float.Epsilon) & (BaseArray[index].PowerType == Enums.ePowerType.Click)))
                    continue;
                var str1 = EnhArray[index].Effects[durationEffectId].EffectType != Enums.eEffectType.Mez
                    ? Enums.GetEffectName(EnhArray[index].Effects[durationEffectId].EffectType)
                    : Enums.GetMezName((Enums.eMezShort) EnhArray[index].Effects[durationEffectId].MezType);
                if (EnhArray[index].Effects[durationEffectId].Mag < 0.0)
                    str1 = "-" + str1;
                var displayName = BaseArray[index].DisplayName;
                if (num1 < (double) nEnh)
                    num1 = nEnh;
                if (num1 < (double) nBase)
                    num1 = nBase;
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                string str2;
                if (Graph.Style == Enums.GraphStyle.baseOnly)
                {
                    str2 = $"{displayName} ({str1}): {Convert.ToDecimal(nBase):##0.#)}";
                }
                else
                {
                    str2 = $"{displayName} ({str1}): {Convert.ToDecimal(nEnh):##0.#)}";
                }

                if (Math.Abs(nBase - (double) nEnh) > float.Epsilon)
                {
                    str2 = $"{str2} ({Convert.ToDecimal(nBase):##0.#)})";
                }

                var iTip = str2 + "s";
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                Graph.AddItem(BaseArray[index].DisplayName, nBase, nEnh, iTip);
            }

            GraphMax = num1 * 1.025f;
        }

        private void Graph_End()
        {
            var num1 = 1f;
            var num2 = BaseArray.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var nBase = BaseArray[index].EndCost;
                if (Math.Abs(nBase) < float.Epsilon)
                    continue;
                var nEnh = EnhArray[index].EndCost;
                var displayName = BaseArray[index].DisplayName;
                if (num1 < (double) nEnh)
                    num1 = nEnh;
                if (num1 < (double) nBase)
                    num1 = nBase;
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                string iTip;
                if (Graph.Style != Enums.GraphStyle.baseOnly)
                {
                    iTip = $"{displayName}: {Convert.ToDecimal(nEnh):##0.##)}";
                }
                else
                {
                    iTip = $"{displayName}: {Convert.ToDecimal(nBase):##0.##)}";
                }

                if (Math.Abs(nBase - (double) nEnh) > float.Epsilon)
                {
                    iTip = $"{iTip} ({Convert.ToDecimal(nBase):##0.##)})";
                }

                if (BaseArray[index].PowerType == Enums.ePowerType.Toggle)
                    iTip += "\r\n(Per Second)";
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                Graph.AddItem(BaseArray[index].DisplayName, nBase, nEnh, iTip);
            }

            GraphMax = num1 * 1.025f;
        }

        private void Graph_EPS()
        {
            var num1 = 1f;
            var num2 = BaseArray.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var nBase = BaseArray[index].EndCost;
                if (Math.Abs(nBase) < float.Epsilon)
                    continue;
                var nEnh = EnhArray[index].EndCost;
                switch (BaseArray[index].PowerType)
                {
                    case Enums.ePowerType.Click:
                    {
                        if (EnhArray[index].RechargeTime + (double) EnhArray[index].CastTime +
                            EnhArray[index].InterruptTime > 0.0)
                        {
                            nBase = BaseArray[index].EndCost /
                                    (BaseArray[index].RechargeTime + BaseArray[index].CastTime +
                                     BaseArray[index].InterruptTime);
                            nEnh = EnhArray[index].EndCost /
                                   (EnhArray[index].RechargeTime + EnhArray[index].CastTime +
                                    EnhArray[index].InterruptTime);
                        }

                        break;
                    }
                    case Enums.ePowerType.Toggle:
                        nBase = BaseArray[index].EndCost / BaseArray[index].ActivatePeriod;
                        nEnh = EnhArray[index].EndCost / EnhArray[index].ActivatePeriod;
                        break;
                }

                var displayName = BaseArray[index].DisplayName;
                if (num1 < (double) nEnh)
                    num1 = nEnh;
                if (num1 < (double) nBase)
                    num1 = nBase;
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }


                string iTip;
                if (Graph.Style != Enums.GraphStyle.baseOnly)
                {
                    iTip = $"{displayName}: {Convert.ToDecimal(nEnh):##0.##)}";
                }
                else
                {
                    iTip = $"{displayName}: {Convert.ToDecimal(nBase):##0.##)}/s";
                }

                if (Math.Abs(nBase - (double)nEnh) > float.Epsilon)
                {
                    iTip = $"{iTip} ({Convert.ToDecimal(nBase):##0.##)})";
                }
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                Graph.AddItem(BaseArray[index].DisplayName, nBase, nEnh, iTip);
            }

            GraphMax = num1 * 1.025f;
        }

        private void Graph_Heal()
        {
            var num1 = 1f;
            var num2 = BaseArray.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var nBase = BaseArray[index].GetEffectMagSum(Enums.eEffectType.Heal).Sum;
                if (Math.Abs(nBase) < float.Epsilon)
                    continue;
                var nEnh = EnhArray[index].GetEffectMagSum(Enums.eEffectType.Heal).Sum;
                var displayName = BaseArray[index].DisplayName;
                if (num1 < (double) nEnh)
                    num1 = nEnh;
                if (num1 < (double) nBase)
                    num1 = nBase;
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                var num4 = (float) (nBase / (double) MidsContext.Archetype.Hitpoints * 100.0);
                var num5 = (float) (nEnh / (double) MidsContext.Archetype.Hitpoints * 100.0);



                string iTip;
                if (Graph.Style == Enums.GraphStyle.baseOnly)
                {
                    iTip = $"{displayName}: {Convert.ToDecimal(num4):##0.#)}%";
                }
                else
                {
                    iTip = $"{displayName}\r\n Enhanced: {Convert.ToDecimal(num5):##0.#)}% ({Convert.ToDecimal(nEnh):##0.#} HP)";
                }

                if (Math.Abs(nBase - (double)nEnh) > float.Epsilon)
                {
                    iTip = $"{iTip}\r\n Base: {Convert.ToDecimal(num4):##0.#)}% ({Convert.ToDecimal(nBase):##0.#} HP)";
                }
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                    iTip = iTip.Replace("Enhanced", "Active").Replace("Base", "Alternate");
                }

                Graph.AddItem(BaseArray[index].DisplayName, nBase, nEnh, iTip);
            }

            GraphMax = num1 * 1.025f;
        }

        private void Graph_HealPE()
        {
            var num1 = 1f;
            var num2 = BaseArray.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var nBase = BaseArray[index].GetEffectMagSum(Enums.eEffectType.Heal).Sum;
                if (Math.Abs(nBase) < float.Epsilon)
                    continue;
                var nEnh = EnhArray[index].GetEffectMagSum(Enums.eEffectType.Heal).Sum;
                if (EnhArray[index].EndCost > 0.0)
                {
                    nBase /= BaseArray[index].EndCost;
                    nEnh /= EnhArray[index].EndCost;
                }

                var displayName = BaseArray[index].DisplayName;
                if (num1 < (double) nEnh)
                    num1 = nEnh;
                if (num1 < (double) nBase)
                    num1 = nBase;
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                var num4 = (float) (nBase / (double) MidsContext.Archetype.Hitpoints * 100.0);
                var num5 = (float) (nEnh / (double) MidsContext.Archetype.Hitpoints * 100.0);
                string iTip;
                if (Graph.Style == Enums.GraphStyle.baseOnly)
                {
                    iTip = $"{displayName}: {Convert.ToDecimal(nBase):##0.##)}%";
                }
                else
                {
                    iTip = $"{displayName}\r\n Enhanced Heal per unit of End: {Convert.ToDecimal(num5):##0.##)}% ({Convert.ToDecimal(nEnh):##0.##} HP)";
                }

                if (Math.Abs(nBase - (double)nEnh) > float.Epsilon)
                {
                    iTip = $"{iTip}\r\n Base Heal per unit of End: {Convert.ToDecimal(num4):##0.##)}% ({Convert.ToDecimal(nBase):##0.##} HP)";
                }

                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                    iTip = iTip.Replace("Enhanced", "Acive").Replace("Base", "Alternate");
                }

                Graph.AddItem(BaseArray[index].DisplayName, nBase, nEnh, iTip);
            }

            GraphMax = num1 * 1.025f;
        }

        private void Graph_HealPS()
        {
            var num1 = 1f;
            var num2 = BaseArray.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var nBase = BaseArray[index].GetEffectMagSum(Enums.eEffectType.Heal).Sum;
                if (Math.Abs(nBase) < float.Epsilon)
                    continue;
                var nEnh = EnhArray[index].GetEffectMagSum(Enums.eEffectType.Heal).Sum;
                switch (BaseArray[index].PowerType)
                {
                    case Enums.ePowerType.Click:
                    {
                        if (EnhArray[index].RechargeTime + (double) EnhArray[index].CastTime +
                            EnhArray[index].InterruptTime > 0.0)
                        {
                            nBase /= BaseArray[index].RechargeTime + BaseArray[index].CastTime +
                                     BaseArray[index].InterruptTime;
                            nEnh /= EnhArray[index].RechargeTime + EnhArray[index].CastTime +
                                    EnhArray[index].InterruptTime;
                        }

                        break;
                    }
                    case Enums.ePowerType.Toggle:
                        nBase /= BaseArray[index].ActivatePeriod;
                        nEnh /= EnhArray[index].ActivatePeriod;
                        break;
                }

                var displayName = BaseArray[index].DisplayName;
                if (num1 < (double) nEnh)
                    num1 = nEnh;
                if (num1 < (double) nBase)
                    num1 = nBase;
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                var num4 = (float) (nBase / (double) MidsContext.Archetype.Hitpoints * 100.0);
                var num5 = (float) (nEnh / (double) MidsContext.Archetype.Hitpoints * 100.0);
                string iTip;

                if (Graph.Style == Enums.GraphStyle.baseOnly)
                {
                    iTip = $"{displayName}: {Convert.ToDecimal(num4):##0.##)}%";
                }
                else
                {
                    iTip = $"{displayName}\r\n Enhanced: {Convert.ToDecimal(num5):##0.##)}%/s ({Convert.ToDecimal(nEnh):##0.##} HP)";
                }

                if (Math.Abs(nBase - (double)nEnh) > float.Epsilon)
                {
                    iTip = $"{iTip}\r\n Base: {Convert.ToDecimal(num4):##0.#)}%/s ({Convert.ToDecimal(nBase):##0.##} HP)";
                }
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                    iTip = iTip.Replace("Enhanced", "Active").Replace("Base", "Alternate");
                }

                Graph.AddItem(BaseArray[index].DisplayName, nBase, nEnh, iTip);
            }

            GraphMax = num1 * 1.025f;
        }

        private void Graph_Range()
        {
            var num1 = 1f;
            var num2 = BaseArray.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var nBase = 0.0f;
                var nEnh = 0.0f;
                if (BaseArray[index].Range > 0.0)
                {
                    nBase = BaseArray[index].Range;
                    nEnh = EnhArray[index].Range;
                }
                else if (BaseArray[index].Radius > 0.0)
                {
                    nBase = BaseArray[index].Radius;
                    nEnh = EnhArray[index].Radius;
                }

                if (Math.Abs(nBase) < float.Epsilon)
                    continue;
                var displayName = BaseArray[index].DisplayName;
                if (num1 < (double) nEnh)
                    num1 = nEnh;
                if (num1 < (double) nBase)
                    num1 = nBase;
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                string str;


                if (Graph.Style != Enums.GraphStyle.baseOnly)
                {
                    str = $"{displayName}: {Convert.ToDecimal(nEnh):##0.#)}";
                }
                else
                {
                    str = $"{displayName}: {Convert.ToDecimal(nBase):##0.#)}";
                }

                if (Math.Abs(nBase - (double)nEnh) > float.Epsilon)
                {
                    str = $"{str} ({Convert.ToDecimal(nBase):##0.#)})";
                }
                var iTip = str + "ft";
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                Graph.AddItem(BaseArray[index].DisplayName, nBase, nEnh, iTip);
            }

            GraphMax = num1 * 1.025f;
        }

        private void Graph_Recharge()
        {
            var num1 = 1f;
            var num2 = BaseArray.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var nBase = BaseArray[index].RechargeTime;
                if (Math.Abs(nBase) < float.Epsilon)
                    continue;
                var nEnh = EnhArray[index].RechargeTime;
                var displayName = BaseArray[index].DisplayName;
                if (num1 < (double) nEnh)
                    num1 = nEnh;
                if (num1 < (double) nBase)
                    num1 = nBase;
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                string iTip;
                if (Graph.Style != Enums.GraphStyle.baseOnly)
                {
                    iTip = $"{displayName}: {Convert.ToDecimal(nEnh):##0.##)}";
                }
                else
                {
                    iTip = $"{displayName}: {Convert.ToDecimal(nBase):##0.##)}s";
                }

                if (Math.Abs(nBase - (double)nEnh) > float.Epsilon)
                {
                    iTip = $"{iTip} ({Convert.ToDecimal(nBase):##0.##)})";
                }
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                Graph.AddItem(BaseArray[index].DisplayName, nBase, nEnh, iTip);
            }

            GraphMax = num1 * 1.025f;
        }

        private void Graph_Regen()
        {
            var num1 = 1f;
            var num2 = MidsContext.Character.DisplayStats.HealthHitpointsNumeric(false);
            var num3 = BaseArray.Length - 1;
            for (var index = 0; index <= num3; ++index)
            {
                var nBase = BaseArray[index].GetEffectMagSum(Enums.eEffectType.Regeneration).Sum;
                if (Math.Abs(nBase) < float.Epsilon)
                    continue;
                var nEnh = EnhArray[index].GetEffectMagSum(Enums.eEffectType.Regeneration).Sum;
                var num4 = (float) (num2 / 12.0 * (0.05 + 0.05 * ((nBase - 100.0) / 100.0)));
                var num5 = (float) (num4 / (double) num2 * 100.0);
                var num6 = (float) (num2 / 12.0 * (0.05 + 0.05 * ((nEnh - 100.0) / 100.0)));
                var num7 = (float) (num6 / (double) num2 * 100.0);
                var displayName = BaseArray[index].DisplayName;
                if (num1 < (double) nEnh)
                    num1 = nEnh;
                if (num1 < (double) nBase)
                    num1 = nBase;
                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                    (num4, num6) = (num6, num4);
                    (num5, num7) = (num7, num5);
                }

                string iTip;
                if (Graph.Style == Enums.GraphStyle.baseOnly)
                {
                    var str = $"{displayName}: {Convert.ToDecimal(nBase):##0.#)}%";
                    iTip = $"Health regenerated per second: {Convert.ToDecimal(num5):##0.##)}%\r\n Hitpoints regenerated per second at level 50: {Convert.ToDecimal(num4):##0.#)} HP";
                }
                else if (Math.Abs(nBase - (double) nEnh) < float.Epsilon)
                {
                    iTip = $"{displayName}: {Convert.ToDecimal(nBase):##0.#)}%\r\n Health regenerated per second: {Convert.ToDecimal(num5):##0.##)}%\r\n Hitpoints regenerated per second at level 50: {Convert.ToDecimal(num4):##0.#)} HP";
                }
                else
                {
                    iTip = $"{displayName}: {Convert.ToDecimal(nEnh):##0.#)}% ({Convert.ToDecimal(nBase):##0.#}%)\r\n Health regenerated per second: {Convert.ToDecimal(num7):##0.##)}% ({Convert.ToDecimal(num5):##0.##)})\r\n Hitpoints regenerated per second at level 50: {Convert.ToDecimal(num6):##0.#)} HP ({Convert.ToDecimal(num4):##0.##)})";
                }

                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                Graph.AddItem(BaseArray[index].DisplayName, nBase, nEnh, iTip);
            }

            GraphMax = num1 * 1.025f;
        }

        [DebuggerStepThrough]
        private void NewSets()

        {
            cbSet.BeginUpdate();
            var items = cbSet.Items;
            items.Clear();
            items.Add("All Sets");
            items.Add("Primary & Secondary");
            items.Add("Primary (" + MidsContext.Character.Powersets[0].DisplayName + ")");
            items.Add("Secondary (" + MidsContext.Character.Powersets[1].DisplayName + ")");
            items.Add("Ancillary");
            items.Add("Pools");
            items.Add("Powers Taken");
            items.Add("All Toggles");
            items.Add("All Clicks");
            cbSet.SelectedIndex = 1;
            cbSet.EndUpdate();
        }

        private void SetGraphMetrics()
        {
            if (Graph.ItemCount < 13.5)
            {
                Graph.ItemHeight = 18;
                Graph.PaddingY = 6f;
            }
            else if (Graph.ItemCount < 18.0)
            {
                Graph.ItemHeight = 15;
                Graph.PaddingY = 5f;
            }
            else if (Graph.ItemCount > 32)
            {
                Graph.PaddingY = 2f;
                Graph.ItemHeight = 10;
            }
            else if (Graph.ItemCount > 30)
            {
                Graph.PaddingY = 2f;
                Graph.ItemHeight = 11;
            }
            else if (Graph.ItemCount > 27)
            {
                Graph.PaddingY = 2.666667f;
                Graph.ItemHeight = 11;
            }
            else
            {
                Graph.ItemHeight = 12;
                Graph.PaddingY = 4f;
            }
        }

        private void SetGraphType()
        {
            if ((cbStyle.SelectedIndex > -1) & (cbStyle.SelectedIndex < cbStyle.Items.Count - 2))
            {
                Graph.Style = (Enums.GraphStyle) cbStyle.SelectedIndex;
                MidsContext.Config.StatGraphStyle = Graph.Style;
                BaseOverride = false;
            }
            else if (cbStyle.SelectedIndex == cbStyle.Items.Count - 2)
            {
                Graph.Style = Enums.GraphStyle.Twin;
                BaseOverride = true;
            }
            else if (cbStyle.SelectedIndex == cbStyle.Items.Count - 1)
            {
                Graph.Style = Enums.GraphStyle.Stacked;
                BaseOverride = true;
            }

            GetPowerArray();
            if (BaseOverride)
            {
                lblKey1.Text = "Active";
                lblKey2.Text = "Alternate";
            }
            else
            {
                lblKey1.Text = "Base";
                lblKey2.Text = "Enhanced";
            }

            MidsContext.Config.StatGraphStyle = Graph.Style;
        }

        public void SetLocation()
        {
            var rectangle = new Rectangle
            {
                X = MainModule.MidsController.SzFrmStats.X,
                Y = MainModule.MidsController.SzFrmStats.Y,
                Width = MainModule.MidsController.SzFrmStats.Width,
                Height = MainModule.MidsController.SzFrmStats.Height
            };
            if (rectangle.Width < 1)
                rectangle.Width = Width;
            if (rectangle.Height < 1)
                rectangle.Height = Height;
            if (rectangle.Width < MinimumSize.Width)
                rectangle.Width = MinimumSize.Width;
            if (rectangle.Height < MinimumSize.Height)
                rectangle.Height = MinimumSize.Height;
            if (rectangle.X < 1)
                rectangle.X = (int) Math.Round((Screen.PrimaryScreen.Bounds.Width - Width) / 2.0);
            if (rectangle.Y < 32)
                rectangle.Y = (int) Math.Round((Screen.PrimaryScreen.Bounds.Height - Height) / 2.0);
            Top = rectangle.Y;
            Left = rectangle.X;
            Height = rectangle.Height;
            Width = rectangle.Width;
        }

        private void SetScaleLabel()
        {
            lblScale.Text = "Scale: 0 - " + Convert.ToString(Graph.ScaleValue, CultureInfo.InvariantCulture);
        }

        private void StoreLocation()

        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            MainModule.MidsController.SzFrmStats.X = Left;
            MainModule.MidsController.SzFrmStats.Y = Top;
            MainModule.MidsController.SzFrmStats.Width = Width;
            MainModule.MidsController.SzFrmStats.Height = Height;
        }

        private void tbScaleX_Scroll(object sender, EventArgs e)

        {
            Graph.ScaleIndex = tbScaleX.Value;
            SetScaleLabel();
        }

        public void UpdateData(bool NewData)
        {
            BackColor = myParent.BackColor;
            btnClose.IA = myParent.Drawing.pImageAttributes;
            btnClose.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            btnClose.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            chkOnTop.IA = myParent.Drawing.pImageAttributes;
            chkOnTop.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            chkOnTop.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            Graph.BackColor = BackColor;
            if (NewData)
                NewSets();
            SetGraphType();
            GetPowerArray();
            DisplayGraph();
        }
    }
}