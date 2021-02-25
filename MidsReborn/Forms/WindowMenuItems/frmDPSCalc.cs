using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using mrbBase;
using mrbBase.Base.Display;
using mrbBase.Base.Master_Classes;
using mrbControls;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmDPSCalc : Form
    {
        private readonly frmMain myParent;

        private ExtendedBitmap bxRecipe;
        private ColumnHeader chAnimation;
        private ColumnHeader chBuildID;
        private ColumnHeader chDamage;
        private ColumnHeader chDamageBuff;
        private ColumnHeader chDPA;
        private ColumnHeader chEndurance;
        private CheckBox chkDamageBuffs;
        private CheckBox chkSortByLevel;
        private ColumnHeader chPower;
        private ColumnHeader chRecharge;
        private ColumnHeader chResistanceDebuff;

        private float GlobalDamageBuff;
        private PowerList[] GlobalPowerList;

        private ImageButton ibAutoMode;

        private ImageButton ibClear;

        private ImageButton ibClose;

        private ImageButton ibTopmost;
        private ImageList ilAttackChain;
        private Label lblDPS;
        private Label lblDPSNum;
        private Label lblEPS;
        private Label lblEPSNum;
        private Label lblHeader;

        private ListView lvPower;
        private Panel Panel1;
        private Panel Panel2;
        private TextBox tbDPSOutput;
        private ToolTip ToolTip1;

        public frmDPSCalc(frmMain iParent)
        {
            FormClosed += frmDPSCalc_FormClosed;
            Load += frmDPSCalc_Load;
            InitializeComponent();
            Name = nameof(frmDPSCalc);
            var componentResourceManager = new ComponentResourceManager(typeof(frmDPSCalc));
            Icon = Resources.reborn;
            myParent = iParent;
            bxRecipe = new ExtendedBitmap(I9Gfx.GetRecipeName());
        }

        private void chkSortByLevel_CheckedChanged(object sender, EventArgs e)
        {
            FillPowerList();
        }

        private void FillAttackChainWindow(PowerList[] AttackChain)
        {
            if (chkSortByLevel.Checked)
                for (var index = 0; Math.Abs((double) AttackChain[index].DPA) > float.Epsilon; ++index)
                {
                    var strArray = AttackChain[index].PowerName.Split('-');
                    AttackChain[index].PowerName = strArray[1];
                }

            var str1 = AttackChain[0].PowerName;
            var damage = AttackChain[0].Damage;
            var endurance = AttackChain[0].Endurance;
            var animation = AttackChain[0].Animation;
            for (var index = 1; Math.Abs(AttackChain[index].DPA) > float.Epsilon; ++index)
            {
                str1 = str1 + " --> " + AttackChain[index].PowerName;
                damage += AttackChain[index].Damage;
                animation += AttackChain[index].Animation;
                endurance += AttackChain[index].Endurance;
            }

            var lblDpsNum = lblDPSNum;
            var num = damage / animation;
            var str2 = num.ToString(CultureInfo.InvariantCulture);
            lblDpsNum.Text = str2;
            var lblEpsNum = lblEPSNum;
            num = endurance / animation;
            var str3 = num.ToString(CultureInfo.InvariantCulture);
            lblEpsNum.Text = str3;
            tbDPSOutput.Text = str1;
        }

        private void FillPowerList()
        {
            GlobalDamageBuff = 0.0f;
            lvPower.BeginUpdate();
            lvPower.Items.Clear();
            lvPower.Sorting = SortOrder.None;
            lvPower.Items.Add(" - All Powers - ");
            lvPower.Items[lvPower.Items.Count - 1].Tag = -1;
            var num = MidsContext.Character.CurrentBuild.Powers.Count - 1;
            for (var powerLocation = 0; powerLocation <= num; ++powerLocation)
            {
                if (MidsContext.Character.CurrentBuild.Powers[powerLocation].NIDPower <= -1)
                    continue;
                var flag = MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.DisplayName == "Rest";
                for (var index = 0;
                    index < MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects.Length && !flag;
                    ++index)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects[index].EffectType !=
                        Enums.eEffectType.Damage &&
                        (MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects[index].EffectType !=
                         Enums.eEffectType.Resistance ||
                         !(MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects[index].MagPercent <
                           0.0)) &&
                        (MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects[index].EffectType !=
                         Enums.eEffectType.DamageBuff ||
                         !(MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects[index].Mag > 0.0) ||
                         MidsContext.Character.CurrentBuild.Powers[powerLocation].StatInclude) &&
                        MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects[index].EffectType !=
                        Enums.eEffectType.EntCreate)
                        continue;
                    var text = DatabaseAPI.Database
                        .Power[MidsContext.Character.CurrentBuild.Powers[powerLocation].NIDPower].DisplayName;
                    if (chkSortByLevel.Checked)
                        text =
                            Strings.Format(MidsContext.Character.CurrentBuild.Powers[powerLocation].Level + 1, "00") +
                            " - " + text;
                    var damageData = GetDamageData(powerLocation);
                    lvPower.Items.Add(text).SubItems.AddRange(damageData);
                    GlobalDamageBuff += float.Parse(damageData[5]) *
                                        (MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects[index]
                                             .Duration /
                                         float.Parse(damageData[2]));
                    lvPower.Items[lvPower.Items.Count - 1].Tag = powerLocation;
                    flag = true;
                }
            }

            lvPower.Sorting = SortOrder.Ascending;
            lvPower.Sort();
            if (lvPower.Items.Count > 0)
            {
                lvPower.Items[0].Selected = true;
                lvPower.Items[0].Checked = true;
            }

            lvPower.EndUpdate();
        }

        private void frmDPSCalc_FormClosed(object sender, FormClosedEventArgs e)
        {
            StoreLocation();
            myParent.FloatDPSCalc(false);
        }

        private void frmDPSCalc_Load(object sender, EventArgs e)
        {
            ibClose.IA = myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibClose.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            ibTopmost.IA = myParent.Drawing.pImageAttributes;
            ibTopmost.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibTopmost.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
        }

        private void ibClear_ButtonClicked()
        {
            ibClear.Checked = true;
            for (var index = 1; index < lvPower.Items.Count; ++index)
                lvPower.Items[index].Checked = false;
            lvPower.Items[0].Checked = true;
            lvPower.Items[0].Selected = true;
            GlobalPowerList = new PowerList[0];
            tbDPSOutput.Text = "";
            lblDPSNum.Text = " - Null - ";
            lblEPSNum.Text = " - Null - ";
            ibClear.Checked = false;
            lblHeader.ForeColor = Color.FromArgb(192, 192, byte.MaxValue);
            lblHeader.Text = "You may select -All Powers- or just the powers you want to consider.";
            if (ibAutoMode.TextOff != "Automagical")
                return;
            CalculateDPS();
        }

        private void ibClose_ButtonClicked()
        {
            Close();
        }

        private void ibAutoMode_ButtonClicked()
        {
            if (ibAutoMode.TextOff == "Automagical")
            {
                ToolTip1.SetToolTip(ibAutoMode, "Click to enable Automagical Mode");
                ibAutoMode.TextOff = "Manual";
                lvPower.Items[0].Selected = true;
                if (GlobalPowerList.Length > 0)
                {
                    var powerName1 = !chkSortByLevel.Checked
                        ? GlobalPowerList[0].PowerName
                        : GlobalPowerList[0].PowerName.Split('-')[1];
                    tbDPSOutput.Text = powerName1;
                    for (var index = 1; index < GlobalPowerList.Length; ++index)
                    {
                        var powerName2 = !chkSortByLevel.Checked
                            ? GlobalPowerList[index].PowerName
                            : GlobalPowerList[index].PowerName.Split('-')[1];
                        var tbDpsOutput = tbDPSOutput;
                        tbDpsOutput.Text = tbDpsOutput.Text + " --> " + powerName2;
                    }
                }
                else
                {
                    ibClear_ButtonClicked();
                }

                var num = 1;
                while (num < GlobalPowerList.Length)
                    ++num;
            }
            else
            {
                ibAutoMode.TextOff = "Automagical";
                ToolTip1.SetToolTip(ibAutoMode, "Click to enable Manual Mode");
            }

            CalculateDPS();
        }

        private void ibTopmost_ButtonClicked()
        {
            TopMost = ibTopmost.Checked;
            if (!TopMost)
                return;
            BringToFront();
        }

        private void lvPower_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Index == 0)
            {
                if (Operators.ConditionalCompareObjectLess(e.Item.Tag, 0, false) && e.Item.Checked)
                {
                    var num = lvPower.Items.Count - 1;
                    for (var index = 1; index <= num; ++index)
                        lvPower.Items[index].Checked = false;
                }
            }
            else if (e.Item.Checked)
            {
                lvPower.Items[0].Checked = false;
            }

            CalculateDPS();
        }

        private void lvPower_Clicked(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (ibAutoMode.TextOff == "Manual" && e.Item.Index != 0 && e.Item.Selected)
            {
                e.Item.Checked = true;
                var globalPowerList = GlobalPowerList;
                GlobalPowerList = new PowerList[globalPowerList.Length + 1];
                for (var index = 0; index < globalPowerList.Length; ++index)
                    GlobalPowerList[index] = globalPowerList[index];
                GlobalPowerList[GlobalPowerList.Length - 1].PowerName = e.Item.Text;
                var text = !chkSortByLevel.Checked ? e.Item.Text : e.Item.Text.Split('-')[1];
                if (tbDPSOutput.Text == "")
                {
                    tbDPSOutput.Text = text;
                }
                else
                {
                    var tbDpsOutput = tbDPSOutput;
                    tbDpsOutput.Text = tbDpsOutput.Text + " -->" + text;
                }

                GlobalPowerList[GlobalPowerList.Length - 1].Damage =
                    !(e.Item.SubItems[2].Text != "-") ? 0.0f : float.Parse(e.Item.SubItems[2].Text);
                GlobalPowerList[GlobalPowerList.Length - 1].Endurance = float.Parse(e.Item.SubItems[5].Text);
                GlobalPowerList[GlobalPowerList.Length - 1].Recharge = float.Parse(e.Item.SubItems[3].Text);
                GlobalPowerList[GlobalPowerList.Length - 1].DamageBuff = float.Parse(e.Item.SubItems[6].Text);
                GlobalPowerList[GlobalPowerList.Length - 1].ResistanceDeBuff = float.Parse(e.Item.SubItems[7].Text);
                GlobalPowerList[GlobalPowerList.Length - 1].Animation = float.Parse(e.Item.SubItems[4].Text);
                GlobalPowerList[GlobalPowerList.Length - 1].RechargeTimer = 0.0f;
            }

            CalculateDPS();
        }

        private void lvPower_MouseEnter(object sender, EventArgs e)
        {
            lvPower.Focus();
        }

        private static void putInList(ref CountingList[] tl, string item)
        {
            var num = tl.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                if (tl[index].Text != item)
                    continue;
                ++tl[index].Count;
                return;
            }

            tl = (CountingList[]) Utils.CopyArray(tl, new CountingList[tl.Length + 1]);
            tl[tl.Length - 1].Count = 1;
            tl[tl.Length - 1].Text = item;
        }

        public void SetLocation()
        {
            var rectangle = new Rectangle
            {
                X = MainModule.MidsController.SzFrmRecipe.X,
                Y = MainModule.MidsController.SzFrmRecipe.Y,
                Width = 800,
                Height = MainModule.MidsController.SzFrmRecipe.Height
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

        private void StoreLocation()
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            MainModule.MidsController.SzFrmRecipe.X = Left;
            MainModule.MidsController.SzFrmRecipe.Y = Top;
            MainModule.MidsController.SzFrmRecipe.Width = Width;
            MainModule.MidsController.SzFrmRecipe.Height = Height;
        }

        public void UpdateData()
        {
            BackColor = myParent.BackColor;
            ibClose.IA = myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibClose.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            ibTopmost.IA = myParent.Drawing.pImageAttributes;
            ibTopmost.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibTopmost.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            ibClear.IA = myParent.Drawing.pImageAttributes;
            ibClear.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibClear.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            ibAutoMode.IA = myParent.Drawing.pImageAttributes;
            ibAutoMode.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibAutoMode.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            FillPowerList();
        }

        private string[] GetDamageData(int powerLocation)
        {
            var enhancedPower = MainModule.MidsController.Toon.GetEnhancedPower(powerLocation);
            var damageValue = enhancedPower.FXGetDamageValue();
            var rechargeTime = enhancedPower.RechargeTime;
            var num1 = (float) (Math.Ceiling(enhancedPower.CastTimeReal / 0.131999999284744) + 1.0) * 0.132f;
            var endCost = enhancedPower.EndCost;
            var effectMag1 = enhancedPower.GetEffectMag(Enums.eEffectType.DamageBuff, Enums.eToWho.Self);
            var effectMag2 = enhancedPower.GetEffectMag(Enums.eEffectType.Resistance, Enums.eToWho.Target);
            var effectMag3 = enhancedPower.GetEffectMag(Enums.eEffectType.DamageBuff, Enums.eToWho.Ally);
            var effectMag4 = enhancedPower.GetEffectMag(Enums.eEffectType.Resistance, Enums.eToWho.Ally);
            effectMag1.Multiply();
            effectMag2.Multiply();
            effectMag3.Multiply();
            effectMag4.Multiply();
            var num2 = damageValue / num1;
            string[] strArray;
            if (Math.Abs(damageValue) > float.Epsilon)
                strArray = new[]
                {
                    num2.ToString(CultureInfo.InvariantCulture), damageValue.ToString(CultureInfo.InvariantCulture),
                    rechargeTime.ToString(CultureInfo.InvariantCulture), num1.ToString(CultureInfo.InvariantCulture),
                    endCost.ToString(CultureInfo.InvariantCulture),
                    effectMag1.Sum.ToString(CultureInfo.InvariantCulture),
                    effectMag2.Sum.ToString(CultureInfo.InvariantCulture),
                    effectMag3.Sum.ToString(CultureInfo.InvariantCulture),
                    effectMag4.Sum.ToString(CultureInfo.InvariantCulture),
                    powerLocation.ToString()
                };
            else
                strArray = new[]
                {
                    "-", "-", rechargeTime.ToString(CultureInfo.InvariantCulture),
                    num1.ToString(CultureInfo.InvariantCulture),
                    endCost.ToString(CultureInfo.InvariantCulture),
                    effectMag1.Sum.ToString(CultureInfo.InvariantCulture),
                    effectMag2.Sum.ToString(CultureInfo.InvariantCulture),
                    effectMag3.Sum.ToString(CultureInfo.InvariantCulture),
                    effectMag4.Sum.ToString(CultureInfo.InvariantCulture),
                    powerLocation.ToString()
                };
            return strArray;
        }

        private void lvPower_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            lvPower.Sort();
        }

        private PowerList[] IncrementRecharge(
            PowerList[] List,
            float Time)
        {
            for (var index1 = 0; index1 < List.Length; ++index1)
            {
                var index2 = index1;
                List[index2].RechargeTimer -= Time;
            }

            return List;
        }

        private void CalculateDPS()
        {
            if (ibAutoMode.TextOff == "Automagical")
            {
                var array = new PowerList[lvPower.Items.Count - 1];
                var length = 0;
                for (var index = 1; index < lvPower.Items.Count; ++index)
                {
                    if (!lvPower.Items[0].Checked && !lvPower.Items[index].Checked)
                        continue;
                    array[length].PowerName = lvPower.Items[index].Text;
                    if (lvPower.Items[index].SubItems[1].Text != "-")
                    {
                        array[length].Damage = float.Parse(lvPower.Items[index].SubItems[2].Text);
                        if (!chkDamageBuffs.Checked)
                        {
                            var basePower =
                                MainModule.MidsController.Toon.GetBasePower(
                                    int.Parse(lvPower.Items[index].SubItems[8].Text));
                            array[length].Damage += basePower.FXGetDamageValue() * (GlobalDamageBuff / 100f);
                        }

                        array[length].DPA = float.Parse(lvPower.Items[index].SubItems[1].Text);
                        array[length].HidenDPA = float.Parse(lvPower.Items[index].SubItems[1].Text);
                    }

                    array[length].Recharge = float.Parse(lvPower.Items[index].SubItems[3].Text);
                    array[length].Animation = float.Parse(lvPower.Items[index].SubItems[4].Text);
                    array[length].Endurance = float.Parse(lvPower.Items[index].SubItems[5].Text);
                    array[length].DamageBuff = float.Parse(lvPower.Items[index].SubItems[6].Text);
                    array[length].ResistanceDeBuff = float.Parse(lvPower.Items[index].SubItems[7].Text);
                    array[length].RechargeTimer = -1f;
                    if (array[length].DamageBuff > 0.0 && Math.Abs(array[length].DPA) > float.Epsilon)
                    {
                        var basePower =
                            MainModule.MidsController.Toon.GetBasePower(
                                int.Parse(lvPower.Items[index].SubItems[8].Text));
                        array[length].HidenDPA = basePower.FXGetDamageValue();
                        array[length].HidenDPA =
                            array[length].HidenDPA * (array[length].DamageBuff / array[length].Recharge) /
                            array[length].Animation;
                    }

                    ++length;
                }

                if (length < lvPower.Items.Count - 1)
                {
                    var powerListArray = array;
                    array = new PowerList[length];
                    for (var index = 0; index < length; ++index)
                        array[index] = powerListArray[index];
                }

                if (array.Length > 1)
                {
                    Array.Sort(array, (x, y) => x.HidenDPA.CompareTo(y.HidenDPA));
                    var num1 = array[length - 1].Recharge + 5f;
                    var num2 = length - 1;
                    while (num1 > 0.0 && num2 > 0)
                        num1 -= array[num2--].Animation;
                    var List = new PowerList[length - num2];
                    var num3 = 0;
                    for (var index = 0; index < length - num2; ++index)
                        if (array[length - 1 - index].Recharge <= 20.0)
                            List[num3++] = array[length - 1 - index];

                    var num4 = 0.0f;
                    for (var index = 0; index < List.Length; ++index)
                        if (num4 < (double) List[index].Recharge)
                            num4 = List[index].Recharge;

                    var AttackChain = new PowerList[20];
                    var index1 = 1;
                    var index2 = 1;
                    AttackChain[0] = List[0];
                    var animation = AttackChain[0].Animation;
                    List[0].RechargeTimer = List[0].Recharge;
                    while (animation < num4 + 1.0)
                    {
                        for (var index3 = index1; index3 >= 0; --index3)
                            if (index1 >= List.Length)
                            {
                                animation += 0.01f;
                                List = IncrementRecharge(List, 0.01f);
                            }
                            else if (List[index3].RechargeTimer <= 0.0)
                            {
                                index1 = index3;
                            }

                        if (index1 >= List.Length)
                        {
                            --index1;
                            animation += 0.01f;
                            List = IncrementRecharge(List, 0.01f);
                        }

                        while (List[index1].RechargeTimer > 0.0)
                        {
                            ++index1;
                            if (index1 < List.Length)
                                continue;
                            index1 = 0;
                            animation += 0.01f;
                            List = IncrementRecharge(List, 0.01f);
                        }

                        AttackChain[index2] = List[index1];
                        animation += AttackChain[index2].Animation;
                        List = IncrementRecharge(List, AttackChain[index2].Animation);
                        List[index1].RechargeTimer = List[index1].Recharge;
                        ++index1;
                        ++index2;
                    }

                    FillAttackChainWindow(AttackChain);
                }
                else if (array.Length == 1)
                {
                    tbDPSOutput.Text = "You cannot make an attack chain from one attack";
                }
                else
                {
                    tbDPSOutput.Text = "Come on Kiddo, gotta pick something :)";
                }
            }
            else
            {
                var num1 = 0.0f;
                var num2 = 0.0f;
                var num3 = 0.0f;
                var flag = true;
                for (var index = 0; index < GlobalPowerList.Length; ++index)
                {
                    if (GlobalPowerList[index].Damage > 0.0)
                    {
                        num1 += GlobalPowerList[index].Damage;
                        num2 += GlobalPowerList[index].Endurance;
                        num3 += GlobalPowerList[index].Animation;
                        GlobalPowerList[index].RechargeTimer = GlobalPowerList[index].Recharge;
                    }

                    var animation = GlobalPowerList[index].Animation;
                }

                var powerListArray = new PowerList[GlobalPowerList.Length * 2];
                var num4 = 0;
                for (var index = 0; index < powerListArray.Length; ++index)
                {
                    if (index > GlobalPowerList.Length - 1)
                        num4 = index - (GlobalPowerList.Length - 1) - 1;
                    powerListArray[index] = GlobalPowerList[num4++];
                }

                for (var index1 = 0; index1 < powerListArray.Length; ++index1)
                for (var index2 = index1 + 1; index2 < powerListArray.Length; ++index2)
                    if (powerListArray[index1].PowerName != powerListArray[index2].PowerName)
                        powerListArray[index1].RechargeTimer -= powerListArray[index2].Animation;
                    else if (powerListArray[index1].RechargeTimer > 0.0)
                        flag = false;

                for (var index1 = powerListArray.Length - 1; index1 >= 0; --index1)
                for (var index2 = index1 - 1; index2 >= 0; --index2)
                    if (powerListArray[index1].PowerName != powerListArray[index2].PowerName)
                        powerListArray[index1].RechargeTimer -= powerListArray[index2].Animation;
                    else if (powerListArray[index1].RechargeTimer > 0.0)
                        flag = false;

                if (!flag)
                {
                    lblHeader.ForeColor = Color.Red;
                    lblHeader.Text = "Impossible Chain";
                }
                else
                {
                    lblHeader.ForeColor = Color.FromArgb(192, 192, byte.MaxValue);
                    lblHeader.Text = "You may select -All Powers- or just the powers you want to consider.";
                }

                lblDPSNum.Text = (num1 / num3).ToString(CultureInfo.InvariantCulture);
                lblEPSNum.Text = (num2 / num3).ToString(CultureInfo.InvariantCulture);
            }
        }

        private struct CountingList
        {
            public string Text;
            public int Count;
        }

        private struct PowerList
        {
            public string PowerName;
            public float BaseDamage;
            public float Damage;
            public float DPA;
            public float HidenDPA;
            public float Recharge;
            public float Animation;
            public float Endurance;
            public float DamageBuff;
            public float ResistanceDeBuff;
            public float RechargeTimer;
        }
    }
}