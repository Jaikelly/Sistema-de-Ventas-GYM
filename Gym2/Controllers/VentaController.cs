using Gym2.Filters;
using Gym2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Gym2.Controllers
{
    public class VentaController : Controller
    {

        Gym2Entities1 db = new Gym2Entities1();
        private Usuario dbSocio = new Usuario();
        // GET: Venta

        [AuthorizeUser (idOperacion: 4)]
        public ActionResult Index(string Rut)
        {

            if (!String.IsNullOrEmpty(Rut))
            {
                var ventas = db.Venta.Where(sc => sc.idUsuario.Contains(Rut));
                return View(ventas.ToList());
            }

            return View(db.Venta.ToList());
        }

        [AuthorizeUser(idOperacion: 1)]
        public ActionResult CreateSale(string id)
        {
            ViewBag.Plan1 = db.Plan.ToList();

            if (id == null)
            {
                ViewBag.Socio1 = db.Usuario.ToList();
            }
            else {
                ViewBag.Socio1 = db.Usuario.ToList().Where(c => c.rut == id);
            }
            //ViewBag.Socio1 = db.Socio.Find(id);

            return View();
        }

        public ActionResult Agregar(Venta venta)
        {
            db.Venta.Add(venta);
            db.SaveChanges();

            return RedirectToAction("../Socios/Index");
        }

        public ActionResult Editar(int id, string rut)
        {
            Venta venta = db.Venta.Where(identificador => identificador.id == id).FirstOrDefault();
            ViewBag.Plan1 = db.Plan.ToList();


            if (rut == null)
            {
                ViewBag.Socio1 = db.Usuario.ToList();
            }
            else
            {
                ViewBag.Socio1 = db.Usuario.ToList().Where(c => c.rut == rut);
            }
            return View(venta);
        }

        [HttpPost]
        public ActionResult Editado(Venta venta)
        {
            db.Entry(venta).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AuthorizeUser(idOperacion: 3)]
        public ActionResult Delete(int id)
        {
            if (id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Venta venta = db.Venta.Find(id);
            if (venta == null)
            {
                return HttpNotFound();
            }
            return View(venta);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Venta venta = db.Venta.Find(id);
            db.Venta.Remove(venta);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}