using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using EvidentaMercerie;
using Mercerie;
using NivelStocareDate;

namespace InterfataUtilizator_WindowsForms
{
    public partial class Form1 : Form
    {
        private const int LUNGIME_MAXIMA_NUME = 15;
        private const double PRET_MINIM = 0.01;
        private const string TELEFON_REGEX = @"^\d{10}$"; // 10 cifre

        private Label lblTitlu;
        private Label[] lblDenumiri;
        private Label[] lblErori;
        private TextBox txtIdClient;
        private TextBox txtNumeClient;
        private TextBox txtTelefonClient;
        private CheckBox[] chkTipProdus;
        private TextBox txtNumeProdus;
        private TextBox txtPretProdus;
        private Button btnAdauga;
        private Button btnAfiseaza;
        private DataGridView dgvClienti;

        private List<Client> clienti = new List<Client>();
        private AdministrareClientiFisier adminFisier;

        public Form1()
        {
            InitializeComponent();
            adminFisier = new AdministrareClientiFisier();
            clienti = adminFisier.CitesteClienti();
            InitializeazaInterfata();
            StilizeazaForma();
            ActualizeazaTabel();
        }

        private void InitializeazaInterfata()
        {
            // Titlu
            lblTitlu = new Label
            {
                Text = "Gestionare Clienti Mercerie",
                Top = 40,
                Left = 50,
                Width = 500,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 16, FontStyle.Bold)
            };
            this.Controls.Add(lblTitlu);

            // Etichete și câmpuri de introducere
            string[] denumiri = { "ID Client:", "Nume Client:", "Telefon Client:", "Tip Produs:", "Nume Produs:", "Preț Produs (RON):" };
            lblDenumiri = new Label[denumiri.Length];
            lblErori = new Label[5]; // Pentru ID, Nume, Telefon, Nume Produs, Preț

            for (int i = 0; i < lblErori.Length; i++)
            {
                lblErori[i] = new Label
                {
                    ForeColor = Color.Red,
                    AutoSize = true,
                    Top = 100 + i * 50,
                    Left = 450,
                    Font = new Font("Arial", 10)
                };
                this.Controls.Add(lblErori[i]);
            }

            // ID Client
            lblDenumiri[0] = new Label { Text = denumiri[0], Top = 100, Left = 50, AutoSize = true, Font = new Font("Arial", 12) };
            txtIdClient = new TextBox { Top = 100, Left = 200, Width = 200, Font = new Font("Arial", 12) };
            this.Controls.Add(lblDenumiri[0]);
            this.Controls.Add(txtIdClient);

            // Nume Client
            lblDenumiri[1] = new Label { Text = denumiri[1], Top = 150, Left = 50, AutoSize = true, Font = new Font("Arial", 12) };
            txtNumeClient = new TextBox { Top = 150, Left = 200, Width = 200, Font = new Font("Arial", 12) };
            this.Controls.Add(lblDenumiri[1]);
            this.Controls.Add(txtNumeClient);

            // Telefon Client
            lblDenumiri[2] = new Label { Text = denumiri[2], Top = 200, Left = 50, AutoSize = true, Font = new Font("Arial", 12) };
            txtTelefonClient = new TextBox { Top = 200, Left = 200, Width = 200, Font = new Font("Arial", 12) };
            this.Controls.Add(lblDenumiri[2]);
            this.Controls.Add(txtTelefonClient);

            // Tip Produs (Checkbox-uri)
            lblDenumiri[3] = new Label { Text = denumiri[3], Top = 250, Left = 50, AutoSize = true, Font = new Font("Arial", 12) };
            this.Controls.Add(lblDenumiri[3]);

            chkTipProdus = new CheckBox[Enum.GetValues(typeof(TipProdus)).Length];
            int topOffset = 250;
            int index = 0;
            foreach (TipProdus tip in Enum.GetValues(typeof(TipProdus)))
            {
                chkTipProdus[index] = new CheckBox
                {
                    Text = tip.ToString(),
                    Top = topOffset,
                    Left = 200,
                    AutoSize = true,
                    Font = new Font("Arial", 12)
                };
                this.Controls.Add(chkTipProdus[index]);
                topOffset += 30;
                index++;
            }

            // Nume Produs
            lblDenumiri[4] = new Label { Text = denumiri[4], Top = topOffset + 20, Left = 50, AutoSize = true, Font = new Font("Arial", 12) };
            txtNumeProdus = new TextBox { Top = topOffset + 20, Left = 200, Width = 200, Font = new Font("Arial", 12) };
            this.Controls.Add(lblDenumiri[4]);
            this.Controls.Add(txtNumeProdus);

            // Preț Produs
            lblDenumiri[5] = new Label { Text = denumiri[5], Top = topOffset + 70, Left = 50, AutoSize = true, Font = new Font("Arial", 12) };
            txtPretProdus = new TextBox { Top = topOffset + 70, Left = 200, Width = 200, Font = new Font("Arial", 12) };
            this.Controls.Add(lblDenumiri[5]);
            this.Controls.Add(txtPretProdus);

            // Butoane
            btnAdauga = new Button
            {
                Text = "Adaugă",
                Top = topOffset + 120,
                Left = 200,
                Width = 100,
                Height = 40,
                Font = new Font("Arial", 12),
                BackColor = Color.LightGreen
            };
            btnAdauga.Click += BtnAdauga_Click;
            this.Controls.Add(btnAdauga);

            btnAfiseaza = new Button
            {
                Text = "Afișează",
                Top = topOffset + 120,
                Left = 320,
                Width = 100,
                Height = 40,
                Font = new Font("Arial", 12),
                BackColor = Color.LightBlue
            };
            btnAfiseaza.Click += BtnAfiseaza_Click;
            this.Controls.Add(btnAfiseaza);

            // DataGridView pentru afișare
            dgvClienti = new DataGridView
            {
                Top = 100,
                Left = 600,
                Width = 600,
                Height = 600,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true
            };
            dgvClienti.Columns.Add("Id", "ID");
            dgvClienti.Columns.Add("Nume", "Nume");
            dgvClienti.Columns.Add("Telefon", "Telefon");
            dgvClienti.Columns.Add("Comenzi", "Comenzi");
            this.Controls.Add(dgvClienti);
        }

        private void BtnAdauga_Click(object sender, EventArgs e)
        {
            ResetErori();

            bool esteValid = true;
            int idClient;
            double pretProdus = 0;
            TipProdus? tipProdus = null;

            // Validare ID Client
            if (!int.TryParse(txtIdClient.Text, out idClient))
            {
                lblErori[0].Text = "ID invalid! Trebuie să fie un număr întreg.";
                lblDenumiri[0].ForeColor = Color.Red;
                esteValid = false;
            }

            // Validare Nume Client
            if (string.IsNullOrWhiteSpace(txtNumeClient.Text) || txtNumeClient.Text.Length > LUNGIME_MAXIMA_NUME)
            {
                lblErori[1].Text = $"Nume obligatoriu (max {LUNGIME_MAXIMA_NUME} caractere)!";
                lblDenumiri[1].ForeColor = Color.Red;
                esteValid = false;
            }

            // Validare Telefon Client
            if (string.IsNullOrWhiteSpace(txtTelefonClient.Text) || !Regex.IsMatch(txtTelefonClient.Text, TELEFON_REGEX))
            {
                lblErori[2].Text = "Telefon invalid! Trebuie să aibă 10 cifre.";
                lblDenumiri[2].ForeColor = Color.Red;
                esteValid = false;
            }

            // Validare Tip Produs
            int tipSelectat = -1;
            for (int i = 0; i < chkTipProdus.Length; i++)
            {
                if (chkTipProdus[i].Checked)
                {
                    if (tipSelectat != -1)
                    {
                        lblErori[3].Text = "Selectați un singur tip!";
                        lblDenumiri[3].ForeColor = Color.Red;
                        esteValid = false;
                        break;
                    }
                    tipSelectat = i;
                    tipProdus = (TipProdus)(i + 1); // TipProdus începe de la 1
                }
            }
            if (tipSelectat == -1)
            {
                lblErori[3].Text = "Selectați un tip de produs!";
                lblDenumiri[3].ForeColor = Color.Red;
                esteValid = false;
            }

            // Validare Nume și Preț Produs
            if (string.IsNullOrWhiteSpace(txtNumeProdus.Text) || txtNumeProdus.Text.Length > LUNGIME_MAXIMA_NUME)
            {
                lblErori[4].Text = $"Nume produs obligatoriu (max {LUNGIME_MAXIMA_NUME} caractere)!";
                lblDenumiri[4].ForeColor = Color.Red;
                esteValid = false;
            }

            if (!double.TryParse(txtPretProdus.Text, out pretProdus) || pretProdus < PRET_MINIM)
            {
                lblErori[4].Text = $"Preț invalid (>= {PRET_MINIM})!";
                lblDenumiri[5].ForeColor = Color.Red;
                esteValid = false;
            }

            if (!esteValid)
            {
                return;
            }

            // Creare sau actualizare client
            Client client = clienti.Find(c => c.Id == idClient);
            if (client == null)
            {
                client = new Client(idClient, txtNumeClient.Text, txtTelefonClient.Text);
                clienti.Add(client);
            }
            else
            {
                client.Nume = txtNumeClient.Text;
                client.Telefon = txtTelefonClient.Text;
            }

            // Adaugare comanda
            if (tipProdus != null)
            {
                Produs produs = new Produs(client.Comenzi.Count + 1, txtNumeProdus.Text, pretProdus, tipProdus.Value);
                client.AdaugaComanda(produs);
            }

            // Salvăm în fișier
            adminFisier.SalveazaTotiClientii(clienti);
            ActualizeazaTabel();

            // Resetăm câmpurile
            txtIdClient.Clear();
            txtNumeClient.Clear();
            txtTelefonClient.Clear();
            txtNumeProdus.Clear();
            txtPretProdus.Clear();
            foreach (var chk in chkTipProdus)
            {
                chk.Checked = false;
            }

            MessageBox.Show("Client și comanda salvate cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnAfiseaza_Click(object sender, EventArgs e)
        {
            ActualizeazaTabel();
        }

        private void ActualizeazaTabel()
        {
            dgvClienti.Rows.Clear();
            foreach (var client in clienti)
            {
                string comenziStr = client.Comenzi.Count > 0 ? string.Join("; ", client.Comenzi.Select(p => $"{p.Nume} ({p.Pret} RON)")) : "Nicio comandă";
                dgvClienti.Rows.Add(client.Id, client.Nume, client.Telefon, comenziStr);
            }
        }

        private void ResetErori()
        {
            for (int i = 0; i < lblErori.Length; i++)
            {
                lblErori[i].Text = string.Empty;
                if (i < lblDenumiri.Length)
                {
                    lblDenumiri[i].ForeColor = Color.DarkSlateGray;
                }
            }
        }

        private void StilizeazaForma()
        {
            this.Text = "Gestione Mercerie";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 248, 255);

            lblTitlu.ForeColor = Color.Navy;
            lblTitlu.BackColor = Color.LightGray;
            foreach (var lbl in lblDenumiri)
            {
                lbl.ForeColor = Color.DarkSlateGray;
            }
            txtIdClient.BackColor = Color.White;
            txtNumeClient.BackColor = Color.White;
            txtTelefonClient.BackColor = Color.White;
            txtNumeProdus.BackColor = Color.White;
            txtPretProdus.BackColor = Color.White;
            btnAdauga.ForeColor = Color.Black;
            btnAfiseaza.ForeColor = Color.Black;
        }
    }
}