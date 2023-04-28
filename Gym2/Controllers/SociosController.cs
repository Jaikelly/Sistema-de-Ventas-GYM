using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Gym2.Filters;
using Gym2.Models;

namespace Gym2.Controllers
{


    public class SociosController : Controller
    {
        private Gym2Entities1 db = new Gym2Entities1();

        [AuthorizeUser(idOperacion: 4)]
        public ActionResult Index(string Rut)
        {
            var usuario = db.Usuario.Include(u => u.Rol);
            if (!String.IsNullOrEmpty(Rut))
            {
                usuario = usuario.Where(sc => sc.rut.Contains(Rut));
            }

            
            return View(usuario.ToList().Where(s => s.idRol == 2));
        }

        [AuthorizeUser(idOperacion: 4)]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuario usuario = db.Usuario.Find(id);
            ViewBag.Venta1 = db.Venta.ToList().Where(rut => rut.idUsuario == id);

            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        public bool Existe(string id)
        {
            var result = db.Usuario.Any(x => x.rut == id);

            if (result){
                ViewBag.Mensaje = "El usuario ya existe";
                return true;
            }
            else
            {
                return false;
            }


        }

        [AuthorizeUser(idOperacion: 1)]
        public ActionResult Create()
        {
            ViewBag.idRol = new SelectList(db.Rol.Where(id => id.id == 2), "id", "nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,rut,nombres,apellidos,fech_nacimiento,telefono,direccion,email,password,idRol")] Usuario usuario)
        {
                if (!Existe(usuario.rut)) {

                    if (ModelState.IsValid)
                    {
                        string pass = Encriptar2(usuario.password);
                        //string pass = Encriptar(Guid.NewGuid().ToString());
                        usuario.password = pass;
                        db.Usuario.Add(usuario);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }

                }

            ViewBag.idRol = new SelectList(db.Rol, "id", "nombre", usuario.idRol);
            return View(usuario);
        }

        [AuthorizeUser(idOperacion: 2)]
        public ActionResult Edit(string id)
        {

            Usuario usuario = db.Usuario.Find(id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            Usuario user = (Usuario)Session["usuario"];

            if (user.idRol == 3)
            {
                if (usuario.idRol == 3 || usuario.idRol == 1)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (id == null || id == user.rut)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ViewBag.idRol = new SelectList(db.Rol.Where(d => d.id == 2), "id", "nombre", usuario.idRol);
            }

            if (user.idRol == 1)
            {
                ViewBag.idRol = new SelectList(db.Rol, "id", "nombre", usuario.idRol);
            }

            
            return View(usuario);
        }

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
            ViewBag.idRol = new SelectList(db.Rol.Where(id => id.id == 2), "id", "nombre", usuario.idRol);
            return View(usuario);
        }

        [AuthorizeUser(idOperacion: 3)]
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

        /// Encripta una cadena
        public string Encriptar2( string _cadenaAencriptar)
        {
            string result = string.Empty;
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(_cadenaAencriptar);
            result = Convert.ToBase64String(encryted);
            return result;
        }

        /// Esta función desencripta la cadena que le envíamos en el parámentro de entrada.
        public string DesEncriptar( string _cadenaAdesencriptar)
        {
            string result = string.Empty;
            byte[] decryted = Convert.FromBase64String(_cadenaAdesencriptar);
            //result = System.Text.Encoding.Unicode.GetString(decryted, 0, decryted.ToArray().Length);
            result = System.Text.Encoding.Unicode.GetString(decryted);
            return result;
        }


        public ActionResult Ventas(int id)
        {
            Venta venta = db.Venta.Where(identificador => identificador.id == id).FirstOrDefault();
            ViewBag.Plan1 = db.Plan.Find(id);
            return View(venta);
        }



    }
}
