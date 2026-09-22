using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ProyectoFinalPIV.Datos;
using ProyectoFinalPIV.Modelos;
using ProyectoFinalPIV.Utilidades;

namespace ProyectoFinalPIV.Formularios
{
    public class FormNotificaciones : Form
    {
        private readonly Usuario _usuarioActual;
        private readonly TiqueteRepositorio _repositorio = new TiqueteRepositorio();
        private FlowLayoutPanel panelLista;

        public FormNotificaciones(Usuario usuarioActual)
        {
            _usuarioActual = usuarioActual;
            InicializarComponentes();
            CargarNotificaciones();
        }

        private void InicializarComponentes()
        {
            this.Text = "Notificaciones";
            this.Width = 800;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 243, 248);

            var encabezado = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = EstiloUI.Navy };
            var lblTitulo = new Label
            {
                Text = "🔔 Notificaciones",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Left = 20,
                Top = 15,
                AutoSize = true
            };
            encabezado.Controls.Add(lblTitulo);

            var btnRefrescar = new Button { Text = "Refrescar", Width = 100, Height = 32, Top = 14 };
            btnRefrescar.Left = this.ClientSize.Width - btnRefrescar.Width - 20;
            btnRefrescar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefrescar.Click += (s, e) => CargarNotificaciones();
            EstiloUI.Estilizar(btnRefrescar, EstiloUI.Azul);
            encabezado.Controls.Add(btnRefrescar);

            panelLista = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(15),
                BackColor = Color.FromArgb(240, 243, 248)
            };

            this.Controls.Add(panelLista);
            this.Controls.Add(encabezado);
        }

        private void CargarNotificaciones()
        {
            panelLista.Controls.Clear();

            try
            {
                if (_usuarioActual.Rol == "Administrador TI")
                {
                    var pendientes = _repositorio.ObtenerPendientesDetalle();
                    MostrarTitulo($"Tiquetes pendientes de asignar ({pendientes.Rows.Count})");
                    if (pendientes.Rows.Count == 0)
                        MostrarVacio("No hay tiquetes pendientes de asignar. ¡Al día!");
                    foreach (DataRow fila in pendientes.Rows)
                        AgregarTarjeta(
                            $"Tiquete #{fila["Id_Tiquete"]} — {fila["TipoIncidencia"]}",
                            $"Empleado: {fila["Empleado"]}\n{fila["Descripcion"]}\nCreado: {fila["FechaCreacion"]:dd/MM/yyyy hh:mm tt}",
                            EstiloUI.Naranja);
                }
                else if (_usuarioActual.Rol == "Tecnico TI")
                {
                    var asignados = _repositorio.ObtenerAsignadosDetalle(_usuarioActual.Cedula);
                    MostrarTitulo($"Tiquetes asignados a mí pendientes de atender ({asignados.Rows.Count})");
                    if (asignados.Rows.Count == 0)
                        MostrarVacio("No tiene tiquetes pendientes de atender. ¡Al día!");
                    foreach (DataRow fila in asignados.Rows)
                        AgregarTarjeta(
                            $"Tiquete #{fila["Id_Tiquete"]} — {fila["TipoIncidencia"]} ({fila["Estado"]})",
                            $"Empleado: {fila["Empleado"]}\n{fila["Descripcion"]}\nCreado: {fila["FechaCreacion"]:dd/MM/yyyy hh:mm tt}",
                            fila["Estado"].ToString() == "En Atencion" ? EstiloUI.Morado : EstiloUI.Azul);
                }
                else // Empleado: estado de sus propios tiquetes
                {
                    var propios = _repositorio.ObtenerPorEmpleado(_usuarioActual.Cedula);
                    MostrarTitulo($"Estado de mis tiquetes ({propios.Rows.Count})");
                    if (propios.Rows.Count == 0)
                        MostrarVacio("Aún no ha generado ningún tiquete.");
                    foreach (DataRow fila in propios.Rows)
                        AgregarTarjeta(
                            $"Tiquete #{fila["Id_Tiquete"]} — {fila["Estado"]}",
                            $"{fila["Descripcion"]}\nCreado: {fila["FechaCreacion"]:dd/MM/yyyy hh:mm tt}",
                            ColorPorEstado(fila["Estado"].ToString()));
                }
            }
            catch (Exception ex)
            {
                MostrarVacio("No fue posible cargar las notificaciones.\n" + ex.Message);
            }
        }

        private static Color ColorPorEstado(string estado) => estado switch
        {
            "Pendiente" => EstiloUI.Naranja,
            "Asignado" => EstiloUI.Azul,
            "En Atencion" => EstiloUI.Morado,
            "Resuelto" => EstiloUI.Verde,
            "Cerrado" => EstiloUI.Gris,
            _ => EstiloUI.Gris
        };

        private void MostrarTitulo(string texto)
        {
            panelLista.Controls.Add(new Label
            {
                Text = texto,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = EstiloUI.Navy,
                AutoSize = true,
                Margin = new Padding(5, 10, 5, 10)
            });
        }

        private void MostrarVacio(string texto)
        {
            panelLista.Controls.Add(new Label
            {
                Text = texto,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Margin = new Padding(5, 5, 5, 15)
            });
        }

        private void AgregarTarjeta(string titulo, string detalle, Color color)
        {
            var tarjeta = new Panel
            {
                Width = panelLista.ClientSize.Width - 40,
                Height = 90,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 10)
            };

            var franja = new Panel { Dock = DockStyle.Left, Width = 8, BackColor = color };

            var lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = EstiloUI.Navy,
                Left = 25,
                Top = 10,
                AutoSize = true
            };
            var lblDetalle = new Label
            {
                Text = detalle,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(70, 70, 70),
                Left = 25,
                Top = 32,
                Width = tarjeta.Width - 50,
                Height = 50
            };

            tarjeta.Controls.Add(lblDetalle);
            tarjeta.Controls.Add(lblTitulo);
            tarjeta.Controls.Add(franja);
            panelLista.Controls.Add(tarjeta);
        }
    }
}
