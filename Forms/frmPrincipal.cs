using System;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace Compactor.Screens
{
    public partial class frmPrincipal : Form
    {
        #region ..:: Variaveis ::..

        private FileInfo[] arquivo;
        private string fileName;

        #endregion

        #region ..:: Construtor ::..
        public frmPrincipal()
        {
            InitializeComponent();
        }
        #endregion

        #region ..:: Eventos ::.. 

        private void btnSair_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscaPasta();
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            LimparCampos();
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            if (txtCaminho.Text.Trim().Length < 0)
                return;

            if (MessageBox.Show("Deseja realmente compactar os arquivos deste diretório?", "AVISO!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Cursor.Current = Cursors.WaitCursor;
                DesativaComponentes(labelProcessando: true, iniciar: false, formEnable: false);

                ZipaPasta(txtCaminho.Text, $"{txtCaminho.Text}.zip");

                MessageBox.Show("Pasta compactada", "AVISO!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Cursor.Current = Cursors.Default;
                AtivaComponentes(labelProcessando: false, iniciar: true, formEnable: true);
                LimparCampos();
            }
        }

        #endregion

        #region ..:: Métodos ::..
        private void BuscaPasta()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() != DialogResult.OK)
                return;

            txtCaminho.Text = folderBrowser.SelectedPath;

            ltbArquivos.Items.Clear();
            DirectoryInfo dir = new DirectoryInfo(txtCaminho.Text.Trim());
            arquivo = dir.GetFiles("*", SearchOption.TopDirectoryOnly);
            foreach (FileInfo item in arquivo)
            {
                fileName = item.FullName.Replace(dir.FullName, "").Remove(0, 1);
                ltbArquivos.Items.Add(fileName);
                lblQtd.Text = "Total: " + ltbArquivos.Items.Count;
            }

            btnIniciar.Enabled = ltbArquivos.Items.Count > 0;
        }

        private void DesativaComponentes(bool labelProcessando, bool iniciar, bool formEnable)
        {
            Invoke(new Action(() =>
            {
                lblProcessando.Visible = labelProcessando;
                btnIniciar.Enabled = iniciar;
                this.Enabled = formEnable;
            }));
        }

        private void AtivaComponentes(bool labelProcessando, bool iniciar, bool formEnable)
        {
            Invoke(new Action(() =>
            {
                lblProcessando.Visible = labelProcessando;
                btnIniciar.Enabled = iniciar;
                this.Enabled = formEnable;
            }));
        }

        private void ZipaPasta(string caminhoPasta, string destinationPasta)
        {
            using (FileStream zip = new FileStream(destinationPasta, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zip, ZipArchiveMode.Create))
                {
                    foreach (string filePath in Directory.GetFiles(caminhoPasta, "*.*", SearchOption.AllDirectories))
                    {
                        string entryName = filePath.Substring(caminhoPasta.Length + 1);
                        ZipArchiveEntry entry = archive.CreateEntryFromFile(filePath, entryName);
                    }
                }
            }
        }

        private void LimparCampos()
        {
            ltbArquivos.Items.Clear();
            txtCaminho.Clear();
            chkDeletaOriginal.Checked = false;
            chkUnico.Checked = false;
            lblQtd.Text = "0";
        }

        #endregion
    }
}
