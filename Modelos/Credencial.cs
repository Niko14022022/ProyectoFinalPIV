using System;

namespace ProyectoFinalPIV.Modelos
{
    /// <summary>
    /// Representa como se autentica un usuario: por clave propia del sistema
    /// o validando contra la sesion de Windows activa.
    /// </summary>
    public class Credencial
    {
        public string Cedula { get; set; }
        public string TipoAutenticacion { get; set; } // "Sistema" o "Windows"
        public string Clave { get; set; }              // solo si TipoAutenticacion == "Sistema"
        public string UsuarioWindows { get; set; }      // solo si TipoAutenticacion == "Windows"
        public DateTime? FechaUltimoCambioClave { get; set; }

        public const string TipoSistema = "Sistema";
        public const string TipoWindows = "Windows";
    }
}
