using System;
using System.Windows.Forms;

namespace KillTracker
{

    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        internal void SetKills(int kills)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetKills(kills)));
                return;
            }

            killCounter.Text = kills.ToString();
        }
    }
}
