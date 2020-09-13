using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Base.Master_Classes;

namespace midsControls
{
    public partial class ctlTableList : UserControl
    {

        public ItemPair[] _items;
        public Color ValueColorD;
        private int CurrentHighlight;
        private bool Highlightable;

        public bool ForceBold { get; set; }
        public Color HighlightColor { get; set; }
        public Color HighlightTextColor { get; set; }
        public Color NameColor { get; set; }
        public Color ItemColor { get; set; }
        public Color ItemColorAlt { get; set; }
        public Color ItemColorSpecial { get; set; }
        public Color ItemColorSpecCase { get; set; }
        public int ItemCount => _items.Length;

        public void SetText()
        {
            var config = MidsContext.Config;
            var fontStyle = ForceBold ? FontStyle.Bold : FontStyle.Regular;
            var font = new Font("Arial", config.RtFont.PairedBase, fontStyle, GraphicsUnit.Point);
            var backColor = BackColor;
            var textColor = ForeColor;

            if (_items != null)
            {
                for (var i = 0; i <= ItemCount - 1; i++)
                {
                    if (Highlightable & (CurrentHighlight == i))
                    {
                        backColor = HighlightColor;
                        textColor = HighlightTextColor;
                    }
                    else
                    {
                        backColor = NameColor;
                    }

                    var text = _items[i].Name;

                }
            }

            for (var index = 0; index < tableLayoutPanel1.Controls.Count; index++)
            {
               
            }
        }

        public ctlTableList(ItemPair[] items)
        {
            Load += ctlTableList_Load;
            ValueColorD = Color.Aqua;
            InitializeComponent();
        }

        private void ctlTableList_Load(object sender, EventArgs e)
        {
            CurrentHighlight = -1;
            _items = new ItemPair[0];
            AddItem(new ItemPair("Item 1:", "Value", false));
            AddItem(new ItemPair("Item 2:", "Alternate", true));
            AddItem(new ItemPair("Item 3:", "1000", false));
            AddItem(new ItemPair("Item 4:", "1,000", false));
            AddItem(new ItemPair("1234567890:", "12345678901234567890", false));
            AddItem(new ItemPair("1234567890:", "12345678901234567890", true));
            AddItem(new ItemPair("1 2 3 4 5 6 7 8 9 0:", "1 2 3 4 5 6 7 8 9 0", false));
            AddItem(new ItemPair("1 2 3 4 5 6 7 8 9 0:", "1 2 3 4 5 6 7 8 9 0", true));
        }

        public void AddItem(ItemPair iItem)
        {
            checked
            {
                Array.Copy(_items, new ItemPair[_items.Length + 1], _items.Length + 1);
                _items[_items.Length - 1] = new ItemPair(iItem);
            }
        }

        public void SetUnique()
        {
            if (_items.Length > 0) _items[checked(_items.Length - 1)].UniqueColor = true;
        }

        public bool IsSpecialColor()
        {
            return _items[_items.Length - 1].VerySpecialColor;
        }

    }

    public class ItemPair
    {
        public readonly bool AlternateColor;
        public readonly bool ProbColor;
        private readonly string SpecialTip;
        public readonly bool VerySpecialColor;
        public string Name;
        public Enums.ShortFX TagID;
        public bool UniqueColor;
        public string Value;

        public ItemPair(string iName, string iValue, bool iAlternate, bool iProbability, bool iSpecialCase, string iTip)
        {
            Name = iName;
            Value = iValue;
            AlternateColor = iAlternate;
            ProbColor = iProbability;
            VerySpecialColor = iSpecialCase;
            TagID.Add(-1, 0f);
            SpecialTip = iTip;
        }

        public ItemPair(string iName, string iValue, bool iAlternate, bool iProbability = false, bool iSpecialCase = false, int iTagID = -1)
        {
            Name = iName;
            Value = iValue;
            AlternateColor = iAlternate;
            ProbColor = iProbability;
            VerySpecialColor = iSpecialCase;
            TagID.Add(iTagID, 0f);
            SpecialTip = "";
        }

        public ItemPair(string iName, string iValue, bool iAlternate, bool iProbability, bool iSpecialCase, Enums.ShortFX iTagID)
        {
            Name = iName;
            Value = iValue;
            AlternateColor = iAlternate;
            ProbColor = iProbability;
            VerySpecialColor = iSpecialCase;
            TagID.Assign(iTagID);
            SpecialTip = "";
        }

        public ItemPair(ItemPair iItem)
        {
            Name = iItem.Name;
            Value = iItem.Value;
            AlternateColor = iItem.AlternateColor;
            ProbColor = iItem.ProbColor;
            VerySpecialColor = iItem.VerySpecialColor;
            TagID.Assign(iItem.TagID);
            SpecialTip = iItem.SpecialTip;
        }
    }
}
