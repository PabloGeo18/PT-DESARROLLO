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

namespace MVC2013.Areas.Tickets.Controllers
{
    public class Tickets_GeneracionController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Tickets/Tickets_Generacion
        public ActionResult Index()
        {
            var tickets_Generacion = db.Tickets_Generacion.Include(t => t.Usuarios).Include(t => t.Usuarios1).Include(t => t.Usuarios2).Include(t => t.Tickets_Tipo)
                .OrderBy(x  => x.id_ticket_tipo).ThenBy(x => x.desde).ThenBy(x => x.hasta).ThenBy(x => x.fecha_creacion);
            return View(tickets_Generacion.ToList());
        }

        // GET: Tickets/Tickets_Generacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Generacion tickets_Generacion = db.Tickets_Generacion.Find(id);
            if (tickets_Generacion == null)
            {
                return HttpNotFound();
            }
            return View(tickets_Generacion);
        }

        // GET: Tickets/Tickets_Generacion/Create
        public ActionResult Create()
        {
            ViewBag.id_ticket_tipo = new SelectList(db.Tickets_Tipo.Where(x=>x.activo && !x.eliminado && x.valor==10), "id_ticket_tipo", "nombre");
            return View();
        }

        // POST: Tickets/Tickets_Generacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tickets_Generacion tickets_Generacion)
        {
            bool Bandera_Existe = false;
            bool Bandera_desde_mayor = false;
            bool Bandera_mayor = false;
            if (ModelState.IsValid)
            {
                if (tickets_Generacion.desde > 0 && tickets_Generacion.hasta > 0)
                {
                    if (tickets_Generacion.desde <= tickets_Generacion.hasta)
                    {
                        var tg_temp = db.Tickets_Generacion.Where(x => (x.desde <= tickets_Generacion.desde && x.hasta >= tickets_Generacion.desde) || (x.desde <= tickets_Generacion.hasta && x.hasta >= tickets_Generacion.hasta) || (x.desde >= tickets_Generacion.desde && x.hasta <= tickets_Generacion.hasta)).Where(x => x.id_ticket_tipo == tickets_Generacion.id_ticket_tipo).ToList();
                        if (tg_temp.Count == 0)
                        {
                            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                            tickets_Generacion.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                            tickets_Generacion.fecha_creacion = DateTime.Now;
                            tickets_Generacion.activo = true;
                            tickets_Generacion.eliminado = false;
                            tickets_Generacion.cantidad = (tickets_Generacion.hasta - tickets_Generacion.desde) + 1;
                            db.Tickets_Generacion.Add(tickets_Generacion);
                            
                            Tickets_InventarioController Inventario_c = new Tickets_InventarioController();
                            
                            var Inv_Temp = db.Tickets_Inventario.Where(x => x.id_usuario == usuarioTO.usuario.id_usuario && x.id_ticket_tipo == tickets_Generacion.id_ticket_tipo).ToList();
                            if (Inv_Temp.Count == 0)
                            {
                                Tickets_Inventario Inventario = new Tickets_Inventario();
                                Inventario.id_ticket_tipo = tickets_Generacion.id_ticket_tipo;
                                Inventario.id_usuario = usuarioTO.usuario.id_usuario;
                                Inventario.existencia = tickets_Generacion.cantidad;
                                Inventario.activo = true;
                                Inventario.eliminado = false;
                                Inventario.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                Inventario.fecha_creacion = DateTime.Now;
                                db.Tickets_Inventario.Add(Inventario);
                            }
                            else {
                                Inv_Temp[0].fecha_modificacion = DateTime.Now;
                                Inv_Temp[0].id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                Inv_Temp[0].existencia += tickets_Generacion.cantidad;
                                db.Entry(Inv_Temp[0]).State = EntityState.Modified;
                            }

                            Tickets_Movimiento Nuevo_Movimiento = new Tickets_Movimiento();
                            Nuevo_Movimiento.activo = true;
                            Nuevo_Movimiento.eliminado = false;
                            Nuevo_Movimiento.id_ticket_tipo = tickets_Generacion.id_ticket_tipo;
                            Nuevo_Movimiento.id_ticket_tipo_movimiento = 1;
                            Nuevo_Movimiento.id_usuario = usuarioTO.usuario.id_usuario;
                            Nuevo_Movimiento.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                            Nuevo_Movimiento.debe = 0;
                            Nuevo_Movimiento.haber = tickets_Generacion.cantidad;
                            Nuevo_Movimiento.desde = tickets_Generacion.desde;
                            Nuevo_Movimiento.hasta = tickets_Generacion.hasta;
                            Nuevo_Movimiento.fecha_creacion = DateTime.Now;
                            db.Tickets_Movimiento.Add(Nuevo_Movimiento);
                            db.SaveChanges();

                            return RedirectToAction("Index");
                        }
                        else ModelState.AddModelError("Existe", "Alguna parte de la cantidad especificada ya existe");
                    }
                    else ModelState.AddModelError("Desde_Mayor", "La cantidad especificada en \"desde\" debe ser menor que en \"hasta\"");
                }
                else ModelState.AddModelError("Mayor", "La cantidad especificada en \"desde\" o en \"hasta\" deben ser mayor a 0");
                    
            }
            //if (Bandera_desde_mayor)
                
            //if (Bandera_Existe)
                
            //if (Bandera_mayor)
                
            if (tickets_Generacion.id_ticket_tipo == 0)
                ModelState.AddModelError("Seleccion_Tipo", "Se debe seleccionar un tipo de ticket");
            ViewBag.id_ticket_tipo = new SelectList(db.Tickets_Tipo, "id_ticket_tipo", "nombre", tickets_Generacion.id_ticket_tipo);
            return View(tickets_Generacion);
        }

        // GET: Tickets/Tickets_Generacion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Generacion tickets_Generacion = db.Tickets_Generacion.Find(id);
            if (tickets_Generacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_ticket_tipo = new SelectList(db.Tickets_Tipo.Where(x => x.activo && !x.eliminado && x.valor == 10), "id_ticket_tipo", "nombre", tickets_Generacion.id_ticket_tipo);
            return View(tickets_Generacion);
        }

        // POST: Tickets/Tickets_Generacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tickets_Generacion tickets_Generacion)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            if (ModelState.IsValid)
            {
                Tickets_Generacion tgEdit = db.Tickets_Generacion.Find(tickets_Generacion.id_ticket_generacion);
                tgEdit.id_ticket_tipo = tickets_Generacion.id_ticket_tipo;
                tgEdit.desde = tickets_Generacion.desde;
                tgEdit.hasta = tickets_Generacion.hasta;
                tgEdit.cantidad = (tickets_Generacion.hasta - tickets_Generacion.desde) + 1;
                tgEdit.activo = true;
                tgEdit.eliminado = false;
                tgEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                tgEdit.fecha_modificacion = DateTime.Now;
                db.Entry(tgEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_ticket_tipo = new SelectList(db.Tickets_Tipo, "id_ticket_tipo", "nombre", tickets_Generacion.id_ticket_tipo);
            return View(tickets_Generacion);
        }

        // GET: Tickets/Tickets_Generacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Generacion tickets_Generacion = db.Tickets_Generacion.Find(id);
            if (tickets_Generacion == null)
            {
                return HttpNotFound();
            }
            return View(tickets_Generacion);
        }

        // POST: Tickets/Tickets_Generacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tickets_Generacion tickets_Generacion = db.Tickets_Generacion.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            tickets_Generacion.activo = false;
            tickets_Generacion.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            tickets_Generacion.fecha_eliminacion = DateTime.Now;
            tickets_Generacion.eliminado = true;
            db.Entry(tickets_Generacion).State = EntityState.Modified;
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
