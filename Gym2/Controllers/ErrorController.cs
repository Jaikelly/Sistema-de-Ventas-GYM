using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gym2.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        [HttpGet]
        public ActionResult UnathorizedOperation(String operacion, string mensaje)
        {

            ViewBag.operacion = operacion;
            ViewBag.mensaje = mensaje;

            return View();
        }
    }
}