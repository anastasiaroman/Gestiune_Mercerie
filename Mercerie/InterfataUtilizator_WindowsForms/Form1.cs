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
        private GroupBox grpClient;
        private Label[] lblDenumiri;
        private Label[] lblErori;
        private TextBox txtIdClient;
        private TextBox txtNumeClient;
        private TextBox txtTelefonClient;
        private ComboBox cmbTipProdus;
        private TextBox txtNumeProdus;
        private TextBox txtPretProdus;
        private Button btnAdauga;
        private Button btnAfiseaza;
        private Button btnReset;
        private Button btnActualizeaza;
        private TextBox txtCautare;
        private ComboBox cmbCriteriuCautare; // Adăugat pentru criteriul de căutare
        private Button btnCautare;
        private DataGridView dgvClienti;
        private ToolTip toolTip;

        private List<Client> clienti = new List<Client>();
        private AdministrareClientiFisier adminFisier;
        private AdministrareClientiMemorie adminMemorie;
        private Client clientSelectat;
        private Produs comandaSelectata;

        public Form1()
        {
            InitializeComponent();
            adminFisier = new AdministrareClientiFisier();
            adminMemorie = new AdministrareClientiMemorie();
            try
            {
                clienti = adminFisier.CitesteClienti();
                foreach (var client in clienti)
                {
                    adminMemorie.AdaugaClient(client);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la citirea datelor din fișier: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            InitializeazaInterfata();
            StilizeazaForma();
            ActualizeazaTabel();
        }

        private void InitializeazaInterfata()
        {
            toolTip = new ToolTip();

            this.Size = new Size(1280, 720);

            // Titlu
            lblTitlu = new Label
            {
                Text = "Gestionare Clienti Mercerie",
                Top = 10,
                Width = 600,
                Height = 50,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.Black
            };
            lblTitlu.Left = (this.ClientSize.Width - lblTitlu.Width) / 2;
            this.Controls.Add(lblTitlu);

            // GroupBox pentru introducere date
            grpClient = new GroupBox
            {
                Text = "Introducere Client și Comanda",
                Top = 70,
                Left = 50,
                Width = 600,
                Height = 500,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray
            };
            this.Controls.Add(grpClient);

            // Etichete și câmpuri de introducere
            string[] denumiri = { "ID Client:", "Nume Client:", "Telefon Client:", "Tip Produs:", "Nume Produs:", "Preț Produs (RON):" };
            lblDenumiri = new Label[denumiri.Length];
            lblErori = new Label[5];

            for (int i = 0; i < lblErori.Length; i++)
            {
                lblErori[i] = new Label
                {
                    ForeColor = Color.Red,
                    AutoSize = false,
                    Top = 40 + i * 60,
                    Left = 300,
                    Width = 280,
                    Height = 40,
                    Font = new Font("Segoe UI", 10),
                    AutoEllipsis = true
                };
                grpClient.Controls.Add(lblErori[i]);
            }

            // ID Client
            lblDenumiri[0] = new Label { Text = denumiri[0], Top = 40, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 12) };
            txtIdClient = new TextBox { Top = 40, Left = 150, Width = 140, Font = new Font("Segoe UI", 12) };
            grpClient.Controls.Add(lblDenumiri[0]);
            grpClient.Controls.Add(txtIdClient);

            // Nume Client
            lblDenumiri[1] = new Label { Text = denumiri[1], Top = 100, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 12) };
            txtNumeClient = new TextBox { Top = 100, Left = 150, Width = 140, Font = new Font("Segoe UI", 12) };
            grpClient.Controls.Add(lblDenumiri[1]);
            grpClient.Controls.Add(txtNumeClient);

            // Telefon Client
            lblDenumiri[2] = new Label { Text = denumiri[2], Top = 160, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 12) };
            txtTelefonClient = new TextBox { Top = 160, Left = 150, Width = 140, Font = new Font("Segoe UI", 12) };
            grpClient.Controls.Add(lblDenumiri[2]);
            grpClient.Controls.Add(txtTelefonClient);

            // Tip Produs (ComboBox)
            lblDenumiri[3] = new Label { Text = denumiri[3], Top = 220, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 12) };
            cmbTipProdus = new ComboBox
            {
                Top = 220,
                Left = 150,
                Width = 140,
                Font = new Font("Segoe UI", 12),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (TipProdus tip in Enum.GetValues(typeof(TipProdus)))
            {
                cmbTipProdus.Items.Add(tip);
            }
            cmbTipProdus.SelectedIndex = 0;
            grpClient.Controls.Add(lblDenumiri[3]);
            grpClient.Controls.Add(cmbTipProdus);

            // Nume Produs
            lblDenumiri[4] = new Label { Text = denumiri[4], Top = 280, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 12) };
            txtNumeProdus = new TextBox { Top = 280, Left = 150, Width = 140, Font = new Font("Segoe UI", 12) };
            grpClient.Controls.Add(lblDenumiri[4]);
            grpClient.Controls.Add(txtNumeProdus);

            // Preț Produs
            lblDenumiri[5] = new Label { Text = denumiri[5], Top = 340, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 12) };
            txtPretProdus = new TextBox { Top = 340, Left = 150, Width = 140, Font = new Font("Segoe UI", 12) };
            grpClient.Controls.Add(lblDenumiri[5]);
            grpClient.Controls.Add(txtPretProdus);

            // Butoane
            btnAdauga = new Button
            {
                Text = "Adaugă",
                Top = 410,
                Left = 20,
                Width = 100,
                Height = 40,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(144, 238, 144),
                ForeColor = Color.Black
            };
            btnAdauga.Click += BtnAdauga_Click;
            grpClient.Controls.Add(btnAdauga);

            btnReset = new Button
            {
                Text = "Reset",
                Top = 410,
                Left = 130,
                Width = 100,
                Height = 40,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(255, 182, 193),
                ForeColor = Color.Black
            };
            btnReset.Click += BtnReset_Click;
            grpClient.Controls.Add(btnReset);

            btnAfiseaza = new Button
            {
                Text = "Afișează Toți",
                Top = 410,
                Left = 240,
                Width = 100,
                Height = 40,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(135, 206, 250),
                ForeColor = Color.Black
            };
            btnAfiseaza.Click += BtnAfiseaza_Click;
            grpClient.Controls.Add(btnAfiseaza);

            btnActualizeaza = new Button
            {
                Text = "Actualizează",
                Top = 410,
                Left = 350,
                Width = 100,
                Height = 40,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(255, 165, 0),
                ForeColor = Color.Black
            };
            btnActualizeaza.Click += BtnActualizeaza_Click;
            grpClient.Controls.Add(btnActualizeaza);

            // Căutare
            Label lblCautare = new Label
            {
                Text = "Caută Client:",
                Top = 580,
                Left = 50,
                AutoSize = true,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.DarkSlateGray
            };

            // Adăugăm ComboBox pentru criteriul de căutare
            cmbCriteriuCautare = new ComboBox
            {
                Top = 580,
                Left = 150,
                Width = 100,
                Font = new Font("Segoe UI", 12),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCriteriuCautare.Items.AddRange(new string[] { "ID", "Nume", "Telefon" });
            cmbCriteriuCautare.SelectedIndex = 0; // Setăm implicit pe "ID"

            txtCautare = new TextBox
            {
                Top = 580,
                Left = 260,
                Width = 180,
                Font = new Font("Segoe UI", 12)
            };
            btnCautare = new Button
            {
                Text = "Caută",
                Top = 580,
                Left = 450,
                Width = 100,
                Height = 40,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(255, 215, 0),
                ForeColor = Color.Black
            };
            btnCautare.Click += BtnCautare_Click;
            this.Controls.Add(lblCautare);
            this.Controls.Add(cmbCriteriuCautare);
            this.Controls.Add(txtCautare);
            this.Controls.Add(btnCautare);

            // DataGridView pentru afișare
            dgvClienti = new DataGridView
            {
                Top = 70,
                Left = 700,
                Width = 550,
                Height = 580,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                ReadOnly = true,
                Font = new Font("Segoe UI", 10),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            dgvClienti.Columns.Add("Id", "ID");
            dgvClienti.Columns.Add("Nume", "Nume");
            dgvClienti.Columns.Add("Telefon", "Telefon");
            dgvClienti.Columns.Add("Comenzi", "Comenzi");

            dgvClienti.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            foreach (DataGridViewColumn column in dgvClienti.Columns)
            {
                column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }
            dgvClienti.Columns["Comenzi"].MinimumWidth = 200;

            dgvClienti.CellClick += DgvClienti_CellClick;
            this.Controls.Add(dgvClienti);
        }

        private void DgvClienti_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvClienti.Rows.Count)
            {
                return;
            }

            if (dgvClienti.Rows[e.RowIndex].Cells["Id"].Value == null)
            {
                return;
            }

            try
            {
                int idClient = Convert.ToInt32(dgvClienti.Rows[e.RowIndex].Cells["Id"].Value);
                clientSelectat = clienti.Find(c => c.Id == idClient);

                if (clientSelectat != null)
                {
                    ResetErori();
                    txtIdClient.Text = clientSelectat.Id.ToString();
                    txtIdClient.ReadOnly = true;
                    txtNumeClient.Text = clientSelectat.Nume;
                    txtTelefonClient.Text = clientSelectat.Telefon;

                    comandaSelectata = clientSelectat.Comenzi.LastOrDefault();
                    if (comandaSelectata != null)
                    {
                        cmbTipProdus.SelectedItem = comandaSelectata.Tip;
                        txtNumeProdus.Text = comandaSelectata.Nume;
                        txtPretProdus.Text = comandaSelectata.Pret.ToString();
                    }
                    else
                    {
                        cmbTipProdus.SelectedIndex = 0;
                        txtNumeProdus.Clear();
                        txtPretProdus.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la selecția clientului: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnActualizeaza_Click(object sender, EventArgs e)
        {
            if (clientSelectat == null)
            {
                MessageBox.Show("Selectați un client din tabel pentru a actualiza!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ResetErori();

            bool esteValid = true;
            double pretProdus = 0;
            TipProdus? tipProdus = null;

            if (string.IsNullOrWhiteSpace(txtNumeClient.Text) || txtNumeClient.Text.Length > LUNGIME_MAXIMA_NUME)
            {
                lblErori[1].Text = $"Nume obligatoriu (max {LUNGIME_MAXIMA_NUME} caractere)!";
                lblDenumiri[1].ForeColor = Color.Red;
                toolTip.SetToolTip(txtNumeClient, $"Nume obligatoriu (max {LUNGIME_MAXIMA_NUME} caractere)!");
                esteValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtTelefonClient.Text) || !Regex.IsMatch(txtTelefonClient.Text, TELEFON_REGEX))
            {
                lblErori[2].Text = "Telefon invalid! Trebuie să aibă 10 cifre.";
                lblDenumiri[2].ForeColor = Color.Red;
                toolTip.SetToolTip(txtTelefonClient, "Telefon invalid! Trebuie să aibă 10 cifre.");
                esteValid = false;
            }

            if (cmbTipProdus.SelectedItem == null)
            {
                lblErori[3].Text = "Selectați un tip de produs!";
                lblDenumiri[3].ForeColor = Color.Red;
                toolTip.SetToolTip(cmbTipProdus, "Selectați un tip de produs!");
                esteValid = false;
            }
            else
            {
                tipProdus = (TipProdus)cmbTipProdus.SelectedItem;
            }

            if (string.IsNullOrWhiteSpace(txtNumeProdus.Text) || txtNumeProdus.Text.Length > LUNGIME_MAXIMA_NUME)
            {
                lblErori[4].Text = $"Nume produs obligatoriu (max {LUNGIME_MAXIMA_NUME} caractere)!";
                lblDenumiri[4].ForeColor = Color.Red;
                toolTip.SetToolTip(txtNumeProdus, $"Nume produs obligatoriu (max {LUNGIME_MAXIMA_NUME} caractere)!");
                esteValid = false;
            }

            if (!double.TryParse(txtPretProdus.Text, out pretProdus) || pretProdus < PRET_MINIM)
            {
                lblErori[4].Text = $"Preț invalid (>= {PRET_MINIM})!";
                lblDenumiri[5].ForeColor = Color.Red;
                toolTip.SetToolTip(txtPretProdus, $"Preț invalid (>= {PRET_MINIM})!");
                esteValid = false;
            }

            if (!esteValid)
            {
                return;
            }

            try
            {
                clientSelectat.Nume = txtNumeClient.Text;
                clientSelectat.Telefon = txtTelefonClient.Text;

                if (comandaSelectata != null)
                {
                    comandaSelectata.Nume = txtNumeProdus.Text;
                    comandaSelectata.Pret = pretProdus;
                    comandaSelectata.Tip = tipProdus.Value;
                }
                else if (tipProdus != null)
                {
                    Produs produsNou = new Produs(clientSelectat.Comenzi.Count + 1, txtNumeProdus.Text, pretProdus, tipProdus.Value);
                    clientSelectat.AdaugaComanda(produsNou);
                }

                adminMemorie.ActualizeazaClient(clientSelectat);
                adminFisier.SalveazaTotiClientii(clienti);
                ActualizeazaTabel();

                ResetFormular();
                clientSelectat = null;
                comandaSelectata = null;

                MessageBox.Show("Client și comanda actualizate cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la actualizarea clientului: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdauga_Click(object sender, EventArgs e)
        {
            ResetErori();

            bool esteValid = true;
            int idClient;
            double pretProdus = 0;
            TipProdus? tipProdus = null;

            if (!int.TryParse(txtIdClient.Text, out idClient))
            {
                lblErori[0].Text = "ID invalid! Trebuie să fie un număr întreg.";
                lblDenumiri[0].ForeColor = Color.Red;
                toolTip.SetToolTip(txtIdClient, "ID invalid! Trebuie să fie un număr întreg.");
                esteValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtNumeClient.Text) || txtNumeClient.Text.Length > LUNGIME_MAXIMA_NUME)
            {
                lblErori[1].Text = $"Nume obligatoriu (max {LUNGIME_MAXIMA_NUME} caractere)!";
                lblDenumiri[1].ForeColor = Color.Red;
                toolTip.SetToolTip(txtNumeClient, $"Nume obligatoriu (max {LUNGIME_MAXIMA_NUME} caractere)!");
                esteValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtTelefonClient.Text) || !Regex.IsMatch(txtTelefonClient.Text, TELEFON_REGEX))
            {
                lblErori[2].Text = "Telefon invalid! Trebuie să aibă 10 cifre.";
                lblDenumiri[2].ForeColor = Color.Red;
                toolTip.SetToolTip(txtTelefonClient, "Telefon invalid! Trebuie să aibă 10 cifre.");
                esteValid = false;
            }

            if (cmbTipProdus.SelectedItem == null)
            {
                lblErori[3].Text = "Selectați un tip de produs!";
                lblDenumiri[3].ForeColor = Color.Red;
                toolTip.SetToolTip(cmbTipProdus, "Selectați un tip de produs!");
                esteValid = false;
            }
            else
            {
                tipProdus = (TipProdus)cmbTipProdus.SelectedItem;
            }

            if (string.IsNullOrWhiteSpace(txtNumeProdus.Text) || txtNumeProdus.Text.Length > LUNGIME_MAXIMA_NUME)
            {
                lblErori[4].Text = $"Nume produs obligatoriu (max {LUNGIME_MAXIMA_NUME} caractere)!";
                lblDenumiri[4].ForeColor = Color.Red;
                toolTip.SetToolTip(txtNumeProdus, $"Nume produs obligatoriu (max {LUNGIME_MAXIMA_NUME} caractere)!");
                esteValid = false;
            }

            if (!double.TryParse(txtPretProdus.Text, out pretProdus) || pretProdus < PRET_MINIM)
            {
                lblErori[4].Text = $"Preț invalid (>= {PRET_MINIM})!";
                lblDenumiri[5].ForeColor = Color.Red;
                toolTip.SetToolTip(txtPretProdus, $"Preț invalid (>= {PRET_MINIM})!");
                esteValid = false;
            }

            if (!esteValid)
            {
                return;
            }

            try
            {
                Client client = clienti.Find(c => c.Id == idClient);
                if (client == null)
                {
                    client = new Client(idClient, txtNumeClient.Text, txtTelefonClient.Text);
                    clienti.Add(client);
                    adminMemorie.AdaugaClient(client);
                }
                else
                {
                    client.Nume = txtNumeClient.Text;
                    client.Telefon = txtTelefonClient.Text;
                }

                if (tipProdus != null)
                {
                    Produs produs = new Produs(client.Comenzi.Count + 1, txtNumeProdus.Text, pretProdus, tipProdus.Value);
                    client.AdaugaComanda(produs);
                }

                adminFisier.SalveazaTotiClientii(clienti);
                ActualizeazaTabel();

                ResetFormular();

                MessageBox.Show("Client și comanda salvate cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la adăugarea clientului: {ex.Message}", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAfiseaza_Click(object sender, EventArgs e)
        {
            txtCautare.Clear();
            ActualizeazaTabel();
        }

        private void BtnCautare_Click(object sender, EventArgs e)
        {
            string criteriu = txtCautare.Text.Trim();
            string campCautare = cmbCriteriuCautare.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(campCautare))
            {
                MessageBox.Show("Selectați un criteriu de căutare!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var clientiFiltrati = adminMemorie.CautaClienti(criteriu, campCautare);
            ActualizeazaTabel(clientiFiltrati);

            if (clientiFiltrati.Count == 0)
            {
                MessageBox.Show("Niciun client găsit!", "Căutare", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ResetFormular()
        {
            txtIdClient.Clear();
            txtNumeClient.Clear();
            txtTelefonClient.Clear();
            txtNumeProdus.Clear();
            txtPretProdus.Clear();
            cmbTipProdus.SelectedIndex = 0;
            ResetErori();
            txtIdClient.ReadOnly = false;
        }

        private void ActualizeazaTabel(List<Client> clientiAfisati = null)
        {
            dgvClienti.Rows.Clear();
            var listaAfisare = clientiAfisati ?? clienti;
            foreach (var client in listaAfisare)
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
            toolTip.RemoveAll();
        }

        private void StilizeazaForma()
        {
            this.Text = "Gestiune Mercerie";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 220);

            lblTitlu.BackColor = Color.FromArgb(169, 169, 169);
            grpClient.BackColor = Color.FromArgb(240, 255, 240);
            foreach (var lbl in lblDenumiri)
            {
                lbl.ForeColor = Color.DarkSlateGray;
            }
            txtIdClient.BackColor = Color.White;
            txtNumeClient.BackColor = Color.White;
            txtTelefonClient.BackColor = Color.White;
            txtNumeProdus.BackColor = Color.White;
            txtPretProdus.BackColor = Color.White;
            txtCautare.BackColor = Color.White;
            cmbTipProdus.BackColor = Color.White;
            cmbCriteriuCautare.BackColor = Color.White;
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            ResetFormular();
            clientSelectat = null;
            comandaSelectata = null;
        }
    }
}