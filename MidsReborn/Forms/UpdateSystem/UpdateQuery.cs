using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public partial class UpdateQuery : Form
    {
        private List<string> Updates { get; set; } = new();
        public UpdateQuery(frmMain parent, List<UpdateUtils.UpdateObject> updates)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            foreach (var update in updates)
            {
                switch (update.Type)
                {
                    case UpdateSystem.UpdateUtils.UpdateTypes.Application:
                        Updates.Add($"* {update.Name} v{update.Version}");
                        break;
                    case UpdateSystem.UpdateUtils.UpdateTypes.Database:
                        Updates.Add($"* {update.Name} Database ({update.Version})");
                        break;
                }
            }
            Load += OnQuery_Load;
            InitializeComponent();
        }

        private void OnQuery_Load(object? sender, EventArgs e)
        {
            var sb = new StringBuilder();
            foreach (var update in Updates)
            {
                sb.AppendLine(update);
            }
            updatesLabel.Text =  sb.ToString();
            CenterToParent();
        }
    }
}
