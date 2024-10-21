using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;
using MRBResourceLib;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmMiniList : Form
    {
        private readonly frmMain MyParent;
        private PopUp.PopupData PData;

        public frmMiniList(frmMain iParent)
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            InitializeComponent();
            Name = nameof(frmMiniList);
            //var componentResourceManager = new ComponentResourceManager(typeof(frmMiniList));
            Icon = Resources.MRB_Icon_Concept;
            MyParent = iParent;
        }

        private void frmMiniList_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyParent.UnSetMiniList();
        }

        public void SetData(PopUp.PopupData pData, bool instantDisplay = true)
        {
            PData = pData;
            if (!instantDisplay)
            {
                return;
            }

            DisplayData();
        }

        private void frmMiniList_Load(object sender, System.EventArgs e)
        {
            DisplayData();
        }

        private void DisplayData()
        {
            const int indentMultiplier = 2;
            var r = new Regex(@"Level ([0-9]+)");
            var level = 0;
            var text = RTF.StartRTF();
            text += RTF.Color(RTF.ElementID.Text);
            for (var i = 0; i < PData.Sections.Length; i++)
            {
                for (var j = 0; j < PData.Sections[i].Content.Length; j++)
                {
                    if (i > 0 | j > 0)
                    {
                        text += RTF.Crlf();
                    }

                    var lineText = PData.Sections[i].Content[j].Text.Trim(); 
                    
                    // Detect level change
                    if (r.IsMatch(lineText))
                    {
                        var m = r.Match(lineText);
                        var ret = int.TryParse(m.Groups[1].Value, out var n);
                        if (ret)
                        {
                            if (n > level)
                            {
                                text += RTF.Crlf();
                            }

                            level = n;
                        }
                    }

                    var color = PData.Sections[i].Content[j].tColor switch
                    {
                        _ when lineText.Contains("[Empty]") => RTF.Color(RTF.ElementID.Faded),
                        _ when PData.Sections[i].Content[j].tColor == Color.FromArgb(0, 255, 128) => RTF.Color(RTF.ElementID.Enhancement),
                        _ when PData.Sections[i].Content[j].tColor == Color.FromArgb(0, 255, 255) => RTF.Color(RTF.ElementID.Invention),
                        _ => ""
                    };
                    
                    if (!lineText.Contains(' '))
                    {
                        // Bug: DSync, HO, Titan and Hydra names aren't formatted proper
                        // Fetch long name from short name (if available in db)
                        //
                        // Bug: missing level on these
                        var enh = DatabaseAPI.Database.Enhancements
                            .DefaultIfEmpty(new Enhancement())
                            .FirstOrDefault(e => e.ShortName == lineText);

                        lineText = enh == null || string.IsNullOrEmpty(enh.LongName)
                            ? lineText
                            : enh.LongName;
                    }

                    text += $"{color}{new string(' ', PData.Sections[i].Content[j].tIndent * indentMultiplier)}{lineText}{(color != "" ? RTF.Color(RTF.ElementID.Text) : "")}";
                }
            }

            text += RTF.EndRTF();

            richTextBox1.Rtf = text;
        }
    }
}