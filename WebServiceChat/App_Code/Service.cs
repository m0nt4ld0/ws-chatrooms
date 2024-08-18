using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Web.Configuration;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Service : System.Web.Services.WebService
{
    public Service () {
    
    }

    [WebMethod(EnableSession = true)]
    public bool crearOEditarUsuario(string login, string pwd, string nick)
    {
        //Recibe login(pseudonimo para conectarse), contrasena y nickname (nombre para mostrar)
        //Solamente admin puede crear/editar usuarios y el admin debe estar logueado
        if (Session["login"] == null)
        {
            return false;
        }
        string admin = "SELECT Administrador FROM Usuario WHERE Login = @log_in";
        SqlDataReader drEsAdmin;
        SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["ConexionChat"].ConnectionString);
        conexion.Open();
        SqlCommand consulta_admin = new SqlCommand(admin, conexion);
        consulta_admin.Parameters.AddWithValue("@log_in",Session["login"].ToString());
        drEsAdmin = consulta_admin.ExecuteReader();
        drEsAdmin.Read();
        if ((bool)drEsAdmin["Administrador"] == false)
        {   // No es admin, no tiene permisos
            conexion.Close();
            return false;
        }
        else
        {
            drEsAdmin.Close();
            string consulta = "IF EXISTS (SELECT * FROM Usuario WHERE Login = @login) UPDATE Usuario SET Nombre=@nombre, Password=@password WHERE Login=@login ELSE INSERT INTO Usuario (Login, Nombre, Password) VALUES (@login,@nombre,@password)";
            SqlDataReader drInsercion;
            consulta_admin.CommandText = consulta;
            consulta_admin.Parameters.AddWithValue("@login",login);
            consulta_admin.Parameters.AddWithValue("@nombre",nick);
            consulta_admin.Parameters.AddWithValue("@password",pwd);
            drInsercion = consulta_admin.ExecuteReader();
        }
        conexion.Close();
        return true;
    }

    [WebMethod(EnableSession = true)]
    public bool login(string usuario, string password) {
        //Recibe el pseudonimo que se utiliza para conectarse y la contrasena
        string consulta = "SELECT * FROM Usuario WHERE Login=@UsrName and Password=@Password";
        string actualizacion = "UPDATE Usuario SET FechaHoraUltimaConexion = getDate() WHERE Login=@Nick";
        SqlDataReader drLogin, drActualizacion;
        SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["ConexionChat"].ConnectionString);
        conexion.Open();
        SqlCommand comm_consulta = new SqlCommand(consulta, conexion);
        comm_consulta.Parameters.AddWithValue("@UsrName", usuario);
        comm_consulta.Parameters.AddWithValue("@Password", password);
        drLogin = comm_consulta.ExecuteReader();
        if (drLogin.HasRows)
        {
            Session["login"] = usuario;
            drLogin.Close();
            comm_consulta.CommandText = actualizacion;
            comm_consulta.Parameters.AddWithValue("@Nick", usuario);
            drActualizacion = comm_consulta.ExecuteReader();
            conexion.Close();
            return true;
        }
        else
        {
            return false;
        }
    }

    [WebMethod(EnableSession = true)]
    public bool logout()
    {
        //Devuelve true si estaba conectado y cambia la variable de sesion a null, de lo contrario false
        if (Session["login"] != null) 
        {
            Session["login"] = null;
            return true;
        }
        return false;
    }

    [WebMethod(EnableSession = true)]
    public DateTime ultAcceso()
    {
        //Devuelve 01-01-0001 si no esta logueado, es decir "fecha vacía"
        DateTime fecha = new DateTime();
        if (Session["login"] == null)
            return fecha;
        string consulta = "SELECT FechaHoraUltimaConexion FROM Usuario WHERE Login=@Usuario";
        SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["ConexionChat"].ConnectionString);
        conexion.Open();
        SqlCommand comm_consulta = new SqlCommand(consulta, conexion);
        comm_consulta.Parameters.AddWithValue("@Usuario", Session["login"].ToString());
        SqlDataReader drUltAcceso = comm_consulta.ExecuteReader();
        if(drUltAcceso.Read())
        {
            fecha = (DateTime)drUltAcceso["FechaHoraUltimaConexion"];
        }
        conexion.Close();
        return fecha;
    }

    [WebMethod]
    public List<Sala> obtenerSalas()
    {
        //Devuelve una lista con las salas. Los miembros de información de Sala son Id de sala, nombre y si está activa
        string consulta = "SELECT IdSala,Activa,Nombre FROM Sala";
        SqlDataReader drSalas;
        SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["ConexionChat"].ConnectionString);
        conexion.Open();
        List<Sala> listaSalas = new List<Sala>(); //System.Collections.Generic
        SqlCommand comm_consulta = new SqlCommand(consulta, conexion);
        drSalas = comm_consulta.ExecuteReader();
        while (drSalas.Read())
        {
            Sala aux = new Sala();
            aux.IDSala = (int)drSalas["IdSala"];
            aux.Nombre = drSalas["Nombre"].ToString();
            aux.Activo = (bool)drSalas["Activa"];
            listaSalas.Add(aux);
        }
        conexion.Close();
        return listaSalas;
    }

    [WebMethod(EnableSession = true)]
    public List<Mensaje> obtenerMensajes(int idSala)
    {
        //Devuelve una lista con los mensajes de la sala. Los atributos de informacion de la clase Mensaje son
        //FechaHora, Msj y Login.
        List<Mensaje> listaMsjs = new List<Mensaje>();
        if (Session["login"] == null)
            return null; //No puede recibir mensajes si no esta logueado
        SqlDataReader drMensajes;
        string consulta = "SELECT Login,FechaHora,Texto FROM Mensaje WHERE IdSala=@idSala AND FechaHora BETWEEN @Desde AND @Hasta";
        SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["ConexionChat"].ConnectionString);
        conexion.Open();
        SqlCommand comm_consulta = new SqlCommand(consulta, conexion);
        comm_consulta.Parameters.AddWithValue("@idSala", idSala);
        comm_consulta.Parameters.AddWithValue("@Desde", (DateTime)Session["peticion"]);
        comm_consulta.Parameters.AddWithValue("@Hasta", DateTime.Now);

        drMensajes = comm_consulta.ExecuteReader();
        while (drMensajes.Read())
        {
            Mensaje aux = new Mensaje();
            aux.FechaHora = (DateTime)drMensajes["FechaHora"];
            aux.Msj = drMensajes["Texto"].ToString();
            aux.Login = drMensajes["Login"].ToString();
            listaMsjs.Add(aux);
        }
        conexion.Close();
        return listaMsjs;
    }

    [WebMethod(EnableSession = true)]
    public List<Mensaje> obtenerReporte(int idSala, DateTime fv, DateTime fn)
    {
        //Recibe Id de sala, fecha desde y fecha hasta.
        //Devuelve una lista con los mensajes entre dos fechas.
        List<Mensaje> listaMsjs = new List<Mensaje>();
        if (Session["login"] == null)
            return null; //No puede recibir mensajes si no esta logueado
        SqlDataReader drMensajes;
        string consulta = "SELECT Login,FechaHora,Texto FROM Mensaje WHERE IdSala=@idSala AND FechaHora BETWEEN @Desde AND @Hasta";
        SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["ConexionChat"].ConnectionString);
        conexion.Open();
        SqlCommand comm_consulta = new SqlCommand(consulta, conexion);
        comm_consulta.Parameters.AddWithValue("@idSala", idSala);
        comm_consulta.Parameters.AddWithValue("@Desde", fv);
        comm_consulta.Parameters.AddWithValue("@Hasta", fn);

        drMensajes = comm_consulta.ExecuteReader();
        while (drMensajes.Read())
        {
            Mensaje aux = new Mensaje();
            aux.FechaHora = (DateTime)drMensajes["FechaHora"];
            aux.Msj = drMensajes["Texto"].ToString();
            aux.Login = drMensajes["Login"].ToString();
            listaMsjs.Add(aux);
        }
        conexion.Close();
        return listaMsjs;
    }

    [WebMethod(EnableSession = true)]
    public List<Mensaje> enviarMensaje(int idSala, string mensaje)
    {
        //Envia el mensaje en la sala dada, luego devuelve una lista con los mensajes de los ultimos diez segundos
        List<Mensaje> listaMsjs = new List<Mensaje>();
        if (Session["login"] == null)
            return null; //No puede enviar mensaje si no esta logueado
        if (Session["peticion"] == null)
        {
            Session["peticion"] = DateTime.Now; //Al momento de enviar si no habia peticion la creo en este momento
        }
        string insercion = "INSERT INTO Mensaje (Login, IdSala, FechaHora, Texto) VALUES (@login,@idsala,getDate(),@mensaje)";
        SqlDataReader drInsercion;
        SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["ConexionChat"].ConnectionString);
        conexion.Open();
        //Insercion del nuevo mensaje
        SqlCommand comm_consulta = new SqlCommand(insercion, conexion);
        comm_consulta.Parameters.AddWithValue("@login",Session["login"].ToString());
        comm_consulta.Parameters.AddWithValue("@idsala", idSala);
        comm_consulta.Parameters.AddWithValue("@mensaje", mensaje);
        drInsercion = comm_consulta.ExecuteReader();
        drInsercion.Close();
        listaMsjs = obtenerReporte(idSala,DateTime.Now.AddSeconds(-10),DateTime.Now);
        conexion.Close();
        return listaMsjs;
    }

    [WebMethod(EnableSession = true)]
    public List<Usuario> obtenerUsuariosActivos(int idsala) {
        //Devuelve una lista de usuarios que escribieron algun mensaje durante los ultimos diez minutos
        List<Usuario> listaUsrs = new List<Usuario>();
        string activos = "SELECT Login FROM Mensaje WHERE idSala=@id AND FechaHora BETWEEN @fh AND getDate() GROUP BY Login";
        SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["ConexionChat"].ConnectionString);
        SqlDataReader drUsrsActivos;
        SqlCommand sqlUsrsActivos = new SqlCommand(activos, conexion);
        conexion.Open();
        sqlUsrsActivos.Parameters.AddWithValue("@id", idsala);
        //Se considera usuario activo al que escribio un mensaje hace diez minutos o menos
        sqlUsrsActivos.Parameters.AddWithValue("@fh", DateTime.Now.AddMinutes(-10));
        drUsrsActivos = sqlUsrsActivos.ExecuteReader();
        while (drUsrsActivos.Read())
        {
            Usuario usr = new Usuario();
            usr.Activo = true;
            usr.Login = drUsrsActivos["Login"].ToString();
            listaUsrs.Add(usr);
        }
        drUsrsActivos.Close();
        conexion.Close();
        return listaUsrs;
    }
}
