using System.Windows.Forms;

namespace CrozzleApplication
{
    public partial class ErrorsViewer : Form
    {
        #region properties
        public WebBrowser WebBrowser
        {
            get
            {
                return (webBrowser1);
            }
        }
        #endregion

        #region constructors
        public ErrorsViewer()
        {
            InitializeComponent();
        }
        #endregion

        #region form hiding, instead of closing
        private void ErrorList_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }        
        #endregion
    }
}
