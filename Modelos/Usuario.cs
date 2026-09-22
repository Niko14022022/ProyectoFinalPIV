using System;

namespace ProyectoFinalPIV.Modelos
{
    /// <summary>
    /// Datos de perfil del usuario. Las credenciales de acceso viven en la
    /// tabla/clase Credencial, separadas por seguridad y para soportar
    /// dos formas distintas de autenticacion (Sistema y Windows).
    /// </summary>
    public class Usuario
    {
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public string Rol { get; set; }
        public DateTime Fecha_Creacion_Usuario { get; set; }
        public bool Activo { get; set; }

        public string NombreCompleto =>
            $"{Nombre} {Apellido1} {Apellido2}".Replace("  ", " ").Trim();
    }
}
