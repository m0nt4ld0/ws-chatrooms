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
/// Summary description for Mensaje
/// </summary>
public class Mensaje
{
    private string msj, login;
    private DateTime fechaHora;

	public Mensaje()
	{
	}

    public string Msj {
        get { return this.msj; }
        set { this.msj = value; }
    }

    public DateTime FechaHora {
        get { return this.fechaHora; }
        set { this.fechaHora = value; }
    }

    public string Login {
        get { return this.login; }
        set { login = value; }
    }
}
