using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Principal;
using System.Windows.Forms;
using ProyectoFinalPIV.Datos;
using ProyectoFinalPIV.Modelos;
using ProyectoFinalPIV.Utilidades;

namespace ProyectoFinalPIV.Formularios
{
    public class FormLogin : Form
    {
        private static readonly Color ColorFondoOscuro1 = Color.FromArgb(18, 28, 46);
        private static readonly Color ColorFondoOscuro2 = Color.FromArgb(45, 65, 95);
        private static readonly Color ColorTarjeta = Color.FromArgb(38, 56, 82);
        private static readonly Color ColorCampo = Color.FromArgb(232, 236, 245);
        private static readonly Color ColorBotonOscuro = Color.FromArgb(18, 28, 46);

        private MaskedTextBox txtCedula;
        private TextBox txtClave;
        private Label lblClave;
        private ComboBox cmbTipoAutenticacion;
        private Label lblUsuarioWindows;
        private Button btnIngresar;
        private Panel panelTarjeta;
        private Panel panelClave;

        public FormLogin()
        {
            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            this.Text = "Sistema de Gestion de TI - Ingreso";
            this.Width = 480;
            this.Height = 560;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackgroundImage = GenerarFondoDegradado(this.Width, this.Height);
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // ---------- Tarjeta central redondeada ----------
            panelTarjeta = new Panel { Left = 55, Top = 110, Width = 370, Height = 400, BackColor = ColorTarjeta };
            panelTarjeta.Region = new Region(CrearRectanguloRedondeado(panelTarjeta.ClientRectangle, 24));
            this.Controls.Add(panelTarjeta);

            // ---------- Avatar circular, sobre el borde superior de la tarjeta ----------
            var avatar = new PictureBox
            {
                Width = 90,
                Height = 90,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = IconosGenerador.IconoAvatarCircular(ColorBotonOscuro, Color.White, 90)
            };
            avatar.Left = (this.ClientSize.Width - avatar.Width) / 2;
            avatar.Top = panelTarjeta.Top - avatar.Height / 2;
            this.Controls.Add(avatar);
            avatar.BringToFront();

            var lblTitulo = new Label
            {
                Text = "Sistema de Gestion de TI",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Left = 0,
                Top = 55,
                Width = panelTarjeta.Width,
                Height = 30
            };
            panelTarjeta.Controls.Add(lblTitulo);

            // ---------- Campo Cedula (estilo "pill") ----------
            var lblCedulaIcono = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI", 10),
                Left = 15,
                Top = 8,
                Width = 20,
                ForeColor = Color.FromArgb(120, 110, 140)
            };
            txtCedula = new MaskedTextBox
            {
                Mask = "0000000000",
                PromptChar = '_',
                BorderStyle = BorderStyle.None,
                BackColor = ColorCampo,
                Font = new Font("Segoe UI", 10.5f),
                Left = 40,
                Top = 8,
                Width = 250
            };
            var panelCedula = CrearPanelCampo(30, 105, lblCedulaIcono, txtCedula);
            panelTarjeta.Controls.Add(panelCedula);

            // ---------- Campo Clave (estilo "pill") ----------
            var lblClaveIcono = new Label
            {
                Text = "🔒",
                Font = new Font("Segoe UI", 10),
                Left = 15,
                Top = 8,
                Width = 20,
                ForeColor = Color.FromArgb(120, 110, 140)
            };
            txtClave = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = ColorCampo,
                Font = new Font("Segoe UI", 10.5f),
                UseSystemPasswordChar = true,
                Left = 40,
                Top = 8,
                Width = 250
            };
            lblUsuarioWindows = new Label
            {
                Left = 40,
                Top = 10,
                Width = 250,
                Visible = false,
                ForeColor = Color.FromArgb(60, 50, 80),
                Font = new Font("Segoe UI", 8.5f),
                Text = "Windows: " + WindowsIdentity.GetCurrent().Name
            };
            panelClave = CrearPanelCampo(30, 160, lblClaveIcono, txtClave);
            panelClave.Controls.Add(lblUsuarioWindows);
            panelTarjeta.Controls.Add(panelClave);

            // ---------- Tipo de acceso ----------
            var lblTipoAutenticacion = new Label
            {
                Text = "Tipo de acceso:",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                Left = 30,
                Top = 220,
                Width = 110
            };
            cmbTipoAutenticacion = new ComboBox
            {
                Left = 150,
                Top = 217,
                Width = 190,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cmbTipoAutenticacion.Items.Add(Credencial.TipoSistema);
            cmbTipoAutenticacion.Items.Add(Credencial.TipoWindows);
            cmbTipoAutenticacion.SelectedIndex = 0;
            cmbTipoAutenticacion.SelectedIndexChanged += CmbTipoAutenticacion_SelectedIndexChanged;
            panelTarjeta.Controls.Add(lblTipoAutenticacion);
            panelTarjeta.Controls.Add(cmbTipoAutenticacion);

            // ---------- Boton Ingresar (estilo "pill" oscuro) ----------
            btnIngresar = new Button
            {
                Text = "INGRESAR",
                Left = 30,
                Top = 300,
                Width = 310,
                Height = 44,
                BackColor = ColorBotonOscuro,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.FlatAppearance.MouseOverBackColor = Color.FromArgb(55, 45, 70);
            btnIngresar.Region = new Region(CrearRectanguloRedondeado(new Rectangle(0, 0, btnIngresar.Width, btnIngresar.Height), 22));
            btnIngresar.Click += BtnIngresar_Click;
            panelTarjeta.Controls.Add(btnIngresar);

            var lblAyuda = new Label
            {
                Text = "Usuario por defecto: 0501250125 / 12345678",
                ForeColor = Color.FromArgb(220, 210, 230),
                Font = new Font("Segoe UI", 8),
                TextAlign = ContentAlignment.MiddleCenter,
                Left = 0,
                Top = 360,
                Width = panelTarjeta.Width
            };
            panelTarjeta.Controls.Add(lblAyuda);

            this.AcceptButton = btnIngresar;
        }

        private Panel CrearPanelCampo(int left, int top, Control icono, Control campo)
        {
            var panel = new Panel { Left = left, Top = top, Width = 310, Height = 38, BackColor = ColorCampo };
            panel.Region = new Region(CrearRectanguloRedondeado(new Rectangle(0, 0, panel.Width, panel.Height), 19));
            panel.Controls.Add(icono);
            panel.Controls.Add(campo);
            return panel;
        }

        private static GraphicsPath CrearRectanguloRedondeado(Rectangle bounds, int radio)
        {
            int diametro = radio * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diametro, diametro, 180, 90);
            path.AddArc(bounds.Right - diametro, bounds.Y, diametro, diametro, 270, 90);
            path.AddArc(bounds.Right - diametro, bounds.Bottom - diametro, diametro, diametro, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diametro, diametro, diametro, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static Image GenerarFondoDegradado(int ancho, int alto)
        {
            var bitmap = new Bitmap(ancho, alto);
            using (var g = Graphics.FromImage(bitmap))
            using (var pincel = new LinearGradientBrush(
                new Rectangle(0, 0, ancho, alto), ColorFondoOscuro1, ColorFondoOscuro2, LinearGradientMode.ForwardDiagonal))
            {
                g.FillRectangle(pincel, 0, 0, ancho, alto);
            }
            return bitmap;
        }

        private void CmbTipoAutenticacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool esSistema = cmbTipoAutenticacion.SelectedItem?.ToString() == Credencial.TipoSistema;
            txtClave.Visible = esSistema;
            lblUsuarioWindows.Visible = !esSistema;
        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            string cedula = txtCedula.Text.Trim();
            string tipo = cmbTipoAutenticacion.SelectedItem?.ToString() ?? Credencial.TipoSistema;

            if (string.IsNullOrWhiteSpace(cedula))
            {
                MessageBox.Show("Debe ingresar la cedula.", "Datos incompletos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (tipo == Credencial.TipoSistema && string.IsNullOrWhiteSpace(txtClave.Text))
            {
                MessageBox.Show("Debe ingresar la clave.", "Datos incompletos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var repositorio = new UsuarioRepositorio();
                Usuario usuario = tipo == Credencial.TipoSistema
                    ? repositorio.AutenticarPorSistema(cedula, txtClave.Text)
                    : repositorio.AutenticarPorWindows(cedula, WindowsIdentity.GetCurrent().Name);

                if (usuario == null)
                {
                    MessageBox.Show(
                        tipo == Credencial.TipoSistema
                            ? "Cedula o clave incorrectos, o el usuario esta inactivo."
                            : "La cedula no tiene configurado el acceso por Windows con este usuario, o esta inactivo.",
                        "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.Hide();
                var formPrincipal = new FormPrincipal(usuario);
                formPrincipal.FormClosed += (s, args) => this.Close();
                formPrincipal.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible conectar con la base de datos.\n\nDetalle: " + ex.Message,
                    "Error de conexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
