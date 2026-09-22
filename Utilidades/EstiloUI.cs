using System.Drawing;
using System.Windows.Forms;

namespace ProyectoFinalPIV.Utilidades
{
    /// <summary>
    /// Aplica la misma paleta de colores (usada en el menu lateral y las tarjetas del
    /// tablero principal) a los botones y groupbox de las pantallas de los modulos,
    /// para que todo el sistema se vea visualmente consistente.
    /// </summary>
    public static class EstiloUI
    {
        public static readonly Color Navy = Color.FromArgb(27, 42, 65);
        public static readonly Color Azul = Color.FromArgb(41, 128, 185);
        public static readonly Color Verde = Color.FromArgb(39, 174, 96);
        public static readonly Color Naranja = Color.FromArgb(230, 126, 34);
        public static readonly Color Rojo = Color.FromArgb(192, 57, 43);
        public static readonly Color Gris = Color.FromArgb(120, 130, 140);
        public static readonly Color Morado = Color.FromArgb(142, 68, 173);

        public static void Estilizar(Button boton, Color color)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.BackColor = color;
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            boton.FlatAppearance.BorderSize = 0;
            boton.Cursor = Cursors.Hand;
        }

        public static void Estilizar(GroupBox grupo)
        {
            grupo.ForeColor = Navy;
            grupo.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }
    }
}
