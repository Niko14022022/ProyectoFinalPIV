using System;
using System.Windows.Forms;
using ProyectoFinalPIV.Datos;
using ProyectoFinalPIV.Modelos;
using ProyectoFinalPIV.Utilidades;

namespace ProyectoFinalPIV.Formularios
{
    public class FormUsuarios : Form
    {
        private readonly UsuarioRepositorio _repositorio = new UsuarioRepositorio();
        private DataGridView grid;
        private MaskedTextBox txtCedula;
        private TextBox txtNombre, txtApellido1, txtApellido2;
        private ComboBox cmbRol, cmbTipoAutenticacion;
        private TextBox txtClave, txtUsuarioWindows;
        private Label lblClave, lblUsuarioWindows;
        private CheckBox chkActivo;
        private Button btnIncluir, btnActualizar, btnEliminar, btnConsultar, btnLimpiar;

        public FormUsuarios()
        {
            InicializarComponentes();
            try
            {
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
            this.Text = "Mantenimiento de Usuarios";
            this.Width = 900;
            this.Height = 620;
            this.StartPosition = FormStartPosition.CenterParent;

            var panelDatos = new GroupBox { Text = "Datos del usuario", Left = 10, Top = 10, Width = 860, Height = 210 };
            EstiloUI.Estilizar(panelDatos);

            var lblCedula = new Label { Text = "Cedula:", Left = 10, Top = 25, Width = 90 };
            txtCedula = new MaskedTextBox { Left = 110, Top = 22, Width = 150, Mask = "0000000000", PromptChar = '_' };

            var lblNombre = new Label { Text = "Nombre:", Left = 280, Top = 25, Width = 80 };
            txtNombre = new TextBox { Left = 365, Top = 22, Width = 150 };

            var lblApellido1 = new Label { Text = "Apellido 1:", Left = 10, Top = 60, Width = 90 };
            txtApellido1 = new TextBox { Left = 110, Top = 57, Width = 150 };

            var lblApellido2 = new Label { Text = "Apellido 2:", Left = 280, Top = 60, Width = 80 };
            txtApellido2 = new TextBox { Left = 365, Top = 57, Width = 150 };

            var lblRol = new Label { Text = "Rol:", Left = 10, Top = 95, Width = 90 };
            cmbRol = new ComboBox { Left = 110, Top = 92, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRol.Items.AddRange(new object[] { "Empleado", "Tecnico TI", "Administrador TI" });

            chkActivo = new CheckBox { Text = "Activo", Left = 280, Top = 95, Width = 100, Checked = true };

            var lblTipoAuth = new Label { Text = "Tipo de acceso:", Left = 10, Top = 130, Width = 90 };
            cmbTipoAutenticacion = new ComboBox { Left = 110, Top = 127, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTipoAutenticacion.Items.AddRange(new object[] { Credencial.TipoSistema, Credencial.TipoWindows });
            cmbTipoAutenticacion.SelectedIndexChanged += (s, e) => ActualizarVisibilidadCredencial();

            lblClave = new Label { Text = "Clave:", Left = 280, Top = 130, Width = 80 };
            txtClave = new TextBox { Left = 365, Top = 127, Width = 150, UseSystemPasswordChar = true };

            lblUsuarioWindows = new Label { Text = "Usuario de Windows:", Left = 280, Top = 130, Width = 130, Visible = false };
            txtUsuarioWindows = new TextBox { Left = 415, Top = 127, Width = 200, Visible = false, Text = @"DOMINIO\usuario" };

            btnIncluir = new Button { Text = "Incluir", Left = 110, Top = 170, Width = 90 };
            btnIncluir.Click += (s, e) => Incluir();
            EstiloUI.Estilizar(btnIncluir, EstiloUI.Verde);
            btnActualizar = new Button { Text = "Actualizar", Left = 210, Top = 170, Width = 90, Enabled = false };
            btnActualizar.Click += (s, e) => Actualizar();
            EstiloUI.Estilizar(btnActualizar, EstiloUI.Naranja);
            btnEliminar = new Button { Text = "Eliminar (desactivar)", Left = 310, Top = 170, Width = 150, Enabled = false };
            btnEliminar.Click += (s, e) => Eliminar();
            EstiloUI.Estilizar(btnEliminar, EstiloUI.Rojo);
            btnConsultar = new Button { Text = "Consultar", Left = 470, Top = 170, Width = 90 };
            btnConsultar.Click += (s, e) => Consultar();
            EstiloUI.Estilizar(btnConsultar, EstiloUI.Azul);
            btnLimpiar = new Button { Text = "Limpiar", Left = 570, Top = 170, Width = 90 };
            btnLimpiar.Click += (s, e) => LimpiarCampos();
            EstiloUI.Estilizar(btnLimpiar, EstiloUI.Gris);

            panelDatos.Controls.AddRange(new Control[] {
                lblCedula, txtCedula, lblNombre, txtNombre, lblApellido1, txtApellido1, lblApellido2, txtApellido2,
                lblRol, cmbRol, chkActivo, lblTipoAuth, cmbTipoAutenticacion, lblClave, txtClave,
                lblUsuarioWindows, txtUsuarioWindows, btnIncluir, btnActualizar, btnEliminar, btnConsultar, btnLimpiar
            });

            grid = new DataGridView
            {
                Left = 10,
                Top = 230,
                Width = 860,
                Height = 340,
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

            this.Controls.Add(panelDatos);
            this.Controls.Add(grid);

            ActualizarVisibilidadCredencial();
        }

        private void ActualizarVisibilidadCredencial()
        {
            bool esSistema = cmbTipoAutenticacion.SelectedItem?.ToString() != Credencial.TipoWindows;
            lblClave.Visible = esSistema;
            txtClave.Visible = esSistema;
            lblUsuarioWindows.Visible = !esSistema;
            txtUsuarioWindows.Visible = !esSistema;
        }

        private void Consultar()
        {
            var lista = _repositorio.ObtenerTodos();
            grid.DataSource = lista;
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !grid.Columns.Contains("Cedula")) return;
            var fila = grid.Rows[e.RowIndex];

            txtCedula.Text = fila.Cells["Cedula"].Value?.ToString();
            txtCedula.ReadOnly = true; // no se cambia la llave primaria desde aqui
            txtNombre.Text = fila.Cells["Nombre"].Value?.ToString();
            txtApellido1.Text = fila.Cells["Apellido1"].Value?.ToString();
            txtApellido2.Text = fila.Cells["Apellido2"].Value?.ToString();
            cmbRol.SelectedItem = fila.Cells["Rol"].Value?.ToString();
            chkActivo.Checked = Convert.ToBoolean(fila.Cells["Activo"].Value);
            btnActualizar.Enabled = true;
            btnEliminar.Enabled = true;
        }

        private bool ValidarCampos(bool esInclusion)
        {
            if (string.IsNullOrWhiteSpace(txtCedula.Text) || string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido1.Text) || cmbRol.SelectedItem == null)
            {
                MessageBox.Show("Cedula, nombre, apellido1 y rol son obligatorios.", "Datos incompletos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (esInclusion)
            {
                if (cmbTipoAutenticacion.SelectedItem == null)
                {
                    MessageBox.Show("Seleccione el tipo de acceso.", "Datos incompletos",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                bool esSistema = cmbTipoAutenticacion.SelectedItem.ToString() == Credencial.TipoSistema;
                if (esSistema && string.IsNullOrWhiteSpace(txtClave.Text))
                {
                    MessageBox.Show("Indique la clave para el acceso tipo Sistema.", "Datos incompletos",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (esSistema && txtClave.Text.Length < 4)
                {
                    MessageBox.Show("La clave debe tener al menos 4 caracteres.", "Datos incompletos",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (!esSistema && string.IsNullOrWhiteSpace(txtUsuarioWindows.Text))
                {
                    MessageBox.Show("Indique el usuario de Windows para el acceso tipo Windows.", "Datos incompletos",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }

        private void Incluir()
        {
            if (!ValidarCampos(esInclusion: true)) return;

            var usuario = new Usuario
            {
                Cedula = txtCedula.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Apellido1 = txtApellido1.Text.Trim(),
                Apellido2 = string.IsNullOrWhiteSpace(txtApellido2.Text) ? null : txtApellido2.Text.Trim(),
                Rol = cmbRol.SelectedItem.ToString(),
                Activo = true
            };

            bool esSistema = cmbTipoAutenticacion.SelectedItem.ToString() == Credencial.TipoSistema;
            var credencial = new Credencial
            {
                Cedula = usuario.Cedula,
                TipoAutenticacion = cmbTipoAutenticacion.SelectedItem.ToString(),
                Clave = esSistema ? txtClave.Text : null,
                UsuarioWindows = esSistema ? null : txtUsuarioWindows.Text.Trim()
            };

            try
            {
                _repositorio.Incluir(usuario, credencial);
                MessageBox.Show("Usuario incluido correctamente.", "Listo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                Consultar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al incluir: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Actualizar()
        {
            if (!ValidarCampos(esInclusion: false)) return;

            var usuario = new Usuario
            {
                Cedula = txtCedula.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Apellido1 = txtApellido1.Text.Trim(),
                Apellido2 = string.IsNullOrWhiteSpace(txtApellido2.Text) ? null : txtApellido2.Text.Trim(),
                Rol = cmbRol.SelectedItem.ToString(),
                Activo = chkActivo.Checked
            };

            try
            {
                _repositorio.Actualizar(usuario);
                MessageBox.Show("Usuario actualizado correctamente.", "Listo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                Consultar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Eliminar()
        {
            if (string.IsNullOrWhiteSpace(txtCedula.Text))
            {
                MessageBox.Show("Seleccione primero un usuario de la lista.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("¿Confirma desactivar este usuario? (no se borra fisicamente, para no romper referencias)",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                _repositorio.Eliminar(txtCedula.Text.Trim());
                LimpiarCampos();
                Consultar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desactivar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            txtCedula.Clear();
            txtCedula.ReadOnly = false;
            btnActualizar.Enabled = false;
            btnEliminar.Enabled = false;
            txtNombre.Clear();
            txtApellido1.Clear();
            txtApellido2.Clear();
            cmbRol.SelectedIndex = -1;
            chkActivo.Checked = true;
            cmbTipoAutenticacion.SelectedIndex = -1;
            txtClave.Clear();
            txtUsuarioWindows.Text = @"DOMINIO\usuario";
        }
    }
}
