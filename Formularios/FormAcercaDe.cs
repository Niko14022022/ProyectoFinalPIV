using System.Drawing;
using System.Windows.Forms;
using ProyectoFinalPIV.Utilidades;

namespace ProyectoFinalPIV.Formularios
{
    public class FormAcercaDe : Form
    {
        public FormAcercaDe()
        {
            this.Text = "Acerca de...";
            this.Width = 460;
            this.Height = 340;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            var encabezado = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = EstiloUI.Navy };
            var lblTitulo = new Label
            {
                Text = "Sistema de Gestion de TI",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            encabezado.Controls.Add(lblTitulo);

            var panelDatos = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(25, 20, 25, 20),
                BackColor = Color.White
            };
            panelDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            panelDatos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));

            AgregarFila(panelDatos, "Numero de carnet:", "24112501");
            AgregarFila(panelDatos, "Nombre del grupo:", "Nikolas Gabriel Marin Arias");
            AgregarFila(panelDatos, "Sigla del curso:", "IC-2026");
            AgregarFila(panelDatos, "Nombre del curso:", "Programacion IV (Grupo G1)");
            AgregarFila(panelDatos, "Cuatrimestre actual:", "II Cuatrimestre 2026");

            this.Controls.Add(panelDatos);
            this.Controls.Add(encabezado);
        }

        private void AgregarFila(TableLayoutPanel panel, string etiqueta, string valor)
        {
            panel.Controls.Add(new Label
            {
                Text = etiqueta,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = EstiloUI.Navy,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            });
            panel.Controls.Add(new Label
            {
                Text = valor,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(60, 60, 60),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            });
        }
    }
}
