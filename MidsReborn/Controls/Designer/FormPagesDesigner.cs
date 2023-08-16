using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;

namespace Mids_Reborn.Controls.Designer
{
    internal partial class FormPagesDesigner : ParentControlDesigner
    {
        private FormPages? FormPages => Control as FormPages;

        public override SelectionRules SelectionRules => SelectionRules.Moveable |
                                                         SelectionRules.LeftSizeable |
                                                         SelectionRules.RightSizeable |
                                                         SelectionRules.TopSizeable |
                                                         SelectionRules.BottomSizeable;



        public override void InitializeNewComponent(IDictionary? defaultValues)
        {
            base.InitializeNewComponent(defaultValues);
            if (Component is FormPages formPages)
            {
                try
                {
                    var host = GetService(typeof(IDesignerHost)) as IDesignerHost;
                    var componentChangeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                    using var transaction = Host.CreateTransaction("AddDefaultPage");
                    var page = (Page)host.CreateComponent(typeof(Page));
                    page.Size = formPages.ClientRectangle.Size;
                    page.Dock = DockStyle.Fill;
                    page.Title = "My First Page";
                    componentChangeService?.OnComponentChanging(Component, null);
                    if (formPages.Pages.Count == 0)
                    {
                        formPages.Pages.Add(page);
                        formPages.Controls.Add(page);
                        // Set the newly added page as the SelectedPage
                        formPages.SelectedPage = page;
                    }
                    componentChangeService?.OnComponentChanged(formPages, null, null, null);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Log or handle the exception
                    throw new Exception("Error while committing transaction: " + ex.Message);
                }
            }
        }

        private DesignerActionListCollection? _actionLists;

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection
                    {
                        new FormPagesActionList(this)
                    };
                }
                return _actionLists;
            }
        }
    }
}
