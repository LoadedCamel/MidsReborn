
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Base.Display;
using Base.Master_Classes;
using midsControls;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmDPSCalc : Form
    {
        CheckBox chkSortByLevel;
        CheckBox chkDamageBuffs;
        ColumnHeader chPower;
        ColumnHeader chDPA;
        ColumnHeader chDamage;
        ColumnHeader chRecharge;
        ColumnHeader chAnimation;
        ColumnHeader chEndurance;
        ColumnHeader chDamageBuff;
        ColumnHeader chResistanceDebuff;
        ColumnHeader chBuildID;

        ImageButton ibClear;

        ImageButton ibClose;

        ImageButton ibAutoMode;

        ImageButton ibTopmost;
        ImageList ilAttackChain;
        Label lblHeader;
        Label lblDPS;
        Label lblEPS;
        Label lblDPSNum;
        Label lblEPSNum;
        TextBox tbDPSOutput;

        ListView lvPower;
        Panel Panel1;
        Panel Panel2;
        ToolTip ToolTip1;

        private ExtendedBitmap bxRecipe;

        readonly frmMain myParent;
        PowerList[] GlobalPowerList;

        float GlobalDamageBuff;

        public frmDPSCalc(frmMain iParent)
        {
            FormClosed += frmDPSCalc_FormClosed;
            Load += frmDPSCalc_Load;
            InitializeComponent();
            Name = nameof(frmDPSCalc);
            var componentResourceManager = new ComponentResourceManager(typeof(frmDPSCalc));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
            myParent = iParent;
            bxRecipe = new ExtendedBitmap(I9Gfx.GetRecipeName());
        }

        void chkSortByLevel_CheckedChanged(object sender, EventArgs e)
        {
            FillPowerList();
        }

        void FillAttackChainWindow(PowerList[] AttackChain)
        {
            if (chkSortByLevel.Checked)
            {
                for (int index = 0; Math.Abs((double) AttackChain[index].DPA) > float.Epsilon; ++index)
                {
                    string[] strArray = AttackChain[index].PowerName.Split('-');
                    AttackChain[index].PowerName = strArray[1];
                }
            }

            string str1 = AttackChain[0].PowerName;
            float damage = AttackChain[0].Damage;
            float endurance = AttackChain[0].Endurance;
            float animation = AttackChain[0].Animation;
            for (int index = 1; Math.Abs(AttackChain[index].DPA) > float.Epsilon; ++index)
            {
                str1 = str1 + " --> " + AttackChain[index].PowerName;
                damage += AttackChain[index].Damage;
                animation += AttackChain[index].Animation;
                endurance += AttackChain[index].Endurance;
            }

            Label lblDpsNum = lblDPSNum;
            float num = damage / animation;
            string str2 = num.ToString(CultureInfo.InvariantCulture);
            lblDpsNum.Text = str2;
            Label lblEpsNum = lblEPSNum;
            num = endurance / animation;
            string str3 = num.ToString(CultureInfo.InvariantCulture);
            lblEpsNum.Text = str3;
            tbDPSOutput.Text = str1;
        }

        void FillPowerList()
        {
            GlobalDamageBuff = 0.0f;
            lvPower.BeginUpdate();
            lvPower.Items.Clear();
            lvPower.Sorting = SortOrder.None;
            lvPower.Items.Add(" - All Powers - ");
            lvPower.Items[lvPower.Items.Count - 1].Tag = -1;
            int num = MidsContext.Character.CurrentBuild.Powers.Count - 1;
            for (int powerLocation = 0; powerLocation <= num; ++powerLocation)
            {
                if (MidsContext.Character.CurrentBuild.Powers[powerLocation].NIDPower <= -1)
                    continue;
                bool flag = MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.DisplayName == "Rest";
                for (int index = 0; index < MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects.Length && !flag; ++index)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects[index].EffectType != Enums.eEffectType.Damage &&
                        (MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects[index].EffectType !=
                         Enums.eEffectType.Resistance ||
                         !(MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects[index].MagPercent < 0.0)) &&
                        (MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects[index].EffectType !=
                         Enums.eEffectType.DamageBuff ||
                         !(MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects[index].Mag > 0.0) ||
                         MidsContext.Character.CurrentBuild.Powers[powerLocation].StatInclude) &&
                        MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects[index].EffectType !=
                        Enums.eEffectType.EntCreate)
                        continue;
                    string text = DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[powerLocation].NIDPower].DisplayName;
                    if (chkSortByLevel.Checked)
                        text = Strings.Format(MidsContext.Character.CurrentBuild.Powers[powerLocation].Level + 1, "00") + " - " + text;
                    string[] damageData = GetDamageData(powerLocation);
                    lvPower.Items.Add(text).SubItems.AddRange(damageData);
                    GlobalDamageBuff += float.Parse(damageData[5]) *
                                        (MidsContext.Character.CurrentBuild.Powers[powerLocation].Power.Effects[index].Duration /
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

        void frmDPSCalc_FormClosed(object sender, FormClosedEventArgs e)
        {
            StoreLocation();
            myParent.FloatDPSCalc(false);
        }

        void frmDPSCalc_Load(object sender, EventArgs e)
        {
            ibClose.IA = myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[2].Bitmap : myParent.Drawing.bxPower[4].Bitmap;
            ibClose.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            ibTopmost.IA = myParent.Drawing.pImageAttributes;
            ibTopmost.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[2].Bitmap : myParent.Drawing.bxPower[4].Bitmap;
            ibTopmost.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
        }

        void ibClear_ButtonClicked()
        {
            ibClear.Checked = true;
            for (int index = 1; index < lvPower.Items.Count; ++index)
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

        void ibClose_ButtonClicked()
        {
            Close();
        }

        void ibAutoMode_ButtonClicked()
        {
            if (ibAutoMode.TextOff == "Automagical")
            {
                ToolTip1.SetToolTip(ibAutoMode, "Click to enable Automagical Mode");
                ibAutoMode.TextOff = "Manual";
                lvPower.Items[0].Selected = true;
                if (GlobalPowerList.Length > 0)
                {
                    var powerName1 = !chkSortByLevel.Checked ? GlobalPowerList[0].PowerName : GlobalPowerList[0].PowerName.Split('-')[1];
                    tbDPSOutput.Text = powerName1;
                    for (int index = 1; index < GlobalPowerList.Length; ++index)
                    {
                        var powerName2 = !chkSortByLevel.Checked
                            ? GlobalPowerList[index].PowerName
                            : GlobalPowerList[index].PowerName.Split('-')[1];
                        TextBox tbDpsOutput = tbDPSOutput;
                        tbDpsOutput.Text = tbDpsOutput.Text + " --> " + powerName2;
                    }
                }
                else
                    ibClear_ButtonClicked();

                int num = 1;
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

        void ibTopmost_ButtonClicked()
        {
            TopMost = ibTopmost.Checked;
            if (!TopMost)
                return;
            BringToFront();
        }

        void lvPower_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Index == 0)
            {
                if (Operators.ConditionalCompareObjectLess(e.Item.Tag, 0, false) && e.Item.Checked)
                {
                    int num = lvPower.Items.Count - 1;
                    for (int index = 1; index <= num; ++index)
                        lvPower.Items[index].Checked = false;
                }
            }
            else if (e.Item.Checked)
                lvPower.Items[0].Checked = false;

            CalculateDPS();
        }

        void lvPower_Clicked(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (ibAutoMode.TextOff == "Manual" && e.Item.Index != 0 && e.Item.Selected)
            {
                e.Item.Checked = true;
                PowerList[] globalPowerList = GlobalPowerList;
                GlobalPowerList = new PowerList[globalPowerList.Length + 1];
                for (int index = 0; index < globalPowerList.Length; ++index)
                    GlobalPowerList[index] = globalPowerList[index];
                GlobalPowerList[GlobalPowerList.Length - 1].PowerName = e.Item.Text;
                var text = !chkSortByLevel.Checked ? e.Item.Text : e.Item.Text.Split('-')[1];
                if (tbDPSOutput.Text == "")
                {
                    tbDPSOutput.Text = text;
                }
                else
                {
                    TextBox tbDpsOutput = tbDPSOutput;
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

        void lvPower_MouseEnter(object sender, EventArgs e)
        {
            lvPower.Focus();
        }

        static void putInList(ref CountingList[] tl, string item)
        {
            int num = tl.Length - 1;
            for (int index = 0; index <= num; ++index)
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
            Rectangle rectangle = new Rectangle
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

        void StoreLocation()
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
            ibClose.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[2].Bitmap : myParent.Drawing.bxPower[4].Bitmap;
            ibClose.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            ibTopmost.IA = myParent.Drawing.pImageAttributes;
            ibTopmost.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[2].Bitmap : myParent.Drawing.bxPower[4].Bitmap;
            ibTopmost.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            ibClear.IA = myParent.Drawing.pImageAttributes;
            ibClear.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[2].Bitmap : myParent.Drawing.bxPower[4].Bitmap;
            ibClear.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            ibAutoMode.IA = myParent.Drawing.pImageAttributes;
            ibAutoMode.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[2].Bitmap : myParent.Drawing.bxPower[4].Bitmap;
            ibAutoMode.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            FillPowerList();
        }

        string[] GetDamageData(int powerLocation)
        {
            IPower enhancedPower = MainModule.MidsController.Toon.GetEnhancedPower(powerLocation);
            float damageValue = enhancedPower.FXGetDamageValue();
            float rechargeTime = enhancedPower.RechargeTime;
            float num1 = (float) (Math.Ceiling(enhancedPower.CastTimeReal / 0.131999999284744) + 1.0) * 0.132f;
            float endCost = enhancedPower.EndCost;
            Enums.ShortFX effectMag1 = enhancedPower.GetEffectMag(Enums.eEffectType.DamageBuff, Enums.eToWho.Self);
            Enums.ShortFX effectMag2 = enhancedPower.GetEffectMag(Enums.eEffectType.Resistance, Enums.eToWho.Target);
            effectMag1.Multiply();
            effectMag2.Multiply();
            float num2 = damageValue / num1;
            string[] strArray;
            if (Math.Abs(damageValue) > float.Epsilon)
                strArray = new[]
                {
                    num2.ToString(CultureInfo.InvariantCulture), damageValue.ToString(CultureInfo.InvariantCulture),
                    rechargeTime.ToString(CultureInfo.InvariantCulture), num1.ToString(CultureInfo.InvariantCulture),
                    endCost.ToString(CultureInfo.InvariantCulture), effectMag1.Sum.ToString(CultureInfo.InvariantCulture),
                    effectMag2.Sum.ToString(CultureInfo.InvariantCulture), powerLocation.ToString()
                };
            else
                strArray = new[]
                {
                    "-", "-", rechargeTime.ToString(CultureInfo.InvariantCulture), num1.ToString(CultureInfo.InvariantCulture),
                    endCost.ToString(CultureInfo.InvariantCulture), effectMag1.Sum.ToString(CultureInfo.InvariantCulture),
                    effectMag2.Sum.ToString(CultureInfo.InvariantCulture), powerLocation.ToString()
                };
            return strArray;
        }

        void lvPower_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            lvPower.Sort();
        }

        PowerList[] IncrementRecharge(
            PowerList[] List,
            float Time)
        {
            for (int index1 = 0; index1 < List.Length; ++index1)
            {
                int index2 = index1;
                List[index2].RechargeTimer -= Time;
            }

            return List;
        }

        void CalculateDPS()
        {
            if (ibAutoMode.TextOff == "Automagical")
            {
                PowerList[] array = new PowerList[lvPower.Items.Count - 1];
                int length = 0;
                for (int index = 1; index < lvPower.Items.Count; ++index)
                {
                    if (!lvPower.Items[0].Checked && !lvPower.Items[index].Checked)
                        continue;
                    array[length].PowerName = lvPower.Items[index].Text;
                    if (lvPower.Items[index].SubItems[1].Text != "-")
                    {
                        array[length].Damage = float.Parse(lvPower.Items[index].SubItems[2].Text);
                        if (!chkDamageBuffs.Checked)
                        {
                            IPower basePower = MainModule.MidsController.Toon.GetBasePower(int.Parse(lvPower.Items[index].SubItems[8].Text));
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
                        IPower basePower = MainModule.MidsController.Toon.GetBasePower(int.Parse(lvPower.Items[index].SubItems[8].Text));
                        array[length].HidenDPA = basePower.FXGetDamageValue();
                        array[length].HidenDPA = array[length].HidenDPA * (array[length].DamageBuff / array[length].Recharge) /
                                                 array[length].Animation;
                    }

                    ++length;
                }

                if (length < lvPower.Items.Count - 1)
                {
                    PowerList[] powerListArray = array;
                    array = new PowerList[length];
                    for (int index = 0; index < length; ++index)
                        array[index] = powerListArray[index];
                }

                if (array.Length > 1)
                {
                    Array.Sort(array, (x, y) => x.HidenDPA.CompareTo(y.HidenDPA));
                    float num1 = array[length - 1].Recharge + 5f;
                    int num2 = length - 1;
                    while (num1 > 0.0 && num2 > 0)
                        num1 -= array[num2--].Animation;
                    PowerList[] List = new PowerList[length - num2];
                    int num3 = 0;
                    for (int index = 0; index < length - num2; ++index)
                    {
                        if (array[length - 1 - index].Recharge <= 20.0)
                            List[num3++] = array[length - 1 - index];
                    }

                    float num4 = 0.0f;
                    for (int index = 0; index < List.Length; ++index)
                    {
                        if (num4 < (double) List[index].Recharge)
                            num4 = List[index].Recharge;
                    }

                    PowerList[] AttackChain = new PowerList[20];
                    int index1 = 1;
                    int index2 = 1;
                    AttackChain[0] = List[0];
                    float animation = AttackChain[0].Animation;
                    List[0].RechargeTimer = List[0].Recharge;
                    while (animation < num4 + 1.0)
                    {
                        for (int index3 = index1; index3 >= 0; --index3)
                        {
                            if (index1 >= List.Length)
                            {
                                animation += 0.01f;
                                List = IncrementRecharge(List, 0.01f);
                            }
                            else if (List[index3].RechargeTimer <= 0.0)
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
                    tbDPSOutput.Text = "You cannot make an attack chain from one attack";
                else
                    tbDPSOutput.Text = "Come on Kiddo, gotta pick something :)";
            }
            else
            {
                float num1 = 0.0f;
                float num2 = 0.0f;
                float num3 = 0.0f;
                bool flag = true;
                for (int index = 0; index < GlobalPowerList.Length; ++index)
                {
                    if (GlobalPowerList[index].Damage > 0.0)
                    {
                        num1 += GlobalPowerList[index].Damage;
                        num2 += GlobalPowerList[index].Endurance;
                        num3 += GlobalPowerList[index].Animation;
                        GlobalPowerList[index].RechargeTimer = GlobalPowerList[index].Recharge;
                    }

                    float animation = GlobalPowerList[index].Animation;
                }

                var powerListArray = new PowerList[GlobalPowerList.Length * 2];
                int num4 = 0;
                for (int index = 0; index < powerListArray.Length; ++index)
                {
                    if (index > GlobalPowerList.Length - 1)
                        num4 = index - (GlobalPowerList.Length - 1) - 1;
                    powerListArray[index] = GlobalPowerList[num4++];
                }

                for (int index1 = 0; index1 < powerListArray.Length; ++index1)
                {
                    for (int index2 = index1 + 1; index2 < powerListArray.Length; ++index2)
                    {
                        if (powerListArray[index1].PowerName != powerListArray[index2].PowerName)
                            powerListArray[index1].RechargeTimer -= powerListArray[index2].Animation;
                        else if (powerListArray[index1].RechargeTimer > 0.0)
                            flag = false;
                    }
                }

                for (int index1 = powerListArray.Length - 1; index1 >= 0; --index1)
                {
                    for (int index2 = index1 - 1; index2 >= 0; --index2)
                    {
                        if (powerListArray[index1].PowerName != powerListArray[index2].PowerName)
                            powerListArray[index1].RechargeTimer -= powerListArray[index2].Animation;
                        else if (powerListArray[index1].RechargeTimer > 0.0)
                            flag = false;
                    }
                }

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

        struct CountingList
        {
            public string Text;
            public int Count;
        }

        private struct PowerList
        {
            public string PowerName;
            public float baseDamage;
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