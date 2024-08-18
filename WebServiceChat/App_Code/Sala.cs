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
/// Sala de chat, cada tematica tiene una sala distinta
/// </summary>
public class Sala
{
    private string nombre;
    private bool activo;
    private int idSala;

	public Sala()
	{
		
	}

    public string Nombre{
        get{ return nombre; }
        set{ nombre=value; }
    }

    public bool Activo
    {
        get { return activo; }
        set { activo = value; }
    }

    public int IDSala
    {
        get { return idSala; }
        set { idSala = value; }
    }
}
