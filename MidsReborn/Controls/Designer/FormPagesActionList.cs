using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using MetaControls.Converters;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using Mids_Reborn.Controls;

namespace MetaControls.Designer
{
    internal partial class FormPagesDesigner
    {
        internal class FormPagesActionList : DesignerActionList
        {
            private const string Current = "Current Page";
            private const string Config = "Configuration";
            private readonly DesignerActionUIService _actionUiService;
            private readonly FormPages _formPages;

            private readonly ComponentDesigner _designer;

            public FormPagesActionList(ComponentDesigner designer) : base(designer.Component)
            {
                _designer = designer;
                _actionUiService = (DesignerActionUIService)GetService(typeof(DesignerActionUIService))!;
                _formPages = (FormPages)Component!;
            }

            public BindingList<Page> PageCollection
            {
                get => _formPages.Pages;
                set
                {
                    TypeDescriptor.GetProperties(Component!)[nameof(PageCollection)]!.SetValue(Component, value);
                    Refresh();
                }
            }

            // New DropDown property for selecting the page index
            [TypeConverter(typeof(PageIndexConverter))]
            public int SelectedPageIndexDropDown
            {
                get => _formPages.SelectedIndex;
                set
                {
                    _formPages.SelectedIndex = value;
                    Refresh();
                }
            }

            public string? SelectedPageName
            {
                get => _formPages.SelectedPage is null ? "Null" : _formPages.SelectedPage.Name;
                set
                {
                    if (_formPages.SelectedPage is null) return;
                    _formPages.SelectedPage.Name = value;
                    RefreshPage(_formPages.SelectedPage);
                }
            }

            public string? SelectedPageTitle
            {
                get => _formPages.SelectedPage is null ? "Null" : _formPages.SelectedPage.Title;
                set
                {
                    if (_formPages.SelectedPage is null) return;
                    _formPages.SelectedPage.Title = value;
                    RefreshPage(_formPages.SelectedPage);
                }
            }

            public Color SelectedPageBackColor
            {
                get => _formPages.SelectedPage?.BackColor ?? Color.Empty;
                set
                {
                    if (_formPages.SelectedPage is null) return;
                    _formPages.SelectedPage.BackColor = value;
                    RefreshPage(_formPages.SelectedPage);
                }
            }

            public Color SelectedPageForeColor
            {
                get => _formPages.SelectedPage?.ForeColor ?? Color.Empty;
                set
                {
                    if (_formPages.SelectedPage is null) return;
                    _formPages.SelectedPage.ForeColor = value;
                    RefreshPage(_formPages.SelectedPage);
                }
            }

            private string CollectionCountText
            {
                get
                {
                    return PageCollection.Count switch
                    {
                        0 => $"There are {PageCollection.Count} pages in the collection",
                        > 1 => $"There are {PageCollection.Count} pages in the collection",
                        _ => $"There is {PageCollection.Count} page in the collection"
                    };
                }
            }

            private void RefreshPage(Control page)
            {
                page.Invalidate();
            }

            internal void Refresh()
            {
                _actionUiService.Refresh(_formPages);
            }

            public override DesignerActionItemCollection GetSortedActionItems()
            {
                var items = new DesignerActionItemCollection
                {
                    new DesignerActionHeaderItem(Config),
                    new DesignerActionHeaderItem(Current),
                    new DesignerActionPropertyItem(nameof(PageCollection), "Pages", Config, "The collection of pages"),
                    new DesignerActionPropertyItem(nameof(SelectedPageIndexDropDown), "Selected Page Index", Config, "Select the page index from the dropdown"),
                    new DesignerActionTextItem(CollectionCountText, Config),
                    new DesignerActionPropertyItem(nameof(SelectedPageName), "Name", Current, "The page name"),
                    new DesignerActionPropertyItem(nameof(SelectedPageTitle), "Title", Current, "The page title"),
                    new DesignerActionPropertyItem(nameof(SelectedPageBackColor), "BackColor", Current, "The page back color"),
                    new DesignerActionPropertyItem(nameof(SelectedPageForeColor), "ForeColor", Current, "The page fore color")
                };
                return items;
            }

            private void InvokePropertyEditor(string propertyName)
            {
                if (Component == null) return;
                var property = TypeDescriptor.GetProperties(Component)[propertyName];
                var typeDescriptor = new TypeDescriptorContext(Component, property);

                var editor = (UITypeEditor)property?.GetEditor(typeof(UITypeEditor))!;
                var value = property?.GetValue(Component);
                var newValue = editor.EditValue(typeDescriptor, value);

                if (!Equals(newValue, value))
                {
                    property?.SetValue(Component, newValue);
                }
            }
        }
    }
}
