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
using System.Globalization;
using MVC2013.Src.Comun.View;

namespace MVC2013.Areas.rrhh.Controllers
{
    public class Tipo_PuestoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Tipo_Puesto
        public ActionResult Index()
        {
            return View(db.Tipo_Puesto.Where(e=>!e.eliminado).OrderBy(t=>t.nombre));
        }

        // GET: rrhh/Tipo_Puesto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Puesto tipo_Puesto = db.Tipo_Puesto.Find(id);
            if (tipo_Puesto == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Puesto);
        }

        // GET: rrhh/Tipo_Puesto/Create
        public ActionResult Create()
        {
            ViewBag.empresa = new SelectList(db.Empresa.Where(e => e.activo), "id_empresa", "nombre");
            Tipo_Puesto tipo_puesto = new Tipo_Puesto();
            return View(tipo_puesto);
        }

        // POST: rrhh/Tipo_Puesto/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Tipo_Puesto tipo_puesto)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    tipo_puesto.salario_maximo = Convert.ToDecimal(Request["salario_maximo"], CultureInfo.InvariantCulture);
                    tipo_puesto.salario_minimo = Convert.ToDecimal(Request["salario_minimo"], CultureInfo.InvariantCulture);
                    tipo_puesto.activo = true;
                    tipo_puesto.eliminado = false;
                    tipo_puesto.fecha_creacion = DateTime.Now;
                    tipo_puesto.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Tipo_Puesto.Add(tipo_puesto);
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "No se pudo crear correctamente el Tipo de Puesto");
                    msg.ReturnUrl = Url.Action("Create");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje", "Home");
                }
            }
        }

        public Tipo_Puesto NuevoTipoPuesto()
        {
            Tipo_Puesto tipo_puesto = new Tipo_Puesto();
            tipo_puesto.activo = true;
            tipo_puesto.eliminado = false;
            tipo_puesto.fecha_creacion = DateTime.Now;
            tipo_puesto.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return tipo_puesto;
        }
        

        // GET: rrhh/Tipo_Puesto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Puesto tipo_Puesto = db.Tipo_Puesto.Find(id);
            if (tipo_Puesto == null)
            {
                return HttpNotFound();
            }
            //ViewBag.empresa = new SelectList(db.Empresa.Where(e => e.activo), "id_empresa", "nombre");
            return View(tipo_Puesto);
        }

        // POST: rrhh/Tipo_Puesto/Edit/5
        [HttpPost]
        public ActionResult Edit(Tipo_Puesto tipo_puesto)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Tipo_Puesto editTipo_puesto = db.Tipo_Puesto.SingleOrDefault(t => t.id_tipo_puesto == tipo_puesto.id_tipo_puesto && t.activo);
                    if(editTipo_puesto == null)
                    {
                        return HttpNotFound();
                    }
                    //editTipo_puesto.id_empresa = tipo_puesto.id_empresa;
                    editTipo_puesto.nombre = tipo_puesto.nombre;
                    editTipo_puesto.genera_estado_fuerza = tipo_puesto.genera_estado_fuerza;
                    editTipo_puesto.salario_maximo = Convert.ToDecimal(Request["salario_maximo"], CultureInfo.InvariantCulture);
                    editTipo_puesto.salario_minimo = Convert.ToDecimal(Request["salario_minimo"], CultureInfo.InvariantCulture);
                    editTipo_puesto.fecha_modificacion = DateTime.Now;
                    editTipo_puesto.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(editTipo_puesto).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "No se pudo modificar correctamente el Tipo de Puesto");
                    msg.ReturnUrl = Url.Action("Edit");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje", "Home");
                }
            }
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Tipo_Puesto tipo_puesto = db.Tipo_Puesto.Find(id);
                    tipo_puesto.fecha_eliminacion = DateTime.Now;
                    tipo_puesto.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    tipo_puesto.activo = false;
                    tipo_puesto.eliminado = true;
                    db.Entry(tipo_puesto).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return Json(new { msg = "", response = true });
                }
                catch
                {
                    tran.Rollback();
                    return Json(new { msg = "Cambios no guardados.", response = false });
                }
            }
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
