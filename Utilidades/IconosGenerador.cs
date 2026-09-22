using System.Drawing;
using System.Drawing.Drawing2D;

namespace ProyectoFinalPIV.Utilidades
{
    /// <summary>
    /// Genera iconos sencillos por codigo (GDI+), con color y tamano parametrizables,
    /// para no depender de archivos de imagen externos que se puedan perder.
    /// </summary>
    public static class IconosGenerador
    {
        public static Bitmap IconoComputadora(Color color, int tamano = 22)
        {
            var bmp = new Bitmap(tamano, tamano);
            using (var g = Preparar(bmp))
            using (var pluma = new Pen(color, 2))
            {
                float e = tamano / 20f;
                g.DrawRectangle(pluma, 2 * e, 3 * e, 15 * e, 10 * e);
                g.DrawLine(pluma, 7 * e, 16 * e, 12 * e, 16 * e);
                g.DrawLine(pluma, 9 * e, 13 * e, 9 * e, 16 * e);
            }
            return bmp;
        }

        public static Bitmap IconoSoftware(Color color, int tamano = 22)
        {
            var bmp = new Bitmap(tamano, tamano);
            using (var g = Preparar(bmp))
            using (var pluma = new Pen(color, 2))
            {
                float e = tamano / 20f;
                g.DrawEllipse(pluma, 3 * e, 3 * e, 13 * e, 13 * e);
                g.DrawEllipse(pluma, 7 * e, 7 * e, 5 * e, 5 * e);
            }
            return bmp;
        }

        public static Bitmap IconoTiquete(Color color, int tamano = 22)
        {
            var bmp = new Bitmap(tamano, tamano);
            using (var g = Preparar(bmp))
            using (var pluma = new Pen(color, 2))
            {
                float e = tamano / 20f;
                var puntos = new[] {
                    new PointF(2 * e, 5 * e), new PointF(17 * e, 5 * e),
                    new PointF(17 * e, 15 * e), new PointF(2 * e, 15 * e)
                };
                g.DrawPolygon(pluma, puntos);
                g.DrawLine(pluma, 6 * e, 5 * e, 6 * e, 15 * e);
            }
            return bmp;
        }

        public static Bitmap IconoUsuario(Color color, int tamano = 22)
        {
            var bmp = new Bitmap(tamano, tamano);
            using (var g = Preparar(bmp))
            using (var pincel = new SolidBrush(color))
            {
                float e = tamano / 20f;
                g.FillEllipse(pincel, 6 * e, 2 * e, 8 * e, 8 * e);
                g.FillPie(pincel, 2 * e, 9 * e, 16 * e, 14 * e, 180, 180);
            }
            return bmp;
        }

        public static Bitmap IconoInfo(Color color, int tamano = 22)
        {
            var bmp = new Bitmap(tamano, tamano);
            using (var g = Preparar(bmp))
            using (var pluma = new Pen(color, 2))
            using (var fuente = new Font("Segoe UI", tamano * 0.5f, FontStyle.Bold))
            using (var pincelTexto = new SolidBrush(color))
            {
                float e = tamano / 20f;
                g.DrawEllipse(pluma, 2 * e, 2 * e, 16 * e, 16 * e);
                g.DrawString("i", fuente, pincelTexto, 7.5f * e, 4f * e);
            }
            return bmp;
        }

        public static Bitmap IconoSalir(Color color, int tamano = 22)
        {
            var bmp = new Bitmap(tamano, tamano);
            using (var g = Preparar(bmp))
            using (var pluma = new Pen(color, 2))
            {
                float e = tamano / 20f;
                g.DrawRectangle(pluma, 2 * e, 2 * e, 9 * e, 15 * e);
                g.DrawLine(pluma, 9 * e, 9 * e, 18 * e, 9 * e);
                g.DrawLine(pluma, 14 * e, 5 * e, 18 * e, 9 * e);
                g.DrawLine(pluma, 14 * e, 13 * e, 18 * e, 9 * e);
            }
            return bmp;
        }

        public static Bitmap IconoTablero(Color color, int tamano = 22)
        {
            var bmp = new Bitmap(tamano, tamano);
            using (var g = Preparar(bmp))
            using (var pluma = new Pen(color, 2))
            {
                float e = tamano / 20f;
                g.DrawRectangle(pluma, 2 * e, 2 * e, 7 * e, 7 * e);
                g.DrawRectangle(pluma, 11 * e, 2 * e, 7 * e, 4 * e);
                g.DrawRectangle(pluma, 11 * e, 8 * e, 7 * e, 10 * e);
                g.DrawRectangle(pluma, 2 * e, 11 * e, 7 * e, 7 * e);
            }
            return bmp;
        }

        public static Bitmap IconoAvatarCircular(Color colorFondo, Color colorIcono, int tamano = 90)
        {
            var bmp = new Bitmap(tamano, tamano);
            using (var g = Preparar(bmp))
            using (var pincelFondo = new SolidBrush(colorFondo))
            using (var pluma = new Pen(colorIcono, tamano * 0.045f))
            {
                g.FillEllipse(pincelFondo, 0, 0, tamano, tamano);
                float e = tamano / 90f;
                g.DrawEllipse(pluma, 33 * e, 22 * e, 24 * e, 24 * e); // cabeza
                g.DrawArc(pluma, 18 * e, 52 * e, 54 * e, 42 * e, 195, 150); // hombros
            }
            return bmp;
        }

        public static Bitmap IconoCampana(Color color, int tamano = 22)
        {
            var bmp = new Bitmap(tamano, tamano);
            using (var g = Preparar(bmp))
            using (var pluma = new Pen(color, 2))
            {
                float e = tamano / 20f;
                g.DrawArc(pluma, 4 * e, 3 * e, 12 * e, 12 * e, 180, 180);
                g.DrawLine(pluma, 4 * e, 9 * e, 3 * e, 15 * e);
                g.DrawLine(pluma, 16 * e, 9 * e, 17 * e, 15 * e);
                g.DrawLine(pluma, 3 * e, 15 * e, 17 * e, 15 * e);
                g.DrawArc(pluma, 8 * e, 15 * e, 4 * e, 4 * e, 0, 180);
            }
            return bmp;
        }

        private static Graphics Preparar(Bitmap bmp)
        {
            var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);
            return g;
        }
    }
}
