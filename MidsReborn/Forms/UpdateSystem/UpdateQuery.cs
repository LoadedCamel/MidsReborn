using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public partial class UpdateQuery : Form
    {
        private List<string> Updates { get; set; } = new();
        public UpdateQuery(List<UpdateObject> updates)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            foreach (var update in updates)
            {
                switch (update.Type)
                {
                    case PatchType.Application:
                        Updates.Add($"* {update.Name} v{update.Version}");
                        break;
                    case PatchType.Database:
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
