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
    public class DepartamentosController : Controller
    {
        private AppEntities db = new AppEntities(Constantes.getDataSource());

        // GET: Administracion/Departamentos
        public ActionResult Index()
        {
            var departamentos = db.Departamentos.Where(x => x.eliminado == false).Include(d => d.Paises);
            return View(departamentos.ToList());
        }

        // GET: Administracion/Departamentos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departamentos departamentos = db.Departamentos.Find(id);
            if (departamentos == null)
            {
                return HttpNotFound();
            }
            return View(departamentos);
        }

        // GET: Administracion/Departamentos/Create
        public ActionResult Create()
        {
            ViewBag.id_pais = new SelectList(db.Paises.Where(x => x.activo == true).Where(y => y.eliminado == false), "id_pais", "nombre");
            return View();
        }

        // POST: Administracion/Departamentos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_departamento,id_pais,nombre")] Departamentos departamentos)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                departamentos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                departamentos.fecha_creacion = DateTime.Now;
                departamentos.activo = true;
                departamentos.eliminado = false;
                db.Departamentos.Add(departamentos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_pais = new SelectList(db.Paises.Where(x => x.eliminado == false).Where(y => y.activo == true), "id_pais", "nombre", departamentos.id_pais);
            return View(departamentos);
        }

        // GET: Administracion/Departamentos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departamentos departamentos = db.Departamentos.Find(id);
            if (departamentos == null)
            {
                return HttpNotFound();
            }

            ViewBag.id_pais = new SelectList(db.Paises.Where(x => x.activo == true).Where(y => y.eliminado == false), "id_pais", "nombre", departamentos.id_pais);
            return View(departamentos);
        }

        // POST: Administracion/Departamentos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_departamento,id_pais,nombre,activo")] Departamentos departamentos)
        {
            if (ModelState.IsValid)
            {
                Departamentos departamentoEdit = db.Departamentos.Find(departamentos.id_departamento);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                departamentoEdit.nombre = departamentos.nombre;
                departamentoEdit.activo = departamentos.activo;
                departamentoEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                departamentoEdit.fecha_modificacion = DateTime.Now;
                departamentoEdit.eliminado = false;
                db.Entry(departamentoEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_pais = new SelectList(db.Paises.Where(x => x.eliminado == false).Where(y => y.activo == true), "id_pais", "nombre", departamentos.id_pais);
            return View(departamentos);
        }

        // GET: Administracion/Departamentos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departamentos departamentos = db.Departamentos.Find(id);
            if (departamentos == null)
            {
                return HttpNotFound();
            }
            return View(departamentos);
        }

        // POST: Administracion/Departamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Departamentos departamentos = db.Departamentos.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            departamentos.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            departamentos.fecha_eliminacion = DateTime.Now;
            departamentos.eliminado = true;
            db.Entry(departamentos).State = EntityState.Modified;
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

        public ActionResult CreateDepMunicipio(int idDepartamento, string municipio)
        {

            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Municipios municipioNew = new Municipios();
            municipioNew.id_usuario_creacion = usuarioTO.usuario.id_usuario;
            municipioNew.nombre = municipio;
            municipioNew.id_departamento = idDepartamento;
            municipioNew.fecha_creacion = DateTime.Now;
            municipioNew.activo = true;
            municipioNew.eliminado = false;
            db.Municipios.Add(municipioNew);
            db.SaveChanges();
            return new EmptyResult();
        }

        public ActionResult RemoveDepMunicipio(int idMunicipio, int idDepartamento)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Municipios municipio = db.Municipios.Find(idMunicipio);

            municipio.eliminado = true;
            municipio.fecha_eliminacion = DateTime.Now;
            municipio.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            db.Entry(municipio).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = idDepartamento }); ;
        }




    }
}
