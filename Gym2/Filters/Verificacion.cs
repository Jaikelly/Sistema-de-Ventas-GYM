using Gym2.Controllers;
using Gym2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gym2.Filters
{

                                //herencia
    public class Verificacion : ActionFilterAttribute
    {
        private Usuario usu;

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                base.OnActionExecuted(filterContext);

                //verificamos la sesion
                usu = (Usuario)HttpContext.Current.Session["usuario"];
                if(usu == null)
                {
                    if (filterContext.Controller is AuthController == false)
                    {
                        //si no hay sesion activa siempre se va a ir a el Login
                        //filterContext.HttpContext.Response.Redirect("~/Auth/Index");
                    }
                }
            }
            catch
            {

            }
        }

    }
}