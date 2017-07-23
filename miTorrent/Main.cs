using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
namespace miTorrent
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        

        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            

            Torrent t = new Torrent();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
