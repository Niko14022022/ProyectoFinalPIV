using System;
using System.Data;
using System.Windows.Forms;
using ProyectoFinalPIV.Datos;
using ProyectoFinalPIV.Modelos;
using ProyectoFinalPIV.Utilidades;

namespace ProyectoFinalPIV.Formularios
{
    public class FormTiquetes : Form
    {
        private readonly Usuario _usuarioActual;
        private readonly TiqueteRepositorio _repositorio = new TiqueteRepositorio();
        private DataGridView grid;

        // Empleado: crear tiquete
        private GroupBox panelCrear;
        private ComboBox cmbTipoIncidencia;
        private TextBox txtDescripcion;
        private Button btnCrearTiquete;

        // Administrador: asignar tecnico
        private GroupBox panelAsignar;
        private NumericUpDown txtIdTiqueteAsignar;
        private ComboBox cmbTecnicoAsignar;
        private Button btnAsignarTecnico;

        // Tecnico / Administrador: cambiar estado
        private GroupBox panelEstado;
        private NumericUpDown txtIdTiqueteEstado;
        private ComboBox cmbNuevoEstado;
        private Button btnCambiarEstado;

        // Administrador: actualizar descripcion / eliminar tiquete
        private GroupBox panelAdministrar;
        private NumericUpDown txtIdTiqueteAdmin;
        private TextBox txtNuevaDescripcion;
        private Button btnActualizarDescripcion;
        private Button btnEliminarTiquete;

        private Button btnConsultar;
        private ComboBox cmbReportes;
        private Button btnGenerarReporte;

        public FormTiquetes(Usuario usuarioActual)
        {
            _usuarioActual = usuarioActual;
            InicializarComponentes();
            try
            {
                AjustarSegunRol();
                Consultar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible cargar los datos iniciales.\n\nDetalle: " + ex.Message,
                    "Error de conexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InicializarComponentes()
        {
            this.Text = "Elaboracion de Tiquetes de Atencion";
            this.Width = 950;
            this.Height = 840;
            this.StartPosition = FormStartPosition.CenterParent;

            // ---- Panel Empleado: crear tiquete ----
            panelCrear = new GroupBox { Text = "Generar tiquete de incidencia", Left = 10, Top = 10, Width = 910, Height = 100 };
            EstiloUI.Estilizar(panelCrear);
            var lblTipoIncidencia = new Label { Text = "Tipo de incidencia:", Left = 10, Top = 27, Width = 110 };
            cmbTipoIncidencia = new ComboBox { Left = 125, Top = 24, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTipoIncidencia.Items.AddRange(new object[] { "Hardware", "Software", "Red", "Cuenta de usuario", "Otro" });
            cmbTipoIncidencia.SelectedIndex = 0;
            var lblDescripcion = new Label { Text = "Descripcion:", Left = 10, Top = 62, Width = 110 };
            txtDescripcion = new TextBox { Left = 125, Top = 59, Width = 575, Height = 30, Multiline = true };
            btnCrearTiquete = new Button { Text = "Generar tiquete", Left = 720, Top = 40, Width = 160 };
            btnCrearTiquete.Click += (s, e) => CrearTiquete();
            EstiloUI.Estilizar(btnCrearTiquete, EstiloUI.Verde);
            panelCrear.Controls.AddRange(new Control[] { lblTipoIncidencia, cmbTipoIncidencia, lblDescripcion, txtDescripcion, btnCrearTiquete });

            // ---- Panel Administrador: asignar tecnico ----
            panelAsignar = new GroupBox { Text = "Asignar tecnico a un tiquete", Left = 10, Top = 115, Width = 910, Height = 65 };
            EstiloUI.Estilizar(panelAsignar);
            var lblIdAsignar = new Label { Text = "Id tiquete:", Left = 10, Top = 27, Width = 70 };
            txtIdTiqueteAsignar = new NumericUpDown { Left = 85, Top = 24, Width = 60, Minimum = 1, Maximum = 999999 };
            var lblTecnico = new Label { Text = "Tecnico:", Left = 165, Top = 27, Width = 60 };
            cmbTecnicoAsignar = new ComboBox { Left = 230, Top = 24, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            btnAsignarTecnico = new Button { Text = "Asignar", Left = 500, Top = 22, Width = 120 };
            btnAsignarTecnico.Click += (s, e) => AsignarTecnico();
            EstiloUI.Estilizar(btnAsignarTecnico, EstiloUI.Morado);
            panelAsignar.Controls.AddRange(new Control[] { lblIdAsignar, txtIdTiqueteAsignar, lblTecnico, cmbTecnicoAsignar, btnAsignarTecnico });

            // ---- Panel Tecnico/Administrador: cambiar estado ----
            panelEstado = new GroupBox { Text = "Cambiar estado / atender incidencia", Left = 10, Top = 185, Width = 910, Height = 65 };
            EstiloUI.Estilizar(panelEstado);
            var lblIdEstado = new Label { Text = "Id tiquete:", Left = 10, Top = 27, Width = 70 };
            txtIdTiqueteEstado = new NumericUpDown { Left = 85, Top = 24, Width = 60, Minimum = 1, Maximum = 999999 };
            var lblEstado = new Label { Text = "Nuevo estado:", Left = 165, Top = 27, Width = 90 };
            cmbNuevoEstado = new ComboBox { Left = 260, Top = 24, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbNuevoEstado.Items.AddRange(new object[] { Tiquete.EnAtencion, Tiquete.Resuelto, Tiquete.Cerrado });
            btnCambiarEstado = new Button { Text = "Actualizar estado", Left = 460, Top = 22, Width = 160 };
            btnCambiarEstado.Click += (s, e) => CambiarEstado();
            EstiloUI.Estilizar(btnCambiarEstado, EstiloUI.Naranja);
            panelEstado.Controls.AddRange(new Control[] { lblIdEstado, txtIdTiqueteEstado, lblEstado, cmbNuevoEstado, btnCambiarEstado });

            // ---- Panel Administrador: actualizar descripcion / eliminar tiquete ----
            panelAdministrar = new GroupBox { Text = "Actualizar o eliminar un tiquete", Left = 10, Top = 255, Width = 910, Height = 90 };
            EstiloUI.Estilizar(panelAdministrar);
            var lblIdAdmin = new Label { Text = "Id tiquete:", Left = 10, Top = 27, Width = 70 };
            txtIdTiqueteAdmin = new NumericUpDown { Left = 85, Top = 24, Width = 60, Minimum = 1, Maximum = 999999 };
            var lblNuevaDescripcion = new Label { Text = "Nueva descripcion:", Left = 165, Top = 27, Width = 120 };
            txtNuevaDescripcion = new TextBox { Left = 290, Top = 24, Width = 400 };
            btnActualizarDescripcion = new Button { Text = "Actualizar descripcion", Left = 700, Top = 22, Width = 180 };
            btnActualizarDescripcion.Click += (s, e) => ActualizarDescripcion();
            EstiloUI.Estilizar(btnActualizarDescripcion, EstiloUI.Azul);
            btnEliminarTiquete = new Button { Text = "Eliminar (cancelar) tiquete", Left = 85, Top = 55, Width = 220 };
            btnEliminarTiquete.Click += (s, e) => EliminarTiquete();
            EstiloUI.Estilizar(btnEliminarTiquete, EstiloUI.Rojo);
            panelAdministrar.Controls.AddRange(new Control[] {
                lblIdAdmin, txtIdTiqueteAdmin, lblNuevaDescripcion, txtNuevaDescripcion,
                btnActualizarDescripcion, btnEliminarTiquete
            });

            btnConsultar = new Button { Text = "Consultar / refrescar", Left = 10, Top = 355, Width = 160 };
            btnConsultar.Click += (s, e) => Consultar();
            EstiloUI.Estilizar(btnConsultar, EstiloUI.Gris);

            grid = new DataGridView
            {
                Left = 10,
                Top = 390,
                Width = 910,
                Height = 300,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(27, 42, 65);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            grid.CellFormatting += Grid_CellFormatting;
            grid.CellClick += Grid_CellClick;

            var panelReportes = new GroupBox { Text = "Reportes (Administrador TI)", Left = 10, Top = 700, Width = 910, Height = 90 };
            EstiloUI.Estilizar(panelReportes);
            var lblReporte = new Label { Text = "Seleccione un reporte:", Left = 10, Top = 30, Width = 140 };
            cmbReportes = new ComboBox { Left = 155, Top = 27, Width = 460, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbReportes.Items.AddRange(new object[] {
                "1. Tiquetes pendientes",
                "2. Cantidad de tiquetes por tecnico",
                "3. Cantidad de tiquetes por estado",
                "4. Tiempo promedio de atencion (horas) por tecnico",
                "5. Historial completo de tiquetes"
            });
            cmbReportes.SelectedIndex = 0;
            btnGenerarReporte = new Button { Text = "Generar reporte", Left = 625, Top = 25, Width = 140 };
            btnGenerarReporte.Click += (s, e) => GenerarReporte();
            EstiloUI.Estilizar(btnGenerarReporte, EstiloUI.Navy);
            panelReportes.Controls.AddRange(new Control[] { lblReporte, cmbReportes, btnGenerarReporte });

            this.Controls.Add(panelCrear);
            this.Controls.Add(panelAsignar);
            this.Controls.Add(panelEstado);
            this.Controls.Add(panelAdministrar);
            this.Controls.Add(btnConsultar);
            this.Controls.Add(grid);
            this.Controls.Add(panelReportes);
        }

        /// <summary>
        /// Muestra/oculta los paneles segun el rol del usuario logueado,
        /// tal como pide el enunciado (Empleado / Tecnico TI / Administrador TI).
        /// </summary>
        private void AjustarSegunRol()
        {
            bool esEmpleado = _usuarioActual.Rol == "Empleado";
            bool esTecnico = _usuarioActual.Rol == "Tecnico TI";
            bool esAdmin = _usuarioActual.Rol == "Administrador TI";

            panelCrear.Visible = esEmpleado || esAdmin;
            panelAsignar.Visible = esAdmin;
            panelEstado.Visible = esTecnico || esAdmin;
            panelAdministrar.Visible = esAdmin;

            if (esAdmin)
            {
                CargarTecnicos();
            }
        }

        private void CargarTecnicos()
        {
            cmbTecnicoAsignar.Items.Clear();
            var usuarios = new UsuarioRepositorio().ObtenerTodos();
            foreach (var u in usuarios)
                if (u.Rol == "Tecnico TI")
                    cmbTecnicoAsignar.Items.Add(u.Cedula + " - " + u.NombreCompleto);
        }

        private void Consultar()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                DataTable resultado = _usuarioActual.Rol switch
                {
                    "Empleado" => _repositorio.ObtenerPorEmpleado(_usuarioActual.Cedula),
                    "Tecnico TI" => _repositorio.ObtenerPorTecnico(_usuarioActual.Cedula),
                    _ => _repositorio.ObtenerTodos()
                };
                grid.DataSource = resultado;
                AplicarEncabezadosAmigables();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (grid.Columns[e.ColumnIndex].Name != "Estado" || e.Value == null) return;

            System.Drawing.Color color = e.Value.ToString() switch
            {
                "Pendiente" => System.Drawing.Color.FromArgb(230, 126, 34),
                "Asignado" => System.Drawing.Color.FromArgb(41, 128, 185),
                "En Atencion" => System.Drawing.Color.FromArgb(142, 68, 173),
                "Resuelto" => System.Drawing.Color.FromArgb(39, 174, 96),
                "Cerrado" => System.Drawing.Color.FromArgb(120, 120, 120),
                _ => System.Drawing.Color.Gray
            };

            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = color;
            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = System.Drawing.Color.White;
            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void AplicarEncabezadosAmigables()
        {
            RenombrarColumna("Id_Tiquete", "ID");
            RenombrarColumna("Empleado", "Empleado");
            RenombrarColumna("Tecnico", "Tecnico");
            RenombrarColumna("TipoIncidencia", "Tipo de incidencia");
            RenombrarColumna("Descripcion", "Descripcion");
            RenombrarColumna("Estado", "Estado");
            RenombrarColumna("FechaCreacion", "Fecha creacion");
            RenombrarColumna("FechaAtencion", "Fecha atencion");
        }

        private void RenombrarColumna(string nombreColumna, string encabezado)
        {
            if (grid.Columns.Contains(nombreColumna))
                grid.Columns[nombreColumna].HeaderText = encabezado;
        }

        private void GenerarReporte()
        {
            if (_usuarioActual.Rol != "Administrador TI")
            {
                MessageBox.Show("Solo el Administrador de TI puede generar estos reportes.", "Acceso restringido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable resultado = cmbReportes.SelectedIndex switch
            {
                0 => _repositorio.ReporteTiquetesPendientes(),
                1 => _repositorio.ReportePorTecnico(),
                2 => _repositorio.ReportePorEstado(),
                3 => _repositorio.ReporteTiempoPromedioAtencion(),
                4 => _repositorio.ReporteHistorialCompleto(),
                _ => _repositorio.ObtenerTodos()
            };
            grid.DataSource = resultado;
            AplicarEncabezadosAmigables();
        }

        private void CrearTiquete()
        {
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("Describa la incidencia.", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                _repositorio.CrearTiquete(_usuarioActual.Cedula, cmbTipoIncidencia.SelectedItem.ToString(), txtDescripcion.Text.Trim());
                MessageBox.Show("Tiquete generado. El administrador de TI sera notificado.", "Listo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtDescripcion.Clear();
                Consultar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el tiquete: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AsignarTecnico()
        {
            if (cmbTecnicoAsignar.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un tecnico.", "Datos incompletos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int idTiquete = (int)txtIdTiqueteAsignar.Value;
            try
            {
                string cedulaTecnico = cmbTecnicoAsignar.SelectedItem.ToString().Split(' ')[0];
                _repositorio.AsignarTecnico(idTiquete, cedulaTecnico);
                MessageBox.Show("Tecnico asignado. Se le notificara la nueva incidencia.", "Listo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtIdTiqueteAsignar.Value = txtIdTiqueteAsignar.Minimum;
                Consultar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asignar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CambiarEstado()
        {
            if (cmbNuevoEstado.SelectedItem == null)
            {
                MessageBox.Show("Seleccione el nuevo estado.", "Datos incompletos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int idTiquete = (int)txtIdTiqueteEstado.Value;
            try
            {
                _repositorio.CambiarEstado(idTiquete, cmbNuevoEstado.SelectedItem.ToString());
                MessageBox.Show("Estado actualizado.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtIdTiqueteEstado.Value = txtIdTiqueteEstado.Minimum;
                Consultar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar el estado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Al hacer clic en una fila de la tabla, autocompleta el Id de tiquete en los
        /// paneles de asignar/cambiar estado/administrar, y la descripcion actual en el
        /// panel de administrar, para no tener que escribir el numero a mano.
        /// </summary>
        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !grid.Columns.Contains("Id_Tiquete")) return;
            var fila = grid.Rows[e.RowIndex];
            if (fila.Cells["Id_Tiquete"].Value == null) return;

            int id = Convert.ToInt32(fila.Cells["Id_Tiquete"].Value);

            if (panelAsignar.Visible)
                txtIdTiqueteAsignar.Value = Math.Min(Math.Max(id, (int)txtIdTiqueteAsignar.Minimum), (int)txtIdTiqueteAsignar.Maximum);
            if (panelEstado.Visible)
                txtIdTiqueteEstado.Value = Math.Min(Math.Max(id, (int)txtIdTiqueteEstado.Minimum), (int)txtIdTiqueteEstado.Maximum);
            if (panelAdministrar.Visible)
            {
                txtIdTiqueteAdmin.Value = Math.Min(Math.Max(id, (int)txtIdTiqueteAdmin.Minimum), (int)txtIdTiqueteAdmin.Maximum);
                if (grid.Columns.Contains("Descripcion") && fila.Cells["Descripcion"].Value != null)
                    txtNuevaDescripcion.Text = fila.Cells["Descripcion"].Value.ToString();
            }
        }

        private void ActualizarDescripcion()
        {
            if (string.IsNullOrWhiteSpace(txtNuevaDescripcion.Text))
            {
                MessageBox.Show("Indique la nueva descripcion.", "Datos incompletos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int idTiquete = (int)txtIdTiqueteAdmin.Value;
            try
            {
                _repositorio.ActualizarDescripcion(idTiquete, txtNuevaDescripcion.Text.Trim());
                MessageBox.Show("Descripcion actualizada.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtIdTiqueteAdmin.Value = txtIdTiqueteAdmin.Minimum;
                txtNuevaDescripcion.Clear();
                Consultar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarTiquete()
        {
            int idTiquete = (int)txtIdTiqueteAdmin.Value;
            if (MessageBox.Show("¿Confirma eliminar (cancelar) este tiquete?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                _repositorio.Eliminar(idTiquete);
                txtIdTiqueteAdmin.Value = txtIdTiqueteAdmin.Minimum;
                txtNuevaDescripcion.Clear();
                Consultar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
