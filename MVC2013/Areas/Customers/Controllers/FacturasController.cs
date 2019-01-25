using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using System.Globalization;
using MVC2013.Src.Comun.Util;
using MVC2013.Src.Seguridad.To;

namespace MVC2013.Areas.Customers.Controllers
{
    public class FacturasController : Controller
    {
        private AppEntities db = new AppEntities();
        CultureInfo cultura_Chapina = new CultureInfo("es-GT");

        // GET: Customers/Facturas
        public ActionResult Index()
        {
            var facturas = db.Facturas.Include(f => f.Usuarios).Include(f => f.Usuarios1).Include(f => f.Usuarios2).Include(f => f.Cat_Estados_Factura).Include(f => f.Series_Facturas).Include(f => f.Razones_Sociales);
            return View(facturas.ToList());
        }

        public ActionResult Index_Hoy()
        {
            var Facturas = db.Facturas.Where(x => x.activo && !x.eliminado &&
                                            x.fecha_creacion.Day == DateTime.Now.Day &&
                                            x.fecha_creacion.Month == DateTime.Now.Month &&
                                            x.fecha_creacion.Year == DateTime.Now.Year)
                                            .OrderBy(x => x.numero_factura).ToList();
            return View(Facturas);
        }

        public ActionResult Consulta_Factura()
        {
            ViewBag.serie = new SelectList(db.Series_Facturas.Where(x => x.activo && !x.eliminado).Select(x => new { id = x.id_serie_factura, nombre = x.serie}), "id", "nombre");
            return View();
        }

        public JsonResult Obtener_Razones_Sociales_Facturas_Hoy(int id)
        {
            List<object> resultados = new List<object>();
            var clientes = db.Clientes.Find(id);
            var facturas = db.Facturas.Where(x => x.Razones_Sociales.id_cliente == id && x.fecha_creacion.Year == DateTime.Now.Year && x.fecha_creacion.Month == DateTime.Now.Month && x.fecha_creacion.Day == DateTime.Now.Day && x.activo && !x.eliminado && x.id_cat_estado_factura == (int)Catalogos.Estado_Factura.creada).ToList();
            foreach (var item_fac in facturas)
            {
                if (!resultados.Contains(new { id = item_fac.id_razon_social, nombre = item_fac.Razones_Sociales.razon_social }))
                {
                    resultados.Add(new { id = item_fac.id_razon_social, nombre = item_fac.Razones_Sociales.razon_social });
                }
            }
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Obtener_Correlativo(int id)
        {
            int correlativo = db.Series_Facturas.Find(id).correlativo - 1;
            return Json(correlativo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Obtener_Factura(int serie, int corr)
        {
            var lista_fac = db.Facturas.Where(x =>  x.activo && !x.eliminado &&
                                                    x.id_serie_factura == serie && x.numero_factura == corr).ToList();
            int num = 0;
            String Desc = "";
            String fecha = "";
            String Serie = "";
            String Dirr = "";
            String Nit = "";
            String Cliente = "";
            String Razon_Social = "";
            List<object> detalles = new List<object>();
            String Creador = "";
            String Total = "Q 0.00";
            String Creacion = "";
            int estado_int = 0;
            String Modificador = "";
            String comentario = "";
            String Modificacion = "";
            String estado = "Factura No Existe";
            bool estado_temp = true;
            if (lista_fac.Count() > 0)
            {
                var factura = lista_fac.FirstOrDefault();
                num = factura.numero_factura;
                Desc = (factura.descripcion != null) ? factura.descripcion : "----";
                fecha = (factura.fecha_factura != null) ? factura.fecha_factura.ToString("dd/MM/yyyy") : "----";
                Serie = (factura.serie != null) ? factura.serie : "----";
                Dirr = (factura.direccion != null) ? factura.direccion : "----";
                Nit = (factura.nit != null) ? factura.nit : "----";
                comentario = (factura.comentario_anulacion != null) ? factura.comentario_anulacion : "----";
                Cliente = factura.Razones_Sociales.Clientes.nombre;
                estado_int = factura.id_cat_estado_factura;
                Razon_Social = factura.Razones_Sociales.razon_social;
                Creador = factura.Usuarios.nombre_completo_usuario;
                Creacion = factura.fecha_creacion.ToString("dd/MM/yyyy");
                Modificador = (factura.id_usuario_modificacion.HasValue) ? factura.Usuarios2.nombre_completo_usuario : "----";
                Modificacion = (factura.fecha_modificacion.HasValue) ? factura.fecha_creacion.ToString("dd/MM/yyyy") : "----";
                estado = factura.Cat_Estados_Factura.nombre;
                Total = factura.total.ToString("C", cultura_Chapina);
                foreach (var item in factura.Facturas_Detalle)
                {
                    detalles.Add(new {
                        ubicacion = (item.id_contrato_agente.HasValue) ?
                                        (item.Contratos_Agentes.id_ubicacion.HasValue) ?
                                            item.Contratos_Agentes.Ubicaciones.nombre : "----" :
                                        (item.id_contrato_otro_servicio.HasValue) ?
                                            (item.Contratos_Otros_Servicios.id_ubicacion.HasValue) ?
                                                item.Contratos_Otros_Servicios.Ubicaciones.nombre : "----" :
                                            "----",
                        servicio = (item.descripcion != null) ? item.descripcion : "----",
                        cantidad = item.cantidad,
                        precio = item.precio_venta_unitario.ToString("C", cultura_Chapina),
                        importe = (item.precio_venta_unitario * item.cantidad).ToString("C", cultura_Chapina)
                    });
                }

                if (factura.id_cat_estado_factura == (int)Catalogos.Estado_Factura.Anulada || factura.id_cat_estado_factura == (int)Catalogos.Estado_Factura.Eliminada)
                {
                    estado_temp = false;
                }
                return Json(new {num, Desc, fecha, Serie, Dirr, Nit, detalles, estado_temp = estado_temp, id = factura.id_factura, Cliente, Razon_Social, Creador, Creacion, Modificador, Modificacion, estado, Total, comentario, estado_int}, JsonRequestBehavior.AllowGet);
            }
            estado_temp = false;
            return Json(new { num, Desc, fecha, Serie, Dirr, Nit, detalles, estado_temp = estado_temp, Cliente, Razon_Social, Creador, Creacion, Modificador, Modificacion, estado, Total, comentario, estado_int }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index_Mensual()
        {
            var facturas = db.Facturas.Select(x => new { mes = x.fecha_factura.Month, anio = x.fecha_factura.Year }).GroupBy(x => new { x.mes, x.anio }).OrderByDescending(x => x.Key.anio).ThenByDescending(x => x.Key.mes).ToList();
            
            List<String[]> fechas = new List<String[]>();
            foreach (var item in facturas)
            {
                String[] Nuevo = new String[5];
                Nuevo[0] = cultura_Chapina.DateTimeFormat.GetMonthName(item.Key.mes).ToUpper() + " " + item.Key.anio.ToString();
                Nuevo[1] = item.Key.mes.ToString();
                Nuevo[2] = item.Key.anio.ToString();
                var temp = db.Facturas.Where(x => x.fecha_factura.Month == item.Key.mes && x.fecha_factura.Year == item.Key.anio);
                Nuevo[3] = temp.Where(x => x.activo && !x.eliminado).ToList().Count().ToString();
                Nuevo[4] = temp.Where(x => !x.activo && x.eliminado).ToList().Count().ToString();
                fechas.Add(Nuevo);
            }
            ViewBag.fechas = fechas;
            return View();
        }

        public ActionResult Index_rs(int id)
        {
            ViewBag.razon_social = db.Razones_Sociales.Find(id);
            var facturas = db.Facturas.Where(x => x.activo && !x.eliminado && x.id_razon_social == id).Where(x => x.fecha_creacion.Day == DateTime.Now.Day
                                                                                                                  && x.fecha_creacion.Month == DateTime.Now.Month
                                                                                                                  && x.fecha_creacion.Year == DateTime.Now.Year).ToList();
            return View(facturas);
        }

        public ActionResult Index_Facturas_M(String mes, String anio) {
            int Mes = Convert.ToInt32(mes);
            int Anio = Convert.ToInt32(anio);
            ViewBag.fecha_factura = cultura_Chapina.DateTimeFormat.GetMonthName(Mes).ToUpper() + " " + Anio;
            var facturas = db.Facturas.Where(x => x.activo && !x.eliminado && x.fecha_factura.Month == Mes && x.fecha_factura.Year == Anio).OrderBy(x => x.numero_factura).ToList();
            return View(facturas);
        }

        public ActionResult Index_Facturas_Eliminadas(String mes, String anio)
        {
            int Mes = Convert.ToInt32(mes);
            int Anio = Convert.ToInt32(anio);
            ViewBag.fecha_factura = cultura_Chapina.DateTimeFormat.GetMonthName(Mes).ToUpper() + " " + Anio;
            var facturas = db.Facturas.Where(x => !x.activo && x.eliminado && x.fecha_factura.Month == Mes && x.fecha_factura.Year == Anio).OrderBy(x => x.numero_factura).ToList();
            return View(facturas);
        }

        public ActionResult Index_Facturas_Serie(int? serie, int? corr)
        {
            if (serie != null && corr != null)
            {
                var facturas = db.Facturas.Where(x => x.id_serie_factura == serie && x.numero_factura == corr).OrderBy(x => x.Razones_Sociales.Clientes.nombre).ThenBy(x => x.Razones_Sociales.razon_social).ToList();
                ViewBag.cont = facturas.Count();
                return View(facturas);
            }
            else
            {
                //var facturas = db.Facturas.Where(x => x.id_serie_factura == serie && x.numero_factura == corr).OrderBy(x => x.Razones_Sociales.Clientes.nombre).ThenBy(x => x.Razones_Sociales.razon_social).ToList();
                ViewBag.cont = 0;
                return View();
            }
            
        }

        // GET: Customers/Facturas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Facturas facturas = db.Facturas.Find(id);
            if (facturas == null)
            {
                return HttpNotFound();
            }
            return View(facturas);
        }

        // GET: Customers/Facturas/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_cat_estado_factura = new SelectList(db.Cat_Estados_Factura, "id_cat_estado_factura", "nombre");
            ViewBag.id_cat_tipo_facturacion = new SelectList(db.Cat_Tipos_Facturacion, "id_cat_tipo_facturacion", "nombre");
            ViewBag.id_serie_factura = new SelectList(db.Series_Facturas, "id_serie_factura", "serie");
            ViewBag.id_razon_social = new SelectList(db.Razones_Sociales, "id_razon_social", "razon_social");
            return View();
        }

        // POST: Customers/Facturas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_factura,id_serie_factura,serie,numero_factura,id_cat_tipo_facturacion,id_razon_social,nombre_factura,nit,direccion,fecha_factura,descripcion,total,id_cat_estado_factura,fecha_cobro,fecha_pago,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Facturas facturas)
        {
            if (ModelState.IsValid)
            {
                db.Facturas.Add(facturas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", facturas.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", facturas.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", facturas.id_usuario_modificacion);
            ViewBag.id_cat_estado_factura = new SelectList(db.Cat_Estados_Factura, "id_cat_estado_factura", "nombre", facturas.id_cat_estado_factura);
            ViewBag.id_serie_factura = new SelectList(db.Series_Facturas, "id_serie_factura", "serie", facturas.id_serie_factura);
            ViewBag.id_razon_social = new SelectList(db.Razones_Sociales, "id_razon_social", "razon_social", facturas.id_razon_social);
            return View(facturas);
        }

        // GET: Customers/Facturas/Edit/5
        public ActionResult Edit(int? id)
        {
            Facturas facturas = db.Facturas.Find(id);
            if (facturas == null)
            {
                return HttpNotFound();
            }
            return View(facturas);
        }

        // POST: Customers/Facturas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Facturas facturas)
        {
            if (facturas.orden_aceptacion_numero != null)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                var temp_fac = db.Facturas.Find(facturas.id_factura);
                temp_fac.orden_aceptacion_numero = facturas.orden_aceptacion_numero;
                temp_fac.fecha_modificacion = DateTime.Now;
                temp_fac.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                db.Entry(temp_fac).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Facturas_Detalle", new { id = temp_fac.id_factura });
            }

            return View(facturas);
        }

        // GET: Customers/Facturas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Facturas facturas = db.Facturas.Find(id);
            if (facturas == null)
            {
                return HttpNotFound();
            }
            return View(facturas);
        }

        // POST: Customers/Facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Facturas facturas = db.Facturas.Find(id);
            db.Facturas.Remove(facturas);
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
