using System;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;
using MRBResourceLib;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmServerData : Form
    {
        private readonly ServerData _serverData;

        public frmServerData()
        {
            _serverData = DatabaseAPI.ServerData;
            Load += frmServerData_OnLoad;
            InitializeComponent();
            Icon = Resources.MRB_Icon_Concept;
        }

        private void frmServerData_OnLoad(object? sender, EventArgs e)
        {
            udBaseFlySpeed.Value = Convert.ToDecimal(_serverData.BaseFlySpeed);
            udBaseJumpSpeed.Value = Convert.ToDecimal(_serverData.BaseJumpSpeed);
            udBaseJumpHeight.Value = Convert.ToDecimal(_serverData.BaseJumpHeight);
            udBaseRunSpeed.Value = Convert.ToDecimal(_serverData.BaseRunSpeed);
            udBasePerception.Value = Convert.ToDecimal(_serverData.BasePerception);
            udBaseToHit.Value = new decimal(_serverData.BaseToHit * 100f);
            udMaxFlySpeed.Value = Convert.ToDecimal(_serverData.MaxFlySpeed);
            udMaxJumpSpeed.Value = Convert.ToDecimal(_serverData.MaxJumpSpeed);
            udMaxJumpHeight.Value = Convert.ToDecimal(_serverData.MaxJumpHeight);
            udMaxRunSpeed.Value = Convert.ToDecimal(_serverData.MaxRunSpeed);
            udMaxMaxFlySpeed.Value = Convert.ToDecimal(_serverData.MaxMaxFlySpeed);
            udMaxMaxJumpSpeed.Value = Convert.ToDecimal(_serverData.MaxMaxJumpSpeed);
            udMaxMaxRunSpeed.Value = Convert.ToDecimal(_serverData.MaxMaxRunSpeed);
            
            udMaxSlots.Value = _serverData.MaxSlots;
            chkEnableInhSlot.Checked = _serverData.EnableInherentSlotting;
            udHealthSlots.Value = _serverData.HealthSlots;
            udHealthFirst.Value = _serverData.HealthSlot1Level;
            udHealthSecond.Value = _serverData.HealthSlot2Level;
            udStaminaSlots.Value = _serverData.StaminaSlots;
            udStaminaFirst.Value = _serverData.StaminaSlot1Level;
            udStaminaSecond.Value = _serverData.StaminaSlot2Level;
            tbManifestUrl.Text = _serverData.ManifestUri;
            foreach (var chkBox in groupBox2.Controls.OfType<CheckBox>())
            {
                chkBox.Checked = _serverData.EnabledIncarnates[chkBox.Text];
            }
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
            var loadingText = DatabaseAPI.ServerData.ManifestUri;
            if (DatabaseAPI.DatabaseName != "Homecoming" && DatabaseAPI.ServerData.ManifestUri.Contains("https://midsreborn.com"))
            {
                DatabaseAPI.ServerData.ManifestUri = "";
                loadingText = "Enter URL Here";
            }
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
            _serverData.BaseFlySpeed = Convert.ToSingle(udBaseFlySpeed.Value);
            _serverData.BaseJumpSpeed = Convert.ToSingle(udBaseJumpSpeed.Value);
            _serverData.BaseJumpHeight = Convert.ToSingle(udBaseJumpHeight.Value);
            _serverData.BaseRunSpeed = Convert.ToSingle(udBaseRunSpeed.Value);
            _serverData.BasePerception = Convert.ToSingle(udBasePerception.Value);
            _serverData.BaseToHit = Convert.ToSingle(decimal.Divide(udBaseToHit.Value, new decimal(100)));
            _serverData.MaxFlySpeed = Convert.ToSingle(udMaxFlySpeed.Value);
            _serverData.MaxJumpSpeed = Convert.ToSingle(udMaxJumpSpeed.Value);
            _serverData.MaxRunSpeed = Convert.ToSingle(udMaxRunSpeed.Value);
            _serverData.MaxSlots = Convert.ToInt32(udMaxSlots.Value);
            _serverData.HealthSlots = Convert.ToInt32(udHealthSlots.Value);
            _serverData.HealthSlot1Level = Convert.ToInt32(udHealthFirst.Value);
            _serverData.HealthSlot2Level = Convert.ToInt32(udHealthSecond.Value);
            _serverData.StaminaSlots = Convert.ToInt32(udStaminaSlots.Value);
            _serverData.StaminaSlot1Level = Convert.ToInt32(udStaminaFirst.Value);
            _serverData.StaminaSlot2Level = Convert.ToInt32(udStaminaSecond.Value);
            foreach (var chkBox in groupBox2.Controls.OfType<CheckBox>())
            {
                _serverData.EnabledIncarnates[chkBox.Text] = chkBox.Checked;
            }
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
