using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;

namespace Browser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var q = HttpUtility.ParseQueryString(e.Url.OriginalString.Split(new string[] { "#" }, StringSplitOptions.None)[1]);
            MessageBox.Show(e.Url.OriginalString.Split(new string[] { "#" }, StringSplitOptions.None)[1]);
            MessageBox.Show(q["access_token"]);
            
        }
    }
}
