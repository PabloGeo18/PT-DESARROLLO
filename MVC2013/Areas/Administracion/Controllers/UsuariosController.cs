using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Src.Seguridad.Util;
using System.Web.UI.WebControls;
using MVC2013.Src.Comun.Util;

namespace MVC2013.Areas.Administracion.Controllers
{
    public class UsuariosController : Controller
    {
        private AppEntities db = new AppEntities(Constantes.getDataSource());
        private Usuarios Usuario {
            get {
                return db.Usuarios.Find(Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario);
            }
        }
            // GET: Administracion/Usuarios
        public ActionResult Index()
        {
            return View(db.Usuarios.Where(u => !u.eliminado).ToList());
        }

        // GET: Administracion/Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = db.Usuarios.Find(id);
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            return View(usuarios);
        }

        // GET: Administracion/Usuarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "email,password_hash,usuario,nombre_completo_usuario,telefono_trabajo,telefono_movil,usuario_externo")] Usuarios usuarios)
        {
            if (ModelState.IsValid)
            {
                usuarios.password_hash = CipherUtil.Encrypt(usuarios.password_hash);
                usuarios.conteo_accesos_fallidos = 0;
                usuarios.bloqueo_habilitado = false;
                db.Usuarios.Add(usuarios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usuarios);
        }

        // GET: Administracion/Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = db.Usuarios.Find(id);
            usuarios.password_hash = CipherUtil.Decrypt(usuarios.password_hash);

            if (usuarios == null)
            {
                return HttpNotFound();
            }

            ViewBag.id_aplicacion = new SelectList(db.Aplicaciones, "id_aplicacion", "nombre");
            return View(usuarios);
        }

        // POST: Administracion/Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_usuario,email,password_hash,bloqueo_habilitado,usuario,nombre_completo_usuario,usuario_externo")] Usuarios usuarios)
        {
            if (ModelState.IsValid)
            {
                usuarios.password_hash = CipherUtil.Encrypt(usuarios.password_hash);
                db.Entry(usuarios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuarios);
        }

        // GET: Administracion/Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = db.Usuarios.Find(id);
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            return View(usuarios);
        }

        // POST: Administracion/Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuarios usuarios = db.Usuarios.SingleOrDefault(u => !u.eliminado && u.id_usuario == id);
            if (usuarios == null) return HttpNotFound();

            usuarios.id_usuario_eliminacion = Usuario.id_usuario;
            usuarios.fecha_eliminacion = DateTime.Now;
            usuarios.eliminado = true;
            db.Entry(usuarios).State = EntityState.Modified;

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

        ///////////////////////

        public ActionResult GetRolesByApplication(int? id)
        {
            List<object> listReturn = new List<object>();
            if (id == null)
                return Json(listReturn);

            Aplicaciones aplicaciones = db.Aplicaciones.Find(id);    
            foreach(Roles rol in aplicaciones.Roles){
                listReturn.Add(new {nombre = rol.nombre, id = rol.id_rol});
            }

            return Json(listReturn);
        }
        public ActionResult GetClientsByUser(int? id)
        {
            List<object> listReturn = new List<object>();
            Usuarios usuarios = db.Usuarios.Find(id);
            List<int> listCli = new List<int>();

            usuarios.Clientes2.ToList().ForEach(c => listCli.Add(c.id_cliente)); //Previamente usuarios.Clientes3

            db.Clientes.Where(c=> !listCli.Contains(c.id_cliente) && c.activo && !c.eliminado).OrderBy(x=>x.nombre).ToList().ForEach(x => listReturn.Add(new { nombre = x.nombre , id = x.id_cliente }));
         

            return Json(listReturn);
        }

        public ActionResult CreateUserRole(int idRol, int idUsuario) 
        {
            Roles roles = db.Roles.Find(idRol);
            Usuarios usuarios = db.Usuarios.Find(idUsuario);

            usuarios.Roles.Add(roles);
            db.SaveChanges();

            return new EmptyResult();
        }

        public ActionResult RemoveUserRole(int idRol, int idUsuario)
        {
            Roles roles = db.Roles.Find(idRol);
            Usuarios usuarios = db.Usuarios.Find(idUsuario);

            usuarios.Roles.Remove(roles);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = idUsuario });
        }

        public ActionResult CreateUserClient(int idCliente, int idUsuario)
        {
            //Clientes Cliente = db.Clientes.Find(idCliente);
            //Usuarios usuarios = db.Usuarios.Find(idUsuario);

            //usuarios.Clientes3.Add(Cliente);
            //db.SaveChanges();

            return new EmptyResult();
        }

        public ActionResult RemoveUserClient(int idCliente, int idUsuario)
        {
            //Clientes Cliente = db.Clientes.Find(idCliente);
            //Usuarios usuarios = db.Usuarios.Find(idUsuario);

            //usuarios.Clientes3.Remove(Cliente);
            //db.SaveChanges();

            return RedirectToAction("Edit", new { id = idUsuario });
        }
   
    }
}
