using System;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoFinalPIV.Modelos;

namespace ProyectoFinalPIV.Datos
{
    public class ComputadoraRepositorio
    {
        public DataTable ObtenerTodas()
        {
            const string sql = @"SELECT c.Id_Computadora, c.Marca, c.TipoEquipo, c.Serie, c.Patrimonio,
                                         c.Departamento, c.Cedula_Empleado,
                                         u.Nombre + ' ' + u.Apellido1 AS EmpleadoACargo
                                  FROM dbo.Computadoras c
                                  LEFT JOIN dbo.Usuarios u ON u.Cedula = c.Cedula_Empleado
                                  ORDER BY c.Marca";
            return EjecutarConsulta(sql);
        }

        public void Incluir(Computadora c)
        {
            if (ExisteSerie(c.Serie))
                throw new InvalidOperationException($"Ya existe una computadora registrada con la serie '{c.Serie}'.");

            const string sql = @"INSERT INTO dbo.Computadoras
                                  (Marca, TipoEquipo, Serie, Patrimonio, Departamento, Cedula_Empleado)
                                  VALUES (@Marca, @Tipo, @Serie, @Patrimonio, @Departamento, @Cedula)";
            EjecutarNonQuery(sql, c);
        }

        public bool ExisteSerie(string serie)
        {
            const string sql = "SELECT COUNT(*) FROM dbo.Computadoras WHERE Serie = @Serie";
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Serie", serie);
                conexion.Open();
                return (int)comando.ExecuteScalar() > 0;
            }
        }

        public void Actualizar(Computadora c)
        {
            const string sql = @"UPDATE dbo.Computadoras
                                  SET Marca = @Marca, TipoEquipo = @Tipo, Serie = @Serie,
                                      Patrimonio = @Patrimonio, Departamento = @Departamento,
                                      Cedula_Empleado = @Cedula
                                  WHERE Id_Computadora = @Id";
            EjecutarNonQuery(sql, c);
        }

        public void Eliminar(int idComputadora)
        {
            const string sql = "DELETE FROM dbo.Computadoras WHERE Id_Computadora = @Id";
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Id", idComputadora);
                conexion.Open();
                try
                {
                    comando.ExecuteNonQuery();
                }
                catch (SqlException ex) when (ex.Number == 547)
                {
                    throw new InvalidOperationException(
                        "No se puede eliminar esta computadora porque tiene software asignado. " +
                        "Quite primero las asignaciones de software antes de eliminarla.");
                }
            }
        }

        /// <summary>
        /// Sugiere el siguiente numero de serie consecutivo (formato SN-0001, SN-0002, ...)
        /// para que el usuario no tenga que inventarlo, aunque lo puede editar despues.
        /// </summary>
        public string SugerirSiguienteSerie() => SugerirSiguienteConsecutivo("Serie", "SN-");

        /// <summary>
        /// Sugiere el siguiente numero de patrimonio consecutivo (formato PAT-100, PAT-101, ...).
        /// </summary>
        public string SugerirSiguientePatrimonio() => SugerirSiguienteConsecutivo("Patrimonio", "PAT-");

        private string SugerirSiguienteConsecutivo(string columna, string prefijo)
        {
            string sql = $@"SELECT MAX(TRY_CAST(SUBSTRING({columna}, LEN(@Prefijo) + 1, 10) AS INT))
                             FROM dbo.Computadoras WHERE {columna} LIKE @Patron";
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Prefijo", prefijo);
                comando.Parameters.AddWithValue("@Patron", prefijo + "%");
                conexion.Open();
                object resultado = comando.ExecuteScalar();
                int siguiente = (resultado == null || resultado == DBNull.Value) ? 1 : Convert.ToInt32(resultado) + 1;
                return prefijo + siguiente.ToString("D4");
            }
        }

        private void EjecutarNonQuery(string sql, Computadora c)
        {
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Id", c.Id_Computadora);
                comando.Parameters.AddWithValue("@Marca", c.Marca);
                comando.Parameters.AddWithValue("@Tipo", c.TipoEquipo);
                comando.Parameters.AddWithValue("@Serie", c.Serie);
                comando.Parameters.AddWithValue("@Patrimonio", c.Patrimonio);
                comando.Parameters.AddWithValue("@Departamento", (object)c.Departamento ?? System.DBNull.Value);
                comando.Parameters.AddWithValue("@Cedula", (object)c.Cedula_Empleado ?? System.DBNull.Value);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        // ---------------- Reportes ----------------

        public DataTable ReporteTodasLasComputadoras() => ObtenerTodas();

        public DataTable ReportePorUbicacion() => EjecutarConsulta(
            @"SELECT ISNULL(Departamento, 'Sin asignar') AS Departamento, COUNT(*) AS CantidadEquipos
              FROM dbo.Computadoras GROUP BY Departamento ORDER BY Departamento");

        public DataTable ReportePorTipoEquipo() => EjecutarConsulta(
            @"SELECT TipoEquipo, COUNT(*) AS Cantidad
              FROM dbo.Computadoras GROUP BY TipoEquipo ORDER BY TipoEquipo");

        public DataTable ReportePorMarca() => EjecutarConsulta(
            @"SELECT Marca, COUNT(*) AS Cantidad
              FROM dbo.Computadoras GROUP BY Marca ORDER BY Marca");

        public DataTable ReporteSinEmpleadoAsignado() => EjecutarConsulta(
            @"SELECT Id_Computadora, Marca, TipoEquipo, Serie, Patrimonio, Departamento
              FROM dbo.Computadoras WHERE Cedula_Empleado IS NULL ORDER BY Marca");

        public DataTable ObtenerParaCombo() => EjecutarConsulta(
            "SELECT Id_Computadora, Marca + ' - ' + Serie AS Descripcion FROM dbo.Computadoras ORDER BY Marca");

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
