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

namespace MVC2013.Areas.Customers.Controllers
{
    public class Facturas_DetalleController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Customers/Facturas_Detalle
        public ActionResult Index()
        {
            var facturas_Detalle = db.Facturas_Detalle.Include(f => f.Usuarios).Include(f => f.Usuarios1).Include(f => f.Usuarios2).Include(f => f.Facturas).Include(f => f.Contratos_Agentes).Include(f => f.Contratos_Otros_Servicios).Include(f => f.Cat_Tipos_Agente);
            return View(facturas_Detalle.ToList());
        }

        // GET: Customers/Facturas_Detalle/Details/5
        public ActionResult Details(int? id)
        {
            var factura = db.Facturas.Where(x => x.id_factura == id).FirstOrDefault();
            return View(factura);
        }

        // GET: Customers/Facturas_Detalle/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_factura = new SelectList(db.Facturas, "id_factura", "serie");
            ViewBag.id_contrato_agente = new SelectList(db.Contratos_Agentes, "id_contrato_agente", "descripcion_eliminacion");
            ViewBag.id_contrato_otro_servicio = new SelectList(db.Contratos_Otros_Servicios, "id_contrato_otro_servicio", "descripcion_eliminacion");
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente, "id_cat_tipo_agente", "nombre");
            ViewBag.id_cat_otro_servicio = new SelectList(db.Contratos_Otros_Servicios, "id_contrato_otro_servicio", "descripcion_eliminacion");
            return View();
        }

        // POST: Customers/Facturas_Detalle/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_factura_detalle,id_factura,id_contrato_agente,id_contrato_otro_servicio,descripcion,cantidad,costo,precio_venta_unitario,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_cat_tipo_agente,id_cat_otro_servicio")] Facturas_Detalle facturas_Detalle)
        {
            if (ModelState.IsValid)
            {
                db.Facturas_Detalle.Add(facturas_Detalle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", facturas_Detalle.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", facturas_Detalle.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", facturas_Detalle.id_usuario_modificacion);
            ViewBag.id_factura = new SelectList(db.Facturas, "id_factura", "serie", facturas_Detalle.id_factura);
            ViewBag.id_contrato_agente = new SelectList(db.Contratos_Agentes, "id_contrato_agente", "descripcion_eliminacion", facturas_Detalle.id_contrato_agente);
            ViewBag.id_contrato_otro_servicio = new SelectList(db.Contratos_Otros_Servicios, "id_contrato_otro_servicio", "descripcion_eliminacion", facturas_Detalle.id_contrato_otro_servicio);
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente, "id_cat_tipo_agente", "nombre", facturas_Detalle.id_cat_tipo_agente);
            ViewBag.id_cat_otro_servicio = new SelectList(db.Contratos_Otros_Servicios, "id_contrato_otro_servicio", "descripcion_eliminacion", facturas_Detalle.id_cat_otro_servicio);
            return View(facturas_Detalle);
        }

        // GET: Customers/Facturas_Detalle/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Facturas_Detalle facturas_Detalle = db.Facturas_Detalle.Find(id);
            if (facturas_Detalle == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", facturas_Detalle.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", facturas_Detalle.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", facturas_Detalle.id_usuario_modificacion);
            ViewBag.id_factura = new SelectList(db.Facturas, "id_factura", "serie", facturas_Detalle.id_factura);
            ViewBag.id_contrato_agente = new SelectList(db.Contratos_Agentes, "id_contrato_agente", "descripcion_eliminacion", facturas_Detalle.id_contrato_agente);
            ViewBag.id_contrato_otro_servicio = new SelectList(db.Contratos_Otros_Servicios, "id_contrato_otro_servicio", "descripcion_eliminacion", facturas_Detalle.id_contrato_otro_servicio);
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente, "id_cat_tipo_agente", "nombre", facturas_Detalle.id_cat_tipo_agente);
            ViewBag.id_cat_otro_servicio = new SelectList(db.Contratos_Otros_Servicios, "id_contrato_otro_servicio", "descripcion_eliminacion", facturas_Detalle.id_cat_otro_servicio);
            return View(facturas_Detalle);
        }

        // POST: Customers/Facturas_Detalle/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_factura_detalle,id_factura,id_contrato_agente,id_contrato_otro_servicio,descripcion,cantidad,costo,precio_venta_unitario,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_cat_tipo_agente,id_cat_otro_servicio")] Facturas_Detalle facturas_Detalle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(facturas_Detalle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", facturas_Detalle.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", facturas_Detalle.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", facturas_Detalle.id_usuario_modificacion);
            ViewBag.id_factura = new SelectList(db.Facturas, "id_factura", "serie", facturas_Detalle.id_factura);
            ViewBag.id_contrato_agente = new SelectList(db.Contratos_Agentes, "id_contrato_agente", "descripcion_eliminacion", facturas_Detalle.id_contrato_agente);
            ViewBag.id_contrato_otro_servicio = new SelectList(db.Contratos_Otros_Servicios, "id_contrato_otro_servicio", "descripcion_eliminacion", facturas_Detalle.id_contrato_otro_servicio);
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente, "id_cat_tipo_agente", "nombre", facturas_Detalle.id_cat_tipo_agente);
            ViewBag.id_cat_otro_servicio = new SelectList(db.Contratos_Otros_Servicios, "id_contrato_otro_servicio", "descripcion_eliminacion", facturas_Detalle.id_cat_otro_servicio);
            return View(facturas_Detalle);
        }

        // GET: Customers/Facturas_Detalle/Delete/5
        public ActionResult Delete(int id)
        {
            var factura = db.Facturas.Where(x => x.activo && !x.eliminado && x.id_factura == id).Take(1).FirstOrDefault();
            if (factura == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_cat_razon_eliminacion = new SelectList(db.Cat_Razones_Eliminacion.Where(x => x.activo && !x.eliminado).Select(x => new { id = x.id_cat_razon_eliminacion, nombre = x.nombre }), "id", "nombre");
            return View(factura);
        }

        // POST: Customers/Facturas_Detalle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, String comentario_eliminacion, int id_cat_razon_eliminacion)
        {
            Facturas factura = db.Facturas .Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            foreach (var item_factura in factura.Facturas_Detalle.ToList())
            {
                var proceso_facturacion_detalle = db.Procesos_Facturacion_Detalle.Find(item_factura.id_proceso_facturacion_detalle);
                proceso_facturacion_detalle.id_cat_estado_proceso_facturacion_detalle = (int)Catalogos.Estado_Proceso_Facturacion_Detalle.Activo;
                proceso_facturacion_detalle.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                proceso_facturacion_detalle.fecha_modificacion = DateTime.Now;
                db.Entry(proceso_facturacion_detalle).State = EntityState.Modified;
                db.SaveChanges();
                item_factura.activo = false;
                item_factura.eliminado = true;
                item_factura.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                item_factura.fecha_eliminacion = DateTime.Now;
                db.Entry(proceso_facturacion_detalle).State = EntityState.Modified;
                db.SaveChanges();
            }
            factura.activo = false;
            factura.id_cat_razon_eliminacion = id_cat_razon_eliminacion;
            factura.eliminado = true;
            factura.fecha_eliminacion = DateTime.Now;
            factura.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            factura.id_cat_estado_factura = (int)Catalogos.Estado_Factura.Eliminada;
            factura.comentario_eliminacion = comentario_eliminacion;
            db.Entry(factura).State = EntityState.Modified;
            db.SaveChanges();
            var Serie = db.Series_Facturas.Find(factura.id_serie_factura);
            Serie.correlativo = Serie.correlativo - 1;
            db.Entry(Serie).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index_rs", "Facturas", new { id =  factura.id_razon_social});
        }

        // GET: Customers/Facturas_Detalle/Delete/5
        public ActionResult Anular(int id)
        {
            var factura = db.Facturas.Where(x => x.activo && !x.eliminado && x.id_factura == id
                                                && (x.id_cat_estado_factura == (int)Catalogos.Estado_Factura.creada
                                                    || x.id_cat_estado_factura == (int)Catalogos.Estado_Factura.En_Cobro
                                                    || x.id_cat_estado_factura == (int)Catalogos.Estado_Factura.Pagada)).Take(1).FirstOrDefault();
            if (factura == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_cat_razon_anulacion = new SelectList(db.Cat_Razones_Anulacion.Where(x => x.activo && !x.eliminado).Select(x => new { id = x.id_cat_razon_anulacion, nombre = x.nombre }), "id", "nombre");
            return View(factura);
        }

        // POST: Customers/Facturas_Detalle/Delete/5
        [HttpPost, ActionName("Anular")]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm_Anular(int id, Boolean? anulacion, DateTime? fecha, String comentario_anulacion, int id_cat_razon_anulacion)
        {
            if (anulacion == null)
            {
                anulacion = false;
            }
            if (!anulacion.Value)
            {
                Facturas factura = db.Facturas.Find(id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                foreach (var item_factura in factura.Facturas_Detalle.ToList())
                {
                    if (item_factura.id_proceso_facturacion_detalle.HasValue)
                    {
                        var proceso_facturacion_detalle = db.Procesos_Facturacion_Detalle.Find(item_factura.id_proceso_facturacion_detalle);
                        proceso_facturacion_detalle.id_cat_estado_proceso_facturacion_detalle = (int)Catalogos.Estado_Proceso_Facturacion_Detalle.Activo;
                        proceso_facturacion_detalle.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        proceso_facturacion_detalle.fecha_modificacion = DateTime.Now;
                        db.Entry(proceso_facturacion_detalle).State = EntityState.Modified;
                        db.SaveChanges();
                        item_factura.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        item_factura.fecha_modificacion = DateTime.Now;
                        db.Entry(proceso_facturacion_detalle).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                factura.fecha_modificacion = DateTime.Now;
                factura.comentario_anulacion = comentario_anulacion;
                factura.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                factura.id_cat_estado_factura = (int)Catalogos.Estado_Factura.Anulada;
                factura.id_cat_razon_anulacion = id_cat_razon_anulacion;
                db.Entry(factura).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = factura.id_factura });
            }
            else
            {
                Facturas factura = db.Facturas.Find(id);
                if (fecha.HasValue)
                {
                    var razon = db.Razones_Sociales.Find(factura.id_razon_social);
                    UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                    factura.fecha_modificacion = DateTime.Now;
                    factura.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                    factura.comentario_anulacion = comentario_anulacion;
                    factura.id_cat_razon_anulacion = id_cat_razon_anulacion;
                    factura.id_cat_estado_factura = (int)Catalogos.Estado_Factura.Anulada;
                    db.Entry(factura).State = EntityState.Modified;
                    db.SaveChanges();
                    var Serie = db.Series_Facturas.Find(factura.id_serie_factura);

                    Facturas refacturacion = new Facturas();
                    refacturacion.activo = true;
                    refacturacion.eliminado = false;
                    refacturacion.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    refacturacion.fecha_creacion = DateTime.Now;
                    refacturacion.total = factura.total;
                    refacturacion.fecha_factura = fecha.Value;
                    refacturacion.orden_aceptacion_numero = factura.orden_aceptacion_numero;
                    refacturacion.id_razon_social = razon.id_razon_social;
                    refacturacion.fecha_cobro = fecha.Value.AddDays(razon.dias_credito);
                    refacturacion.id_serie_factura = factura.id_serie_factura;
                    refacturacion.serie = factura.serie;
                    refacturacion.numero_factura = Serie.correlativo;
                    refacturacion.nombre_factura = factura.nombre_factura;
                    refacturacion.direccion = factura.direccion;
                    refacturacion.descripcion = factura.descripcion;
                    refacturacion.nit = factura.nit;
                    refacturacion.id_cat_estado_factura = (int)Catalogos.Estado_Factura.creada;
                    db.Facturas.Add(refacturacion);
                    db.SaveChanges();
                    Serie.correlativo++;
                    db.Entry(Serie).State = EntityState.Modified;
                    db.SaveChanges();
                    foreach (var item_factura in factura.Facturas_Detalle.ToList())
                    {
                        Facturas_Detalle temp_factura = new Facturas_Detalle();
                        temp_factura.id_factura = refacturacion.id_factura;
                        temp_factura.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        temp_factura.fecha_creacion = DateTime.Now;
                        temp_factura.id_proceso_facturacion_detalle = item_factura.id_proceso_facturacion_detalle;
                        temp_factura.id_contrato_agente = item_factura.id_contrato_agente;
                        temp_factura.id_contrato_otro_servicio = item_factura.id_contrato_otro_servicio;
                        temp_factura.id_cat_otro_servicio = item_factura.id_cat_otro_servicio;
                        temp_factura.id_cat_tipo_agente = item_factura.id_cat_tipo_agente;
                        temp_factura.cantidad = item_factura.cantidad;
                        temp_factura.descripcion = item_factura.descripcion;
                        temp_factura.costo = item_factura.costo;
                        temp_factura.precio_venta_unitario = item_factura.precio_venta_unitario;
                        temp_factura.activo = true;
                        temp_factura.eliminado = false;
                        db.Facturas_Detalle.Add(temp_factura);
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = refacturacion.id_factura });
                }
                else
                {
                    return RedirectToAction("Anular", new { id = id });
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
