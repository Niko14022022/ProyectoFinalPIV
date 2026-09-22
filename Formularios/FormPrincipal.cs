using System;
using System.Drawing;
using System.Windows.Forms;
using ProyectoFinalPIV.Datos;
using ProyectoFinalPIV.Modelos;
using ProyectoFinalPIV.Utilidades;

namespace ProyectoFinalPIV.Formularios
{
    public class FormPrincipal : Form
    {
        private static readonly Color ColorBarraLateral = Color.FromArgb(27, 42, 65);
        private static readonly Color ColorBarraLateralHover = Color.FromArgb(45, 65, 95);
        private static readonly Color ColorAcento = Color.FromArgb(61, 90, 128);
        private static readonly Color ColorTextoClaro = Color.White;

        private readonly Usuario _usuarioActual;
        private Panel barraLateral;
        private Panel barraSuperior;
        private Label lblFecha;
        private Label lblHora;
        private Timer temporizadorReloj;

        public FormPrincipal(Usuario usuarioActual)
        {
            _usuarioActual = usuarioActual;
            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            this.Text = "Sistema de Gestion de TI - Menu Principal";
            this.WindowState = FormWindowState.Maximized;
            this.IsMdiContainer = true;
            this.BackColor = Color.FromArgb(240, 243, 248);

            ConstruirBarraLateral();
            ConstruirBarraSuperior();
            AsignarFondoMdi();

            temporizadorReloj = new Timer { Interval = 1000 };
            temporizadorReloj.Tick += (s, e) => ActualizarFechaHora();
            temporizadorReloj.Start();
            ActualizarFechaHora();

            MostrarNotificacionesDeIngreso();
        }

        // ---------------- Barra lateral (menu tipo panel de administracion) ----------------

        private void ConstruirBarraLateral()
        {
            barraLateral = new Panel { Dock = DockStyle.Left, Width = 230, BackColor = ColorBarraLateral };

            var lblLogo = new Label
            {
                Text = "Gestion de TI",
                ForeColor = ColorTextoClaro,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 70,
                TextAlign = ContentAlignment.MiddleCenter
            };
            barraLateral.Controls.Add(lblLogo);

            int y = 90;
            bool puedeGestionarEquipos = _usuarioActual.Rol == "Tecnico TI" || _usuarioActual.Rol == "Administrador TI";

            if (puedeGestionarEquipos)
            {
                AgregarBotonMenu("Control de Computadoras", IconosGenerador.IconoComputadora(ColorTextoClaro), y,
                    () => AbrirFormulario(new FormComputadoras(_usuarioActual)));
                y += 55;
                AgregarBotonMenu("Control de Software", IconosGenerador.IconoSoftware(ColorTextoClaro), y,
                    () => AbrirFormulario(new FormSoftware()));
                y += 55;
            }

            AgregarBotonMenu("Tiquetes de Atencion", IconosGenerador.IconoTiquete(ColorTextoClaro), y,
                () => AbrirFormulario(new FormTiquetes(_usuarioActual)));
            y += 55;

            AgregarBotonMenu("Notificaciones", IconosGenerador.IconoCampana(ColorTextoClaro), y,
                () => AbrirFormulario(new FormNotificaciones(_usuarioActual)));
            y += 55;

            if (_usuarioActual.Rol == "Administrador TI")
            {
                AgregarBotonMenu("Mantenimiento de Usuarios", IconosGenerador.IconoUsuario(ColorTextoClaro), y,
                    () => AbrirFormulario(new FormUsuarios()));
                y += 55;
            }

            AgregarBotonMenu("Acerca de...", IconosGenerador.IconoInfo(ColorTextoClaro), y, () =>
            {
                using (var form = new FormAcercaDe())
                    form.ShowDialog();
            });

            // Boton Salir, anclado abajo del todo
            var btnSalir = CrearBoton("Salir", IconosGenerador.IconoSalir(Color.FromArgb(240, 140, 140)));
            btnSalir.Dock = DockStyle.Bottom;
            btnSalir.Top = 0;
            btnSalir.Click += (s, e) => Application.Exit();
            barraLateral.Controls.Add(btnSalir);

            this.Controls.Add(barraLateral);
        }

        private void AgregarBotonMenu(string texto, Image icono, int top, Action alHacerClic)
        {
            var boton = CrearBoton(texto, icono);
            boton.Top = top;
            boton.Click += (s, e) => alHacerClic();
            barraLateral.Controls.Add(boton);
        }

        private Button CrearBoton(string texto, Image icono)
        {
            var boton = new Button
            {
                Text = "   " + texto,
                Image = icono,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Padding = new Padding(15, 0, 0, 0),
                Left = 0,
                Width = 230,
                Height = 48,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorTextoClaro,
                BackColor = ColorBarraLateral,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            boton.FlatAppearance.BorderSize = 0;
            boton.FlatAppearance.MouseOverBackColor = ColorBarraLateralHover;
            return boton;
        }

        // ---------------- Barra superior (usuario, indicadores, fecha/hora) ----------------

        private void ConstruirBarraSuperior()
        {
            barraSuperior = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.White };

            var lblBienvenida = new Label
            {
                Text = $"Bienvenido, {_usuarioActual.NombreCompleto}  ({_usuarioActual.Rol})",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 42, 65),
                AutoSize = true,
                Left = 25,
                Top = 22
            };
            barraSuperior.Controls.Add(lblBienvenida);

            // El ancho del saludo varia segun el nombre y el rol del usuario, asi que las
            // tarjetas de indicadores arrancan despues del texto medido, no en una posicion fija
            // (evita que se encimen con nombres largos, como paso antes).
            int anchoTexto = TextRenderer.MeasureText(lblBienvenida.Text, lblBienvenida.Font).Width;
            int inicioTarjetas = lblBienvenida.Left + anchoTexto + 40;

            lblFecha = new Label { AutoSize = true, Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, Top = 20 };
            lblHora = new Label { AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray, Top = 36 };
            barraSuperior.Controls.Add(lblFecha);
            barraSuperior.Controls.Add(lblHora);
            barraSuperior.Resize += (s, e) => PosicionarReloj();

            AgregarTarjetasIndicadores(inicioTarjetas);

            this.Controls.Add(barraSuperior);
            PosicionarReloj();
        }

        private void PosicionarReloj()
        {
            lblFecha.Left = barraSuperior.Width - 150;
            lblHora.Left = barraSuperior.Width - 150;
        }

        /// <summary>
        /// Pequenas tarjetas de indicadores (estilo "dashboard") con conteos rapidos,
        /// para que la pantalla principal se vea mas informativa y visual.
        /// </summary>
        private void AgregarTarjetasIndicadores(int inicioTarjetas)
        {
            try
            {
                var repositorioTiquetes = new TiqueteRepositorio();
                int pendientes = repositorioTiquetes.ContarPendientes();

                var repositorioComputadoras = new ComputadoraRepositorio();
                int totalComputadoras = repositorioComputadoras.ObtenerTodas().Rows.Count;

                var repositorioSoftware = new SoftwareRepositorio();
                int totalSoftware = repositorioSoftware.ObtenerTodos().Rows.Count;

                CrearTarjeta("Tiquetes pendientes", pendientes.ToString(), Color.FromArgb(230, 126, 34), inicioTarjetas);
                CrearTarjeta("Computadoras", totalComputadoras.ToString(), Color.FromArgb(41, 128, 185), inicioTarjetas + 180);
                CrearTarjeta("Software registrado", totalSoftware.ToString(), Color.FromArgb(39, 174, 96), inicioTarjetas + 360);
            }
            catch
            {
                // Si falla el conteo (por ejemplo, sin conexion momentanea), simplemente no se muestran las tarjetas.
            }
        }

        private void CrearTarjeta(string titulo, string valor, Color color, int left)
        {
            var tarjeta = new Panel { Left = left, Top = 10, Width = 160, Height = 45, BackColor = color };
            var lblValor = new Label
            {
                Text = valor,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Left = 10,
                Top = 2,
                AutoSize = true
            };
            var lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.White,
                Left = 10,
                Top = 26,
                AutoSize = true
            };
            tarjeta.Controls.Add(lblValor);
            tarjeta.Controls.Add(lblTitulo);
            barraSuperior.Controls.Add(tarjeta);
        }

        // ---------------- Utilidades ----------------

        private void AbrirFormulario(Form formulario)
        {
            formulario.MdiParent = this;
            formulario.WindowState = FormWindowState.Maximized;
            formulario.Show();
        }

        private void ActualizarFechaHora()
        {
            lblFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lblHora.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        /// <summary>
        /// El fondo NO se agrega como control normal (eso tapa las ventanas hijas en un
        /// formulario MDI); se asigna directamente al MdiClient interno.
        /// </summary>
        private void AsignarFondoMdi()
        {
            foreach (Control control in this.Controls)
            {
                if (control is MdiClient mdiClient)
                {
                    mdiClient.BackColor = Color.FromArgb(240, 243, 248);
                    mdiClient.BackgroundImage = GenerarImagenFondo(1600, 900);
                    mdiClient.BackgroundImageLayout = ImageLayout.Stretch;
                    break;
                }
            }
        }

        /// <summary>
        /// Al iniciar sesion, si hay tiquetes que atender (pendientes para el Administrador,
        /// asignados para el Tecnico), abre directamente la pantalla de Notificaciones.
        /// </summary>
        private void MostrarNotificacionesDeIngreso()
        {
            try
            {
                var repositorioTiquetes = new TiqueteRepositorio();
                bool hayNotificaciones = false;

                if (_usuarioActual.Rol == "Administrador TI")
                    hayNotificaciones = repositorioTiquetes.ContarPendientes() > 0;
                else if (_usuarioActual.Rol == "Tecnico TI")
                    hayNotificaciones = repositorioTiquetes.ContarAsignadosATecnico(_usuarioActual.Cedula) > 0;

                if (hayNotificaciones)
                    AbrirFormulario(new FormNotificaciones(_usuarioActual));
            }
            catch
            {
                // Si falla la notificacion (por ejemplo, sin conexion momentanea) no se interrumpe el ingreso.
            }
        }

        private static Image GenerarImagenFondo(int ancho, int alto)
        {
            var bitmap = new Bitmap(ancho, alto);
            using (var g = Graphics.FromImage(bitmap))
            {
                using (var fondoDegradado = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Rectangle(0, 0, ancho, alto), Color.FromArgb(240, 243, 248), Color.FromArgb(222, 230, 242),
                    System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
                {
                    g.FillRectangle(fondoDegradado, 0, 0, ancho, alto);
                }

                using (var fuente = new Font("Segoe UI", 40, FontStyle.Bold))
                using (var pincelTexto = new SolidBrush(Color.FromArgb(35, 70, 90, 120)))
                {
                    g.DrawString("Sistema de Gestion de TI", fuente, pincelTexto, ancho / 2f - 400, alto / 2f - 30);
                }
            }
            return bitmap;
        }
    }
}
