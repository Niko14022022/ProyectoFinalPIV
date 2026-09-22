using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ProyectoFinalPIV.Modelos;

namespace ProyectoFinalPIV.Datos
{
    /// <summary>
    /// Acceso a datos para la tabla Usuarios: autenticacion y mantenimiento (CRUD).
    /// </summary>
    public class UsuarioRepositorio
    {
        /// <summary>
        /// Obtiene la fila de Credenciales asociada a una cedula, o null si no existe.
        /// Sirve para que el formulario de login sepa que tipo de autenticacion
        /// mostrar (clave, o solo el boton de "ingresar con Windows").
        /// </summary>
        public Credencial ObtenerCredencial(string cedula)
        {
            const string sql = @"SELECT Cedula, TipoAutenticacion, Clave, UsuarioWindows,
                                         FechaUltimoCambioClave
                                  FROM dbo.Credenciales WHERE Cedula = @Cedula";

            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Cedula", cedula);
                conexion.Open();
                using (var lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        return new Credencial
                        {
                            Cedula = lector.GetString(0),
                            TipoAutenticacion = lector.GetString(1),
                            Clave = lector.IsDBNull(2) ? null : lector.GetString(2),
                            UsuarioWindows = lector.IsDBNull(3) ? null : lector.GetString(3),
                            FechaUltimoCambioClave = lector.IsDBNull(4) ? (DateTime?)null : lector.GetDateTime(4)
                        };
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Autenticacion tipo "Sistema": valida cedula + clave contra Credenciales,
        /// y trae el perfil desde Usuarios. Devuelve null si no coincide o esta inactivo.
        /// </summary>
        public Usuario AutenticarPorSistema(string cedula, string clave)
        {
            const string sql = @"SELECT u.Cedula, u.Nombre, u.Apellido1, u.Apellido2, u.Rol,
                                         u.Fecha_Creacion_Usuario, u.Activo
                                  FROM dbo.Usuarios u
                                  INNER JOIN dbo.Credenciales c ON c.Cedula = u.Cedula
                                  WHERE u.Cedula = @Cedula
                                    AND c.TipoAutenticacion = 'Sistema'
                                    AND c.Clave = @Clave
                                    AND u.Activo = 1";

            return EjecutarConsultaUsuario(sql, cedula, clave);
        }

        /// <summary>
        /// Autenticacion tipo "Windows": valida que la cedula tenga registrado el
        /// mismo usuario de Windows que tiene la sesion activa (sin pedir clave).
        /// </summary>
        public Usuario AutenticarPorWindows(string cedula, string usuarioWindowsActual)
        {
            const string sql = @"SELECT u.Cedula, u.Nombre, u.Apellido1, u.Apellido2, u.Rol,
                                         u.Fecha_Creacion_Usuario, u.Activo
                                  FROM dbo.Usuarios u
                                  INNER JOIN dbo.Credenciales c ON c.Cedula = u.Cedula
                                  WHERE u.Cedula = @Cedula
                                    AND c.TipoAutenticacion = 'Windows'
                                    AND c.UsuarioWindows = @UsuarioWindows
                                    AND u.Activo = 1";

            return EjecutarConsultaUsuario(sql, cedula, usuarioWindowsActual, nombreParametro2: "@UsuarioWindows");
        }

        private Usuario EjecutarConsultaUsuario(string sql, string cedula, string valorSecundario,
            string nombreParametro2 = "@Clave")
        {
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Cedula", cedula);
                comando.Parameters.AddWithValue(nombreParametro2, valorSecundario);

                conexion.Open();
                using (var lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        return new Usuario
                        {
                            Cedula = lector.GetString(0),
                            Nombre = lector.GetString(1),
                            Apellido1 = lector.GetString(2),
                            Apellido2 = lector.IsDBNull(3) ? "" : lector.GetString(3),
                            Rol = lector.GetString(4),
                            Fecha_Creacion_Usuario = lector.GetDateTime(5),
                            Activo = lector.GetBoolean(6)
                        };
                    }
                }
            }
            return null;
        }

        public List<Usuario> ObtenerTodos()
        {
            var lista = new List<Usuario>();
            const string sql = @"SELECT Cedula, Nombre, Apellido1, Apellido2, Rol,
                                         Fecha_Creacion_Usuario, Activo
                                  FROM dbo.Usuarios ORDER BY Nombre";

            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                conexion.Open();
                using (var lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(new Usuario
                        {
                            Cedula = lector.GetString(0),
                            Nombre = lector.GetString(1),
                            Apellido1 = lector.GetString(2),
                            Apellido2 = lector.IsDBNull(3) ? "" : lector.GetString(3),
                            Rol = lector.GetString(4),
                            Fecha_Creacion_Usuario = lector.GetDateTime(5),
                            Activo = lector.GetBoolean(6)
                        });
                    }
                }
            }
            return lista;
        }

        /// <summary>
        /// Inserta el perfil en Usuarios y su credencial correspondiente en Credenciales,
        /// dentro de la misma transaccion (o ambas tablas quedan consistentes, o ninguna).
        /// </summary>
        public bool ExisteCedula(string cedula)
        {
            const string sql = "SELECT COUNT(*) FROM dbo.Usuarios WHERE Cedula = @Cedula";
            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Cedula", cedula);
                conexion.Open();
                return (int)comando.ExecuteScalar() > 0;
            }
        }

        public void Incluir(Usuario usuario, Credencial credencial)
        {
            if (ExisteCedula(usuario.Cedula))
                throw new InvalidOperationException($"Ya existe un usuario registrado con la cedula '{usuario.Cedula}'.");

            const string sqlUsuario = @"INSERT INTO dbo.Usuarios
                                  (Cedula, Nombre, Apellido1, Apellido2, Rol)
                                  VALUES (@Cedula, @Nombre, @Apellido1, @Apellido2, @Rol)";

            const string sqlCredencial = @"INSERT INTO dbo.Credenciales
                                  (Cedula, TipoAutenticacion, Clave, UsuarioWindows, FechaUltimoCambioClave)
                                  VALUES (@Cedula, @TipoAutenticacion, @Clave, @UsuarioWindows, GETDATE())";

            using (var conexion = Conexion.ObtenerConexion())
            {
                conexion.Open();
                using (var transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        using (var comando = new SqlCommand(sqlUsuario, conexion, transaccion))
                        {
                            comando.Parameters.AddWithValue("@Cedula", usuario.Cedula);
                            comando.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                            comando.Parameters.AddWithValue("@Apellido1", usuario.Apellido1);
                            comando.Parameters.AddWithValue("@Apellido2", (object)usuario.Apellido2 ?? DBNull.Value);
                            comando.Parameters.AddWithValue("@Rol", usuario.Rol);
                            comando.ExecuteNonQuery();
                        }

                        using (var comando = new SqlCommand(sqlCredencial, conexion, transaccion))
                        {
                            comando.Parameters.AddWithValue("@Cedula", credencial.Cedula);
                            comando.Parameters.AddWithValue("@TipoAutenticacion", credencial.TipoAutenticacion);
                            comando.Parameters.AddWithValue("@Clave", (object)credencial.Clave ?? DBNull.Value);
                            comando.Parameters.AddWithValue("@UsuarioWindows", (object)credencial.UsuarioWindows ?? DBNull.Value);
                            comando.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Actualizar(Usuario usuario)
        {
            const string sql = @"UPDATE dbo.Usuarios
                                  SET Nombre = @Nombre, Apellido1 = @Apellido1, Apellido2 = @Apellido2,
                                      Rol = @Rol, Activo = @Activo
                                  WHERE Cedula = @Cedula";

            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Cedula", usuario.Cedula);
                comando.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                comando.Parameters.AddWithValue("@Apellido1", usuario.Apellido1);
                comando.Parameters.AddWithValue("@Apellido2", (object)usuario.Apellido2 ?? DBNull.Value);
                comando.Parameters.AddWithValue("@Rol", usuario.Rol);
                comando.Parameters.AddWithValue("@Activo", usuario.Activo);

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Actualiza unicamente la credencial de un usuario (por ejemplo, al cambiar
        /// de clave o al cambiar el tipo de autenticacion de Sistema a Windows).
        /// </summary>
        public void ActualizarCredencial(Credencial credencial)
        {
            const string sql = @"UPDATE dbo.Credenciales
                                  SET TipoAutenticacion = @TipoAutenticacion,
                                      Clave = @Clave,
                                      UsuarioWindows = @UsuarioWindows,
                                      FechaUltimoCambioClave = GETDATE()
                                  WHERE Cedula = @Cedula";

            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Cedula", credencial.Cedula);
                comando.Parameters.AddWithValue("@TipoAutenticacion", credencial.TipoAutenticacion);
                comando.Parameters.AddWithValue("@Clave", (object)credencial.Clave ?? DBNull.Value);
                comando.Parameters.AddWithValue("@UsuarioWindows", (object)credencial.UsuarioWindows ?? DBNull.Value);

                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void Eliminar(string cedula)
        {
            // Eliminacion logica en lugar de borrado fisico, para no romper llaves foraneas
            const string sql = "UPDATE dbo.Usuarios SET Activo = 0 WHERE Cedula = @Cedula";

            using (var conexion = Conexion.ObtenerConexion())
            using (var comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@Cedula", cedula);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }
    }
}
