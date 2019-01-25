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

namespace MVC2013.Areas.Customers.Controllers
{
    public class Contratos_ServiciosController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Contratos_Servicios
        public ActionResult Index()
        {
            var contratos_Servicios = db.Contratos_Servicios;
            return View(contratos_Servicios.ToList());
        }

        // GET: Administracion/Contratos_Servicios/Details/5
        public ActionResult Details(int? id)
        {
            Contratos_Servicios contratos_Servicios = db.Contratos_Servicios.Find(id);
            if (contratos_Servicios == null)
            {
                return HttpNotFound();
            }
            
            return View(contratos_Servicios);
        }

        // GET: Administracion/Contratos_Servicios/Create
        public ActionResult Create(int id)
        {
            var contrato = db.Contratos_Servicios.Find(id);
            ViewBag.razon_social = contrato.Razones_Sociales;
            ViewBag.cliente = contrato.Razones_Sociales.Clientes.nombre;
            ViewBag.id_agente = new SelectList(db.Cat_Tipos_Agente.Where(x => x.activo && !x.eliminado), "id_cat_tipo_agente", "nombre");                            
            ViewBag.id_servicio = new SelectList(db.Cat_Otros_Servicios.Where(x => x.activo && !x.eliminado), "id_cat_otro_servicio", "nombre");
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones.Where(x => x.activo && !x.eliminado && x.id_razon_social == contrato.id_razon_social).Select(s => new { id_ubicacion = s.id_ubicacion, nombre = s.id_ubicacion + ". " + s.nombre }), "id_ubicacion", "nombre");
            ViewBag.id_razon_social_grupo_factura = new SelectList(db.Razon_Social_Grupos_Factura.Where(x => x.activo && !x.eliminado && x.id_razon_social == contrato.id_razon_social).Select(s => new { id_razon_social_grupo_factura = s.id_razon_social_grupo_factura, nombre = s.correlativo + ". " + s.nombre }), "id_razon_social_grupo_factura", "nombre");
            
            return View(contrato);
        }

        // POST: Administracion/Contratos_Servicios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contratos_Servicios contratos_Servicios, int id_razon_social)
        {

            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item_agente in Request.Form.AllKeys.Where(x => x.Contains("id_agente_")).Select(x => x.Replace("id_agente_", "")))
                    {

                        int temp_agente = Convert.ToInt32(Request["id_agente_" + item_agente]);
                        int temp_cantidad = Convert.ToInt32(Request["cant_agt_" + item_agente]);
                        if (!String.IsNullOrEmpty(Request["cant_agt_" + item_agente]))
                        {
                            temp_cantidad = Convert.ToInt32(Request["cant_agt_" + item_agente]);
                        }

                        Decimal temp_costo = 0;
                        if (!String.IsNullOrEmpty(Request["costo_unitario_agt_" + item_agente]))
                        {
                            temp_costo = Convert.ToDecimal(Request["costo_unitario_agt_" + item_agente].ToString().Replace(",", "").Replace(".", ","));
                        }

                        Decimal temp_venta = 0;
                        if (!String.IsNullOrEmpty(Request["precio_venta_unitario_agt_" + item_agente]))
                        {
                            temp_venta = Convert.ToDecimal(Request["precio_venta_unitario_agt_" + item_agente].ToString().Replace(",", "").Replace(".", ","));
                        }
                        DateTime temp_inicio = Convert.ToDateTime(Request["fecha_inicio_agt_" + item_agente]);
                        if (!String.IsNullOrEmpty(Request["fecha_inicio_agt_" + item_agente]))
                        {
                            temp_inicio = Convert.ToDateTime(Request["fecha_inicio_agt_" + item_agente]);
                        }
                        DateTime? temp_fin = null;
                        if (!String.IsNullOrEmpty(Request["fecha_fin_agt_" + item_agente]))
                        {
                            temp_fin = Convert.ToDateTime(Request["fecha_fin_agt_" + item_agente]);
                        }
                        int? temp_agrupacion = null;
                        if (!String.IsNullOrEmpty(Request["id_agrupacion_agt_" + item_agente]))
                        {
                            temp_agrupacion = Convert.ToInt32(Request["id_agrupacion_agt_" + item_agente]);
                        }
                        int? temp_ubicacion = null;
                        if (!String.IsNullOrEmpty(Request["id_ubicacion_agt_" + item_agente]))
                        {
                            temp_ubicacion = Convert.ToInt32(Request["id_ubicacion_agt_" + item_agente]);
                        }
                        Contratos_Agentes agente_con = Nuevo_Agente_Contratado();
                        agente_con.id_razon_social = id_razon_social;
                        agente_con.id_cat_tipo_agente = temp_agente;
                        agente_con.cantidad = temp_cantidad;
                        agente_con.costo = temp_costo;
                        agente_con.precio_venta_unitario = temp_venta;
                        agente_con.fecha_inicio = temp_inicio;
                        agente_con.fecha_fin = temp_fin;
                        agente_con.id_contrato_servicio = contratos_Servicios.id_contrato_servicio;
                        agente_con.id_razon_social_grupo_factura = temp_agrupacion;
                        agente_con.id_ubicacion = temp_ubicacion;
                        db.Contratos_Agentes.Add(agente_con);


                    }
                    foreach (var item_servicio in Request.Form.AllKeys.Where(x => x.Contains("id_cat_otro_servicio_")).Select(x => x.Replace("id_cat_otro_servicio_", "")))
                    {
                        int temp_servicio = Convert.ToInt32(Request["id_cat_otro_servicio_" + item_servicio]);
                        int temp_cantidad = Convert.ToInt32(Request["cant_serv_" + item_servicio]);
                        if (!String.IsNullOrEmpty(Request["cant_serv_" + item_servicio]))
                        {
                            temp_cantidad = Convert.ToInt32(Request["cant_serv_" + item_servicio]);
                        }

                        Decimal temp_costo = 0;
                        if (!String.IsNullOrEmpty(Request["costo_serv_" + item_servicio]))
                        {
                            temp_costo = Convert.ToDecimal(Request["costo_serv_" + item_servicio].ToString().Replace(",", "").Replace(".", ","));
                        }

                        Decimal temp_venta = 0;
                        if (!String.IsNullOrEmpty(Request["venta_serv_" + item_servicio]))
                        {
                            temp_venta = Convert.ToDecimal(Request["venta_serv_" + item_servicio].ToString().Replace(",", "").Replace(".", ","));
                        }
                        DateTime temp_inicio = Convert.ToDateTime(Request["fecha_inicio_serv_" + item_servicio]);
                        if (!String.IsNullOrEmpty(Request["fecha_inicio_serv_" + item_servicio]))
                        {
                            temp_inicio = Convert.ToDateTime(Request["fecha_inicio_serv_" + item_servicio]);
                        }
                        DateTime? temp_fin = null;
                        if (!String.IsNullOrEmpty(Request["fecha_fin_serv_" + item_servicio]))
                        {
                            temp_fin = Convert.ToDateTime(Request["fecha_fin_serv_" + item_servicio]);
                        }
                        int? temp_agrupacion = null;
                        if (!String.IsNullOrEmpty(Request["id_agrupacion_serv_" + item_servicio]))
                        {
                            temp_agrupacion = Convert.ToInt32(Request["id_agrupacion_serv_" + item_servicio]);
                        }
                        int? temp_ubicacion = null;
                        if (!String.IsNullOrEmpty(Request["id_ubicacion_serv_" + item_servicio]))
                        {
                            temp_ubicacion = Convert.ToInt32(Request["id_ubicacion_serv_" + item_servicio]);
                        }
                        Contratos_Otros_Servicios servicio_con = Nuevo_Servicio_Contratado();
                        servicio_con.id_razon_social = id_razon_social;
                        servicio_con.id_cat_otro_servicio = temp_servicio;
                        servicio_con.cantidad = temp_cantidad;
                        servicio_con.fecha_inicio = temp_inicio;
                        servicio_con.fecha_fin = temp_fin;
                        servicio_con.costo = temp_costo;
                        servicio_con.precio_venta_unitario = temp_venta;
                        servicio_con.id_contrato_servicio = contratos_Servicios.id_contrato_servicio;
                        servicio_con.id_razon_social_grupo_factura = temp_agrupacion;
                        servicio_con.id_ubicacion = temp_ubicacion;
                        db.Contratos_Otros_Servicios.Add(servicio_con);
                    }
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Details", new { id = contratos_Servicios.id_contrato_servicio });
                }
                catch (Exception)
                {
                    ViewBag.servicios = db.Contratos_Servicios.Where(x => x.activo && !x.eliminado).Where(x => x.id_razon_social == id_razon_social && x.Cat_Tipos_Facturacion.ciclica == true
                                                                && (x.id_cat_estado_servicio_contratado != (int)Catalogos.Estado_Contrato_Servicio.Finalizado
                                                                || x.id_cat_estado_servicio_contratado != (int)Catalogos.Estado_Contrato_Servicio.Cancelado)).ToList();
                    var temp_razon_social = db.Razones_Sociales.Find(id_razon_social);
                    ViewBag.razon_social = temp_razon_social;
                    ViewBag.cliente = temp_razon_social.Clientes.nombre;
                    //ContextMessage msg = new ContextMessage(ContextMessage.Warning, "Error con la conexión al servidor. Datos no guardados.");
                    tran.Rollback();
                    //msg.ReturnUrl = Url.Action("Create");
                    //TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Create", new {id = contratos_Servicios.id_contrato_servicio});
                }
            }

        }

        private Contratos_Agentes Nuevo_Agente_Contratado()
        {
            Contratos_Agentes nuevo_contrato = new Contratos_Agentes();
            nuevo_contrato.activo = true;
            nuevo_contrato.eliminado = false;
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            nuevo_contrato.id_usuario_creacion = usuarioTO.usuario.id_usuario;
            nuevo_contrato.fecha_creacion = DateTime.Now;
            return nuevo_contrato;
        }

        private Contratos_Otros_Servicios Nuevo_Servicio_Contratado()
        {
            Contratos_Otros_Servicios nuevo_contrato = new Contratos_Otros_Servicios();
            nuevo_contrato.activo = true;
            nuevo_contrato.eliminado = false;
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            nuevo_contrato.id_usuario_creacion = usuarioTO.usuario.id_usuario;
            nuevo_contrato.fecha_creacion = DateTime.Now;
            return nuevo_contrato;
        }

        // GET: Administracion/Contratos_Servicios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratos_Servicios contratos_Servicios = db.Contratos_Servicios.Find(id);
            if (contratos_Servicios == null)
            {
                return HttpNotFound();
            }
            //ViewBag.id_ubicacion = new SelectList(db.Ubicaciones, "id_ubicacion", "direccion", contratos_Servicios.id_ubicacion);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_modificacion);
            ViewBag.id_cat_estado_servicio_contratado = new SelectList(db.Cat_Estados_Servicio_Contratado, "id_cat_estado_servicio_contratado", "nombre", contratos_Servicios.id_cat_estado_servicio_contratado);
            ViewBag.id_cat_tipo_facturacion = new SelectList(db.Cat_Tipos_Facturacion, "id_cat_tipo_facturacion", "nombre", contratos_Servicios.id_cat_tipo_facturacion);
            return View(contratos_Servicios);
        }

        // POST: Administracion/Contratos_Servicios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_contrato_servicio,id_ubicacion,id_cat_tipo_facturacion,id_cat_estado_servicio_contratado,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Contratos_Servicios contratos_Servicios)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contratos_Servicios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.id_ubicacion = new SelectList(db.Ubicaciones, "id_ubicacion", "direccion", contratos_Servicios.id_ubicacion);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_modificacion);
            ViewBag.id_cat_estado_servicio_contratado = new SelectList(db.Cat_Estados_Servicio_Contratado, "id_cat_estado_servicio_contratado", "nombre", contratos_Servicios.id_cat_estado_servicio_contratado);
            ViewBag.id_cat_tipo_facturacion = new SelectList(db.Cat_Tipos_Facturacion, "id_cat_tipo_facturacion", "nombre", contratos_Servicios.id_cat_tipo_facturacion);
            return View(contratos_Servicios);
        }

        // GET: Administracion/Contratos_Servicios/Delete/5
        public ActionResult Delete(int id)
        {
            Contratos_Servicios contratos_Servicios = db.Contratos_Servicios.Find(id);
            if (contratos_Servicios == null)
            {
                return HttpNotFound();
            }
            return View(contratos_Servicios);
        }

        // POST: Administracion/Contratos_Servicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Contratos_Servicios contratos_Servicios = db.Contratos_Servicios.Find(id);
            contratos_Servicios.fecha_eliminacion = DateTime.Now;
            contratos_Servicios.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            contratos_Servicios.id_cat_estado_servicio_contratado = (int)Catalogos.Estado_Contrato_Servicio.Cancelado;
            contratos_Servicios.activo = false;
            contratos_Servicios.eliminado = true;
            db.Entry(contratos_Servicios).State = EntityState.Modified;
            db.SaveChanges();
            foreach (var item in contratos_Servicios.Contratos_Agentes)
            {
                item.activo = false;
                item.eliminado = true;
                item.fecha_eliminacion = DateTime.Now;
                item.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            
            return RedirectToAction("Details_RS", "Clientes", new { id_Razon_Social = contratos_Servicios.id_razon_social});
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
