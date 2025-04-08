using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EvidentaMercerie;
using Mercerie;
using NivelStocareDate;

namespace InterfataUtilizator_WindowsForms
{
    public partial class Form1 : Form
    {
        private Label lblTitlu;
        private Label[] lblDenumiri;
        private TextBox txtId;
        private TextBox txtNume;
        private TextBox txtPret;
        private ComboBox cmbTip;
        private Button btnSalveaza;

        public Form1()
        {
            InitializeComponent();
            InitializeazaInterfata();
            StilizeazaForma();
        }

        private void InitializeazaInterfata()
        {
            // Label titlu
            lblTitlu = new Label
            {
                Text = "Adaugă produs de mercerie",
                Top = 20,
                Left = 50,
                Width = 300,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            this.Controls.Add(lblTitlu);

            // Label denumiri și controale de introducere
            string[] denumiri = { "ID:", "Nume:", "Preț (RON):", "Tip:" };
            lblDenumiri = new Label[4];

            // ID
            lblDenumiri[0] = new Label
            {
                Text = denumiri[0],
                Top = 60,
                Left = 50,
                AutoSize = true,
                Font = new Font("Arial", 10)
            };
            txtId = new TextBox
            {
                Top = 60,
                Left = 150,
                Width = 150,
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(lblDenumiri[0]);
            this.Controls.Add(txtId);

            // Nume
            lblDenumiri[1] = new Label
            {
                Text = denumiri[1],
                Top = 90,
                Left = 50,
                AutoSize = true,
                Font = new Font("Arial", 10)
            };
            txtNume = new TextBox
            {
                Top = 90,
                Left = 150,
                Width = 150,
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(lblDenumiri[1]);
            this.Controls.Add(txtNume);

            // Preț
            lblDenumiri[2] = new Label
            {
                Text = denumiri[2],
                Top = 120,
                Left = 50,
                AutoSize = true,
                Font = new Font("Arial", 10)
            };
            txtPret = new TextBox
            {
                Top = 120,
                Left = 150,
                Width = 150,
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(lblDenumiri[2]);
            this.Controls.Add(txtPret);

            // Tip
            lblDenumiri[3] = new Label
            {
                Text = denumiri[3],
                Top = 150,
                Left = 50,
                AutoSize = true,
                Font = new Font("Arial", 10)
            };
            cmbTip = new ComboBox
            {
                Top = 150,
                Left = 150,
                Width = 150,
                Font = new Font("Arial", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTip.Items.AddRange(Enum.GetNames(typeof(TipProdus))); // Populează cu valorile din TipProdus
            cmbTip.SelectedIndex = 0; // Setează prima valoare implicit
            this.Controls.Add(lblDenumiri[3]);
            this.Controls.Add(cmbTip);

            // Buton salvare
            btnSalveaza = new Button
            {
                Text = "Salvează",
                Top = 190,
                Left = 150,
                Width = 100,
                Font = new Font("Arial", 10),
                BackColor = Color.LightGreen
            };
            btnSalveaza.Click += BtnSalveaza_Click;
            this.Controls.Add(btnSalveaza);
        }

        private void BtnSalveaza_Click(object sender, EventArgs e)
        {
            try
            {
                // Validare date
                if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtNume.Text) || string.IsNullOrWhiteSpace(txtPret.Text))
                {
                    MessageBox.Show("Toate câmpurile sunt obligatorii!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int id = int.Parse(txtId.Text);
                string nume = txtNume.Text;
                double pret = double.Parse(txtPret.Text);
                TipProdus tip = (TipProdus)Enum.Parse(typeof(TipProdus), cmbTip.SelectedItem.ToString());

                // Creare obiect Produs
                Produs produs = new Produs(id, nume, pret, tip);

                // Salvare în fișier
                string numeFisier = ConfigurationManager.AppSettings["NumeFisier"];
                string locatieFisierSolutie = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                string caleCompletaFisier = Path.Combine(locatieFisierSolutie, numeFisier);

                AdministrareProduseFisier adminProduse = new AdministrareProduseFisier(caleCompletaFisier);
                adminProduse.AdaugaProdus(produs);

                MessageBox.Show("Produs salvat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Resetare câmpuri
                txtId.Clear();
                txtNume.Clear();
                txtPret.Clear();
                cmbTip.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvare: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StilizeazaForma()
        {
            // Proprietăți formă
            this.Text = "Gestiune Mercerie";
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 248, 255); // AliceBlue

            // Stilizare controale
            lblTitlu.ForeColor = Color.Navy;
            lblTitlu.BackColor = Color.LightGray;
            foreach (var lbl in lblDenumiri)
            {
                lbl.ForeColor = Color.DarkSlateGray;
            }
            txtId.BackColor = Color.White;
            txtNume.BackColor = Color.White;
            txtPret.BackColor = Color.White;
            cmbTip.BackColor = Color.White;
            btnSalveaza.ForeColor = Color.Black;
        }
    }
}
