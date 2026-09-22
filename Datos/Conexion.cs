using System;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace ProyectoFinalPIV.Datos
{
    /// <summary>
    /// Encapsula la creacion de conexiones a la base de datos SQL Server.
    /// </summary>
    public static class Conexion
    {
        public static string CadenaConexion
        {
            get
            {
                string cadena = ConfigurationManager.AppSettings["ConnectionString"];
                if (string.IsNullOrWhiteSpace(cadena))
                    throw new InvalidOperationException(
                        "No se encontro la cadena de conexion en App.config.");
                return cadena;
            }
        }

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(CadenaConexion);
        }
    }
}
