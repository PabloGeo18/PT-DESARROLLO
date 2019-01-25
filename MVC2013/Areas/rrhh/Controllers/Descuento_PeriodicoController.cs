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
using System.Globalization;

namespace MVC2013.Areas.rrhh.Controllers
{
    public class Descuento_PeriodicoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Descuento_Periodico
        public ActionResult Index()
        {
            var descuento_Periodico = db.Descuento_Periodico.Where(d => !d.eliminado).OrderBy(d => !d.activo).OrderBy(d => d.Empleado.primer_apellido).OrderBy(d => d.Empleado.segundo_apellido);
            return View(descuento_Periodico.ToList());
        }

        // GET: rrhh/Descuento_Periodico/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Descuento_Periodico descuento_Periodico = db.Descuento_Periodico.Find(id);
            if (descuento_Periodico == null)
            {
                return HttpNotFound();
            }
            return View(descuento_Periodico);
        }

        // GET: rrhh/Descuento_Periodico/Create
        public ActionResult Create()
        {
            ViewBag.empleados = db.Empleado.Where(e => !e.eliminado && e.activo).ToList();
            ViewBag.id_tipo_prestamo = new SelectList(db.Tipo_Prestamo.Where(t => !t.eliminado).OrderBy(t => t.nombre), "id_tipo_prestamo", "nombre");
            ViewBag.id_tipo_plan_pago = new SelectList(db.Tipo_Plan_Pago.Where(t => !t.eliminado).OrderBy(t => t.nombre), "id_tipo_plan_pago", "nombre");

            return View();
        }

        // POST: rrhh/Descuento_Periodico/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Descuento_Periodico descuento_Periodico, DateTime fecha_inicio_prestamo, string cantidad_a_pagar)
        {
            ModelState.Clear();
            if(fecha_inicio_prestamo == null)
            {
                ModelState.AddModelError("", "Ingrese la cantidad del préstamo.");
            }
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        descuento_Periodico.cantidad_a_pagar = Convert.ToDecimal(cantidad_a_pagar, CultureInfo.InvariantCulture);
                        descuento_Periodico = InicializarDescuentoPeriodico(descuento_Periodico);
                        descuento_Periodico.fecha_inicio = fecha_inicio_prestamo;
                        decimal cuota = (descuento_Periodico.cantidad_a_pagar / descuento_Periodico.total_pagos);
                        db.Descuento_Periodico.Add(descuento_Periodico);
                        db.SaveChanges();
                        DateTime fecha_fin = GenerarDescuentos(descuento_Periodico, cuota); //Genera los descuentos y envia la fecha final del prestamo
                        descuento_Periodico.fecha_fin = fecha_fin;
                        db.Entry(descuento_Periodico).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Error. Cambios no guardados.");
                        tran.Rollback();
                    }
                }
            }
            ViewBag.id_tipo_prestamo = new SelectList(db.Tipo_Prestamo.Where(t => !t.eliminado).OrderBy(t => t.nombre), "id_tipo_prestamo", "nombre", descuento_Periodico.id_tipo_prestamo);
            ViewBag.id_tipo_plan_pago = new SelectList(db.Tipo_Plan_Pago.Where(t => !t.eliminado).OrderBy(t => t.nombre), "id_tipo_plan_pago", "nombre", descuento_Periodico.id_tipo_plan_pago);
            ViewBag.fecha_inicio_prestamo = fecha_inicio_prestamo.ToString("dd/MM/yyyy");
            Empleado empleado = db.Empleado.Find(descuento_Periodico.id_empleado);
            if (empleado != null)
            {
                ViewBag.id_empleado = descuento_Periodico.id_empleado;
                ViewBag.nombre = empleado.primer_apellido + " " + empleado.segundo_apellido + ", " + empleado.primer_nombre + " " + empleado.segundo_nombre;
            }
            ViewBag.empleados = db.Empleado.Where(e => !e.eliminado && e.activo).ToList();
            return View(descuento_Periodico);
        }

        #region Mantenimiento de Cuotas

        public ActionResult Cuotas(int id, bool error)
        {
            var cuotas = db.Descuento.Where(d => d.id_descuento_periodico == id && !d.eliminado).OrderBy(d=>d.fecha);
            ViewBag.cuotas = cuotas;
            Descuento_Periodico dp = db.Descuento_Periodico.Find(id);
            if(error)
            {
                ModelState.AddModelError("", "Ocurrio un error durante la operación. Cambios no guardados.");
            }
            return View(dp);
        }
        /*
        [HttpPost]
        public ActionResult Agregar_Cuotas(decimal [] cantidades, string [] fechas, int id_descuento_periodico)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    Descuento_Periodico dp = db.Descuento_Periodico.Find(id_descuento_periodico);
                    for (int i = 0; i < cantidades.Length; i++)
                    {
                        Descuento descuento = new Descuento();
                        descuento.total = cantidades[i];
                        descuento.fecha = DateTime.ParseExact(fechas[i], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        descuento.id_descuento_periodico = id_descuento_periodico;
                        descuento.activo = true;
                        descuento.eliminado = false;
                        descuento.fecha_creacion = DateTime.Now;
                        descuento.id_usuario_creacion = id_usuario;
                        descuento.id_tipo_descuento = 3; //Tipo: Prestamo (id=3)
                        dp.cantidad_a_pagar += cantidades[i];
                        dp.total_pagos++;
                        db.Descuento.Add(descuento);
                    }
                    dp.fecha_modificacion = DateTime.Now;
                    dp.id_usuario_modificacion = id_usuario;
                    db.Entry(dp).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return Json(new { response = true });
                }
                catch
                {
                    tran.Rollback();
                    return Json(new { response = false });
                }
            }
        }*/

        [HttpPost]
        public ActionResult AgregarCuota(int id_descuento_periodico, string fecha, string cantidad)
        {
            bool error = false;
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    Descuento_Periodico dp = db.Descuento_Periodico.Find(id_descuento_periodico);
                    Descuento descuento = new Descuento();
                    descuento.total = Convert.ToDecimal(cantidad, CultureInfo.InvariantCulture);
                    descuento.fecha = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    descuento.id_descuento_periodico = id_descuento_periodico;
                    descuento.activo = true;
                    descuento.eliminado = false;
                    descuento.fecha_creacion = DateTime.Now;
                    descuento.id_usuario_creacion = id_usuario;
                    descuento.id_tipo_descuento = 3; //Tipo: Prestamo (id=3)
                    dp.cantidad_a_pagar += Convert.ToDecimal(cantidad, CultureInfo.InvariantCulture);
                    dp.total_pagos++;
                    db.Descuento.Add(descuento);
                    dp.fecha_modificacion = DateTime.Now;
                    dp.id_usuario_modificacion = id_usuario;
                    db.Entry(dp).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    error = true;
                }
            }
            return RedirectToAction("Cuotas", new { id = id_descuento_periodico, error = error });
        }

        [HttpPost]
        public ActionResult EditarCuota(string cantidad, DateTime fecha, int id_descuento_periodico, int id_descuento)
        {
            bool error = false;
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    Descuento descuento = db.Descuento.Find(id_descuento);
                    decimal cantidad_previa = descuento.total;
                    descuento.total = Convert.ToDecimal(cantidad, CultureInfo.InvariantCulture);
                    descuento.fecha = fecha;
                    descuento.fecha_modificacion = DateTime.Now;
                    descuento.id_usuario_modificacion = id_usuario;
                    Descuento_Periodico dp = db.Descuento_Periodico.Find(id_descuento_periodico);
                    dp.fecha_modificacion = DateTime.Now;
                    dp.id_usuario_modificacion = id_usuario;
                    if(cantidad_previa > descuento.total) //descuento.total ya tiene el nuevo total de la cuota
                    {
                        dp.cantidad_a_pagar -= (cantidad_previa - descuento.total);
                    }
                    else
                    {
                        dp.cantidad_a_pagar += (descuento.total - cantidad_previa);
                    }
                    db.Entry(dp).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    error = true;
                }
            }
            return RedirectToAction("Cuotas", new { id = id_descuento_periodico, error = error });
        }

        [HttpPost]
        public ActionResult Eliminar_Cuota(int id_descuento)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    Descuento descuento = db.Descuento.Find(id_descuento);
                    Descuento_Periodico dp = db.Descuento_Periodico.Find(descuento.id_descuento_periodico);
                    descuento.activo = false;
                    descuento.eliminado = true;
                    descuento.fecha_eliminacion = DateTime.Now;
                    descuento.id_usuario_eliminacion = id_usuario;
                    dp.total_pagos--;
                    dp.cantidad_a_pagar -= descuento.total;
                    dp.fecha_modificacion = DateTime.Now;
                    dp.id_usuario_modificacion = id_usuario;
                    db.Entry(dp).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return Json(new { msg = "", response = true });
                }
                catch
                {
                    tran.Rollback();
                    return Json(new { msg = "Ocurrio un error durante la operación. Cambios no guardados.", response = false });
                }
            }
        }

        [HttpPost]
        public ActionResult Pagar_Cuota(int id_descuento)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    Descuento descuento = db.Descuento.Find(id_descuento);
                    descuento.activo = false;
                    descuento.fecha_modificacion = DateTime.Now;
                    descuento.id_usuario_modificacion = id_usuario;
                    Descuento_Periodico dp = db.Descuento_Periodico.Find(descuento.id_descuento_periodico);
                    dp.pagos_actuales++;
                    dp.cantidad_pagado += descuento.total;
                    dp.fecha_modificacion = DateTime.Now;
                    dp.id_usuario_modificacion = id_usuario;
                    db.Entry(dp).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return Json(new { msg = "", response = true });
                }
                catch
                {
                    tran.Rollback();
                    return Json(new { msg = "Ocurrio un error durante la operación. Cambios no guardados.", response = false });
                }
            }
            
        }

        #endregion



        public Descuento_Periodico InicializarDescuentoPeriodico(Descuento_Periodico dp)
        {
            dp.activo = true;
            dp.eliminado = false;
            dp.cantidad_pagado = 0;
            dp.pagos_actuales = 0;
            dp.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            dp.fecha_creacion = DateTime.Now;
            return dp;
        }

        public DateTime GenerarDescuentos(Descuento_Periodico dp, decimal cuota)
        {
            DateTime fecha = dp.fecha_inicio;
            int total_pagos = dp.total_pagos;
            switch (dp.id_tipo_plan_pago)
            {
                case 1: //Quincenal
                    fecha = new DateTime(dp.fecha_inicio.Year, dp.fecha_inicio.Month, 15); //quincena;
                    for (int i = 0; i < total_pagos; i++)
                    {
                        fecha = new DateTime(fecha.Year, fecha.Month, 15);
                        GuardarDescuento(fecha, cuota, dp.id_descuento_periodico);
                        i++;
                        if (i < total_pagos)
                        {
                            fecha = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month)); //fin de mes
                            GuardarDescuento(fecha, cuota, dp.id_descuento_periodico);
                            fecha = fecha.AddMonths(1);
                        }
                    }
                    break;
                case 2: //Mensual
                    fecha = new DateTime(dp.fecha_inicio.Year, dp.fecha_inicio.Month, 1);
                    for (int i = 0; i < total_pagos; i++)
                    {
                        DateTime fecha_aux = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month));
                        GuardarDescuento(fecha_aux, cuota, dp.id_descuento_periodico);
                        fecha = fecha.AddMonths(1);
                    }
                    break;
                case 3: //Bimensual
                    fecha = new DateTime(dp.fecha_inicio.Year, dp.fecha_inicio.Month, 1);
                    for (int i = 0; i < total_pagos; i++)
                    {
                        DateTime fecha_aux = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month));
                        GuardarDescuento(fecha_aux, cuota, dp.id_descuento_periodico);
                        fecha = fecha.AddMonths(2);
                    }
                    break;
                case 4: //Trimestral
                    fecha = new DateTime(dp.fecha_inicio.Year, dp.fecha_inicio.Month, 1);
                    for (int i = 0; i < total_pagos; i++)
                    {
                        DateTime fecha_aux = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month));
                        GuardarDescuento(fecha_aux, cuota, dp.id_descuento_periodico);
                        fecha = fecha.AddMonths(3);
                    }
                    break;
                case 5: //Anual
                    fecha = new DateTime(dp.fecha_inicio.Year, dp.fecha_inicio.Month, 1);
                    for (int i = 0; i < total_pagos; i++)
                    {
                        DateTime fecha_aux = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month));
                        GuardarDescuento(fecha_aux, cuota, dp.id_descuento_periodico);
                        fecha = fecha.AddYears(1);
                    }
                    break;
                case 6: //Semestral
                    fecha = new DateTime(dp.fecha_inicio.Year, dp.fecha_inicio.Month, 1);
                    for (int i = 0; i < total_pagos; i++)
                    {
                        DateTime fecha_aux = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month));
                        GuardarDescuento(fecha_aux, cuota, dp.id_descuento_periodico);
                        fecha = fecha.AddMonths(6);
                    }
                    break;
            }
            return fecha;
        }
        
        public JsonResult GenerarPlanPago(DateTime fecha_inicio, string prestamo, int id_tipo_plan_pago, int total_pagos)
        {
            var plan_pago = new List<object>();
            var cuota = Convert.ToDecimal(prestamo, CultureInfo.InvariantCulture) / total_pagos;
            DateTime fecha;
            switch(id_tipo_plan_pago)
            {
                case 1: //Quincenal
                    fecha = new DateTime(fecha_inicio.Year, fecha_inicio.Month, 15); //quincena;
                    for (int i = 0; i < total_pagos; i++)
                    {
                        fecha = new DateTime(fecha.Year, fecha.Month, 15);
                        plan_pago.Add(new { fecha = fecha.ToString("dd/MM/yyyy") });
                        i++;
                        if (i < total_pagos)
                        {
                            fecha = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month)); //fin de mes
                            plan_pago.Add(new { fecha = fecha.ToString("dd/MM/yyyy") });
                            fecha = fecha.AddMonths(1);
                        }
                    }
                    break;
                case 2: //Mensual
                    fecha = new DateTime(fecha_inicio.Year, fecha_inicio.Month, 1);
                    for (int i = 0; i < total_pagos; i++)
                    {
                        DateTime fecha_aux = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month));
                        plan_pago.Add(new { fecha = fecha_aux.ToString("dd/MM/yyyy") });
                        fecha = fecha.AddMonths(1);
                    }
                    break;
                case 3: //Bimensual
                    fecha = new DateTime(fecha_inicio.Year, fecha_inicio.Month, 1);
                    for (int i = 0; i < total_pagos; i++)
                    {
                        DateTime fecha_aux = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month));
                        plan_pago.Add(new { fecha = fecha_aux.ToString("dd/MM/yyyy") });
                        fecha = fecha.AddMonths(2);
                    }
                    break;
                case 4: //Trimestral
                    fecha = new DateTime(fecha_inicio.Year, fecha_inicio.Month, 1);
                    for (int i = 0; i < total_pagos; i++)
                    {
                        DateTime fecha_aux = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month));
                        plan_pago.Add(new { fecha = fecha_aux.ToString("dd/MM/yyyy") });
                        fecha = fecha.AddMonths(3);
                    }
                    break;
                case 5: //Anual
                    fecha = new DateTime(fecha_inicio.Year, fecha_inicio.Month, 1);
                    for (int i = 0; i < total_pagos; i++)
                    {
                        DateTime fecha_aux = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month));
                        plan_pago.Add(new { fecha = fecha_aux.ToString("dd/MM/yyyy") });
                        fecha = fecha.AddYears(1);
                    }
                    break;
                case 6: //Semestral
                    fecha = new DateTime(fecha_inicio.Year, fecha_inicio.Month, 1);
                    for (int i = 0; i < total_pagos; i++)
                    {
                        DateTime fecha_aux = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month));
                        plan_pago.Add(new { fecha = fecha_aux.ToString("dd/MM/yyyy") });
                        fecha = fecha.AddMonths(6);
                    }
                    break;
            }
            return Json(new { cuota = cuota, plan_pago = plan_pago }, JsonRequestBehavior.AllowGet);
        }

        public void GuardarDescuento(DateTime fecha, decimal total, int id_descuento_periodico)
        {
            Descuento descuento = NuevoDescuento();
            descuento.fecha = fecha;
            descuento.id_tipo_descuento = 3; //Prestamo
            descuento.total = total;
            descuento.id_descuento_periodico = id_descuento_periodico;
            db.Descuento.Add(descuento);
            db.SaveChanges();
        }

        public Descuento NuevoDescuento()
        {
            Descuento dp = new Descuento();
            dp.activo = true;
            dp.eliminado = false;
            dp.fecha_creacion = DateTime.Now;
            dp.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return dp;
        }

        // GET: rrhh/Descuento_Periodico/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Descuento_Periodico descuento_Periodico = db.Descuento_Periodico.Find(id);
            ViewBag.tipo_prestamo = new SelectList(db.Tipo_Prestamo.Where(t => !t.eliminado), "id_tipo_prestamo", "nombre", descuento_Periodico.id_tipo_prestamo);
            ViewBag.tipos_pago = new SelectList(db.Tipo_Plan_Pago.Where(t => !t.eliminado), "id_tipo_plan_pago", "nombre", descuento_Periodico.id_tipo_plan_pago); ;
            ViewBag.fecha_inicio_prestamo = descuento_Periodico.fecha_inicio.ToString("MM/yyyy");
            if (descuento_Periodico == null)
            {
                return HttpNotFound();
            }
            return View(descuento_Periodico);
        }

        // POST: rrhh/Descuento_Periodico/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(Descuento_Periodico descuento_Periodico, DateTime fecha_inicio_prestamo)
        {
            ModelState.Clear();
            descuento_Periodico.fecha_inicio = fecha_inicio_prestamo;
            if (ModelState.IsValid)
            {

                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Descuento_Periodico dp = db.Descuento_Periodico.Find(descuento_Periodico.id_descuento_periodico);
                        dp.descripcion = descuento_Periodico.descripcion;
                        dp.id_tipo_plan_pago = descuento_Periodico.id_tipo_plan_pago;
                        dp.id_tipo_prestamo = dp.id_tipo_prestamo;
                        dp.fecha_inicio = descuento_Periodico.fecha_inicio;
                        dp.fecha_modificacion = DateTime.Now;
                        dp.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        db.Entry(dp).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        tran.Rollback();
                        ModelState.AddModelError("", "Error. Cambios no guardados.");
                    }
                }
            }
            ViewBag.fecha_inicio_prestamo = fecha_inicio_prestamo.ToString("MM/yyyy");
            ViewBag.id_tipo_prestamo = new SelectList(db.Tipo_Prestamo.Where(t => !t.eliminado).OrderBy(t => t.nombre), "id_tipo_prestamo", "nombre", descuento_Periodico.id_tipo_prestamo);
            ViewBag.id_tipo_plan_pago = new SelectList(db.Tipo_Plan_Pago.Where(t => !t.eliminado).OrderBy(t => t.nombre), "id_tipo_plan_pago", "nombre", descuento_Periodico.id_tipo_plan_pago);
            return View(descuento_Periodico);
        }

        // GET: rrhh/Descuento_Periodico/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Descuento_Periodico descuento_Periodico = db.Descuento_Periodico.Find(id);
            if (descuento_Periodico == null)
            {
                return HttpNotFound();
            }
            return View(descuento_Periodico);
        }

        // POST: rrhh/Descuento_Periodico/Delete/5
        public ActionResult Eliminar(int id)
        {
            using(DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    Descuento_Periodico dp = db.Descuento_Periodico.Find(id);
                    dp.activo = false;
                    dp.eliminado = true;
                    dp.fecha_eliminacion = DateTime.Now;
                    dp.id_usuario_eliminacion = id_usuario;
                    db.Entry(dp).State = EntityState.Modified;
                    foreach (var prestamo in db.Descuento.Where(d=>d.id_descuento_periodico == id && d.activo && d.eliminado))
                    {
                        prestamo.activo = false;
                        prestamo.eliminado = true;
                        prestamo.fecha_eliminacion = DateTime.Now;
                        prestamo.id_usuario_eliminacion = id_usuario;
                    }
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    tran.Rollback();
                }
            }
            return Delete(id);
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
