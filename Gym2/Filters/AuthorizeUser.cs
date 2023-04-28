using Gym2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gym2.Filters
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeUser : AuthorizeAttribute 
    {
        private Usuario usu;
        private Gym2Entities1 db = new Gym2Entities1();
        private int idOperacion;

        public AuthorizeUser(int idOperacion = 0)
        {
            this.idOperacion = idOperacion;
        }


        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            String nombre = "";

            try
            {
                usu = (Usuario)HttpContext.Current.Session["usuario"];
                var operaciones = from m in db.Rol_Operaciones
                                  where m.idRol == usu.idRol &&
                                  m.IdOperacion == idOperacion select m;


                if (operaciones.ToList().Count() < 1)
                {
                    var op = db.Operaciones.Find(idOperacion);
                    
                    nombre = getNombreDeOperacion(idOperacion);
                    //nombre = nombre.Replace(' ', '+');
                    filterContext.Result = new RedirectResult("~/Error/UnathorizedOperation?operacion= " + nombre);
                }
            }
            catch (Exception e)
            {
                filterContext.Result = new RedirectResult("~/Error/UnathorizedOperation?operacion= " + nombre);
            }
            {

            }

        }

        public string getNombreDeOperacion(int id)
        {
            var op = from o in db.Operaciones
                     where o.id == id
                     select o.nombre;

            String nombre;
            try
            {
                nombre = op.First();
            }
            catch (Exception)
            {
                nombre = "";
            }

            return nombre;
        }



    }
}