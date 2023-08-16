using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;
using MRBResourceLib;

namespace Mids_Reborn.Forms
{
    public partial class FrmIncarnate : Form
    {
        private readonly ImageButton[] _buttonArray;
        private readonly frmMain? _myParent;
        private bool _locked;
        private IPower?[]? _myPowers;

        public FrmIncarnate(ref frmMain iParent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Icon = Resources.MRB_Icon_Concept;
            _myParent = iParent;
            Load += frmIncarnate_Load;
            FormClosing += FrmIncarnate_FormClosing;
            _myPowers = Array.Empty<IPower>();
            _locked = false;
            _buttonArray = new ImageButton[10];
            _myPowers = DatabaseAPI.GetPowersetByName("Alpha", Enums.ePowerSetType.Incarnate)?.Powers;
            InitializeComponent();
            // PopInfo events
            _popInfo!.MouseWheel += PopInfo_MouseWheel;
            _popInfo.MouseEnter += PopInfo_MouseEnter;
        }

        private void FrmIncarnate_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                _myParent.ibIncarnatePowersEx.ToggleState = ImageButtonEx.States.ToggledOff;
            }

            if (DialogResult == DialogResult.Cancel)
            {
                _myParent.ibIncarnatePowersEx.ToggleState = ImageButtonEx.States.ToggledOff;
            }
        }

        private void ChangedScrollFrameContents()
        {
            _vScrollBar1.Value = 0;
            _vScrollBar1.Maximum = (int) Math.Round(_popInfo.lHeight * (_vScrollBar1.LargeChange / (double) _panel1.Height));
            VScrollBar1_Scroll(_vScrollBar1, new ScrollEventArgs(ScrollEventType.EndScroll, 0));
        }

        private void FillLists(string setName)
        {
            var newPowerList = new List<IPower?>();
            switch (setName)
            {
                case "Alpha":
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Agility"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Cardiac"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Intuition"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Musculature"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Nerve"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Resilient"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Spiritual"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Vigor"));
                    break;
                case "Judgement":
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Cryonic"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Ion"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Mighty"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Pyronic"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Void"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Vorpal"));
                    break;
                case "Interface":
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Cognitive"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Degenerative"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Diamagnetic"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Gravitic"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Paralytic"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Preemptive"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Reactive"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Spectral"));
                    break;
                case "Lore":
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Arachnos"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Banished Pantheon"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Carnival"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Cimeroran"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Clockwork"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Demons"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "IDF"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Knives of Vengeance"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Longbow"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Nemesis"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Phantom"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Polar Lights"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Rikti"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Robotic Drones"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Rularuu"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Seers"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Storm Elemental"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Talons of Vengeance"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Tsoo"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Vanguard"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Warworks"));
                    break;
                case "Destiny":
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Ageless"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Barrier"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Clarion"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Incandescence"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Rebirth"));
                    break;
                case "Hybrid":
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Assault"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Control"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Melee"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Support"));
                    break;
                case "Genesis":
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Data"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Fate"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Socket"));
                    newPowerList.AddRange(ParseIncarnate(_myPowers.ToList(), setName, "Verdict"));
                    break;
            }

            _myPowers = newPowerList.ToArray();
            LlLeft.SuspendRedraw = true;
            LlRight.SuspendRedraw = true;
            LlLeft.ClearItems();
            LlRight.ClearItems();
            var num1 = _myPowers.Length - 1;
            for (var index = 0; index <= num1; ++index)
            {
                ListLabelV3.LlItemState iState;
                if (MidsContext.Character?.CurrentBuild != null && !MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[index]))
                {
                    if (_myPowers[index].DisplayName != "Nothing")
                    {
                        iState = ListLabelV3.LlItemState.Enabled;
                    }
                    else
                    {
                        iState = ListLabelV3.LlItemState.Disabled;
                    }
                }
                else
                {
                    iState = ListLabelV3.LlItemState.Selected;
                }

                var iItem = !MidsContext.Config.RtFont.PairedBold ? new ListLabelV3.ListLabelItemV3(_myPowers[index].DisplayName, iState) : new ListLabelV3.ListLabelItemV3(_myPowers[index].DisplayName, iState, -1, -1, -1, "", ListLabelV3.LlFontFlags.Bold);
                if (index < _myPowers.Length / 2)
                {
                    LlLeft.AddItem(iItem);
                }
                else
                {
                    LlRight.AddItem(iItem);
                }
            }

            LlLeft.SuspendRedraw = false;
            LlRight.SuspendRedraw = false;
            LlLeft.Refresh();
            LlRight.Refresh();
        }

        private static IEnumerable<IPower?> ParseIncarnate(List<IPower?> powerList, string order, string name)
        {
            var pairList = new List<KeyValuePair<int, IPower?>>();
            var tList = powerList.FindAll(x => x != null && x.DisplayName.Contains(name));
            var pos = 0;
            foreach (var power in tList)
            {
                if (power != null)
                {
                    var nSplit = power.DisplayName.Split();
                    var value = power.DisplayName.Replace($"{nSplit[0]} ", "");
                    switch (order)
                    {
                        case "Alpha":
                            pos = (int)Enum.Parse(typeof(Enums.eAlphaOrder), value.Replace(" ", "_"));
                            break;
                        case "Judgement":
                            pos = (int)Enum.Parse(typeof(Enums.eJudgementOrder), value.Replace(" ", "_"));
                            break;
                        case "Interface":
                            pos = (int)Enum.Parse(typeof(Enums.eInterfaceOrder), value.Replace(" ", "_"));
                            break;
                        case "Lore":
                            switch (name)
                            {
                                case "Banished Pantheon":
                                    value = power.DisplayName.Replace($"{nSplit[0]} {nSplit[1]} ", "");
                                    break;
                                case "Knives of Vengeance":
                                    value = power.DisplayName.Replace($"{nSplit[0]} {nSplit[1]} {nSplit[2]} ", "");
                                    break;
                                case "Polar Lights":
                                    value = power.DisplayName.Replace($"{nSplit[0]} {nSplit[1]} ", "");
                                    break;
                                case "Robotic Drones":
                                    value = power.DisplayName.Replace($"{nSplit[0]} {nSplit[1]} ", "");
                                    break;
                                case "Storm Elemental":
                                    value = power.DisplayName.Replace($"{nSplit[0]} {nSplit[1]} ", "");
                                    break;
                                case "Talons of Vengeance":
                                    value = power.DisplayName.Replace($"{nSplit[0]} {nSplit[1]} {nSplit[2]} ", "");
                                    break;
                            }
                            pos = (int)Enum.Parse(typeof(Enums.eLoreOrder), value.Replace(" ", "_"));
                            break;
                        case "Destiny":
                            pos = (int)Enum.Parse(typeof(Enums.eDestinyOrder), value.Replace(" ", "_"));
                            break;
                        case "Hybrid":
                            pos = (int)Enum.Parse(typeof(Enums.eHybridOrder), value.Replace(" ", "_"));
                            break;
                        case "Genesis":
                            pos = (int)Enum.Parse(typeof(Enums.eGenesisOrder), value.Replace(" ", "_"));
                            break;
                    }
                }

                pairList.Add(new KeyValuePair<int, IPower?>(pos, power));
            }

            var oList = pairList.OrderBy(x => x.Key);

            return oList.Select(power => power.Value).ToList();
        }

        private void frmIncarnate_Load(object? sender, EventArgs e)
        {
            if (_myParent != null)
            {
                var x = _myParent.Left + (_myParent.Width - Width) / 2;
                var y = _myParent.Top + (_myParent.Height - Height) / 2;
                Location = new Point(x, y);
            }
            _buttonArray[0] = _alphaBtn;
            _buttonArray[1] = _destinyBtn;
            _buttonArray[2] = _hybridBtn;
            _buttonArray[3] = _interfaceBtn;
            _buttonArray[4] = _judgementBtn;
            _buttonArray[5] = _loreBtn;
            _buttonArray[6] = _genesisButton;
            _buttonArray[7] = _stanceButton;
            _buttonArray[8] = _vitaeButton;
            _buttonArray[9] = _omegaButton;
            foreach (var button in _buttonArray)
            {
                button.Enabled = DatabaseAPI.ServerData.EnabledIncarnates[button.TextOn];
                if (button.Enabled)
                {
                    button.IA = _myParent.Drawing.pImageAttributes;
                    button.ImageOff = MidsContext.Character.IsHero()
                        ? _myParent.Drawing.bxPower[2].Bitmap
                        : _myParent.Drawing.bxPower[4].Bitmap;
                    button.ImageOn = MidsContext.Character.IsHero()
                        ? _myParent.Drawing.bxPower[3].Bitmap
                        : _myParent.Drawing.bxPower[5].Bitmap;
                }
                else
                {
                    button.IA = _myParent.Drawing.pImageAttributes;
                    button.ImageOff = _myParent.Drawing.bxPower[1].Bitmap;
                    button.ImageOn = _myParent.Drawing.bxPower[1].Bitmap;
                }
            }

            BackColor = _myParent.BackColor;
            _popInfo.ForeColor = BackColor;
            var llLeft = LlLeft;
            UpdateLlColours(ref llLeft);
            LlLeft = llLeft;
            var llRight = LlRight;
            UpdateLlColours(ref llRight);
            LlRight = llRight;
            _ibClose.IA = _myParent.Drawing.pImageAttributes;
            _ibClose.ImageOff = MidsContext.Character.IsHero() ? _myParent.Drawing.bxPower[2].Bitmap : _myParent.Drawing.bxPower[4].Bitmap;
            _ibClose.ImageOn = MidsContext.Character.IsHero() ? _myParent.Drawing.bxPower[3].Bitmap : _myParent.Drawing.bxPower[5].Bitmap;
            var iPopup = new PopUp.PopupData();
            var index = iPopup.Add();
            iPopup.Sections[index].Add("Click powers to enable/disable them.", PopUp.Colors.Title);
            iPopup.Sections[index]
                .Add("Powers in gray (or your custom 'power disabled' color) cannot be included in your stats.",
                    PopUp.Colors.Text, 0.9f);
            _popInfo.SetPopup(iPopup);
            ChangedScrollFrameContents();
            FillLists("Alpha");
        }

        private void ibClose_ButtonClicked()
        {
            Close();
        }

        private void alphaBtn_ButtonClicked()
        {
            var alphaBtn = this._alphaBtn;
            SetPowerSet("Alpha", ref alphaBtn);
            this._alphaBtn = alphaBtn;
        }

        private void destinyBtn_ButtonClicked()
        {
            var destinyBtn = this._destinyBtn;
            SetPowerSet("Destiny", ref destinyBtn);
            this._destinyBtn = destinyBtn;
        }

        private void GenesisButton_ButtonClicked()
        {
            var genesisButton = _genesisButton;
            SetPowerSet("Genesis", ref genesisButton);
            _genesisButton = genesisButton;
        }

        private void hybridBtn_ButtonClicked()
        {
            var hybridBtn = this._hybridBtn;
            SetPowerSet("Hybrid", ref hybridBtn);
            this._hybridBtn = hybridBtn;
        }

        [DebuggerStepThrough]
        private void interfaceBtn_ButtonClicked()
        {
            var interfaceBtn = this._interfaceBtn;
            SetPowerSet("Interface", ref interfaceBtn);
            this._interfaceBtn = interfaceBtn;
        }

        private void judgementBtn_ButtonClicked()
        {
            var judgementBtn = this._judgementBtn;
            SetPowerSet("Judgement", ref judgementBtn);
            this._judgementBtn = judgementBtn;
        }

        private void loreBtn_ButtonClicked()
        {
            var loreBtn = this._loreBtn;
            SetPowerSet("Lore", ref loreBtn);
            this._loreBtn = loreBtn;
        }

        private void lblLock_Click(object? sender, EventArgs e)
        {
            _locked = false;
            _lblLock.Visible = false;
        }

        private void llLeft_ItemClick(ListLabelV3.ListLabelItemV3 item, MouseButtons button)
        {
            if (button == MouseButtons.Right)
            {
                _locked = false;
                MiniPowerInfo(item.Index);
                _lblLock.Visible = true;
                _locked = true;
                return;
            }

            if (item.ItemState == ListLabelV3.LlItemState.Disabled)
            {
                return;
            }

            var flag = !MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[item.Index]);
            var num1 = LlLeft.Items.Length - 1;
            for (var index = 0; index <= num1; ++index)
            {
                if (LlLeft.Items[index].ItemState == ListLabelV3.LlItemState.Selected)
                {
                    LlLeft.Items[index].ItemState = ListLabelV3.LlItemState.Enabled;
                }

                if (MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[index]))
                {
                    MidsContext.Character.CurrentBuild.RemovePower(_myPowers[index]);
                }
            }

            var num2 = LlRight.Items.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                if (LlRight.Items[index].ItemState == ListLabelV3.LlItemState.Selected)
                {
                    LlRight.Items[index].ItemState = ListLabelV3.LlItemState.Enabled;
                }

                if (MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[index + LlLeft.Items.Length]))
                {
                    MidsContext.Character.CurrentBuild.RemovePower(_myPowers[index + LlLeft.Items.Length]);
                }
            }

            if (flag)
            {
                MidsContext.Character.CurrentBuild.AddPower(_myPowers[item.Index], 49).StatInclude = true;
                item.ItemState = ListLabelV3.LlItemState.Selected;
            }

            LlLeft.Refresh();
            LlRight.Refresh();
            _myParent.PowerModified(true);
            _myParent.DoRefresh();
        }

        private void llLeft_ItemHover(ListLabelV3.ListLabelItemV3 item)
        {
            MiniPowerInfo(item.Index);
        }

        private void llLeft_MouseEnter(object? sender, EventArgs e)
        {
            if (!ContainsFocus)
            {
                return;
            }

            _panel2.Focus();
        }

        private void llRight_ItemClick(ListLabelV3.ListLabelItemV3 item, MouseButtons button)
        {
            var pIdx = item.Index + LlLeft.Items.Length;
            if (button == MouseButtons.Right)
            {
                _locked = false;
                MiniPowerInfo(pIdx);
                _lblLock.Visible = true;
                _locked = true;
            }
            else
            {
                if (item.ItemState == ListLabelV3.LlItemState.Disabled)
                {
                    return;
                }

                var unused = !MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[pIdx]);
                var hasChanges = false;
                for (var index = 0; index <= LlLeft.Items.Length - 1; ++index)
                {
                    if (LlLeft.Items[index].ItemState == ListLabelV3.LlItemState.Selected)
                    {
                        LlLeft.Items[index].ItemState = ListLabelV3.LlItemState.Enabled;
                    }

                    if (!MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[index]))
                    {
                        continue;
                    }

                    MidsContext.Character.CurrentBuild.RemovePower(_myPowers[index]);
                    hasChanges = true;
                }

                for (var index = 0; index <= LlRight.Items.Length - 1; ++index)
                {
                    if (LlRight.Items[index].ItemState == ListLabelV3.LlItemState.Selected)
                    {
                        LlRight.Items[index].ItemState = ListLabelV3.LlItemState.Enabled;
                    }

                    if (!MidsContext.Character.CurrentBuild.PowerUsed(_myPowers[index + LlLeft.Items.Length]))
                    {
                        continue;
                    }

                    MidsContext.Character.CurrentBuild.RemovePower(_myPowers[index + LlLeft.Items.Length]);
                    hasChanges = true;
                }

                if (unused)
                {
                    MidsContext.Character.CurrentBuild.AddPower(_myPowers[pIdx], 49).StatInclude = true;
                    item.ItemState = ListLabelV3.LlItemState.Selected;
                }

                LlLeft.Refresh();
                LlRight.Refresh();
                _myParent.PowerModified(unused || hasChanges);
                _myParent.DoRefresh();
            }
        }

        private void llRight_ItemHover(ListLabelV3.ListLabelItemV3 item)

        {
            MiniPowerInfo(item.Index + LlLeft.Items.Length);
        }

        private void llRight_MouseEnter(object? sender, EventArgs e)

        {
            llLeft_MouseEnter(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void MiniPowerInfo(int pIdx)
        {
            if (_locked)
            {
                return;
            }

            var iPopup = new PopUp.PopupData();
            if (pIdx < 0)
            {
                _popInfo.SetPopup(iPopup);
                ChangedScrollFrameContents();
            }
            else
            {
                IPower power1 = new Power(_myPowers[pIdx]);
                power1.AbsorbPetEffects();
                power1.ApplyGrantPowerEffects();
                var index1 = iPopup.Add();
                var str1 = "";
                switch (power1.PowerType)
                {
                    case Enums.ePowerType.Click:
                        if (power1.ClickBuff)
                        {
                            str1 = "(Click)";
                        }

                        break;
                    case Enums.ePowerType.Auto_:
                        str1 = "(Auto)";
                        break;
                    case Enums.ePowerType.Toggle:
                        str1 = "(Toggle)";
                        break;
                }

                iPopup.Sections?[index1].Add(power1.DisplayName, PopUp.Colors.Title);
                iPopup.Sections?[index1].Add(str1 + " " + power1.DescShort, PopUp.Colors.Text, 0.9f);
                var str2 = power1.DescLong.Replace("\0", "").Replace("<br>", "\r\n");
                iPopup.Sections?[index1].Add(str1 + " " + str2, PopUp.Colors.Common, 1f, FontStyle.Regular);
                var index2 = iPopup.Add();
                if (power1.EndCost > 0.0)
                {
                    if (power1.ActivatePeriod > 0.0)
                    {
                        iPopup.Sections?[index2].Add("End Cost:", PopUp.Colors.Title,
                            Utilities.FixDP(power1.EndCost / power1.ActivatePeriod) + "/s", PopUp.Colors.Title, 0.9f,
                            FontStyle.Bold, 1);
                    }
                    else
                    {
                        iPopup.Sections?[index2].Add("End Cost:", PopUp.Colors.Title, Utilities.FixDP(power1.EndCost),
                            PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                    }
                }

                if ((power1.EntitiesAutoHit == Enums.eEntity.None) | ((power1.Range > 20.0) &
                                                                      power1.I9FXPresentP(Enums.eEffectType.Mez,
                                                                          Enums.eMez.Taunt)))
                {
                    iPopup.Sections?[index2].Add("Accuracy:", PopUp.Colors.Title,
                        Utilities.FixDP((float) (MidsContext.Config.ScalingToHit * (double) power1.Accuracy * 100.0)) + "%",
                        PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                }

                if (power1.RechargeTime > 0.0)
                {
                    iPopup.Sections?[index2].Add("Recharge:", PopUp.Colors.Title,
                        Utilities.FixDP(power1.RechargeTime) + "s", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                }

                var durationEffectId = power1.GetDurationEffectID();
                var iNum = 0.0f;
                if (durationEffectId > -1)
                {
                    iNum = power1.Effects[durationEffectId].Duration;
                }

                if ((power1.PowerType != Enums.ePowerType.Toggle) & (power1.PowerType != Enums.ePowerType.Auto_) &&
                    iNum > 0.0)
                {
                    iPopup.Sections?[index2].Add("Duration:", PopUp.Colors.Title, Utilities.FixDP(iNum) + "s",
                        PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                }

                if (power1.Range > 0.0)
                {
                    iPopup.Sections?[index2].Add("Range:", PopUp.Colors.Title, Utilities.FixDP(power1.Range) + "ft",
                        PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                }

                if (power1.Arc > 0)
                {
                    iPopup.Sections?[index2].Add("Arc:", PopUp.Colors.Title, Convert.ToString(power1.Arc) + "°",
                        PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                }
                else if (power1.Radius > 0.0)
                {
                    iPopup.Sections?[index2].Add("Radius:", PopUp.Colors.Title,
                        Convert.ToString(power1.Radius, CultureInfo.InvariantCulture) + "ft", PopUp.Colors.Title, 0.9f,
                        FontStyle.Bold, 1);
                }

                if (power1.CastTime > 0.0)
                {
                    iPopup.Sections?[index2].Add("Cast Time:", PopUp.Colors.Title,
                        Utilities.FixDP(power1.CastTime) + "s", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                }

                var power2 = power1;
                if (power2.Effects.Length > 0)
                {
                    iPopup.Sections?[index2].Add("Effects:", PopUp.Colors.Title);
                    char[] chArray = {'^'};
                    var num1 = power2.Effects.Length - 1;
                    for (var index3 = 0; index3 <= num1; ++index3)
                    {
                        if (!(((power2.Effects[index3].EffectType != Enums.eEffectType.GrantPower) |
                               power2.Effects[index3].Absorbed_Effect) &
                              (power2.Effects[index3].EffectType != Enums.eEffectType.RevokePower) &
                              (power2.Effects[index3].EffectType != Enums.eEffectType.SetMode)))
                        {
                            continue;
                        }

                        var index4 = iPopup.Add();
                        power1.Effects[index3].SetPower(power1);
                        var strArray = power1.Effects[index3].BuildEffectString(false, "", false, false, false, true)
                            .Replace("[", "\r\n")
                            .Replace("\r\n", "^")
                            .Replace("  ", "")
                            .Replace("]", "")
                            .Split(chArray);
                        var num2 = strArray.Length - 1;
                        for (var index5 = 0; index5 <= num2; ++index5)
                            if (index5 == 0)
                            {
                                iPopup.Sections?[index4].Add(strArray[index5], PopUp.Colors.Effect, 0.9f, FontStyle.Bold,
                                    1);
                            }
                            else
                            {
                                iPopup.Sections?[index4].Add(strArray[index5], PopUp.Colors.Disabled, 0.9f,
                                    FontStyle.Italic, 2);
                            }
                    }
                }

                _popInfo.SetPopup(iPopup);
                ChangedScrollFrameContents();
            }
        }

        private void OmegaButton_ButtonClicked()

        {
            var omegaButton = _omegaButton;
            SetPowerSet("Omega", ref omegaButton);
            _omegaButton = omegaButton;
        }

        private void PopInfo_MouseEnter(object? sender, EventArgs e)

        {
            if (!ContainsFocus)
            {
                return;
            }

            _vScrollBar1.Focus();
        }

        private void PopInfo_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                if (_vScrollBar1.Value + _vScrollBar1.LargeChange <= _vScrollBar1.Maximum)
                {
                    _vScrollBar1.Value += _vScrollBar1.LargeChange;
                }
            }
            else if (_vScrollBar1.Value - _vScrollBar1.LargeChange >= _vScrollBar1.Minimum)
            {
                _vScrollBar1.Value -= _vScrollBar1.LargeChange;
            }
            VScrollBar1_Scroll(RuntimeHelpers.GetObjectValue(sender), new ScrollEventArgs(ScrollEventType.EndScroll, _vScrollBar1.Value));
        }

        private void SetPowerSet(string setname, ref ImageButton button)
        {
            foreach (var button1 in _buttonArray)
            {
                button1.Checked = false;
            }

            button.Checked = true;
            _myPowers = DatabaseAPI.GetPowersetByID(setname, Enums.ePowerSetType.Incarnate).Powers;
            FillLists(setname);
        }

        private void StanceButton_ButtonClicked()
        {
            var stanceButton = _stanceButton;
            SetPowerSet("Stance", ref stanceButton);
            _stanceButton = stanceButton;
        }

        private void UpdateLlColours(ref ListLabelV3 iList)
        {
            iList.UpdateTextColors(ListLabelV3.LlItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            iList.UpdateTextColors(ListLabelV3.LlItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            iList.UpdateTextColors(ListLabelV3.LlItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
            iList.ScrollBarColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenHero
                : MidsContext.Config.RtFont.ColorPowerTakenVillain;
            iList.ScrollButtonColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;
            iList.UpdateTextColors(ListLabelV3.LlItemState.Selected,
                MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenHero
                    : MidsContext.Config.RtFont.ColorPowerTakenVillain);
            iList.UpdateTextColors(ListLabelV3.LlItemState.SelectedDisabled,
                MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                    : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain);
            iList.HoverColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                : MidsContext.Config.RtFont.ColorPowerHighlightVillain;
            var style = !MidsContext.Config.RtFont.PowersSelectBold ? FontStyle.Regular : FontStyle.Bold;
            iList.Font = new Font(iList.Font.FontFamily, MidsContext.Config.RtFont.PowersSelectBase, style, GraphicsUnit.Point);
        }

        private void VitaeButton_ButtonClicked()
        {
            var vitaeButton = _vitaeButton;
            SetPowerSet("Vitae", ref vitaeButton);
            _vitaeButton = vitaeButton;
        }

        private void VScrollBar1_Scroll(object? sender, ScrollEventArgs e)
        {
            if ((_popInfo.lHeight > (double) _panel1.Height) & (_vScrollBar1.Maximum > _vScrollBar1.LargeChange))
            {
                _popInfo.ScrollY = (float) (_vScrollBar1.Value /
                                            (double) (_vScrollBar1.Maximum - _vScrollBar1.LargeChange) *
                                            (_popInfo.lHeight - (double) _panel1.Height));
            }
            else
            {
                _popInfo.ScrollY = 0.0f;
            }
        }

        public class CustomPanel : Panel
        {
            protected override Point ScrollToControl(Control activeControl)
            {
                return DisplayRectangle.Location;
            }
        }
    }
}