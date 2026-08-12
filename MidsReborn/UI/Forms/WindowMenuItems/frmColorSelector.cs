namespace Mids_Reborn.UI.Forms.WindowMenuItems;

public partial class frmColorSelector : Form
{
    public Color SelectedColor { get; private set; }
    public Color InitialColor { get; private set; }
    public string SourceControl {  get; private set; }

    public frmColorSelector(Color initialColor, string sourceControl)
    {
        InitializeComponent();
        InitialColor = initialColor;
        SelectedColor = initialColor;
        SourceControl = sourceControl;
        DialogResult = DialogResult.None;
    }

    private void frmColorSelector_Load(object sender, EventArgs e)
    {
        UpdateControls();
    }

    private void UpdateControls()
    {
        pnlColorPreview.BackColor = SelectedColor;
        trkR.Value = SelectedColor.R;
        trkG.Value = SelectedColor.G;
        trkB.Value = SelectedColor.B;
        nudR.Value = SelectedColor.R;
        nudG.Value = SelectedColor.G;
        nudB.Value = SelectedColor.B;
    }

    public void InitAndShow(Color color, string sourceControl)
    {
        DialogResult = DialogResult.None;
        InitialColor = color;
        SelectedColor = color;
        SourceControl = sourceControl;
        UpdateControls();
        Visible = true;
        Show();
        BringToFront();
        Activate();
    }

    private void nudR_ValueChanged(object sender, EventArgs e)
    {
        trkR.Value = (int)nudR.Value;
    }

    private void nudG_ValueChanged(object sender, EventArgs e)
    {
        trkG.Value = (int)nudG.Value;
    }

    private void nudB_ValueChanged(object sender, EventArgs e)
    {
        trkB.Value = (int)nudB.Value;
    }

    private void trkR_Scroll(object sender, EventArgs e)
    {
        nudR.Value = trkR.Value;
        pnlColorPreview.BackColor = Color.FromArgb((int)nudR.Value, (int)nudG.Value, (int)nudB.Value);
    }

    private void trkG_Scroll(object sender, EventArgs e)
    {
        nudG.Value = trkG.Value;
        pnlColorPreview.BackColor = Color.FromArgb((int)nudR.Value, (int)nudG.Value, (int)nudB.Value);
    }

    private void trkB_Scroll(object sender, EventArgs e)
    {
        nudB.Value = trkB.Value;
        pnlColorPreview.BackColor = Color.FromArgb((int)nudR.Value, (int)nudG.Value, (int)nudB.Value);
    }

    private void colorWheel1_SelectionChanged(object sender, Controls.ColorWheel.SelectedValues selected)
    {
        SelectedColor = selected.Color;
        UpdateControls();
    }

    private void btnResetColor_Click(object sender, EventArgs e)
    {
        SelectedColor = Color.FromArgb(InitialColor.R, InitialColor.G, InitialColor.B);
        UpdateControls();
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Hide();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Hide();
    }
}
