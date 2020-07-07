
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Base.Data_Classes;
using Base.Display;
using Microsoft.VisualBasic;
using midsControls;

namespace Hero_Designer
{
    public partial class frmData : Form
    {
        ctlPopUp pInfo;

        readonly Action _onClosing;

        public frmData(Action onClosing)
        {
            FormClosed += frmData_FormClosed;
            Load += frmData_Load;
            ResizeEnd += frmData_ResizeEnd;
            InitializeComponent();
            Name = nameof(frmData);
            var componentResourceManager = new ComponentResourceManager(typeof(frmData));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            _onClosing = onClosing;
        }

        void frmData_FormClosed(object sender, FormClosedEventArgs e)
        {
            StoreLocation();
            _onClosing();
        }

        void frmData_Load(object sender, EventArgs e)
        {
            pInfo.SetPopup(new PopUp.PopupData());
        }

        void frmData_ResizeEnd(object sender, EventArgs e)
        {
            pInfo.Size = ClientSize;
        }

        public void SetLocation()
        {
            Rectangle rectangle = new Rectangle
            {
                X = MainModule.MidsController.SzFrmData.X,
                Y = MainModule.MidsController.SzFrmData.Y,
                Width = MainModule.MidsController.SzFrmData.Width,
                Height = MainModule.MidsController.SzFrmData.Height
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
                rectangle.X = (int)Math.Round((Screen.PrimaryScreen.Bounds.Width - Width) / 2.0);
            if (rectangle.Y < 32)
                rectangle.Y = (int)Math.Round((Screen.PrimaryScreen.Bounds.Height - Height) / 2.0);
            Top = rectangle.Y;
            Left = rectangle.X;
            Height = rectangle.Height;
            Width = rectangle.Width;
        }

        void StoreLocation()
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            MainModule.MidsController.SzFrmData.X = Left;
            MainModule.MidsController.SzFrmData.Y = Top;
            MainModule.MidsController.SzFrmData.Width = Width;
            MainModule.MidsController.SzFrmData.Height = Height;
        }

        string TwoDP(float iValue)
        {
            return Strings.Format(iValue, "###,##0.00");
        }

        public void UpdateData(int powerID)
        {
            PopUp.PopupData iPopup = new PopUp.PopupData();
            if (powerID > -1)
            {
                IPower power1 = DatabaseAPI.Database.Power[powerID];
                int index1 = iPopup.Add();
                iPopup.Sections[index1].Add(DatabaseAPI.Database.Power[powerID].DisplayName, PopUp.Colors.Title, 1.25f);
                iPopup.Sections[index1].Add("Unbuffed Power Data", PopUp.Colors.Title);
                int index2 = iPopup.Add();
                iPopup.Sections[index2].Add("Attributes:", PopUp.Colors.Title);
                iPopup.Sections[index2].Add("Power Type:", PopUp.Colors.Text, Enum.GetName(power1.PowerType.GetType(), power1.PowerType), PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                iPopup.Sections[index2].Add("Accuracy:", PopUp.Colors.Text, TwoDP(power1.Accuracy), PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                if (power1.ActivatePeriod > 0.0)
                    iPopup.Sections[index2].Add("Activate Interval:", PopUp.Colors.Text, Convert.ToString(power1.ActivatePeriod, CultureInfo.InvariantCulture) + "s", PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                if (power1.Arc > 0)
                    iPopup.Sections[index2].Add("Arc Radius:", PopUp.Colors.Text, Convert.ToString(power1.Arc) + "°", PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                if (power1.AttackTypes != Enums.eVector.None)
                {
                    int[] values = (int[])Enum.GetValues(power1.AttackTypes.GetType());
                    bool flag = true;
                    int num = values.Length - 1;
                    for (int index3 = 1; index3 <= num; ++index3)
                    {
                        if ((power1.AttackTypes & (Enums.eVector) values[index3]) <= Enums.eVector.None)
                            continue;
                        string iText;
                        if (flag)
                        {
                            iText = "Attack Type(s):";
                            flag = false;
                        }
                        else
                            iText = "";
                        iPopup.Sections[index2].Add(iText, PopUp.Colors.Text, Enum.GetName(power1.AttackTypes.GetType(), values[index3]), PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                    }
                }
                iPopup.Sections[index2].Add("Cast Time:", PopUp.Colors.Text, TwoDP(power1.CastTime) + "s", PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                iPopup.Sections[index2].Add("Effect Area:", PopUp.Colors.Text, Enum.GetName(power1.EffectArea.GetType(), power1.EffectArea), PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                iPopup.Sections[index2].Add("End Cost:", PopUp.Colors.Text, TwoDP(power1.EndCost), PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                iPopup.Sections[index2].Add("Auto-Hit:", PopUp.Colors.Text, Enum.GetName(power1.EntitiesAutoHit.GetType(), power1.EntitiesAutoHit), PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                if (Math.Abs(power1.InterruptTime) > float.Epsilon)
                    iPopup.Sections[index2].Add("Interrupt:", PopUp.Colors.Text, TwoDP(power1.InterruptTime) + "s", PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                iPopup.Sections[index2].Add("Level Available:", PopUp.Colors.Text, Convert.ToString(power1.Level), PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                iPopup.Sections[index2].Add("Max Targets:", PopUp.Colors.Text, Convert.ToString(power1.MaxTargets), PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                iPopup.Sections[index2].Add("Notify Mobs:", PopUp.Colors.Text, Enum.GetName(power1.AIReport.GetType(), power1.AIReport), PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                if (Math.Abs(power1.Radius) > float.Epsilon)
                    iPopup.Sections[index2].Add("Radius:", PopUp.Colors.Text, Convert.ToString(power1.Radius, CultureInfo.InvariantCulture) + "ft", PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                if (Math.Abs(power1.Range) > float.Epsilon)
                    iPopup.Sections[index2].Add("Range:", PopUp.Colors.Text, Convert.ToString(power1.Range, CultureInfo.InvariantCulture) + "ft", PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                iPopup.Sections[index2].Add("RechargeTime:", PopUp.Colors.Text, Convert.ToString(power1.RechargeTime, CultureInfo.InvariantCulture) + "s", PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                iPopup.Sections[index2].Add("Target:", PopUp.Colors.Text, Enum.GetName(power1.EntitiesAffected.GetType(), power1.EntitiesAffected), PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                iPopup.Sections[index2].Add("Line of Sight:", PopUp.Colors.Text, Convert.ToString(power1.TargetLoS), PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                iPopup.Sections[index2].Add("Variable:", PopUp.Colors.Text, Convert.ToString(power1.VariableEnabled), PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                if (power1.VariableEnabled)
                {
                    iPopup.Sections[index2].Add("Attrib:", PopUp.Colors.Text, power1.VariableName, PopUp.Colors.Text, 0.9f, FontStyle.Bold, 2);
                    iPopup.Sections[index2].Add("Min:", PopUp.Colors.Text, Convert.ToString(power1.VariableMin), PopUp.Colors.Text, 0.9f, FontStyle.Bold, 2);
                    iPopup.Sections[index2].Add("Max:", PopUp.Colors.Text, Convert.ToString(power1.VariableMax), PopUp.Colors.Text, 0.9f, FontStyle.Bold, 2);
                }
                if (power1.Effects.Length > 0)
                {
                    int index3 = iPopup.Add();
                    iPopup.Sections[index3].Add("Effects:", PopUp.Colors.Title);
                    IPower power2 = new Power(DatabaseAPI.Database.Power[powerID]);
                    char[] chArray = { '^' };
                    int num1 = power1.Effects.Length - 1;
                    for (int index4 = 0; index4 <= num1; ++index4)
                    {
                        int index5 = iPopup.Add();
                        power2.Effects[index4].SetPower(power2);
                        string[] strArray = power2.Effects[index4].BuildEffectString().Replace("[", "\r\n").Replace("\r\n", "^").Replace("  ", "").Replace("]", "").Split(chArray);
                        int num2 = strArray.Length - 1;
                        for (int index6 = 0; index6 <= num2; ++index6)
                        {
                            if (index6 == 0)
                                iPopup.Sections[index5].Add(strArray[index6], PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
                            else
                                iPopup.Sections[index5].Add(strArray[index6], PopUp.Colors.Disabled, 0.9f, FontStyle.Italic, 2);
                        }
                    }
                }
            }
            else
            {
                int index = iPopup.Add();
                iPopup.Sections[index].Add("No Power", PopUp.Colors.Title, 1.25f);
            }
            pInfo.SetPopup(iPopup);
            pInfo.Width = ClientSize.Width;
        }
    }
}