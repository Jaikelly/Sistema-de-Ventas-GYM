using Gym2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Gym2.Controllers
{
    public class AuthController : Controller
    {

        string urlDomain = "http://localhost:1067/";

        Gym2Entities1 db = new Gym2Entities1();

        public ActionResult Index()
        {

            Usuario user = (Usuario)Session["usuario"];


            ViewBag.IdRol = user.idRol;
            ViewBag.Rut = user.rut;
            ViewBag.Nombre = user.nombres;
            ViewBag.Apellido = user.apellidos;
            ViewBag.Telefono = user.telefono;
            ViewBag.Direccion = user.direccion;
            ViewBag.Email = user.email;

            if( user.idRol == 2)
            {
                ViewBag.Venta1 = db.Venta.ToList().Where(rut => rut.idUsuario == user.rut);
                return View();
            }
            

            return View();
            
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string usuario, string password)
        {

            try
            {
                string pass = Encriptar2(password);
                //string pass = Encriptar(Guid.NewGuid().ToString());
                password = pass;

                var user = (from d in db.Usuario where d.email == usuario && d.password == password
                           select d).FirstOrDefault();

                if (user == null)
                {
                    ViewBag.Error = "Credenciales incorrectas";
                    return View();
                }

                Session["usuario"] = user;

                if (user.idRol == 2)
                {
                    return RedirectToAction("Index", "Auth");
                }
                else
                {
                    return RedirectToAction("Index", "Socios");
                }


            }
            catch(Exception e)
            {
                ViewBag.Error2 = e.Message;
                return View();
            }


        }

        public string Encriptar2(string _cadenaAencriptar)
        {
            string result = string.Empty;
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(_cadenaAencriptar);
            result = Convert.ToBase64String(encryted);
            return result;
        }

        public ActionResult LogOut()
        {
            Session["usuario"] = null;

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult StartRecovery()
        {
            Models.ViewModel.RecoveryViewModel model = new Models.ViewModel.RecoveryViewModel();
            return View(model);
        }

        public bool Existe(string email)
        {
            var result = db.Usuario.Any(x => x.email == email);

            if (result)
            {
                ViewBag.Existe = "Email no encontrado, debe ser registrado";
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public ActionResult StartRecovery(Models.ViewModel.RecoveryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string token = Encriptar2(Guid.NewGuid().ToString());


            var user = db.Usuario.Where(d => d.email == model.Email).FirstOrDefault();

            var result = db.Usuario.Any(x => x.email == model.Email);

            if (!result)
            {
                ViewBag.Existe = "Email no registrado";
                return View();
            }


                //guardamos el token en la base de datos, en el campo llamado token
                if (user != null)
                {
                user.token = token;
                //edita el token del user en la bse de datos 
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //enviar el email
                EnviarEmail(user.email, token);
                ViewBag.Enviado = "Email enviado correctamente";
            }

            return View();
        }


        [HttpGet]
        public ActionResult Recovery(string token)
        {

            Models.ViewModel.RecoveryPassword model = new Models.ViewModel.RecoveryPassword();
            model.token = token;


            if (model.token == null || model.token.Trim().Equals(""))
            {
                return View("Login");
            }

            var user = db.Usuario.Where(d => d.token == model.token).FirstOrDefault();
            if (user == null)
            {
                ViewBag.Error2 = "El link ha expirado";
                return View("Login");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Recovery(Models.ViewModel.RecoveryPassword model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = db.Usuario.Where(d => d.token == model.token).FirstOrDefault();

            if (user != null)
            {
                user.password = model.Password;
                user.token = null;
                string pass = Encriptar2(user.password);
                //string pass = Encriptar(Guid.NewGuid().ToString());
                user.password = pass;
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }


            ViewBag.mensaje = "Contraseña modificada con exito";
            return View("Login");
        }

        public string DesEncriptar(string _cadenaAdesencriptar)
        {
            string result = string.Empty;
            byte[] decryted = Convert.FromBase64String(_cadenaAdesencriptar);
            //result = System.Text.Encoding.Unicode.GetString(decryted, 0, decryted.ToArray().Length);
            result = System.Text.Encoding.Unicode.GetString(decryted);
            return result;
        }

        //proceso de envio del correo
        private void EnviarEmail(string destino, string token)
        {
            string origen = "mjaikelly@gmail.com";
            string password = "jaikelly28";
            string url = urlDomain + "Auth/Recovery/?token=" + token;

            //recibe: de donde viene-hacia donde va-asunto-cuerpo del mensaje
            MailMessage mensaje = new MailMessage(origen, destino, "Recuperación de contraseña",
                "<a href='" + url + "'>Click para recuperar</a>");

            mensaje.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Port = 587;
            smtp.Credentials = new System.Net.NetworkCredential(origen, password);

            smtp.Send(mensaje);
            smtp.Dispose();
        }
    }
}