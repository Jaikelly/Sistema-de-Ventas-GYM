using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gym2.Filters;
using Gym2.Models;

namespace Gym2.Controllers
{
    public class PersonalController : Controller
    {
        private Gym2Entities1 db = new Gym2Entities1();


        [AuthorizeUser (idOperacion: 6)]
        // GET: Personal
        public ActionResult Index(string Rut)
        {
            var usuario = db.Usuario.Include(u => u.Rol);
            var list1 = usuario.ToList().Where(s => s.idRol == 3);
            var list2 = usuario.ToList().Where(s => s.idRol == 1);
            var result = list1.Union(list2);

            var usu = db.Usuario.Include(u => u.Rol);
            if (!String.IsNullOrEmpty(Rut))
            {
                result = usu.Where(sc => sc.rut.Contains(Rut));

            }


            return View(result);
        }

        [AuthorizeUser(idOperacion: 6)]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        [AuthorizeUser(idOperacion: 6)]
        public ActionResult Create()
        {
            ViewBag.idRol = new SelectList(db.Rol, "id", "nombre");
            return View();
        }

        public bool Existe(string id)
        {
            var result = db.Usuario.Any(x => x.rut == id);

            if (result)
            {
                ViewBag.Mensaje = "El usuario ya existe";
                return true;
            }
            else
            {
                return false;
            }


        }

        [AuthorizeUser(idOperacion: 6)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,rut,nombres,apellidos,fech_nacimiento,telefono,direccion,email,password,idRol")] Usuario usuario)
        {

            if (!Existe(usuario.rut))
            {
                if (ModelState.IsValid)
                {
                    db.Usuario.Add(usuario);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.idRol = new SelectList(db.Rol, "id", "nombre", usuario.idRol);
            return View(usuario);
        }

        [AuthorizeUser(idOperacion: 6)]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.idRol = new SelectList(db.Rol, "id", "nombre", usuario.idRol);
            return View(usuario);
        }

        [AuthorizeUser(idOperacion: 6)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,rut,nombres,apellidos,fech_nacimiento,telefono,direccion,email,password,idRol")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idRol = new SelectList(db.Rol, "id", "nombre", usuario.idRol);
            return View(usuario);
        }

        [AuthorizeUser(idOperacion: 6)]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        [AuthorizeUser(idOperacion: 6)]
        // POST: Personal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Usuario usuario = db.Usuario.Find(id);
            db.Usuario.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
