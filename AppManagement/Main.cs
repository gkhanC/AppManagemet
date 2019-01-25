using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppManagement
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        HelperProcess helperProcess = new HelperProcess();

        public string selectedPrograms;

        private void Main_Load(object sender, EventArgs e)
        {
            helperProcess.loadXml();
            helperProcess.listDatagridview("AppManagement.xml", dataGridViewProcessApp);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.refreshGrid();
        }

        private void btnKill_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in dataGridViewProcessApp.SelectedRows)
            {
                selectedPrograms = item.Cells[2].Value.ToString();
                helperProcess.killProcess(selectedPrograms);
            }
            this.refreshGrid();
        }

        void refreshGrid()
        {
            helperProcess.loadXml();
            helperProcess.listDatagridview("AppManagement.xml", dataGridViewProcessApp);
        }

        private void cikisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.refreshGrid();
        }

        private void hakkındaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Çalışan uygulamaları yönetme programı v1 \nGeliştirici : Semih Çelikol \nsemihcelikol.com","Hakkında",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void btnAllItemKill_Click(object sender, EventArgs e)
        {
            DialogResult dg = new DialogResult();

            dg = MessageBox.Show("Açık tüm uygulamalar kapanacak, emin misiniz ?", "Dikkat", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dg == DialogResult.Yes)
            {
                helperProcess.killProcessAll();
                MessageBox.Show("Tüm uygulamalar kapatıldı.", "Açıklama",MessageBoxButtons.OK,MessageBoxIcon.Information);
                this.refreshGrid();
            }

        }
    }
}
