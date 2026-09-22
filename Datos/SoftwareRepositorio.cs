using System;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoFinalPIV.Modelos;

namespace ProyectoFinalPIV.Datos
{
    public class SoftwareRepositorio
    {
        public DataTable ObtenerTodos()
        {
            const string sql = @"SELECT s.Id_Software, s.NombreSoftware, s.Version, s.FechaInstalacion,
                                         s.Cedula_Tecnico, u.Nombre + ' ' + u.Apellido1 AS Tecnico,
                                         s.TipoLicenciamiento, s.LicenciasDisponibles, s.Observaciones,
                                         STUFF((
                                             SELECT ', ' + c.Marca + ' (' + c.Serie + ')'
                                             FROM dbo.SoftwarePorComputadora sc
                                             INNER JOIN dbo.Computadoras c ON c.Id_Computadora = sc.Id_Computadora
                                             WHERE sc.Id_Software = s.Id_Software
                                             FOR XML PATH('')
                                         ), 1, 2, '') AS DondeInstalado
                                  FROM dbo.Software s
                                  LEFT JOIN dbo.Usuarios u ON u.Cedula = s.Cedula_Tecnico
                                  ORDER BY s.NombreSoftware";
            return EjecutarConsulta(sql);
        }

        public void Incluir(SoftwareInstalado s)
        {
            const string sql = @"INSERT INTO dbo.Software
                                  (NombreSoftware, Version, FechaInstalacion, Cedula_Tecnico,
                                   TipoLicenciamiento, LicenciasDisponibles, Observaciones)
                                  VALUES (@Nombre, @Version, @Fecha, @Tecnico, @Tipo, @Stock, @Observaciones)";
            EjecutarNonQuery(sql, s);
        }

        public void Actualizar(SoftwareInstalado s)
        {
            const string sql = @"UPDATE dbo.Software
                                  SET NombreSoftware = @Nombre, Version = @Version, FechaInstalacion = @Fecha,
                                      Cedula_Tecnico = @Tecnico, TipoLicenciamiento = @Tipo,
                                      LicenciasDisponibles = @Stock, Observaciones = @Observaciones
                                  WHERE Id_Software = @Id";
            EjecutarNonQuery(sql, s);
        }

        public void Eliminar(int idSoftware)
        {
            const string sql = "DELETE FROM dbo.Software WHERE Id_Software = @Id";
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Id", idSoftware);
                conexion.Open();
                try
                {
                    comando.ExecuteNonQuery();
                }
                catch (SqlException ex) when (ex.Number == 547)
                {
                    throw new InvalidOperationException(
                        "No se puede eliminar este software porque ya esta asignado a una o mas computadoras. " +
                        "Quite primero esas asignaciones antes de eliminarlo.");
                }
            }
        }

        /// <summary>
        /// Asigna un software a una computadora y descuenta una licencia disponible.
        /// Devuelve false (y no asigna nada) si no hay licencias disponibles.
        /// </summary>
        public bool AsignarAComputadora(int idComputadora, int idSoftware)
        {
            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();
                using (var transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        int disponibles;
                        using (var comandoVerificar = new SqlCommand(
                            "SELECT LicenciasDisponibles FROM dbo.Software WHERE Id_Software = @Id",
                            conexion, transaccion))
                        {
                            comandoVerificar.Parameters.AddWithValue("@Id", idSoftware);
                            disponibles = (int)comandoVerificar.ExecuteScalar();
                        }

                        if (disponibles <= 0)
                        {
                            transaccion.Rollback();
                            return false;
                        }

                        using (var comandoAsignar = new SqlCommand(
                            @"INSERT INTO dbo.SoftwarePorComputadora (Id_Computadora, Id_Software)
                              VALUES (@IdComputadora, @IdSoftware)", conexion, transaccion))
                        {
                            comandoAsignar.Parameters.AddWithValue("@IdComputadora", idComputadora);
                            comandoAsignar.Parameters.AddWithValue("@IdSoftware", idSoftware);
                            comandoAsignar.ExecuteNonQuery();
                        }

                        using (var comandoDescontar = new SqlCommand(
                            @"UPDATE dbo.Software SET LicenciasDisponibles = LicenciasDisponibles - 1
                              WHERE Id_Software = @Id", conexion, transaccion))
                        {
                            comandoDescontar.Parameters.AddWithValue("@Id", idSoftware);
                            comandoDescontar.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        private void EjecutarNonQuery(string sql, SoftwareInstalado s)
        {
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Id", s.Id_Software);
                comando.Parameters.AddWithValue("@Nombre", s.NombreSoftware);
                comando.Parameters.AddWithValue("@Version", (object)s.Version ?? DBNull.Value);
                comando.Parameters.AddWithValue("@Fecha", (object)s.FechaInstalacion ?? DBNull.Value);
                comando.Parameters.AddWithValue("@Tecnico", (object)s.Cedula_Tecnico ?? DBNull.Value);
                comando.Parameters.AddWithValue("@Tipo", s.TipoLicenciamiento);
                comando.Parameters.AddWithValue("@Stock", s.LicenciasDisponibles);
                comando.Parameters.AddWithValue("@Observaciones", (object)s.Observaciones ?? DBNull.Value);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        // ---------------- Reportes ----------------

        public DataTable ReporteTodoElSoftware() => ObtenerTodos();

        public DataTable ReportePorComputadora(int idComputadora)
        {
            const string sql = @"SELECT s.NombreSoftware, s.Version, s.TipoLicenciamiento
                                  FROM dbo.SoftwarePorComputadora sc
                                  INNER JOIN dbo.Software s ON s.Id_Software = sc.Id_Software
                                  WHERE sc.Id_Computadora = @IdComputadora
                                  ORDER BY s.NombreSoftware";
            var tabla = new DataTable();
            using (var conexion = Conexion.ObtenerConexion())
            using (var adaptador = new SqlDataAdapter(sql, conexion))
            {
                adaptador.SelectCommand.Parameters.AddWithValue("@IdComputadora", idComputadora);
                adaptador.Fill(tabla);
            }
            return tabla;
        }

        public DataTable ReportePorTipoLicenciamiento() => EjecutarConsulta(
            @"SELECT TipoLicenciamiento, COUNT(*) AS CantidadSoftware, SUM(LicenciasDisponibles) AS LicenciasTotalesDisponibles
              FROM dbo.Software GROUP BY TipoLicenciamiento ORDER BY TipoLicenciamiento");

        public DataTable ReporteStockDisponible() => EjecutarConsulta(
            @"SELECT NombreSoftware, TipoLicenciamiento, LicenciasDisponibles,
                     CASE WHEN LicenciasDisponibles = 0 THEN 'SIN LICENCIAS DISPONIBLES' ELSE 'OK' END AS Estado
              FROM dbo.Software ORDER BY LicenciasDisponibles ASC");

        public DataTable ReporteAsignaciones() => EjecutarConsulta(
            @"SELECT c.Marca, c.Serie, s.NombreSoftware, s.Version
              FROM dbo.SoftwarePorComputadora sc
              INNER JOIN dbo.Computadoras c ON c.Id_Computadora = sc.Id_Computadora
              INNER JOIN dbo.Software s ON s.Id_Software = sc.Id_Software
              ORDER BY c.Marca, s.NombreSoftware");

        public DataTable ObtenerParaCombo() => EjecutarConsulta(
            "SELECT Id_Software, NombreSoftware + ' (' + CAST(LicenciasDisponibles AS VARCHAR) + ' disp.)' AS Descripcion FROM dbo.Software ORDER BY NombreSoftware");

        private DataTable EjecutarConsulta(string sql)
        {
            var tabla = new DataTable();
            using (var conexion = Conexion.ObtenerConexion())
            using (var adaptador = new SqlDataAdapter(sql, conexion))
            {
                adaptador.Fill(tabla);
            }
            return tabla;
        }
    }
}
