using System;
using System.Data;
using System.Windows.Forms;
using ProyectoFinalPIV.Datos;
using ProyectoFinalPIV.Modelos;
using ProyectoFinalPIV.Utilidades;

namespace ProyectoFinalPIV.Formularios
{
    public class FormSoftware : Form
    {
        private readonly SoftwareRepositorio _repositorio = new SoftwareRepositorio();
        private readonly ComputadoraRepositorio _repositorioComputadoras = new ComputadoraRepositorio();
        private DataGridView grid;
        private TextBox txtVersion;
        private TextBox txtObservaciones;
        private ComboBox cmbNombreSoftware;
        private NumericUpDown txtStock;
        private DateTimePicker dtpFechaInstalacion;
        private ComboBox cmbTecnico, cmbTipoLicenciamiento;
        private Button btnIncluir, btnActualizar, btnEliminar, btnConsultar, btnLimpiar;
        private ComboBox cmbReportes, cmbComputadoraAsignar, cmbSoftwareAsignar;
        private Button btnGenerarReporte, btnAsignar;
        private TextBox txtBuscar;
        private int? idSeleccionado = null;

        public FormSoftware()
        {
            InicializarComponentes();
            try
            {
                CargarTecnicos();
                CargarCombosAsignacion();
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
            this.Text = "Control de Software";
            this.Width = 970;
            this.Height = 735;
            this.StartPosition = FormStartPosition.CenterParent;

            var panelDatos = new GroupBox { Text = "Datos del software", Left = 10, Top = 10, Width = 930, Height = 180 };
            EstiloUI.Estilizar(panelDatos);

            var lblNombre = new Label { Text = "Nombre:", Left = 10, Top = 25, Width = 80 };
            cmbNombreSoftware = new ComboBox { Left = 100, Top = 22, Width = 180, DropDownStyle = ComboBoxStyle.DropDown };
            cmbNombreSoftware.Items.AddRange(new object[] {
                "Microsoft Office", "Windows 11", "Adobe Acrobat Reader", "Antivirus Corporativo",
                "Google Chrome", "Zoom", "Visual Studio", "SQL Server Management Studio"
            });

            var lblVersion = new Label { Text = "Version:", Left = 300, Top = 25, Width = 60 };
            txtVersion = new TextBox { Left = 365, Top = 22, Width = 100 };

            var lblFecha = new Label { Text = "Fecha instalacion:", Left = 480, Top = 25, Width = 110 };
            dtpFechaInstalacion = new DateTimePicker { Left = 595, Top = 22, Width = 150, Format = DateTimePickerFormat.Short };

            var lblTecnico = new Label { Text = "Tecnico:", Left = 10, Top = 60, Width = 80 };
            cmbTecnico = new ComboBox { Left = 100, Top = 57, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblTipo = new Label { Text = "Licenciamiento:", Left = 335, Top = 60, Width = 100 };
            cmbTipoLicenciamiento = new ComboBox { Left = 440, Top = 57, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTipoLicenciamiento.Items.AddRange(new object[] { "Con Licencia", "Freeware", "Shareware", "Otro" });

            var lblStock = new Label { Text = "Licencias disp.:", Left = 610, Top = 60, Width = 100 };
            txtStock = new NumericUpDown { Left = 715, Top = 57, Width = 70, Minimum = 0, Maximum = 9999 };

            var lblObservaciones = new Label { Text = "Como se instalo:", Left = 10, Top = 95, Width = 110 };
            txtObservaciones = new TextBox { Left = 125, Top = 92, Width = 780 };

            btnIncluir = new Button { Text = "Incluir", Left = 100, Top = 135, Width = 90 };
            btnIncluir.Click += (s, e) => Incluir();
            EstiloUI.Estilizar(btnIncluir, EstiloUI.Verde);

            btnActualizar = new Button { Text = "Actualizar", Left = 200, Top = 135, Width = 90, Enabled = false };
            btnActualizar.Click += (s, e) => Actualizar();
            EstiloUI.Estilizar(btnActualizar, EstiloUI.Naranja);

            btnEliminar = new Button { Text = "Eliminar", Left = 300, Top = 135, Width = 90, Enabled = false };
            btnEliminar.Click += (s, e) => Eliminar();
            EstiloUI.Estilizar(btnEliminar, EstiloUI.Rojo);

            btnConsultar = new Button { Text = "Consultar (refrescar)", Left = 400, Top = 135, Width = 140 };
            btnConsultar.Click += (s, e) => Consultar();
            EstiloUI.Estilizar(btnConsultar, EstiloUI.Azul);

            btnLimpiar = new Button { Text = "Limpiar", Left = 550, Top = 135, Width = 90 };
            btnLimpiar.Click += (s, e) => LimpiarCampos();
            EstiloUI.Estilizar(btnLimpiar, EstiloUI.Gris);

            panelDatos.Controls.AddRange(new Control[] {
                lblNombre, cmbNombreSoftware, lblVersion, txtVersion, lblFecha, dtpFechaInstalacion,
                lblTecnico, cmbTecnico, lblTipo, cmbTipoLicenciamiento, lblStock, txtStock,
                lblObservaciones, txtObservaciones,
                btnIncluir, btnActualizar, btnEliminar, btnConsultar, btnLimpiar
            });

            var panelAsignacion = new GroupBox { Text = "Asignar software a una computadora", Left = 10, Top = 195, Width = 930, Height = 65 };
            EstiloUI.Estilizar(panelAsignacion);
            var lblComp = new Label { Text = "Computadora:", Left = 10, Top = 27, Width = 90 };
            cmbComputadoraAsignar = new ComboBox { Left = 105, Top = 24, Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
            var lblSoft = new Label { Text = "Software:", Left = 380, Top = 27, Width = 70 };
            cmbSoftwareAsignar = new ComboBox { Left = 455, Top = 24, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            btnAsignar = new Button { Text = "Asignar", Left = 770, Top = 22, Width = 100 };
            btnAsignar.Click += (s, e) => Asignar();
            EstiloUI.Estilizar(btnAsignar, EstiloUI.Morado);
            panelAsignacion.Controls.AddRange(new Control[] { lblComp, cmbComputadoraAsignar, lblSoft, cmbSoftwareAsignar, btnAsignar });

            var lblBuscar = new Label { Text = "Buscar (nombre):", Left = 10, Top = 245, Width = 120 };
            txtBuscar = new TextBox { Left = 135, Top = 242, Width = 250 };
            txtBuscar.TextChanged += (s, e) => FiltrarGrid();

            grid = new DataGridView
            {
                Left = 10,
                Top = 270,
                Width = 930,
                Height = 290,
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

            var panelReportes = new GroupBox { Text = "Reportes", Left = 10, Top = 595, Width = 930, Height = 90 };
            EstiloUI.Estilizar(panelReportes);
            var lblReporte = new Label { Text = "Seleccione un reporte:", Left = 10, Top = 30, Width = 140 };
            cmbReportes = new ComboBox { Left = 155, Top = 27, Width = 460, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbReportes.Items.AddRange(new object[] {
                "1. Listado de todo el software instalado",
                "2. Software instalado en un CPU especifico (usa el combo de arriba)",
                "3. Cantidad de software por tipo de licenciamiento",
                "4. Stock de licencias disponibles",
                "5. Asignaciones software-computadora"
            });
            cmbReportes.SelectedIndex = 0;
            btnGenerarReporte = new Button { Text = "Generar reporte", Left = 625, Top = 25, Width = 140 };
            btnGenerarReporte.Click += (s, e) => GenerarReporte();
            EstiloUI.Estilizar(btnGenerarReporte, EstiloUI.Navy);
            panelReportes.Controls.AddRange(new Control[] { lblReporte, cmbReportes, btnGenerarReporte });

            this.Controls.Add(panelDatos);
            this.Controls.Add(panelAsignacion);
            this.Controls.Add(lblBuscar);
            this.Controls.Add(txtBuscar);
            this.Controls.Add(grid);
            this.Controls.Add(panelReportes);
        }

        private void CargarTecnicos()
        {
            cmbTecnico.Items.Clear();
            cmbTecnico.Items.Add("(Sin asignar)");
            var usuarios = new UsuarioRepositorio().ObtenerTodos();
            foreach (var u in usuarios)
                if (u.Rol == "Tecnico TI" || u.Rol == "Administrador TI")
                    cmbTecnico.Items.Add(u.Cedula + " - " + u.NombreCompleto);
            cmbTecnico.SelectedIndex = 0;
        }

        private void CargarCombosAsignacion()
        {
            cmbComputadoraAsignar.DataSource = _repositorioComputadoras.ObtenerParaCombo();
            cmbComputadoraAsignar.DisplayMember = "Descripcion";
            cmbComputadoraAsignar.ValueMember = "Id_Computadora";

            cmbSoftwareAsignar.DataSource = _repositorio.ObtenerParaCombo();
            cmbSoftwareAsignar.DisplayMember = "Descripcion";
            cmbSoftwareAsignar.ValueMember = "Id_Software";
        }

        private void Asignar()
        {
            if (cmbComputadoraAsignar.SelectedValue == null || cmbSoftwareAsignar.SelectedValue == null)
            {
                MessageBox.Show("Seleccione una computadora y un software.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idComputadora = Convert.ToInt32(cmbComputadoraAsignar.SelectedValue);
            int idSoftware = Convert.ToInt32(cmbSoftwareAsignar.SelectedValue);

            try
            {
                bool exito = _repositorio.AsignarAComputadora(idComputadora, idSoftware);
                if (!exito)
                {
                    MessageBox.Show("No hay licencias disponibles para este software.", "Sin stock",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                MessageBox.Show("Software asignado correctamente.", "Listo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarCombosAsignacion();
                Consultar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asignar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (grid.Columns[e.ColumnIndex].Name != "Estado" || e.Value == null) return;

            bool sinStock = e.Value.ToString() == "SIN LICENCIAS DISPONIBLES";
            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor =
                sinStock ? System.Drawing.Color.FromArgb(192, 57, 43) : System.Drawing.Color.FromArgb(39, 174, 96);
            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = System.Drawing.Color.White;
            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void Consultar()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                grid.DataSource = _repositorio.ObtenerTodos();
                AplicarEncabezadosAmigables();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void AplicarEncabezadosAmigables()
        {
            RenombrarColumna("Id_Software", "ID");
            RenombrarColumna("NombreSoftware", "Nombre");
            RenombrarColumna("Version", "Version");
            RenombrarColumna("FechaInstalacion", "Fecha instalacion");
            RenombrarColumna("Cedula_Tecnico", "Cedula tecnico");
            RenombrarColumna("Tecnico", "Tecnico");
            RenombrarColumna("TipoLicenciamiento", "Licenciamiento");
            RenombrarColumna("LicenciasDisponibles", "Licencias disponibles");
            RenombrarColumna("Observaciones", "Como se instalo");
            RenombrarColumna("DondeInstalado", "Donde esta instalado (computadoras)");
        }

        private void RenombrarColumna(string nombreColumna, string encabezado)
        {
            if (grid.Columns.Contains(nombreColumna))
                grid.Columns[nombreColumna].HeaderText = encabezado;
        }

        private void FiltrarGrid()
        {
            if (grid.DataSource is DataTable tabla)
            {
                string texto = txtBuscar.Text.Replace("'", "''");
                tabla.DefaultView.RowFilter = string.IsNullOrWhiteSpace(texto)
                    ? string.Empty
                    : $"NombreSoftware LIKE '%{texto}%'";
            }
        }

        private void GenerarReporte()
        {
            DataTable resultado;
            switch (cmbReportes.SelectedIndex)
            {
                case 1:
                    if (cmbComputadoraAsignar.SelectedValue == null)
                    {
                        MessageBox.Show("Seleccione una computadora en el combo de asignacion.", "Aviso",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    resultado = _repositorio.ReportePorComputadora(Convert.ToInt32(cmbComputadoraAsignar.SelectedValue));
                    break;
                case 2: resultado = _repositorio.ReportePorTipoLicenciamiento(); break;
                case 3: resultado = _repositorio.ReporteStockDisponible(); break;
                case 4: resultado = _repositorio.ReporteAsignaciones(); break;
                default: resultado = _repositorio.ReporteTodoElSoftware(); break;
            }
            grid.DataSource = resultado;
            AplicarEncabezadosAmigables();
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !grid.Columns.Contains("Id_Software")) return;
            var fila = grid.Rows[e.RowIndex];

            idSeleccionado = Convert.ToInt32(fila.Cells["Id_Software"].Value);
            btnActualizar.Enabled = true;
            btnEliminar.Enabled = true;
            cmbNombreSoftware.Text = fila.Cells["NombreSoftware"].Value?.ToString();
            txtVersion.Text = fila.Cells["Version"].Value?.ToString();
            txtObservaciones.Text = fila.Cells["Observaciones"].Value?.ToString();
            if (fila.Cells["FechaInstalacion"].Value != DBNull.Value && fila.Cells["FechaInstalacion"].Value != null)
                dtpFechaInstalacion.Value = Convert.ToDateTime(fila.Cells["FechaInstalacion"].Value);

            cmbTipoLicenciamiento.SelectedItem = fila.Cells["TipoLicenciamiento"].Value?.ToString();
            if (int.TryParse(fila.Cells["LicenciasDisponibles"].Value?.ToString(), out int stock))
                txtStock.Value = Math.Min(Math.Max(stock, (int)txtStock.Minimum), (int)txtStock.Maximum);

            string cedula = fila.Cells["Cedula_Tecnico"].Value?.ToString();
            cmbTecnico.SelectedIndex = 0;
            if (!string.IsNullOrWhiteSpace(cedula))
            {
                for (int i = 0; i < cmbTecnico.Items.Count; i++)
                {
                    if (cmbTecnico.Items[i].ToString().StartsWith(cedula))
                    {
                        cmbTecnico.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(cmbNombreSoftware.Text) || cmbTipoLicenciamiento.SelectedItem == null)
            {
                MessageBox.Show("Nombre y tipo de licenciamiento son obligatorios.",
                    "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private SoftwareInstalado LeerFormulario()
        {
            string cedula = null;
            if (cmbTecnico.SelectedIndex > 0)
                cedula = cmbTecnico.SelectedItem.ToString().Split(' ')[0];

            return new SoftwareInstalado
            {
                Id_Software = idSeleccionado ?? 0,
                NombreSoftware = cmbNombreSoftware.Text.Trim(),
                Version = string.IsNullOrWhiteSpace(txtVersion.Text) ? null : txtVersion.Text.Trim(),
                FechaInstalacion = dtpFechaInstalacion.Value.Date,
                Cedula_Tecnico = cedula,
                TipoLicenciamiento = cmbTipoLicenciamiento.SelectedItem?.ToString(),
                LicenciasDisponibles = (int)txtStock.Value,
                Observaciones = string.IsNullOrWhiteSpace(txtObservaciones.Text) ? null : txtObservaciones.Text.Trim()
            };
        }

        private void Incluir()
        {
            if (!ValidarCampos()) return;
            try
            {
                _repositorio.Incluir(LeerFormulario());
                MessageBox.Show("Software incluido correctamente.", "Listo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                Consultar();
                CargarCombosAsignacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al incluir: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Actualizar()
        {
            if (idSeleccionado == null)
            {
                MessageBox.Show("Seleccione primero un software de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!ValidarCampos()) return;
            try
            {
                _repositorio.Actualizar(LeerFormulario());
                MessageBox.Show("Software actualizado correctamente.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                Consultar();
                CargarCombosAsignacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Eliminar()
        {
            if (idSeleccionado == null)
            {
                MessageBox.Show("Seleccione primero un software de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("¿Confirma eliminar este software?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                _repositorio.Eliminar(idSeleccionado.Value);
                LimpiarCampos();
                Consultar();
                CargarCombosAsignacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            idSeleccionado = null;
            btnActualizar.Enabled = false;
            btnEliminar.Enabled = false;
            cmbNombreSoftware.Text = "";
            txtVersion.Clear();
            dtpFechaInstalacion.Value = DateTime.Today;
            cmbTecnico.SelectedIndex = 0;
            cmbTipoLicenciamiento.SelectedIndex = -1;
            txtStock.Value = 0;
            txtObservaciones.Clear();
        }
    }
}
