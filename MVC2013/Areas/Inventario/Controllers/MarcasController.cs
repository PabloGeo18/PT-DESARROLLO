using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Src.Comun.Util;
using MVC2013.Src.Seguridad.To;

namespace MVC2013.Areas.Inventario.Controllers
{
    public class MarcasController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Marcas
        public ActionResult Index()
        {
            var marcas = db.Marcas.Include(m => m.Usuarios).Include(m => m.Usuarios1).Include(m => m.Usuarios2);
            return View(marcas.ToList());
        }

        // GET: Inventario/Marcas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Marcas marcas = db.Marcas.Find(id);
            if (marcas == null)
            {
                return HttpNotFound();
            }
            return View(marcas);
        }

        // GET: Inventario/Marcas/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Inventario/Marcas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_marca,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Marcas marcas)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                marcas.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                marcas.fecha_creacion = DateTime.Now;
                marcas.activo = true;
                marcas.eliminado = false;
                db.Marcas.Add(marcas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", marcas.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", marcas.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", marcas.id_usuario_modificacion);
            return View(marcas);
        }

        // GET: Inventario/Marcas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Marcas marcas = db.Marcas.Find(id);
            if (marcas == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", marcas.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", marcas.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", marcas.id_usuario_modificacion);
            ViewBag.id_inventario_tipo = new SelectList(db.Inventario_Tipo, "id_inventario_tipo", "Descripcion");
            return View(marcas);
        }

        // POST: Inventario/Marcas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_marca,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Marcas marcas)
        {
            if (ModelState.IsValid)
            {
                Marcas marcasEdit = db.Marcas.Find(marcas.id_marca);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                marcasEdit.descripcion = marcas.descripcion;
                marcasEdit.activo = marcas.activo;
                marcasEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                marcasEdit.fecha_modificacion = DateTime.Now;
                marcasEdit.eliminado = false;
                db.Entry(marcasEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", marcas.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", marcas.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", marcas.id_usuario_modificacion);
            return View(marcas);
        }

        // GET: Inventario/Marcas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Marcas marcas = db.Marcas.Find(id);
            if (marcas == null)
            {
                return HttpNotFound();
            }
            return View(marcas);
        }

        // POST: Inventario/Marcas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Marcas marcas = db.Marcas.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            marcas.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            marcas.fecha_eliminacion = DateTime.Now;
            marcas.eliminado = true;
            marcas.activo = false;
            db.Entry(marcas).State = EntityState.Modified;
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

        public ActionResult crear_marca_tipo(int id_inventario_tipo, int id_marca)
        {
            if (ModelState.IsValid)
            {
                Marca_Tipo MT = new Marca_Tipo();
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                MT.id_inventario_tipo=id_inventario_tipo;
                MT.id_marca = id_marca;
                MT.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                MT.fecha_creacion = DateTime.Now;
                MT.activo = true;
                MT.eliminado = false;
                db.Marca_Tipo.Add(MT); 
                db.SaveChanges();
                return RedirectToAction("Edit", new { id=id_marca});
            }
            return RedirectToAction("Edit", new { id = id_marca });
        }

        public ActionResult del_marca_tipo(int id_marca_tipo, int id_marca)
        {
            if (ModelState.IsValid)
            {
                Marca_Tipo MT = db.Marca_Tipo.Find(id_marca_tipo);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                MT.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                MT.fecha_eliminacion = DateTime.Now;
                MT.activo = false;
                MT.eliminado = true;
                db.Entry(MT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = id_marca });
            }
            return RedirectToAction("Edit", new { id = id_marca });
        }
    }
}

