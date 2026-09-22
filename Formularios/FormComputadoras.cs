using System;
using System.Data;
using System.Windows.Forms;
using ProyectoFinalPIV.Datos;
using ProyectoFinalPIV.Modelos;
using ProyectoFinalPIV.Utilidades;

namespace ProyectoFinalPIV.Formularios
{
    public class FormComputadoras : Form
    {
        private readonly ComputadoraRepositorio _repositorio = new ComputadoraRepositorio();
        private readonly Usuario _usuarioActual;
        private DataGridView grid;
        private TextBox txtSerie, txtPatrimonio;
        private ComboBox cmbTipoEquipo, cmbEmpleado, cmbDepartamento, cmbMarca;
        private Button btnIncluir, btnActualizar, btnEliminar, btnConsultar, btnLimpiar, btnAutoSerie, btnAutoPatrimonio;
        private ComboBox cmbReportes;
        private Button btnGenerarReporte;
        private TextBox txtBuscar;
        private int? idSeleccionado = null;

        public FormComputadoras(Usuario usuarioActual)
        {
            _usuarioActual = usuarioActual;
            InicializarComponentes();
            AplicarRestriccionesDeRol();
            try
            {
                CargarEmpleados();
                Consultar();
                txtSerie.Text = _repositorio.SugerirSiguienteSerie();
                txtPatrimonio.Text = _repositorio.SugerirSiguientePatrimonio();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible cargar los datos iniciales.\n\nDetalle: " + ex.Message,
                    "Error de conexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// El Tecnico TI solo puede consultar computadoras (para poder instalarles software);
        /// solo el Administrador TI puede incluir, actualizar o eliminar equipos.
        /// </summary>
        private void AplicarRestriccionesDeRol()
        {
            if (_usuarioActual.Rol == "Administrador TI") return;

            btnIncluir.Enabled = false;
            btnActualizar.Enabled = false;
            btnEliminar.Enabled = false;
            btnAutoSerie.Enabled = false;
            btnAutoPatrimonio.Enabled = false;
            cmbMarca.Enabled = false;
            cmbTipoEquipo.Enabled = false;
            txtSerie.Enabled = false;
            txtPatrimonio.Enabled = false;
            cmbDepartamento.Enabled = false;
            cmbEmpleado.Enabled = false;

            this.Text += " (solo consulta - el Administrador TI gestiona los equipos)";
        }

        private void InicializarComponentes()
        {
            this.Text = "Control de Computadoras";
            this.Width = 950;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterParent;

            var panelDatos = new GroupBox { Text = "Datos del equipo", Left = 10, Top = 10, Width = 910, Height = 140 };

            var lblMarca = new Label { Text = "Marca:", Left = 10, Top = 25, Width = 80 };
            cmbMarca = new ComboBox { Left = 95, Top = 22, Width = 150, DropDownStyle = ComboBoxStyle.DropDown };
            cmbMarca.Items.AddRange(new object[] { "Dell", "HP", "Lenovo", "Apple", "Acer", "Asus" });

            var lblTipo = new Label { Text = "Tipo de equipo:", Left = 260, Top = 25, Width = 100 };
            cmbTipoEquipo = new ComboBox { Left = 365, Top = 22, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTipoEquipo.Items.AddRange(new object[] { "Portatil", "Escritorio", "Servidor" });

            var lblSerie = new Label { Text = "Serie:", Left = 540, Top = 25, Width = 60 };
            txtSerie = new TextBox { Left = 605, Top = 22, Width = 110 };
            btnAutoSerie = new Button { Text = "Auto", Left = 720, Top = 21, Width = 55, Height = 24 };
            btnAutoSerie.Click += (s, e) => txtSerie.Text = _repositorio.SugerirSiguienteSerie();

            var lblPatrimonio = new Label { Text = "Patrimonio:", Left = 10, Top = 60, Width = 80 };
            txtPatrimonio = new TextBox { Left = 95, Top = 57, Width = 100 };
            btnAutoPatrimonio = new Button { Text = "Auto", Left = 200, Top = 56, Width = 55, Height = 24 };
            btnAutoPatrimonio.Click += (s, e) => txtPatrimonio.Text = _repositorio.SugerirSiguientePatrimonio();

            var lblDepartamento = new Label { Text = "Departamento:", Left = 260, Top = 60, Width = 100 };
            cmbDepartamento = new ComboBox { Left = 365, Top = 57, Width = 150, DropDownStyle = ComboBoxStyle.DropDown };
            cmbDepartamento.Items.AddRange(new object[] {
                "Administracion", "Contabilidad", "Recursos Humanos", "Ventas", "Soporte TI", "Bodega"
            });

            var lblEmpleado = new Label { Text = "Empleado a cargo:", Left = 540, Top = 60, Width = 110 };
            cmbEmpleado = new ComboBox { Left = 655, Top = 57, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };

            btnIncluir = new Button { Text = "Incluir", Left = 95, Top = 95, Width = 90 };
            btnIncluir.Click += (s, e) => Incluir();
            EstiloUI.Estilizar(btnIncluir, EstiloUI.Verde);

            btnActualizar = new Button { Text = "Actualizar", Left = 195, Top = 95, Width = 90, Enabled = false };
            btnActualizar.Click += (s, e) => Actualizar();
            EstiloUI.Estilizar(btnActualizar, EstiloUI.Naranja);

            btnEliminar = new Button { Text = "Eliminar", Left = 295, Top = 95, Width = 90, Enabled = false };
            btnEliminar.Click += (s, e) => Eliminar();
            EstiloUI.Estilizar(btnEliminar, EstiloUI.Rojo);

            btnConsultar = new Button { Text = "Consultar (refrescar)", Left = 395, Top = 95, Width = 140 };
            btnConsultar.Click += (s, e) => Consultar();
            EstiloUI.Estilizar(btnConsultar, EstiloUI.Azul);

            btnLimpiar = new Button { Text = "Limpiar", Left = 545, Top = 95, Width = 90 };
            btnLimpiar.Click += (s, e) => LimpiarCampos();
            EstiloUI.Estilizar(btnLimpiar, EstiloUI.Gris);
            EstiloUI.Estilizar(btnAutoSerie, EstiloUI.Gris);
            EstiloUI.Estilizar(btnAutoPatrimonio, EstiloUI.Gris);
            EstiloUI.Estilizar(panelDatos);

            panelDatos.Controls.AddRange(new Control[] {
                lblMarca, cmbMarca, lblTipo, cmbTipoEquipo, lblSerie, txtSerie, btnAutoSerie,
                lblPatrimonio, txtPatrimonio, btnAutoPatrimonio, lblDepartamento, cmbDepartamento, lblEmpleado, cmbEmpleado,
                btnIncluir, btnActualizar, btnEliminar, btnConsultar, btnLimpiar
            });

            var lblBuscar = new Label { Text = "Buscar (marca o serie):", Left = 10, Top = 155, Width = 150 };
            txtBuscar = new TextBox { Left = 165, Top = 152, Width = 250 };
            txtBuscar.TextChanged += (s, e) => FiltrarGrid();

            grid = new DataGridView
            {
                Left = 10,
                Top = 185,
                Width = 910,
                Height = 310,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(27, 42, 65);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            grid.CellClick += Grid_CellClick;

            var panelReportes = new GroupBox { Text = "Reportes", Left = 10, Top = 505, Width = 910, Height = 90 };
            EstiloUI.Estilizar(panelReportes);
            var lblReporte = new Label { Text = "Seleccione un reporte:", Left = 10, Top = 30, Width = 140 };
            cmbReportes = new ComboBox { Left = 155, Top = 27, Width = 400, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbReportes.Items.AddRange(new object[] {
                "1. Listado de todas las computadoras",
                "2. Cantidad de equipos por ubicacion/departamento",
                "3. Cantidad de equipos por tipo",
                "4. Cantidad de equipos por marca",
                "5. Equipos sin empleado asignado"
            });
            cmbReportes.SelectedIndex = 0;

            btnGenerarReporte = new Button { Text = "Generar reporte", Left = 565, Top = 25, Width = 140 };
            btnGenerarReporte.Click += (s, e) => GenerarReporte();
            EstiloUI.Estilizar(btnGenerarReporte, EstiloUI.Navy);

            panelReportes.Controls.AddRange(new Control[] { lblReporte, cmbReportes, btnGenerarReporte });

            this.Controls.Add(panelDatos);
            this.Controls.Add(lblBuscar);
            this.Controls.Add(txtBuscar);
            this.Controls.Add(grid);
            this.Controls.Add(panelReportes);
        }

        private void CargarEmpleados()
        {
            cmbEmpleado.Items.Clear();
            cmbEmpleado.Items.Add("(Sin asignar)");
            var usuarios = new UsuarioRepositorio().ObtenerTodos();
            foreach (var u in usuarios)
                cmbEmpleado.Items.Add(u.Cedula + " - " + u.NombreCompleto);
            cmbEmpleado.SelectedIndex = 0;
        }

        private void Consultar()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                grid.DataSource = _repositorio.ObtenerTodas();
                AplicarEncabezadosAmigables();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void AplicarEncabezadosAmigables()
        {
            RenombrarColumna("Id_Computadora", "ID");
            RenombrarColumna("Marca", "Marca");
            RenombrarColumna("TipoEquipo", "Tipo de equipo");
            RenombrarColumna("Serie", "Serie");
            RenombrarColumna("Patrimonio", "Patrimonio");
            RenombrarColumna("Departamento", "Departamento");
            RenombrarColumna("Cedula_Empleado", "Cedula empleado");
            RenombrarColumna("EmpleadoACargo", "Empleado a cargo");
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
                    : $"Marca LIKE '%{texto}%' OR Serie LIKE '%{texto}%'";
            }
        }

        private void GenerarReporte()
        {
            DataTable resultado = cmbReportes.SelectedIndex switch
            {
                0 => _repositorio.ReporteTodasLasComputadoras(),
                1 => _repositorio.ReportePorUbicacion(),
                2 => _repositorio.ReportePorTipoEquipo(),
                3 => _repositorio.ReportePorMarca(),
                4 => _repositorio.ReporteSinEmpleadoAsignado(),
                _ => _repositorio.ObtenerTodas()
            };
            grid.DataSource = resultado;
            AplicarEncabezadosAmigables();
            RenombrarColumna("Cantidad", "Cantidad");
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || grid.Columns.Contains("Id_Computadora") == false) return;
            var fila = grid.Rows[e.RowIndex];

            idSeleccionado = Convert.ToInt32(fila.Cells["Id_Computadora"].Value);
            btnActualizar.Enabled = true;
            btnEliminar.Enabled = true;
            cmbMarca.Text = fila.Cells["Marca"].Value?.ToString();
            cmbTipoEquipo.SelectedItem = fila.Cells["TipoEquipo"].Value?.ToString();
            txtSerie.Text = fila.Cells["Serie"].Value?.ToString();
            txtPatrimonio.Text = fila.Cells["Patrimonio"].Value?.ToString();
            cmbDepartamento.Text = fila.Cells["Departamento"].Value?.ToString();

            string cedula = fila.Cells["Cedula_Empleado"].Value?.ToString();
            cmbEmpleado.SelectedIndex = 0;
            if (!string.IsNullOrWhiteSpace(cedula))
            {
                for (int i = 0; i < cmbEmpleado.Items.Count; i++)
                {
                    if (cmbEmpleado.Items[i].ToString().StartsWith(cedula))
                    {
                        cmbEmpleado.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private Computadora LeerFormulario()
        {
            string cedula = null;
            if (cmbEmpleado.SelectedIndex > 0)
                cedula = cmbEmpleado.SelectedItem.ToString().Split(' ')[0];

            return new Computadora
            {
                Id_Computadora = idSeleccionado ?? 0,
                Marca = cmbMarca.Text.Trim(),
                TipoEquipo = cmbTipoEquipo.SelectedItem?.ToString(),
                Serie = txtSerie.Text.Trim(),
                Patrimonio = txtPatrimonio.Text.Trim(),
                Departamento = string.IsNullOrWhiteSpace(cmbDepartamento.Text) ? null : cmbDepartamento.Text.Trim(),
                Cedula_Empleado = cedula
            };
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(cmbMarca.Text) || cmbTipoEquipo.SelectedItem == null ||
                string.IsNullOrWhiteSpace(txtSerie.Text) || string.IsNullOrWhiteSpace(txtPatrimonio.Text))
            {
                MessageBox.Show("Marca, tipo de equipo, serie y patrimonio son obligatorios.",
                    "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void Incluir()
        {
            if (!ValidarCampos()) return;
            try
            {
                _repositorio.Incluir(LeerFormulario());
                MessageBox.Show("Computadora incluida correctamente.", "Listo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                Consultar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al incluir: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Actualizar()
        {
            if (idSeleccionado == null)
            {
                MessageBox.Show("Seleccione primero una computadora de la lista.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!ValidarCampos()) return;
            try
            {
                _repositorio.Actualizar(LeerFormulario());
                MessageBox.Show("Computadora actualizada correctamente.", "Listo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                Consultar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Eliminar()
        {
            if (idSeleccionado == null)
            {
                MessageBox.Show("Seleccione primero una computadora de la lista.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("¿Confirma eliminar esta computadora?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                _repositorio.Eliminar(idSeleccionado.Value);
                LimpiarCampos();
                Consultar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            idSeleccionado = null;
            btnActualizar.Enabled = false;
            btnEliminar.Enabled = false;
            cmbMarca.Text = "";
            cmbTipoEquipo.SelectedIndex = -1;
            cmbDepartamento.Text = "";
            cmbEmpleado.SelectedIndex = 0;

            // Se sugiere automaticamente el siguiente numero de serie y patrimonio,
            // pero el usuario puede editarlos si lo necesita.
            try
            {
                txtSerie.Text = _repositorio.SugerirSiguienteSerie();
                txtPatrimonio.Text = _repositorio.SugerirSiguientePatrimonio();
            }
            catch
            {
                txtSerie.Clear();
                txtPatrimonio.Clear();
            }
        }
    }
}
