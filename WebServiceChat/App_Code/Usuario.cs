using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Usuario del chat
/// </summary>
public class Usuario
{
    private bool activo=false,esAdmin = false;
    private string login, password, nick;
    private DateTime fultlog; //Fecha de ultimo logueo
	public Usuario()
	{
	}
    
    public bool Activo{
        get{return activo;}
        set{activo=value;}
    }

    public bool EsAdmin
    {
        get { return esAdmin; }
        set { esAdmin = value; }
    }

    public string Login
    {
        get { return login; }
        set { login = value; }
    }

    public string Password
    {
        get { return password; }
        set { password = value; }
    }

    public string Nick
    {
        get { return nick; }
        set { nick = value; }
    }

    public DateTime FUltLog{
        get{ return fultlog; }
        set{ fultlog = value; }
    }
}
