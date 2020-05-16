using Base.Data_Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hero_Designer.Forms.JsonImport
{
    public partial class frmJsonImportMain : Form
    {
        public frmJsonImportMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog1.FileName;
                parseAttriModFile(FileName);
            }
        }

        private void parseAttriModFile(string fileName)
        {
            JsonSerializer serializer = new JsonSerializer();
            string json = File.ReadAllText(fileName);
            List<Class> classes = JsonConvert.DeserializeObject<List<Class>>(json);

            int classId = -1;
            foreach (Class c in classes)
            {
                classId = DatabaseAPI.Database.Classes.FirstOrDefault(cls => string.Equals(c.Name, cls.ClassName, StringComparison.OrdinalIgnoreCase)).Column;
                for (int i = 0; i < c.ModTables.Select(s => s.Name).Count() - 1; i++)
                {
                    for (int k = 0; k < c.ModTables[i].Values.Count - 1; k++)
                    {
                        int tableId = DatabaseAPI.NidFromUidAttribMod(c.ModTables[i].Name);
                        if (tableId > -1)
                            DatabaseAPI.Database.AttribMods.Modifier[tableId].Table[k][classId] = c.ModTables[i].Values[k];
                    }
                }
            }
            DatabaseAPI.Database.AttribMods.Store(My.MyApplication.GetSerializer());
            DatabaseAPI.SaveJSONDatabase(My.MyApplication.GetSerializer());
            MessageBox.Show("Import completed");
        }
    }

    class Class
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mod_table")]
        public List<ModTable> ModTables { get; set; }
    }

    class ModTable
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("values")]
        public List<float> Values { get; set; }
    }

    class AttribArray
    {
        [JsonProperty("damage_type00")]
        public float DamageType00 { get; set; }

        [JsonProperty("damage_type01")]
        public float DamageType01 { get; set; }

        [JsonProperty("damage_type02")]
        public float DamageType02 { get; set; }

        [JsonProperty("damage_type03")]
        public float DamageType03 { get; set; }

        [JsonProperty("damage_type04")]
        public float DamageType04 { get; set; }

        [JsonProperty("damage_type05")]
        public float DamageType05 { get; set; }

        [JsonProperty("damage_type06")]
        public float DamageType06 { get; set; }

        [JsonProperty("damage_type07")]
        public float DamageType07 { get; set; }

        [JsonProperty("damage_type08")]
        public float DamageType08 { get; set; }

        [JsonProperty("damage_type09")]
        public float DamageType09 { get; set; }

        [JsonProperty("damage_type10")]
        public float DamageType10 { get; set; }

        [JsonProperty("damage_type11")]
        public float DamageType11 { get; set; }

        [JsonProperty("damage_type12")]
        public float DamageType12 { get; set; }

        [JsonProperty("damage_type13")]
        public float DamageType13 { get; set; }

        [JsonProperty("damage_type14")]
        public float DamageType14 { get; set; }

        [JsonProperty("damage_type15")]
        public float DamageType15 { get; set; }

        [JsonProperty("damage_type16")]
        public float DamageType16 { get; set; }

        [JsonProperty("damage_type17")]
        public float DamageType17 { get; set; }

        [JsonProperty("damage_type18")]
        public float DamageType18 { get; set; }

        [JsonProperty("damage_type19")]
        public float DamageType19 { get; set; }

        [JsonProperty("hit_points")]
        public float HitPoints { get; set; }

        [JsonProperty("absorb")]
        public float Absorb { get; set; }

        [JsonProperty("endurance")]
        public float Endurance { get; set; }

        [JsonProperty("insight")]
        public float Insight { get; set; }

        [JsonProperty("rage")]
        public float Rage { get; set; }

        [JsonProperty("to_hit")]
        public float ToHit { get; set; }

        [JsonProperty("defense_type00")]
        public float DefenseType00 { get; set; }

        [JsonProperty("defense_type01")]
        public float DefenseType01 { get; set; }

        [JsonProperty("defense_type02")]
        public float DefenseType02 { get; set; }

        [JsonProperty("defense_type03")]
        public float DefenseType03 { get; set; }

        [JsonProperty("defense_type04")]
        public float DefenseType04 { get; set; }

        [JsonProperty("defense_type05")]
        public float DefenseType05 { get; set; }

        [JsonProperty("defense_type06")]
        public float DefenseType06 { get; set; }

        [JsonProperty("defense_type07")]
        public float DefenseType07 { get; set; }

        [JsonProperty("defense_type08")]
        public float DefenseType08 { get; set; }

        [JsonProperty("defense_type09")]
        public float DefenseType09 { get; set; }

        [JsonProperty("defense_type10")]
        public float DefenseType10 { get; set; }

        [JsonProperty("defense_type11")]
        public float DefenseType11 { get; set; }

        [JsonProperty("defense_type12")]
        public float DefenseType12 { get; set; }

        [JsonProperty("defense_type13")]
        public float DefenseType13 { get; set; }

        [JsonProperty("defense_type14")]
        public float DefenseType14 { get; set; }

        [JsonProperty("defense_type15")]
        public float DefenseType15 { get; set; }

        [JsonProperty("defense_type16")]
        public float DefenseType16 { get; set; }

        [JsonProperty("defense_type17")]
        public float DefenseType17 { get; set; }

        [JsonProperty("defense_type18")]
        public float DefenseType18 { get; set; }

        [JsonProperty("defense_type19")]
        public float DefenseType19 { get; set; }

        [JsonProperty("defense")]
        public float Defense { get; set; }

        [JsonProperty("speed_running")]
        public float SpeedRunning { get; set; }

        [JsonProperty("speed_flying")]
        public float SpeedFlying { get; set; }

        [JsonProperty("speed_swimming")]
        public float SpeedSwimming { get; set; }

        [JsonProperty("speed_jumping")]
        public float SpeedJumping { get; set; }

        [JsonProperty("JumpHeight")]
        public float JumpHeight { get; set; }

        [JsonProperty("movement_control")]
        public float MovementControl { get; set; }

        [JsonProperty("movement_friction")]
        public float MovementFriction { get; set; }

        [JsonProperty("stealth")]
        public float Stealth { get; set; }

        [JsonProperty("stealth_radius")]
        public float StealthRadius { get; set; }

        [JsonProperty("stealth_radius_player")]
        public float StealthRadiusPlayer { get; set; }

        [JsonProperty("regeneration")]
        public float Regeneration { get; set; }

        [JsonProperty("recovery")]
        public float Recovery { get; set; }

        [JsonProperty("insight_recovery")]
        public float InsightRecovery { get; set; }

        [JsonProperty("threat_level")]
        public float ThreatLevel { get; set; }

        [JsonProperty("taunt")]
        public float Taunt { get; set; }

        [JsonProperty("placate")]
        public float Placate { get; set; }

        [JsonProperty("confused")]
        public float Confused { get; set; }

        [JsonProperty("afraid")]
        public float Afraid { get; set; }

        [JsonProperty("terrorized")]
        public float Terrorized { get; set; }

        [JsonProperty("held")]
        public float Held { get; set; }

        [JsonProperty("immobilized")]
        public float Immobilized { get; set; }

        [JsonProperty("stunned")]
        public float Stunned { get; set; }

        [JsonProperty("sleep")]
        public float Sleep { get; set; }

        [JsonProperty("fly")]
        public float Fly { get; set; }

        [JsonProperty("jumppack")]
        public float JumpPack { get; set; }

        [JsonProperty("teleport")]
        public float Teleport { get; set; }

        [JsonProperty("untouchable")]
        public float Untouchable { get; set; }

        [JsonProperty("intangible")]
        public float Intangible { get; set; }

        [JsonProperty("only_affects_self")]
        public float OnlyAffectsSelf { get; set; }

        [JsonProperty("experience_gain")]
        public float ExperienceGain { get; set; }

        [JsonProperty("influence_gain")]
        public float InfluenceGain { get; set; }

        [JsonProperty("prestige_gain")]
        public float PrestigeGain { get; set; }

        [JsonProperty("null_bool")]
        public float NullBool { get; set; }

        [JsonProperty("knockup")]
        public float Knockup { get; set; }

        [JsonProperty("knockback")]
        public float Knockback { get; set; }

        [JsonProperty("repel")]
        public float Repel { get; set; }

        [JsonProperty("accuracy")]
        public float Accuracy { get; set; }

        [JsonProperty("radius")]
        public float Radius { get; set; }

        [JsonProperty("arc")]
        public float Arc { get; set; }

        [JsonProperty("range")]
        public float Range { get; set; }

        [JsonProperty("time_to_recharge")]
        public float TimeToRecharge { get; set; }

        [JsonProperty("recharge_time")]
        public float RechargeTime { get; set; }

        [JsonProperty("interrupt_time")]
        public float InterruptTime { get; set; }

        [JsonProperty("endurance_discount")]
        public float EnduranceDiscount { get; set; }

        [JsonProperty("insight_discount")]
        public float InsightDiscount { get; set; }

        [JsonProperty("meter")]
        public float Meter { get; set; }

        [JsonProperty("elusivity00")]
        public float Elusivity00 { get; set; }

        [JsonProperty("elusivity01")]
        public float Elusivity01 { get; set; }

        [JsonProperty("elusivity02")]
        public float Elusivity02 { get; set; }

        [JsonProperty("elusivity03")]
        public float Elusivity03 { get; set; }

        [JsonProperty("elusivity04")]
        public float Elusivity04 { get; set; }

        [JsonProperty("elusivity05")]
        public float Elusivity05 { get; set; }

        [JsonProperty("elusivity06")]
        public float Elusivity06 { get; set; }

        [JsonProperty("elusivity07")]
        public float Elusivity07 { get; set; }

        [JsonProperty("elusivity08")]
        public float Elusivity08 { get; set; }

        [JsonProperty("elusivity09")]
        public float Elusivity09 { get; set; }

        [JsonProperty("elusivity10")]
        public float Elusivity10 { get; set; }

        [JsonProperty("elusivity11")]
        public float Elusivity11 { get; set; }

        [JsonProperty("elusivity12")]
        public float Elusivity12 { get; set; }

        [JsonProperty("elusivity13")]
        public float Elusivity13 { get; set; }

        [JsonProperty("elusivity14")]
        public float Elusivity14 { get; set; }

        [JsonProperty("elusivity15")]
        public float Elusivity15 { get; set; }

        [JsonProperty("elusivity16")]
        public float Elusivity16 { get; set; }

        [JsonProperty("elusivity17")]
        public float Elusivity17 { get; set; }

        [JsonProperty("elusivity18")]
        public float Elusivity18 { get; set; }

        [JsonProperty("elusivity19")]
        public float Elusivity19 { get; set; }

        [JsonProperty("elusivity_base")]
        public float ElusivityBase { get; set; }
    }

}
