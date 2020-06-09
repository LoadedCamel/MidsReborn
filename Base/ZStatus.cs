
using System.Windows.Forms;

public partial class ZStatus : Form
{
    private Label lblStatus2;
    private Label lblStatus1;
    private Label lblTitle;

    public string StatusText1
    {
        set
        {
            if (value == lblStatus1.Text)
                return;
            lblStatus1.Text = value;
            lblStatus1.Refresh();
        }
    }

    public string StatusText2
    {
        set
        {
            if (value == lblStatus2.Text)
                return;
            lblStatus2.Text = value;
            lblStatus2.Refresh();
        }
    }

    public ZStatus()
    {
        InitializeComponent();
    }
}