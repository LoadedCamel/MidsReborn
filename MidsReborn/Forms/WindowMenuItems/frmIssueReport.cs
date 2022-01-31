using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Master_Classes;
using Newtonsoft.Json;
using Octokit;
using RestSharp;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmIssueReport : Form
    {
        private readonly frmMain _myParent;

        private string _operatingSystem;
        private string _issueType;
        private string _issuerName;
        private string _issueTitle;
        private string _issueDescription;

        private List<string> _issueAssets;
        private Image _issueAsset;

        private string _selectedIssueType;
        private readonly List<string> _issueTypes = new() { "app-bug", "content-bug", "feature-request" };

        public frmIssueReport(ref frmMain iParent)
        {
            _myParent = iParent;
            Load += frmIssueReport_Load;
            InitializeComponent();
        }

        private void frmIssueReport_Load(object sender, EventArgs e)
        {
            iBSubmit.IA = _myParent.Drawing.pImageAttributes;
            iBSubmit.ImageOff = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[2].Bitmap
                : _myParent.Drawing.bxPower[4].Bitmap;
            iBSubmit.ImageOn = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[3].Bitmap
                : _myParent.Drawing.bxPower[5].Bitmap;
        }

        private async void iBSubmit_ButtonClicked()
        {
            if (!FieldsValidated(out var reason).Result)
            {
                MessageBox.Show(reason, @"Validation Error", MessageBoxButtons.OK);
                return;
            }

            var assetResult = MessageBox.Show(@"Attach screenshot(s)?", @"Add Attachment(s)?", MessageBoxButtons.YesNo);
            if (assetResult != DialogResult.No)
            {
                _issueAssets = new List<string>();
                using var assetFileDialog = new OpenFileDialog();
                assetFileDialog.Title = @"Select the screenshot(s) to attach";
                assetFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                assetFileDialog.RestoreDirectory = true;
                assetFileDialog.Filter = @"Image Files (*.bmp, *.gif, *.jpeg, *.jpg, *.png)|*.bmp; *.gif; *.jpeg; *.jpg; *.png";
                assetFileDialog.Multiselect = true;
                assetFileDialog.ShowDialog(this);
                foreach (var imageFile in assetFileDialog.FileNames)
                {
                    _issueAssets.Add(imageFile);
                }

                await CreateGitIssue(_issueAssets);
            }

            await CreateGitIssue();
        }

        private struct Data
        {
            public string link { get; set; }
        }

        private struct ImgurResponse
        {
            public Data data { get; set; }
        }

        private async Task CreateGitIssue(List<string> attachments = null)
        {
            var body = string.Empty;
            var sb = new StringBuilder(body);
            sb.Append($"-- This issue was submitted on behalf of {_issuerName} --\r\n\r\nOperating System: {_operatingSystem}\nApp Version: {MidsContext.AppAssemblyVersion}\nDatabase Version: {DatabaseAPI.Database.Version}\r\n");
            sb.AppendLine($"\r\n{_issueDescription}");
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    var binData = File.ReadAllBytes(attachment);
                    var base64 = Convert.ToBase64String(binData);
                    var imgClient = new RestClient("https://api.imgur.com/3/image")
                    {
                        Timeout = -1
                    };
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", "Client-ID 030824e99e8f5a8");
                    request.AlwaysMultipartFormData = true;
                    request.AddParameter("image", base64);
                    var response = await imgClient.ExecuteAsync(request);
                    var imgur = JsonConvert.DeserializeObject<ImgurResponse>(response.Content);
                    sb.AppendLine($"\r\n![]({imgur.data.link})");
                }
            }

            body = sb.ToString();
            var client = new GitHubClient(new ProductHeaderValue("MRB-IssueBot"))
            {
                Connection =
                {
                    Credentials = new Credentials("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")
                }
            };
            var repo = await client.Repository.Get("LoadedCamel", "MidsReborn");
            var issue = new NewIssue(_issueTitle)
            {
                 Body = body,
                 Labels = { _selectedIssueType }
            };
            var createIssue = await client.Issue.Create(repo.Id, issue);
            MessageBox.Show($"Your issue has been submitted.\r\nIssueID: {createIssue.Id}", @"Issue Submitted", MessageBoxButtons.OK);
        }

        private Task<bool> FieldsValidated(out string reason)
        {
            var completionSource = new TaskCompletionSource<bool>();
            if (string.IsNullOrEmpty(_operatingSystem) || _operatingSystem.Contains("--"))
            {
                reason = @"Valid operating system not selected.";
                completionSource.TrySetResult(false);
            }
            else if (string.IsNullOrEmpty(_issueType))
            {
                reason = @"Valid issue type not selected.";
                completionSource.TrySetResult(false);
            }
            else if (string.IsNullOrEmpty(_issuerName) || _issuerName.Contains(@"Discord Tag or Forum Name"))
            {
                reason = @"Issuer name is either empty or invalid.";
                completionSource.TrySetResult(false);
            }
            else if (string.IsNullOrEmpty(_issueTitle))
            {
                reason = @"Issue title cannot be empty.";
                completionSource.TrySetResult(false);
            }
            else if (string.IsNullOrEmpty(_issueDescription))
            {
                reason = @"Issue description cannot be empty.";
                completionSource.TrySetResult(false);
            }
            else
            {
                reason = @"";
                completionSource.TrySetResult(true);
            }

            return completionSource.Task;
        }

        private void cBOS_SelectedIndexChanged(object sender, EventArgs e)
        {
            _operatingSystem = cBOS.SelectedItem.ToString();
        }

        private void cBIssueType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _issueType = cBIssueType.SelectedItem.ToString();
            _selectedIssueType = _issueTypes[cBIssueType.SelectedIndex];
        }

        private void tbName_TextChanged(object sender, EventArgs e)
        {
            _issuerName = tBName.Text;
        }

        private void rTBIssue_TextChanged(object sender, EventArgs e)
        {
            _issueDescription = rTBIssue.Text;
        }

        private void tBIssueTitle_TextChanged(object sender, EventArgs e)
        {
            _issueTitle = tBIssueTitle.Text;
        }
    }
}
