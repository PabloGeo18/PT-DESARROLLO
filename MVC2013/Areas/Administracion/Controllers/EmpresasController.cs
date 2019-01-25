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

namespace MVC2013.Areas.Administracion.Controllers
{
    public class EmpresasController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Empresas
        public ActionResult Index()
        {
            var empresa = db.Empresa.Where(e => e.eliminado == false).OrderBy(e => e.nombre);
            return View(empresa.ToList());
        }

        // GET: Administracion/Empresas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresa.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
        }

        // GET: Administracion/Empresas/Create
        public ActionResult Create()
        {
            ViewBag.pais = new SelectList(db.Paises.Where(e => !e.eliminado), "id_pais", "nombre");
            ViewBag.municipios = new SelectList(db.Municipios.Where(m => !m.eliminado), "id_municipio", "nombre");
            return View();
        }

        // POST: Administracion/Empresas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                empresa.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                empresa.fecha_creacion = DateTime.Now;
                empresa.activo = true;
                empresa.eliminado = false;
                empresa.centro_trabajo = !String.IsNullOrEmpty(empresa.centro_trabajo) ? empresa.centro_trabajo.ToUpper() : "";
                empresa.contacto = !String.IsNullOrEmpty(empresa.centro_trabajo) ? empresa.centro_trabajo.ToUpper() : "";
                empresa.direccion = !String.IsNullOrEmpty(empresa.direccion) ? empresa.direccion.ToUpper() : "";
                empresa.nombre = !String.IsNullOrEmpty(empresa.nombre) ? empresa.nombre.ToUpper() : "";
                empresa.nombre_comercial = !String.IsNullOrEmpty(empresa.nombre_comercial) ? empresa.nombre_comercial.ToUpper() : "";
                db.Empresa.Add(empresa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.pais = new SelectList(db.Paises.Where(e => !e.eliminado), "id_pais", "nombre");
            ViewBag.municipios = new SelectList(db.Municipios.Where(m => !m.eliminado), "id_municipio", "nombre");
            return View(empresa);
        }

        // GET: Administracion/Empresas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresa.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            ViewBag.pais = new SelectList(db.Paises.Where(e => !e.eliminado), "id_pais", "nombre", empresa.id_pais);
            ViewBag.municipios = new SelectList(db.Municipios.Where(m => !m.eliminado), "id_municipio", "nombre", empresa.id_municipio);
            return View(empresa);
        }

        // POST: Administracion/Empresas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                Empresa empresaEdit = db.Empresa.SingleOrDefault(e => e.id_empresa == empresa.id_empresa && !e.eliminado);
                if (empresaEdit == null)
                {
                    return HttpNotFound();
                }
                empresaEdit.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                empresaEdit.fecha_modificacion = DateTime.Now;
                empresaEdit.centro_trabajo = !String.IsNullOrEmpty(empresa.centro_trabajo) ? empresa.centro_trabajo.ToUpper() : "";
                empresaEdit.contacto = !String.IsNullOrEmpty(empresa.centro_trabajo) ? empresa.centro_trabajo.ToUpper() : "";
                empresaEdit.direccion = !String.IsNullOrEmpty(empresa.direccion) ? empresa.direccion.ToUpper() : "";
                empresaEdit.nombre = !String.IsNullOrEmpty(empresa.nombre) ? empresa.nombre.ToUpper() : "";
                empresaEdit.nombre_comercial = !String.IsNullOrEmpty(empresa.nombre_comercial) ? empresa.nombre_comercial.ToUpper() : "";
                empresaEdit.correo = empresa.correo;
                empresaEdit.encabezado_boleta = empresa.encabezado_boleta;
                empresaEdit.fax = empresa.fax;
                empresaEdit.id_municipio = empresa.id_municipio;
                empresaEdit.id_pais = empresa.id_pais;
                empresaEdit.nit = empresa.nit;
                empresaEdit.numero_patronal = empresa.numero_patronal;
                empresaEdit.prestaciones = empresa.prestaciones;
                empresaEdit.telefono = empresa.telefono;
                empresaEdit.zona = empresa.zona;
                db.Entry(empresaEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.pais = new SelectList(db.Paises.Where(e => !e.eliminado), "id_pais", "nombre", empresa.id_pais);
            ViewBag.municipios = new SelectList(db.Municipios.Where(m => !m.eliminado), "id_municipio", "nombre", empresa.id_municipio);
            return View(empresa);
        }

        // GET: Administracion/Empresas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empresa empresa = db.Empresa.Find(id);
            if (empresa == null)
            {
                return HttpNotFound();
            }
            return View(empresa);
        }

        // POST: Administracion/Empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Empresa empresaDelete = db.Empresa.SingleOrDefault(e => e.id_empresa == id && !e.eliminado);
            if (empresaDelete == null)
            {
                return HttpNotFound();
            }
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            empresaDelete.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            empresaDelete.fecha_eliminacion = DateTime.Now;
            empresaDelete.eliminado = true;
            empresaDelete.activo = false;
            db.Entry(empresaDelete).State = EntityState.Modified;
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
