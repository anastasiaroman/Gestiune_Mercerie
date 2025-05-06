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
        private TextBox txtCautare;
        private Button btnCautare;
        private DataGridView dgvClienti;
        private ToolTip toolTip;
        private RadioButton rbAdaugaClient;
        private RadioButton rbEditeazaClient;
        private CheckBox chkSalvareAutomata;
        private ComboBox cmbCriteriuCautare;

        private List<Client> clienti = new List<Client>();
        private AdministrareClientiFisier adminFisier;
        private AdministrareClientiMemorie adminMemorie;

        public Form1()
        {
            InitializeComponent();
            adminFisier = new AdministrareClientiFisier();
            adminMemorie = new AdministrareClientiMemorie();
            clienti = adminFisier.CitesteClienti();
            foreach (var client in clienti)
            {
                adminMemorie.AdaugaClient(client);
            }
            InitializeazaInterfata();
            StilizeazaForma();
            ActualizeazaTabel();
        }

        private void InitializeazaInterfata()
        {
            toolTip = new ToolTip();

            // Setăm dimensiunea ferestrei pentru a acomoda toate elementele
            this.Size = new Size(1400, 720);

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
                Height = 550,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray
            };
            this.Controls.Add(grpClient);

            // RadioButton pentru Adauga/Editare
            rbAdaugaClient = new RadioButton
            {
                Text = "Adaugă Client Nou",
                Top = 30,
                Left = 20,
                Width = 150,
                Font = new Font("Segoe UI", 12),
                Checked = true
            };
            rbEditeazaClient = new RadioButton
            {
                Text = "Editează Client Existent",
                Top = 30,
                Left = 200,
                Width = 180,
                Font = new Font("Segoe UI", 12)
            };
            grpClient.Controls.Add(rbAdaugaClient);
            grpClient.Controls.Add(rbEditeazaClient);

            // CheckBox pentru salvare automată
            chkSalvareAutomata = new CheckBox
            {
                Text = "Salvează automat în fișier",
                Top = 60,
                Left = 20,
                Width = 250, // Mărire lățime pentru a afișa textul complet
                Font = new Font("Segoe UI", 12),
                Checked = true
            };
            grpClient.Controls.Add(chkSalvareAutomata);

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
                    Top = 100 + i * 60,
                    Left = 300,
                    Width = 280,
                    Height = 40,
                    Font = new Font("Segoe UI", 10),
                    AutoEllipsis = true
                };
                grpClient.Controls.Add(lblErori[i]);
            }

            // ID Client
            lblDenumiri[0] = new Label { Text = denumiri[0], Top = 100, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 12) };
            txtIdClient = new TextBox { Top = 100, Left = 150, Width = 180, Font = new Font("Segoe UI", 12) };
            grpClient.Controls.Add(lblDenumiri[0]);
            grpClient.Controls.Add(txtIdClient);

            // Nume Client
            lblDenumiri[1] = new Label { Text = denumiri[1], Top = 160, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 12) };
            txtNumeClient = new TextBox { Top = 160, Left = 150, Width = 180, Font = new Font("Segoe UI", 12) };
            grpClient.Controls.Add(lblDenumiri[1]);
            grpClient.Controls.Add(txtNumeClient);

            // Telefon Client
            lblDenumiri[2] = new Label { Text = denumiri[2], Top = 220, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 12) };
            txtTelefonClient = new TextBox { Top = 220, Left = 150, Width = 180, Font = new Font("Segoe UI", 12) };
            grpClient.Controls.Add(lblDenumiri[2]);
            grpClient.Controls.Add(txtTelefonClient);

            // Tip Produs (ComboBox)
            lblDenumiri[3] = new Label { Text = denumiri[3], Top = 280, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 12) };
            cmbTipProdus = new ComboBox
            {
                Top = 280,
                Left = 150,
                Width = 180,
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
            lblDenumiri[4] = new Label { Text = denumiri[4], Top = 340, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 12) };
            txtNumeProdus = new TextBox { Top = 340, Left = 150, Width = 180, Font = new Font("Segoe UI", 12) };
            grpClient.Controls.Add(lblDenumiri[4]);
            grpClient.Controls.Add(txtNumeProdus);

            // Preț Produs
            lblDenumiri[5] = new Label { Text = denumiri[5], Top = 400, Left = 20, AutoSize = true, Font = new Font("Segoe UI", 12) };
            txtPretProdus = new TextBox { Top = 400, Left = 150, Width = 180, Font = new Font("Segoe UI", 12) };
            grpClient.Controls.Add(lblDenumiri[5]);
            grpClient.Controls.Add(txtPretProdus);

            // Butoane
            btnAdauga = new Button
            {
                Text = "Adaugă/Editează",
                Top = 460,
                Left = 50,
                Width = 120,
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
                Top = 460,
                Left = 190,
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
                Top = 460,
                Left = 310,
                Width = 100,
                Height = 40,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(135, 206, 250),
                ForeColor = Color.Black
            };
            btnAfiseaza.Click += BtnAfiseaza_Click;
            grpClient.Controls.Add(btnAfiseaza);

            // Căutare
            Label lblCautare = new Label
            {
                Text = "Caută Client:",
                Top = 630,
                Left = 50,
                AutoSize = true,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.DarkSlateGray
            };
            cmbCriteriuCautare = new ComboBox
            {
                Top = 630,
                Left = 150,
                Width = 120,
                Font = new Font("Segoe UI", 12),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCriteriuCautare.Items.AddRange(new string[] { "ID", "Nume", "Telefon" });
            cmbCriteriuCautare.SelectedIndex = 0;
            txtCautare = new TextBox
            {
                Top = 630,
                Left = 280,
                Width = 180,
                Font = new Font("Segoe UI", 12)
            };
            btnCautare = new Button
            {
                Text = "Caută",
                Top = 630,
                Left = 470,
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
                Width = 650,
                Height = 580,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                Font = new Font("Segoe UI", 10),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            dgvClienti.Columns.Add("Id", "ID");
            dgvClienti.Columns.Add("Nume", "Nume");
            dgvClienti.Columns.Add("Telefon", "Telefon");
            var comenziColumn = new DataGridViewColumn
            {
                Name = "Comenzi",
                HeaderText = "Comenzi",
                CellTemplate = new DataGridViewTextBoxCell(),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            };
            dgvClienti.Columns.Add(comenziColumn);
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
                toolTip.SetToolTip(txtIdClient, "ID invalid! Trebuie să fie un număr întreg.");
                esteValid = false;
            }

            // Validare Nume Client
            if (string.IsNullOrWhiteSpace(txtNumeClient.Text) || txtNumeClient.Text.Length > LUNGIME_MAXIMA_NUME)
            {
                lblErori[1].Text = $"Nume obligatoriu (max {LUNGIME_MAXIMA_NUME} caractere)!";
                lblDenumiri[1].ForeColor = Color.Red;
                toolTip.SetToolTip(txtNumeClient, $"Nume obligatoriu (max {LUNGIME_MAXIMA_NUME} caractere)!");
                esteValid = false;
            }

            // Validare Telefon Client
            if (string.IsNullOrWhiteSpace(txtTelefonClient.Text) || !Regex.IsMatch(txtTelefonClient.Text, TELEFON_REGEX))
            {
                lblErori[2].Text = "Telefon invalid! Trebuie să aibă 10 cifre.";
                lblDenumiri[2].ForeColor = Color.Red;
                toolTip.SetToolTip(txtTelefonClient, "Telefon invalid! Trebuie să aibă 10 cifre.");
                esteValid = false;
            }

            // Validare Tip Produs
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

            // Validare Nume și Preț Produs
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

            // Logica pentru Adăugare/Editare
            Client client = clienti.Find(c => c.Id == idClient);
            if (rbAdaugaClient.Checked)
            {
                if (client != null)
                {
                    MessageBox.Show("ID-ul există deja! Alegeți un alt ID sau selectați 'Editează Client Existent'.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                client = new Client(idClient, txtNumeClient.Text, txtTelefonClient.Text);
                clienti.Add(client);
                adminMemorie.AdaugaClient(client);
            }
            else // rbEditeazaClient.Checked
            {
                if (client == null)
                {
                    MessageBox.Show("Clientul nu există! Selectați 'Adaugă Client Nou' pentru a crea un client nou.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                client.Nume = txtNumeClient.Text;
                client.Telefon = txtTelefonClient.Text;
            }

            // Adaugare comanda
            if (tipProdus != null)
            {
                Produs produs = new Produs(client.Comenzi.Count + 1, txtNumeProdus.Text, pretProdus, tipProdus.Value);
                client.AdaugaComanda(produs);
            }

            // Salvare în fișier dacă este bifat CheckBox-ul
            if (chkSalvareAutomata.Checked)
            {
                adminFisier.SalveazaTotiClientii(clienti);
            }

            ActualizeazaTabel();

            // Resetăm câmpurile
            ResetFormular();

            MessageBox.Show("Client și comanda salvate cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            if (string.IsNullOrEmpty(campCautare))
            {
                MessageBox.Show("Selectați un câmp pentru căutare!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<Client> clientiFiltrati = new List<Client>();
            switch (campCautare)
            {
                case "ID":
                    if (int.TryParse(criteriu, out int id))
                    {
                        clientiFiltrati = clienti.Where(c => c.Id == id).ToList();
                    }
                    break;
                case "Nume":
                    clientiFiltrati = clienti.Where(c => c.Nume.ToLower().Contains(criteriu.ToLower())).ToList();
                    break;
                case "Telefon":
                    clientiFiltrati = clienti.Where(c => c.Telefon.Contains(criteriu)).ToList();
                    break;
            }

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
            rbAdaugaClient.Checked = true;
            chkSalvareAutomata.Checked = true;
            ResetErori();
        }

        private void ActualizeazaTabel(List<Client> clientiAfisati = null)
        {
            dgvClienti.Rows.Clear();
            var listaAfisare = clientiAfisati ?? clienti;
            foreach (var client in listaAfisare)
            {
                string comenziStr = client.Comenzi.Count > 0 ? string.Join("\n", client.Comenzi.Select(p => $"{p.Nume} ({p.Pret} RON)")) : "Nicio comandă";
                dgvClienti.Rows.Add(client.Id, client.Nume, client.Telefon, comenziStr);
            }
            // Ajustăm lățimea coloanei "Comenzi" pentru a se potrivi conținutului
            dgvClienti.Columns["Comenzi"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgvClienti.AutoResizeColumns();
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
        }
    }
}