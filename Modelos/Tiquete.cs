using System;

namespace ProyectoFinalPIV.Modelos
{
    public class Tiquete
    {
        public int Id_Tiquete { get; set; }
        public string Cedula_Empleado { get; set; }
        public string Cedula_Tecnico { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; } // Pendiente, Asignado, En Atencion, Resuelto, Cerrado
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaAtencion { get; set; }

        public const string Pendiente = "Pendiente";
        public const string Asignado = "Asignado";
        public const string EnAtencion = "En Atencion";
        public const string Resuelto = "Resuelto";
        public const string Cerrado = "Cerrado";
    }
}
