using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Src.Seguridad.To;
using MVC2013.Src.Comun.Util;

namespace MVC2013.Areas.EstadoFuerza.Controllers
{
    public class pt_estadofuerzaController : Controller
    {
        private Protal_webEntities db = new Protal_webEntities();

        // GET: EstadoFuerza/pt_estadofuerza
        public ActionResult Index(DateTime? dateFrom, DateTime? dateTo, int? id_empleado)
        {
            dateFrom = dateFrom ?? DateTime.Today;
            dateTo = dateTo ?? DateTime.Today;
            id_empleado = id_empleado ?? -1;
            //var pt_estadofuerza = db.pt_estadofuerza.Where(e => e.fecha == date).Include(p => p.pt_empleado).Include(p => p.pt_estadofuerza_encabezado).Include(p => p.pt_situacion).Include(p => p.pt_ubicacion).Include(p => p.pt_tipo_ubicacion);
            //var pt_estadofuerza = db.pt_estadofuerza.Where(e => e.fecha == date);
            IEnumerable<fn_detalle_estado_fuerza_dia_Result> estadof = db.fn_detalle_estado_fuerza_dia(dateFrom, dateTo, id_empleado);

            IEnumerable<SelectListItem> empleados = db.pt_empleado.Select(emp => new SelectListItem { Text = emp.PTEMPLEADOID.ToString() + " - " + emp.NOMBRE1 + " " + emp.NOMBRE2, Value = emp.PTEMPLEADOID.ToString() });
            ViewBag.ptempleadoid = empleados;

            

            ViewBag.dateFrom = dateFrom;
            ViewBag.dateTo = dateTo;
            ViewBag.estadof = estadof;
            return View();
        }

        // GET: EstadoFuerza/pt_estadofuerza/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pt_estadofuerza pt_estadofuerza = db.pt_estadofuerza.Find(id);
            if (pt_estadofuerza == null)
            {
                return HttpNotFound();
            }
            pt_empleado emp = db.pt_empleado.Find(pt_estadofuerza.ptempleadoid);
            string primerNombre = emp.NOMBRE1 ?? "";
            string segundoNombre = emp.NOMBRE2 ?? "";
            string primerApellido = emp.APELLIDO1 ?? "";
            string segundoApellido = emp.APELLIDO2 ?? "";
            ViewBag.NombreEmpleado = primerNombre + " " + segundoNombre + " " + primerApellido + " " + segundoApellido;

            return View(pt_estadofuerza);
        }


        // GET: EstadoFuerza/pt_estadofuerza/Details/5
        public ActionResult Administrativo()
        {

            pt_estadofuerza_encabezado encabezado = db.pt_estadofuerza_encabezado.Where(e=> e.estado == 1).FirstOrDefault();
            if (encabezado != null)
            {
                

            }

            DateTime fecha_ultima = db.pt_estadofuerza_encabezado.OrderByDescending(f => f.fecha_inicio).First().fecha_inicio;
            ViewBag.fecha_ultima = fecha_ultima;
            return View(encabezado);
        }

        // GET: TransporteValores/ConsultaSolicitudes
        public ActionResult IniciarDia(DateTime? date)
        {
            if (date == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pt_estadofuerza_encabezado encabezado1 = new pt_estadofuerza_encabezado();

            encabezado1 = db.pt_estadofuerza_encabezado.Where(x => x.fecha_inicio == (DateTime)date).FirstOrDefault();

            if (encabezado1==null)
            { 

            //date = date ?? DateTime.Today;
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            pt_estadofuerza_encabezado encabezado = new pt_estadofuerza_encabezado();
            encabezado.fecha_inicio = (DateTime)date;
            encabezado.estado = 1;
            encabezado.creado_el = DateTime.Now;
            encabezado.creado_por = usuarioTO.usuario.usuario;
            db.pt_estadofuerza_encabezado.Add(encabezado);
            db.SaveChanges();

            }
            
            ViewBag.fecha_ultima = (DateTime)date;
            return View("Administrativo");
        }

        // GET: TransporteValores/ConsultaSolicitudes
        public ActionResult FinalizarDia(int id)
        {
            
            //date = date ?? DateTime.Today;
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            pt_estadofuerza_encabezado encabezado = new pt_estadofuerza_encabezado();
            encabezado  = db.pt_estadofuerza_encabezado.Where(x => x.id == id && x.estado == 1).FirstOrDefault();
            //encabezado.fecha_inicio = (DateTime)date;

            if (encabezado == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            encabezado.estado = 0;
            encabezado.fecha_fin = DateTime.Now;
            encabezado.modificado_el = DateTime.Now;
            encabezado.modificado_por = usuarioTO.usuario.usuario;

            var ausente = db.sp_ausentes_automatico(id, encabezado.fecha_inicio);
            var aus = ausente.First();
            


            db.Entry(encabezado).State = EntityState.Modified;
            db.SaveChanges();

            ViewBag.fecha_ultima = encabezado.fecha_inicio;
            return View("Administrativo");
        }


        // GET: EstadoFuerza/pt_estadofuerza/Create
        public ActionResult Create()
        {

            IEnumerable<SelectListItem> empleados = db.pt_empleado.Select(emp => new SelectListItem { Text = emp.PTEMPLEADOID.ToString() + " - " + emp.NOMBRE1 + " " + emp.NOMBRE2 + " " + emp.APELLIDO1 + " " + emp.APELLIDO2, Value = emp.PTEMPLEADOID.ToString() });
            ViewBag.ptempleadoid = empleados;


            IEnumerable<SelectListItem> estado = db.pt_situacion.Select(sit => new SelectListItem { Text = sit.id_situacion.ToString() + " - " + sit.nombre, Value = sit.id_situacion.ToString() });
            IEnumerable<SelectListItem> ubicacion = db.pt_ubicacion.Select(ub => new SelectListItem { Text = ub.PTUBICACIONID.ToString() + " - " + ub.DIRECCION, Value = ub.PTUBICACIONID.ToString() });
            IEnumerable<SelectListItem> tiposervicio = db.pt_tipo_ubicacion.Select(tp => new SelectListItem { Text = tp.PTTIPOID.ToString() + " - " + tp.DESCRIPCION, Value = tp.PTTIPOID.ToString() });
            ViewBag.estado = estado;
            ViewBag.ptubicacionid = ubicacion;
            ViewBag.pttipoid = tiposervicio;
            //ViewBag.id_encabezado = new SelectList(db.pt_estadofuerza_encabezado, "id", "creado_por");

            return View();
        }

        // POST: EstadoFuerza/pt_estadofuerza/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pt_id,ptempleadoid,ptubicacionid,pttipoid,estado,fecha,observaciones,modificado_el,id_encabezado,usuario_creacion")] pt_estadofuerza pt_estadofuerza)
        {
            if (ModelState.IsValid)
            {
                if (db.pt_estadofuerza.Where(x => x.fecha == pt_estadofuerza.fecha && x.ptempleadoid == pt_estadofuerza.ptempleadoid).Count() == 0) { 
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                pt_estadofuerza.modificado_el = DateTime.Now;
                pt_estadofuerza.usuario_creacion = usuarioTO.usuario.usuario;
                db.pt_estadofuerza.Add(pt_estadofuerza);
                db.SaveChanges();
                return RedirectToAction("Index");
                }
            }

            //IEnumerable<SelectListItem> empleados = db.pt_empleado.Select(emp => new SelectListItem { Value = emp.PTEMPLEADOID.ToString() + " - " + emp.NOMBRE1 + " " + emp.NOMBRE2, Text = emp.PTEMPLEADOID.ToString() });
            //ViewBag.ptempleadoid = empleados;

            ViewBag.id_encabezado = new SelectList(db.pt_estadofuerza_encabezado, "id", "creado_por", pt_estadofuerza.id_encabezado);


            IEnumerable<SelectListItem> estado = db.pt_situacion.Select(sit => new SelectListItem { Text = sit.id_situacion.ToString() + " - " + sit.nombre, Value = sit.id_situacion.ToString() });
            IEnumerable<SelectListItem> ubicacion = db.pt_ubicacion.Select(ub => new SelectListItem { Text = ub.PTUBICACIONID.ToString() + " - " + ub.DIRECCION, Value = ub.PTUBICACIONID.ToString() });
            IEnumerable<SelectListItem> tiposervicio = db.pt_tipo_ubicacion.Select(tp => new SelectListItem { Text = tp.PTTIPOID.ToString() + " - " + tp.DESCRIPCION, Value = tp.PTTIPOID.ToString() });
            ViewBag.estado = estado;
            ViewBag.ptubicacionid = ubicacion;
            ViewBag.pttipoid = tiposervicio;


            return View(pt_estadofuerza);
        }

        // GET: EstadoFuerza/pt_estadofuerza/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pt_estadofuerza pt_estadofuerza = db.pt_estadofuerza.Find(id);
            if (pt_estadofuerza == null)
            {
                return HttpNotFound();
            }
            /*IEnumerable<SelectListItem> empleados = db.pt_empleado
               .Select(emp => new SelectListItem { Value = emp.PTEMPLEADOID.ToString() + " - " + emp.NOMBRE1 + " " + emp.NOMBRE2, Text = emp.PTEMPLEADOID.ToString() });
            ViewBag.ptempleadoid = empleados;*/

            //ViewBag.id_encabezado = new SelectList(db.pt_estadofuerza_encabezado, "id", "creado_por", pt_estadofuerza.id_encabezado);
            pt_empleado emp = db.pt_empleado.Find(pt_estadofuerza.ptempleadoid);
            string primerNombre = emp.NOMBRE1 ?? "";
            string segundoNombre = emp.NOMBRE2 ?? "";
            string primerApellido = emp.APELLIDO1 ?? "";
            string segundoApellido = emp.APELLIDO2 ?? "";
            ViewBag.NombreEmpleado =  primerNombre + " " + segundoNombre + " " + primerApellido + " " + segundoApellido;

            IEnumerable<SelectListItem> estado = db.pt_situacion.Select(sit => new SelectListItem { Text = sit.id_situacion.ToString() + " - " + sit.nombre, Value = sit.id_situacion.ToString() });
            IEnumerable<SelectListItem> ubicacion = db.pt_ubicacion.Select(ub => new SelectListItem { Text = ub.PTUBICACIONID.ToString() + " - " + ub.DIRECCION, Value = ub.PTUBICACIONID.ToString() });
            IEnumerable<SelectListItem> tiposervicio = db.pt_tipo_ubicacion.Select(tp => new SelectListItem { Text = tp.PTTIPOID.ToString() + " - " + tp.DESCRIPCION, Value = tp.PTTIPOID.ToString() });
            //ViewBag.id_encabezado = new SelectList(db.pt_estadofuerza_encabezado, "id", "creado_por", pt_estadofuerza.id_encabezado);
            //ViewBag.estado = new SelectList(db.pt_situacion, "id_situacion", "nombre", pt_estadofuerza.estado);
            ViewBag.estado = estado;
            //ViewBag.ptubicacionid = new SelectList(db.pt_ubicacion, "PTUBICACIONID", "DIRECCION", pt_estadofuerza.ptubicacionid);
            ViewBag.ptubicacionid = ubicacion;
            //ViewBag.pttipoid = new SelectList(db.pt_tipo_ubicacion, "PTTIPOID", "DESCRIPCION", pt_estadofuerza.pttipoid);
            ViewBag.pttipoid = tiposervicio;
            return View(pt_estadofuerza);
        }

        // POST: EstadoFuerza/pt_estadofuerza/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "pt_id,ptempleadoid,ptubicacionid,pttipoid,estado,fecha,observaciones,modificado_el,id_encabezado,usuario_creacion")] pt_estadofuerza pt_estadofuerza)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                pt_estadofuerza.modificado_el = DateTime.Now;
                pt_estadofuerza.usuario_creacion = usuarioTO.usuario.usuario;

                db.Entry(pt_estadofuerza).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            IEnumerable<SelectListItem> empleados = db.pt_empleado
               .Select(emp => new SelectListItem { Value = emp.PTEMPLEADOID.ToString() + " - " + emp.NOMBRE1 + " " + emp.NOMBRE2, Text = emp.PTEMPLEADOID.ToString() });
            ViewBag.ptempleadoid = empleados;

            ViewBag.id_encabezado = new SelectList(db.pt_estadofuerza_encabezado, "id", "creado_por", pt_estadofuerza.id_encabezado);
            ViewBag.estado = new SelectList(db.pt_situacion, "id_situacion", "nombre", pt_estadofuerza.estado);
            ViewBag.ptubicacionid = new SelectList(db.pt_ubicacion, "PTUBICACIONID", "DIRECCION", pt_estadofuerza.ptubicacionid);
            ViewBag.pttipoid = new SelectList(db.pt_tipo_ubicacion, "PTTIPOID", "DESCRIPCION", pt_estadofuerza.pttipoid);
            return View(pt_estadofuerza);
        }

        // GET: EstadoFuerza/pt_estadofuerza/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pt_estadofuerza pt_estadofuerza = db.pt_estadofuerza.Find(id);
            if (pt_estadofuerza == null)
            {
                return HttpNotFound();
            }
            pt_empleado emp = db.pt_empleado.Find(pt_estadofuerza.ptempleadoid);
            string primerNombre = emp.NOMBRE1 ?? "";
            string segundoNombre = emp.NOMBRE2 ?? "";
            string primerApellido = emp.APELLIDO1 ?? "";
            string segundoApellido = emp.APELLIDO2 ?? "";
            ViewBag.NombreEmpleado = primerNombre + " " + segundoNombre + " " + primerApellido + " " + segundoApellido;
            return View(pt_estadofuerza);
        }

        // POST: EstadoFuerza/pt_estadofuerza/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            pt_estadofuerza pt_estadofuerza = db.pt_estadofuerza.Find(id);
            db.pt_estadofuerza.Remove(pt_estadofuerza);
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
