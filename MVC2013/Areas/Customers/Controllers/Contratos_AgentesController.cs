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
    public class Contratos_AgentesController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Contratos_Agentes
        public ActionResult Index()
        {
            var contratos_Agentes = db.Contratos_Agentes.Include(c => c.Usuarios).Include(c => c.Usuarios1).Include(c => c.Usuarios2).Include(c => c.Cat_Tipos_Agente).Include(c => c.Contratos_Servicios);
            return View(contratos_Agentes.ToList());
        }

        // GET: Administracion/Contratos_Agentes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratos_Agentes contratos_Agentes = db.Contratos_Agentes.Find(id);
            if (contratos_Agentes == null)
            {
                return HttpNotFound();
            }
            return View(contratos_Agentes);
        }

        // GET: Administracion/Contratos_Agentes/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente, "id_cat_tipo_agente", "nombre");
            ViewBag.id_contrato_servicio = new SelectList(db.Contratos_Servicios, "id_contrato_servicio", "id_contrato_servicio");
            return View();
        }

        // POST: Administracion/Contratos_Agentes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_contrato_agente,id_contrato_servicio,id_cat_tipo_agente,cantidad,costo,precio_venta_unitario,fecha_inicio,fecha_fin,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Contratos_Agentes contratos_Agentes)
        {
            if (ModelState.IsValid)
            {
                db.Contratos_Agentes.Add(contratos_Agentes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Agentes.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Agentes.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Agentes.id_usuario_modificacion);
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente, "id_cat_tipo_agente", "nombre", contratos_Agentes.id_cat_tipo_agente);
            ViewBag.id_contrato_servicio = new SelectList(db.Contratos_Servicios, "id_contrato_servicio", "id_contrato_servicio", contratos_Agentes.id_contrato_servicio);
            return View(contratos_Agentes);
        }

        // GET: Administracion/Contratos_Agentes/Edit/5
        public ActionResult Edit(int? id)
        {
            Contratos_Agentes agente = db.Contratos_Agentes.Where(x => x.activo && !x.eliminado && x.id_contrato_agente == id).Take(1).FirstOrDefault();
            if (agente == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente.Where(x => x.activo && !x.eliminado), "id_cat_tipo_agente", "nombre", agente.id_cat_tipo_agente);
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones.Where(x => x.activo && !x.eliminado && x.id_razon_social == agente.id_razon_social).Select(s => new { id_ubicacion = s.id_ubicacion, nombre = s.id_ubicacion + ". " + s.nombre }), "id_ubicacion", "nombre", agente.id_ubicacion);
            ViewBag.id_razon_social_grupo_factura = new SelectList(db.Razon_Social_Grupos_Factura.Where(x => x.activo && !x.eliminado && x.id_razon_social == agente.id_razon_social).Select(s => new { id_razon_social_grupo_factura = s.id_razon_social_grupo_factura, nombre = s.correlativo + ". " + s.nombre }), "id_razon_social_grupo_factura", "nombre", agente.id_razon_social_grupo_factura);
            ViewBag.fecha_inicio = agente.fecha_inicio;
            ViewBag.fecha_fin = agente.fecha_fin;
            ViewBag.id_contrato_agente = id;
            return View(agente);
        }

        // POST: Administracion/Contratos_Agentes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Contratos_Agentes contratos_Agentes)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                Contratos_Agentes Temp_contrato_agente = db.Contratos_Agentes.Find(contratos_Agentes.id_contrato_agente);
                Temp_contrato_agente.cantidad = contratos_Agentes.cantidad;
                Temp_contrato_agente.costo = contratos_Agentes.costo;
                Temp_contrato_agente.precio_venta_unitario = contratos_Agentes.precio_venta_unitario;
                Temp_contrato_agente.id_cat_tipo_agente = contratos_Agentes.id_cat_tipo_agente;
                Temp_contrato_agente.fecha_inicio = contratos_Agentes.fecha_inicio;
                Temp_contrato_agente.fecha_fin = contratos_Agentes.fecha_fin;
                Temp_contrato_agente.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                Temp_contrato_agente.id_razon_social_grupo_factura = contratos_Agentes.id_razon_social_grupo_factura;
                Temp_contrato_agente.id_ubicacion = contratos_Agentes.id_ubicacion;
                Temp_contrato_agente.fecha_modificacion = DateTime.Now;
                db.Entry(Temp_contrato_agente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Contratos_Servicios", new { id = Temp_contrato_agente.id_contrato_servicio});
            }

            return View(contratos_Agentes);
        }

        // GET: Administracion/Contratos_Agentes/Edit/5
        public ActionResult Edit_Agente(int? id)
        {
            Contratos_Agentes agente = db.Contratos_Agentes.Where(x => x.activo && !x.eliminado && x.id_contrato_agente == id).Take(1).FirstOrDefault();
            if (agente == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente.Where(x => x.activo && !x.eliminado), "id_cat_tipo_agente", "nombre", agente.id_cat_tipo_agente);
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones.Where(x => x.activo && !x.eliminado && x.id_razon_social == agente.id_razon_social).Select(s => new { id_ubicacion = s.id_ubicacion, nombre = s.id_ubicacion + ". " + s.nombre }), "id_ubicacion", "nombre", agente.id_ubicacion);
            ViewBag.id_razon_social_grupo_factura = new SelectList(db.Razon_Social_Grupos_Factura.Where(x => x.activo && !x.eliminado && x.id_razon_social == agente.id_razon_social).Select(s => new { id_razon_social_grupo_factura = s.id_razon_social_grupo_factura, nombre = s.correlativo + ". " + s.nombre }), "id_razon_social_grupo_factura", "nombre", agente.id_razon_social_grupo_factura);
            ViewBag.fecha_inicio = agente.fecha_inicio;
            ViewBag.fecha_fin = agente.fecha_fin;
            ViewBag.id_contrato_agente = id;
            return View(agente);
        }

        // POST: Administracion/Contratos_Agentes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Agente(Contratos_Agentes contratos_Agentes)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                Contratos_Agentes Temp_contrato_agente = db.Contratos_Agentes.Find(contratos_Agentes.id_contrato_agente);
                Temp_contrato_agente.cantidad = contratos_Agentes.cantidad;
                Temp_contrato_agente.costo = contratos_Agentes.costo;
                Temp_contrato_agente.precio_venta_unitario = contratos_Agentes.precio_venta_unitario;
                Temp_contrato_agente.id_cat_tipo_agente = contratos_Agentes.id_cat_tipo_agente;
                Temp_contrato_agente.fecha_inicio = contratos_Agentes.fecha_inicio;
                Temp_contrato_agente.fecha_fin = contratos_Agentes.fecha_fin;
                Temp_contrato_agente.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                Temp_contrato_agente.fecha_modificacion = DateTime.Now;
                Temp_contrato_agente.id_razon_social_grupo_factura = contratos_Agentes.id_razon_social_grupo_factura;
                Temp_contrato_agente.id_ubicacion = contratos_Agentes.id_ubicacion;
                db.Entry(Temp_contrato_agente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details_RS", "Clientes", new { id_Razon_Social = Temp_contrato_agente.id_razon_social });
            }

            return View(contratos_Agentes);
        }

        // GET: Administracion/Contratos_Agentes/Delete/5
        public ActionResult Delete(int? id)
        {

            Contratos_Agentes contratos_Agentes = db.Contratos_Agentes.Where(x => x.activo && !x.eliminado && x.id_contrato_agente == id).Take(1).FirstOrDefault();
            if (contratos_Agentes == null)
            {
                return HttpNotFound();
            }
            return View(contratos_Agentes);
        }

        // POST: Administracion/Contratos_Agentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, String Comentario)
        {
            
            Contratos_Agentes contratos_Agentes = db.Contratos_Agentes.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            contratos_Agentes.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            contratos_Agentes.descripcion_eliminacion = Comentario;
            contratos_Agentes.eliminado = true;
            contratos_Agentes.activo = false;
            contratos_Agentes.fecha_eliminacion = DateTime.Now;
            
            db.Entry(contratos_Agentes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Contratos_Servicios", new { id = contratos_Agentes.id_contrato_servicio });

        }

        // GET: Administracion/Contratos_Agentes/Delete/5
        public ActionResult Delete_Agente(int? id)
        {

            Contratos_Agentes contratos_Agentes = db.Contratos_Agentes.Where(x => x.activo && !x.eliminado && x.id_contrato_agente == id).Take(1).FirstOrDefault();
            if (contratos_Agentes == null)
            {
                return HttpNotFound();
            }
            return View(contratos_Agentes);
        }

        // POST: Administracion/Contratos_Agentes/Delete/5
        [HttpPost, ActionName("Delete_Agente")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed_RS(int id, String Comentario)
        {
            Contratos_Agentes contratos_Agentes = db.Contratos_Agentes.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            contratos_Agentes.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            contratos_Agentes.descripcion_eliminacion = Comentario;
            contratos_Agentes.eliminado = true;
            contratos_Agentes.activo = false;
            contratos_Agentes.fecha_eliminacion = DateTime.Now;
            db.Entry(contratos_Agentes).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details_RS", "Clientes", new { id_Razon_Social = contratos_Agentes.id_razon_social });
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
