using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Document_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Forms
{
    public partial class frmPrint : Form
    {
        private Print _printer;

        public frmPrint()
        {
            Load += frmPrint_Load;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmPrint));
            Icon = Resources.reborn;
            Name = nameof(frmPrint);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnLayout_Click(object sender, EventArgs e)
        {
            dlgSetup.Document = _printer.Document;
            dlgSetup.ShowDialog();
            lblPrinter.Text = _printer.Document.PrinterSettings.PrinterName;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            MidsContext.Config.LastPrinter = _printer.Document.PrinterSettings.PrinterName;
            MidsContext.Config.PrintHistory = chkPrintHistory.Checked;
            MidsContext.Config.DisablePrintProfileEnh = !chkProfileEnh.Checked;
            MidsContext.Config.I9.DisablePrintIOLevels = !chkPrintHistoryEnh.Checked;
            MidsContext.Config.PrintProfile = !rbProfileShort.Checked
                ? !rbProfileLong.Checked ? ConfigData.PrintOptionProfile.None : ConfigData.PrintOptionProfile.MultiPage
                : ConfigData.PrintOptionProfile.SinglePage;
            if (rbProfileNone.Checked & !chkPrintHistory.Checked)
            {
                MessageBox.Show("You have not selected anything to print!", "Eh?", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                _printer.InitiatePrint();
                Close();
            }
        }

        private void btnPrinter_Click(object sender, EventArgs e)
        {
            var ret = new PrintDialog
            {
                Document = _printer.Document
            }.ShowDialog();
            if (ret == DialogResult.Cancel) return;
            
            lblPrinter.Text = _printer.Document.PrinterSettings.PrinterName;
        }

        private void chkPrintHistory_CheckedChanged(object sender, EventArgs e)
        {
            chkPrintHistoryEnh.Enabled = chkPrintHistory.Checked;
        }

        private void frmPrint_Load(object sender, EventArgs e)
        {
            if (PrinterSettings.InstalledPrinters.Count < 1)
            {
                MessageBox.Show("There are no printers installed!", "Buh...", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                Close();
            }

            _printer = new Print();
            var printerName = "";
            var printerIndex = -1;
            if (_printer.Document.PrinterSettings.IsDefaultPrinter)
                printerName = _printer.Document.PrinterSettings.PrinterName;
            for (var index = 0; index <= PrinterSettings.InstalledPrinters.Count - 1; ++index)
                if (PrinterSettings.InstalledPrinters[index] == MidsContext.Config.LastPrinter)
                {
                    printerIndex = index;
                    _printer.Document.PrinterSettings.PrinterName = PrinterSettings.InstalledPrinters[index];
                }
                else if ((PrinterSettings.InstalledPrinters[index] == printerName) & (printerIndex < 0))
                {
                    printerIndex = index;
                    _printer.Document.PrinterSettings.PrinterName = PrinterSettings.InstalledPrinters[index];
                }

            lblPrinter.Text = _printer.Document.PrinterSettings.PrinterName;
            switch (MidsContext.Config.PrintProfile)
            {
                case ConfigData.PrintOptionProfile.None:
                    rbProfileNone.Checked = true;
                    break;
                case ConfigData.PrintOptionProfile.SinglePage:
                    rbProfileShort.Checked = true;
                    break;
                case ConfigData.PrintOptionProfile.MultiPage:
                    rbProfileLong.Checked = true;
                    break;
                default:
                    rbProfileNone.Checked = true;
                    break;
            }

            chkPrintHistory.Checked = MidsContext.Config.PrintHistory;
            chkPrintHistoryEnh.Checked = !MidsContext.Config.I9.DisablePrintIOLevels;
            chkPrintHistoryEnh.Enabled = chkPrintHistory.Checked;
            chkProfileEnh.Checked = !MidsContext.Config.DisablePrintProfileEnh;
            chkProfileEnh.Enabled = rbProfileShort.Checked;
        }

        private void rbProfileShort_CheckedChanged(object sender, EventArgs e)
        {
            chkProfileEnh.Enabled = rbProfileShort.Checked;
        }
    }
}