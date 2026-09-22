using System;

namespace ProyectoFinalPIV.Modelos
{
    public class SoftwareInstalado
    {
        public int Id_Software { get; set; }
        public string NombreSoftware { get; set; }
        public string Version { get; set; }
        public DateTime? FechaInstalacion { get; set; }
        public string Cedula_Tecnico { get; set; }
        public string TipoLicenciamiento { get; set; } // Con Licencia, Freeware, Shareware, Otro
        public int LicenciasDisponibles { get; set; }
        public string Observaciones { get; set; } // como se instalo
    }
}
