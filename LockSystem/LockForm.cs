using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LockSystem
{
    public partial class LockForm : Form
    {
        public LockForm()
        {
            InitializeComponent();
        }

        private void LockForm_Load(object sender, EventArgs e)
        {   
            lblMessage.Text = "Starting in 10 sec";
            Thread.Sleep(8000);
            lblMessage.Text = "Starting in 2 sec";
            Thread.Sleep(2000);

            GlobalHook hook = new GlobalHook();
            hook.AddKeyBoardHook();
            hook.AddMouseHook();
            
            //Application.OpenForms[0].Hide();
        }
    }
}
