using System;
using System.Windows.Forms;
using Mids_Reborn.Forms.Controls;
using mrbBase;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmServerData : Form
    {
        public frmServerData()
        {
            Load += frmServerData_OnLoad;
            InitializeComponent();
            Icon = Resources.reborn;
        }

        private void frmServerData_OnLoad(object sender, EventArgs e)
        {
            var serverData = DatabaseAPI.ServerData;
            udBaseFlySpeed.Value = Convert.ToDecimal(serverData.BaseFlySpeed);
            udBaseJumpSpeed.Value = Convert.ToDecimal(serverData.BaseJumpSpeed);
            udBaseJumpHeight.Value = Convert.ToDecimal(serverData.BaseJumpHeight);
            udBaseRunSpeed.Value = Convert.ToDecimal(serverData.BaseRunSpeed);
            udBasePerception.Value = Convert.ToDecimal(serverData.BasePerception);
            udBaseToHit.Value = new decimal(serverData.BaseToHit * 100f);
            udMaxFlySpeed.Value = Convert.ToDecimal(serverData.MaxFlySpeed);
            udMaxJumpSpeed.Value = Convert.ToDecimal(serverData.MaxJumpSpeed);
            udMaxRunSpeed.Value = Convert.ToDecimal(serverData.MaxRunSpeed);
            udMaxSlots.Value = serverData.MaxSlots;
            chkEnableInhSlot.Checked = serverData.EnableInherentSlotting;
            udHealthSlots.Value = serverData.HealthSlots;
            udHealthFirst.Value = serverData.HealthSlot1Level;
            udHealthSecond.Value = serverData.HealthSlot2Level;
            udStaminaSlots.Value = serverData.StaminaSlots;
            udStaminaFirst.Value = serverData.StaminaSlot1Level;
            udStaminaSecond.Value = serverData.StaminaSlot2Level;
            tbManifestUrl.Text = serverData.ManifestUri;
        }

        private static void validate_Url(object sender, InputBoxValidatingArgs e)
        {
            if (e.Text.Trim().Length > 0)
            {
                var validResult = Uri.TryCreate(e.Text.Trim(), UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if (!validResult)
                {
                    e.Cancel = true;
                    e.Message = "Valid Url is required";
                }
                else
                {
                    if (e.Text.EndsWith(".xml"))
                    {
                        return;
                    }
                    e.Cancel = true;
                    e.Message = "This is not a valid manifest URL";
                }
            }
            else
            {
                e.Cancel = true;
                e.Message = "Required";
            }
        }

        private void btnSetManifest_Click(object sender, EventArgs e)
        {
            var loadingText = DatabaseAPI.ServerData.ManifestUri ?? "Enter the URL here";
            var iResult = InputBox.Show("Enter your XML manifest URL", "Set Database Manifest URL", false, loadingText, InputBox.InputBoxIcon.Info, validate_Url);
            if (!iResult.OK) return;
            DatabaseAPI.ServerData.ManifestUri = iResult.Text == "Enter the URL here" ? "" : iResult.Text;
            tbManifestUrl.Text = DatabaseAPI.ServerData.ManifestUri;
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            StoreData();
            DatabaseAPI.SaveServerData(MidsContext.Config.DataPath);
            Close();
        }

        private void StoreData()
        {
            var serverData = DatabaseAPI.ServerData;
            serverData.BaseFlySpeed = Convert.ToSingle(udBaseFlySpeed.Value);
            serverData.BaseJumpSpeed = Convert.ToSingle(udBaseJumpSpeed.Value);
            serverData.BaseJumpHeight = Convert.ToSingle(udBaseJumpHeight.Value);
            serverData.BaseRunSpeed = Convert.ToSingle(udBaseRunSpeed.Value);
            serverData.BasePerception = Convert.ToSingle(udBasePerception.Value);
            serverData.BaseToHit = Convert.ToSingle(decimal.Divide(udBaseToHit.Value, new decimal(100)));
            serverData.MaxFlySpeed = Convert.ToSingle(udMaxFlySpeed.Value);
            serverData.MaxJumpSpeed = Convert.ToSingle(udMaxJumpSpeed.Value);
            serverData.MaxRunSpeed = Convert.ToSingle(udMaxRunSpeed.Value);
            serverData.MaxSlots = Convert.ToInt32(udMaxSlots.Value);
            serverData.HealthSlots = Convert.ToInt32(udHealthSlots.Value);
            serverData.HealthSlot1Level = Convert.ToInt32(udHealthFirst.Value);
            serverData.HealthSlot2Level = Convert.ToInt32(udHealthSecond.Value);
            serverData.StaminaSlots = Convert.ToInt32(udStaminaSlots.Value);
            serverData.StaminaSlot1Level = Convert.ToInt32(udStaminaFirst.Value);
            serverData.StaminaSlot2Level = Convert.ToInt32(udStaminaSecond.Value);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chkEnableInhSlot_CheckChanged(object sender, EventArgs e)
        {
            DatabaseAPI.ServerData.EnableInherentSlotting = chkEnableInhSlot.Checked;
        }
    }
}
