using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoFinalPIV.Modelos;

namespace ProyectoFinalPIV.Datos
{
    public class TiqueteRepositorio
    {
        public void CrearTiquete(string cedulaEmpleado, string tipoIncidencia, string descripcion)
        {
            const string sql = @"INSERT INTO dbo.Tiquetes (Cedula_Empleado, TipoIncidencia, Descripcion, Estado)
                                  VALUES (@Cedula, @TipoIncidencia, @Descripcion, 'Pendiente')";
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Cedula", cedulaEmpleado);
                comando.Parameters.AddWithValue("@TipoIncidencia", tipoIncidencia);
                comando.Parameters.AddWithValue("@Descripcion", descripcion);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void AsignarTecnico(int idTiquete, string cedulaTecnico)
        {
            const string sql = @"UPDATE dbo.Tiquetes
                                  SET Cedula_Tecnico = @Tecnico, Estado = 'Asignado'
                                  WHERE Id_Tiquete = @Id";
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Id", idTiquete);
                comando.Parameters.AddWithValue("@Tecnico", cedulaTecnico);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void CambiarEstado(int idTiquete, string nuevoEstado)
        {
            const string sql = @"UPDATE dbo.Tiquetes
                                  SET Estado = @Estado,
                                      FechaAtencion = CASE WHEN @Estado IN ('Resuelto','Cerrado') THEN GETDATE() ELSE FechaAtencion END
                                  WHERE Id_Tiquete = @Id";
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Id", idTiquete);
                comando.Parameters.AddWithValue("@Estado", nuevoEstado);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Actualiza la descripcion de un tiquete que aun no ha sido atendido.
        /// </summary>
        public void ActualizarDescripcion(int idTiquete, string nuevaDescripcion)
        {
            const string sql = "UPDATE dbo.Tiquetes SET Descripcion = @Descripcion WHERE Id_Tiquete = @Id";
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Id", idTiquete);
                comando.Parameters.AddWithValue("@Descripcion", nuevaDescripcion);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Elimina (cancela) un tiquete. Solo pensado para uso del Administrador de TI.
        /// </summary>
        public void Eliminar(int idTiquete)
        {
            const string sql = "DELETE FROM dbo.Tiquetes WHERE Id_Tiquete = @Id";
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Id", idTiquete);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public DataTable ObtenerPorEmpleado(string cedulaEmpleado) => EjecutarConsultaParametrizada(
            @"SELECT Id_Tiquete, TipoIncidencia, Descripcion, Estado, FechaCreacion, FechaAtencion
              FROM dbo.Tiquetes WHERE Cedula_Empleado = @Cedula ORDER BY FechaCreacion DESC",
            "@Cedula", cedulaEmpleado);

        public DataTable ObtenerPorTecnico(string cedulaTecnico) => EjecutarConsultaParametrizada(
            @"SELECT t.Id_Tiquete, t.TipoIncidencia, t.Descripcion, t.Estado, t.FechaCreacion,
                     u.Nombre + ' ' + u.Apellido1 AS Empleado
              FROM dbo.Tiquetes t
              INNER JOIN dbo.Usuarios u ON u.Cedula = t.Cedula_Empleado
              WHERE t.Cedula_Tecnico = @Cedula ORDER BY t.FechaCreacion DESC",
            "@Cedula", cedulaTecnico);

        public DataTable ObtenerTodos()
        {
            const string sql = @"SELECT t.Id_Tiquete,
                                         ue.Nombre + ' ' + ue.Apellido1 AS Empleado,
                                         ut.Nombre + ' ' + ut.Apellido1 AS Tecnico,
                                         t.TipoIncidencia, t.Descripcion, t.Estado, t.FechaCreacion, t.FechaAtencion
                                  FROM dbo.Tiquetes t
                                  INNER JOIN dbo.Usuarios ue ON ue.Cedula = t.Cedula_Empleado
                                  LEFT JOIN dbo.Usuarios ut ON ut.Cedula = t.Cedula_Tecnico
                                  ORDER BY t.FechaCreacion DESC";
            return EjecutarConsulta(sql);
        }

        public int ContarPendientes()
        {
            const string sql = "SELECT COUNT(*) FROM dbo.Tiquetes WHERE Estado = 'Pendiente'";
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                conexion.Open();
                return (int)comando.ExecuteScalar();
            }
        }

        public int ContarAsignadosATecnico(string cedulaTecnico)
        {
            const string sql = @"SELECT COUNT(*) FROM dbo.Tiquetes
                                  WHERE Cedula_Tecnico = @Cedula AND Estado IN ('Asignado', 'En Atencion')";
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Cedula", cedulaTecnico);
                conexion.Open();
                return (int)comando.ExecuteScalar();
            }
        }

        /// <summary>
        /// Detalle de tiquetes pendientes de asignar, para la pantalla de Notificaciones (Administrador TI).
        /// </summary>
        public DataTable ObtenerPendientesDetalle() => EjecutarConsulta(
            @"SELECT t.Id_Tiquete, ue.Nombre + ' ' + ue.Apellido1 AS Empleado, t.TipoIncidencia,
                     t.Descripcion, t.FechaCreacion
              FROM dbo.Tiquetes t
              INNER JOIN dbo.Usuarios ue ON ue.Cedula = t.Cedula_Empleado
              WHERE t.Estado = 'Pendiente' ORDER BY t.FechaCreacion");

        /// <summary>
        /// Detalle de tiquetes asignados a un tecnico y aun no resueltos, para Notificaciones (Tecnico TI).
        /// </summary>
        public DataTable ObtenerAsignadosDetalle(string cedulaTecnico) => EjecutarConsultaParametrizada(
            @"SELECT t.Id_Tiquete, ue.Nombre + ' ' + ue.Apellido1 AS Empleado, t.TipoIncidencia,
                     t.Descripcion, t.Estado, t.FechaCreacion
              FROM dbo.Tiquetes t
              INNER JOIN dbo.Usuarios ue ON ue.Cedula = t.Cedula_Empleado
              WHERE t.Cedula_Tecnico = @Cedula AND t.Estado IN ('Asignado', 'En Atencion')
              ORDER BY t.FechaCreacion",
            "@Cedula", cedulaTecnico);

        // ---------------- Reportes ----------------

        public DataTable ReporteTiquetesPendientes() => EjecutarConsulta(
            @"SELECT t.Id_Tiquete, ue.Nombre + ' ' + ue.Apellido1 AS Empleado, t.Descripcion, t.FechaCreacion
              FROM dbo.Tiquetes t INNER JOIN dbo.Usuarios ue ON ue.Cedula = t.Cedula_Empleado
              WHERE t.Estado = 'Pendiente' ORDER BY t.FechaCreacion");

        public DataTable ReportePorTecnico() => EjecutarConsulta(
            @"SELECT ISNULL(ut.Nombre + ' ' + ut.Apellido1, 'Sin asignar') AS Tecnico, COUNT(*) AS CantidadTiquetes
              FROM dbo.Tiquetes t LEFT JOIN dbo.Usuarios ut ON ut.Cedula = t.Cedula_Tecnico
              GROUP BY ut.Nombre, ut.Apellido1 ORDER BY CantidadTiquetes DESC");

        public DataTable ReportePorEstado() => EjecutarConsulta(
            @"SELECT Estado, COUNT(*) AS Cantidad FROM dbo.Tiquetes GROUP BY Estado ORDER BY Estado");

        public DataTable ReporteTiempoPromedioAtencion() => EjecutarConsulta(
            @"SELECT ISNULL(ut.Nombre + ' ' + ut.Apellido1, 'Sin asignar') AS Tecnico,
                     AVG(DATEDIFF(HOUR, t.FechaCreacion, t.FechaAtencion)) AS HorasPromedioAtencion
              FROM dbo.Tiquetes t LEFT JOIN dbo.Usuarios ut ON ut.Cedula = t.Cedula_Tecnico
              WHERE t.FechaAtencion IS NOT NULL
              GROUP BY ut.Nombre, ut.Apellido1");

        public DataTable ReporteHistorialCompleto() => ObtenerTodos();

        private DataTable EjecutarConsultaParametrizada(string sql, string nombreParametro, string valor)
        {
            var tabla = new DataTable();
            using (var conexion = Conexion.ObtenerConexion())
            using (var adaptador = new SqlDataAdapter(sql, conexion))
            {
                adaptador.SelectCommand.Parameters.AddWithValue(nombreParametro, valor);
                adaptador.Fill(tabla);
            }
            return tabla;
        }

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
