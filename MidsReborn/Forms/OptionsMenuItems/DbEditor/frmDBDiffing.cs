using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using mrbBase;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmDBDiffing : Form
    {
        private readonly ISerialize _serializer;
        private readonly frmPowerBrowser _myParent;
        
        public frmDBDiffing(ISerialize serializer, frmPowerBrowser iParent)
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            _serializer = serializer;
            _myParent = iParent;
            Load += Busy_OnLoad;
        }

        private void SetMessage(string iMsg)
        {
            if (lblmessage.Text == iMsg) return;
            lblmessage.Text = iMsg;
            Refresh();
        }

        private void Busy_OnLoad(object sender, EventArgs e)
        {
            Visible = true;
            SetMessage("Saving and Diffing Main database...");
            UpdateProgress(0, "");
            var path = Files.SelectDataFileSave(Files.MxdbFileDB);
            //var asyncDbTask = new Task(() => DatabaseAPI.SaveMainDatabase(_serializer));
            //asyncDbTask.Start();
            //asyncDbTask.Wait();
            
            //var asyncDiffDb = new Task(() => DiffDatabase(_serializer, path, DatabaseAPI.MainDbName));
            //asyncDiffDb.Start();
            //asyncDiffDb.Wait();
        }

        private void UpdateProgress(int pbValue, string lblText)
        {
            progressBar1.Value = pbValue;
            label1.Text = lblText;
            Refresh();
        }

        private void UpdateProgress(decimal pbValue, string lblText)
        {
            progressBar1.Value = (int) Math.Round(pbValue * 100);
            label1.Text = lblText;
            Refresh();
        }

        private void DiffDatabase(ISerialize serializer, string fn, string name)
        {
            SetMessage("Saving and Diffing Main database...\r\n[ Metadata ]");

            var powersetPowers = DatabaseAPI.Database.Powersets.SelectMany(x => x.Powers).Select(p => p.PowerIndex).Distinct().ToList();
            // only powers that aren't in a powerset
            var powers = DatabaseAPI.Database.Power.Where(p => powersetPowers.Contains(p.PowerIndex) == false).ToList();
            var toSerialize = new
            {
                name,
                DatabaseAPI.Database.Version,
                DatabaseAPI.Database.Date,
                DatabaseAPI.Database.Issue,
                DatabaseAPI.Database.ArchetypeVersion,
                Archetypes = DatabaseAPI.Database.Classes,
                DatabaseAPI.Database.PowersetVersion,
                // out of memory exception
                //DatabaseAPI.Database.Powersets,
                Powers = new
                {
                    DatabaseAPI.Database.PowerVersion,
                    DatabaseAPI.Database.PowerLevelVersion,
                    DatabaseAPI.Database.PowerEffectVersion,
                    DatabaseAPI.Database.IOAssignmentVersion,
                    // out of memory exception
                    //DatabaseAPI.Database.Power
                    // just powers not in power sets
                    Powers = powers
                },
                DatabaseAPI.Database.Entities
            };
            ConfigData.SaveRawMhd(serializer, toSerialize, fn, null);
            var archPowersets = DatabaseAPI.Database.Powersets; // .Where(ps => ps.nArchetype >= 0);
            var dbPath = Path.Combine(Path.GetDirectoryName(fn), "db");
            var playerPath = Path.Combine(dbPath, "Player");
            var otherPath = Path.Combine(dbPath, "Other");
            var toWrite = new List<FHash>();
            foreach (var path in new[] { dbPath, playerPath, otherPath }.Where(p => !Directory.Exists(p)))
            {
                Directory.CreateDirectory(path);
            }
            var metadataPath = Path.Combine(Path.GetDirectoryName(fn), "db_metadata" + Path.GetExtension(fn));
            var (hasPrevious, prev) = ConfigData.LoadRawMhd<FHash[]>(serializer, metadataPath);
            var nbArch = Math.Max(1, archPowersets.Length);
            var nProgress = 0;
            SetMessage("Saving and Diffing Main database...\r\n[ Powersets ]");
            UpdateProgress(0, $"0% (0/{nbArch})");

            foreach (var ps in archPowersets)
            {
                // Variables not used
                //var at = DatabaseAPI.Database.Classes.FirstOrDefault(cl => ps.nArchetype != -1 && cl.Idx == ps.nArchetype);
                //var at2 = DatabaseAPI.Database.Classes.Length > ps.nArchetype && ps.nArchetype != -1 ? DatabaseAPI.Database.Classes[ps.nArchetype] : null;
                if (ps.FullName?.Length == 0 || ps.FullName?.Length > 100)
                {
                    continue;
                }

                if (ps.FullName?.Contains(";") == true || string.IsNullOrWhiteSpace(ps.FullName))
                {
                    Debug.WriteLine($"hmmm: {ps.DisplayName}");
                }

                var psFn = Path.Combine(ps.nArchetype >= 0 ? playerPath : otherPath, ps.ATClass + "_" + ps.FullName + Path.GetExtension(fn));
                if (psFn.Length > 240)
                {
                    continue;
                }

                var psPrevious = hasPrevious ? prev.FirstOrDefault(psm => psm.Fullname == ps.FullName && psm.Archetype == ps.ATClass) : null;
                var lastSaveResult = hasPrevious && psPrevious != null ? new RawSaveResult(hash: psPrevious.Hash, length: psPrevious.Length) : null;
                var saveResult = ConfigData.SaveRawMhd(serializer, ps, psFn, lastSaveResult);
                if (saveResult == null)
                {
                    Close();

                    return;
                }

                toWrite.Add(new FHash(
                    fullname: ps.FullName,
                    archetype: ps.ATClass,
                    hash: saveResult.Hash,
                    length: saveResult.Length
                ));

                var percentProgress = ++nProgress / (decimal) nbArch;
                if (nProgress % 50 != 0) continue;

                UpdateProgress(percentProgress, $"{percentProgress:P1} ({nProgress}/{nbArch})");
            }

            ConfigData.SaveRawMhd(serializer, toWrite, metadataPath, null);
            UpdateProgress(100, "");
            Close();
        }
    }
}