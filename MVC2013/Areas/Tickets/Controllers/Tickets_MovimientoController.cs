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
using MVC2013.Src.Sdc.Reports;

namespace MVC2013.Areas.Tickets.Controllers
{
    public class Tickets_MovimientoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Tickets/Tickets_Movimiento
        public ActionResult Index()
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Tickets_Inventario inventario = db.Tickets_Inventario.Where(x => x.activo && !x.eliminado && x.id_usuario == usuarioTO.usuario.id_usuario).SingleOrDefault();
            ViewBag.inv = inventario;
            var tickets_Movimiento = db.Tickets_Movimiento.Include(t => t.Usuarios)
                .Include(t => t.Empleado).Include(t => t.Tickets_Tipo).Include(t => t.Tickets_Tipo_Movimiento)
                .Where(p => p.activo && !p.eliminado).Where(t => t.Usuarios2.id_usuario == usuarioTO.usuario.id_usuario);
            return View(tickets_Movimiento.ToList());
        }

        public ActionResult MovTicketEmpleado()
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Tickets_Inventario inventario = db.Tickets_Inventario.Where(x => x.activo && !x.eliminado && x.id_usuario == usuarioTO.usuario.id_usuario).SingleOrDefault();
            ViewBag.inv = inventario;
            var tickets_Movimiento = db.Tickets_Movimiento.Include(t => t.Usuarios)
                .Include(t => t.Empleado).Include(t => t.Tickets_Tipo).Include(t => t.Tickets_Tipo_Movimiento)
                .Where(p => p.activo && !p.eliminado).Where(t => t.Usuarios2.id_usuario == usuarioTO.usuario.id_usuario);
            return View(tickets_Movimiento.ToList());
        }

        // GET: Tickets/Tickets_Movimiento/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Movimiento tickets_Movimiento = db.Tickets_Movimiento.Find(id);
            if (tickets_Movimiento == null)
            {
                return HttpNotFound();
            }
            return View(tickets_Movimiento);
        }

        // GET: Tickets/Tickets_Movimiento/Create
        public ActionResult Trasladar_Encargado()
        {
            ViewBag.id_usuario = new SelectList(db.Usuarios.Where(x=>!x.eliminado && !x.bloqueo_habilitado), "id_usuario", "nombre_completo_usuario");
            ViewBag.id_ticket_tipo = new SelectList(db.Tickets_Tipo.Where(p => p.activo && !p.eliminado && p.valor==10), "id_ticket_tipo", "nombre");
            return View();
        }

        // POST: Tickets/Tickets_Movimiento/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Trasladar_Encargado(Tickets_Movimiento Primer_Movimiento)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                int Existencia = db.Tickets_Inventario.Where(x => x.id_usuario == usuarioTO.usuario.id_usuario && x.id_ticket_tipo == Primer_Movimiento.id_ticket_tipo).ToList().FirstOrDefault().existencia;
                Primer_Movimiento.debe = Primer_Movimiento.hasta - Primer_Movimiento.desde + 1;
                if (Existencia >= Primer_Movimiento.debe)
                {
                    var Contiene_Rangos = db.Tickets_Movimiento.Where(x => x.id_usuario == usuarioTO.usuario.id_usuario).Where(x => x.id_ticket_tipo == Primer_Movimiento.id_ticket_tipo)
                        .Where(x => x.debe == 0).Where(x => x.desde <= Primer_Movimiento.desde && x.hasta >= Primer_Movimiento.hasta).ToList();
                    if (Contiene_Rangos.Count > 0)
                    {
                        var Tiene_Tickets = db.Tickets_Movimiento.Where(x => x.id_usuario == usuarioTO.usuario.id_usuario && x.activo && !x.eliminado).Where(x => x.id_ticket_tipo == Primer_Movimiento.id_ticket_tipo)
                            .Where(x => x.haber == 0)
                            .Where(x => (x.desde <= Primer_Movimiento.desde && x.hasta >= Primer_Movimiento.desde)
                                     || (x.desde <= Primer_Movimiento.hasta && x.hasta >= Primer_Movimiento.hasta)
                                     || (x.desde >= Primer_Movimiento.desde && x.hasta <= Primer_Movimiento.hasta)).ToList();
                        if (Tiene_Tickets.Count == 0)
                        {
                            var Tiene_Tickets2 = db.Tickets_Movimiento.Where(x => x.id_usuario == usuarioTO.usuario.id_usuario).Where(x => x.id_ticket_tipo == Primer_Movimiento.id_ticket_tipo)
                                                .Where(x => x.haber == 0)
                                                .Where(x => (x.desde >= Primer_Movimiento.desde && x.hasta <= Primer_Movimiento.hasta)).ToList();
                            if (Primer_Movimiento.desde <= Primer_Movimiento.hasta)
                            {
                                /*TICKETS MOVIMIENTO - PRIMERO*/
                                int Usuario_destino = Primer_Movimiento.id_usuario;

                                Primer_Movimiento.activo = true;
                                Primer_Movimiento.eliminado = false;
                                Primer_Movimiento.id_usuario = usuarioTO.usuario.id_usuario;
                                Primer_Movimiento.fecha_creacion = DateTime.Now;
                                Primer_Movimiento.id_usuario_creacion = usuarioTO.usuario.id_usuario;

                                Primer_Movimiento.haber = 0;
                                Primer_Movimiento.id_ticket_tipo_movimiento = 2;

                                db.Tickets_Movimiento.Add(Primer_Movimiento);

                                /*TICKETS INVENTARIO - PRIMERO*/
                                var Temp_T1 = db.Tickets_Inventario.Where(x => x.id_usuario == Primer_Movimiento.id_usuario).Where(y => y.id_ticket_tipo == Primer_Movimiento.id_ticket_tipo).ToList().FirstOrDefault();
                                Temp_T1.existencia -= Primer_Movimiento.debe;
                                Temp_T1.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                Temp_T1.fecha_modificacion = DateTime.Now;
                                db.Entry(Temp_T1).State = EntityState.Modified;

                                Tickets_Movimiento Segundo_Movimiento = new Tickets_Movimiento();

                                Segundo_Movimiento.activo = true;
                                Segundo_Movimiento.eliminado = false;
                                Segundo_Movimiento.id_usuario = Usuario_destino;
                                Segundo_Movimiento.debe = 0;
                                Segundo_Movimiento.haber = Primer_Movimiento.debe;
                                Segundo_Movimiento.desde = Primer_Movimiento.desde;
                                Segundo_Movimiento.hasta = Primer_Movimiento.hasta;
                                Segundo_Movimiento.id_ticket_tipo = Primer_Movimiento.id_ticket_tipo;
                                Segundo_Movimiento.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                Segundo_Movimiento.fecha_creacion = DateTime.Now;
                                Segundo_Movimiento.id_ticket_tipo_movimiento = 2;

                                db.Tickets_Movimiento.Add(Segundo_Movimiento);

                                /*TICKETS INVENTARIO*/

                                var Existe_Usuario = db.Tickets_Inventario.Where(x => x.id_ticket_tipo == Segundo_Movimiento.id_ticket_tipo).Where(y => y.id_usuario == Segundo_Movimiento.id_usuario).ToList();
                                if (Existe_Usuario.Count > 0)
                                {
                                    Existe_Usuario[0].existencia += Segundo_Movimiento.haber;
                                    Existe_Usuario[0].fecha_modificacion = DateTime.Now;
                                    Existe_Usuario[0].id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    db.Entry(Existe_Usuario[0]).State = EntityState.Modified;
                                }
                                else
                                {
                                    Tickets_Inventario Inventario = new Tickets_Inventario();
                                    Inventario.id_ticket_tipo = Segundo_Movimiento.id_ticket_tipo;
                                    Inventario.id_usuario = Segundo_Movimiento.id_usuario;
                                    Inventario.existencia = Segundo_Movimiento.haber;
                                    Inventario.activo = true;
                                    Inventario.eliminado = false;
                                    Inventario.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    Inventario.fecha_creacion = DateTime.Now;
                                    db.Tickets_Inventario.Add(Inventario);
                                }


                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }
                            else
                                ModelState.AddModelError("Desde_Mayor", "El area especificada como \"desde\" debe ser menor que \"hasta\"");
                        }
                        else
                            ModelState.AddModelError("Traslado", "El usuario ya traslado esos tickets");
                    }
                    else
                        ModelState.AddModelError("Sin_Tickets", "El usuario no contiene ninguno de esos tickets");
                }
                else
                    ModelState.AddModelError("Sin_Existencia", "El encargado no cuenta con la cantidad Existente");
            }

            ViewBag.id_usuario = new SelectList(db.Usuarios, "id_usuario", "nombre_completo_usuario", Primer_Movimiento.id_usuario);
            ViewBag.id_ticket_tipo = new SelectList(db.Tickets_Tipo, "id_ticket_tipo", "nombre", Primer_Movimiento.id_ticket_tipo);
            return View(Primer_Movimiento);
        }

        // GET: Tickets/Tickets_Movimiento/Create
        public ActionResult Trasladar_Empleado()
        {
            //ViewBag.id_usuario = new SelectList(db.Usuarios, "id_usuario", "nombre_completo_usuario");
            ViewBag.id_ticket_tipo = new SelectList(db.Tickets_Tipo.Where(x => x.activo && !x.eliminado && x.valor == 10), "id_ticket_tipo", "nombre");
            //ViewBag.id_empleado = new SelectList(db.Empleado, "id_empleado", "primer_nombre");

            ViewBag.id_empleado = db.Empleado.Where(p => p.activo && !p.eliminado)
                    .Select(p => new SelectListItem
                    {
                        Text = p.id_empleado + "-" + p.primer_nombre + " " + p.primer_apellido,
                        Value = p.id_empleado.ToString()
                    });
            return View();
        }

        // POST: Tickets/Tickets_Movimiento/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Trasladar_Empleado(Tickets_Movimiento Primer_Movimiento)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                int Existencia = db.Tickets_Inventario.Where(x => x.id_usuario == usuarioTO.usuario.id_usuario && x.id_ticket_tipo == Primer_Movimiento.id_ticket_tipo).ToList().FirstOrDefault().existencia;
                Primer_Movimiento.debe = Primer_Movimiento.hasta - Primer_Movimiento.desde + 1;
                if (Existencia >= Primer_Movimiento.debe)
                {
                    var Contiene_Rangos = db.Tickets_Movimiento.Where(x => x.id_usuario == usuarioTO.usuario.id_usuario).Where(x => x.id_ticket_tipo == Primer_Movimiento.id_ticket_tipo)
                        .Where(x => x.debe == 0).Where(x => x.desde <= Primer_Movimiento.desde && x.hasta >= Primer_Movimiento.hasta).ToList();
                    if (Contiene_Rangos.Count > 0)
                    {
                        var Tiene_Tickets = db.Tickets_Movimiento.Where(x => x.id_usuario == usuarioTO.usuario.id_usuario && x.activo && !x.eliminado).Where(x => x.id_ticket_tipo == Primer_Movimiento.id_ticket_tipo)
                            .Where(x => x.haber == 0).Where(x => (x.desde <= Primer_Movimiento.desde && x.hasta >= Primer_Movimiento.desde) || (x.desde <= Primer_Movimiento.hasta && x.hasta >= Primer_Movimiento.hasta)).ToList();
                        if (Tiene_Tickets.Count == 0)
                        {
                            if (Primer_Movimiento.desde <= Primer_Movimiento.hasta)
                            {
                                /*TICKETS MOVIMIENTO - PRIMERO*/
                                int Usuario_destino = Primer_Movimiento.id_usuario;

                                Primer_Movimiento.activo = true;
                                Primer_Movimiento.eliminado = false;
                                Primer_Movimiento.id_usuario = usuarioTO.usuario.id_usuario;
                                Primer_Movimiento.fecha_creacion = DateTime.Now;
                                Primer_Movimiento.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                Primer_Movimiento.debe = Primer_Movimiento.hasta - Primer_Movimiento.desde + 1;
                                Primer_Movimiento.haber = 0;
                                Primer_Movimiento.id_ticket_tipo_movimiento = 3;

                                db.Tickets_Movimiento.Add(Primer_Movimiento);

                                /*TICKETS INVENTARIO - PRIMERO*/
                                var Temp_T1 = db.Tickets_Inventario.Where(x => x.id_usuario == Primer_Movimiento.id_usuario).Where(y => y.id_ticket_tipo == Primer_Movimiento.id_ticket_tipo).ToList().FirstOrDefault();
                                Temp_T1.existencia -= Primer_Movimiento.debe;
                                Temp_T1.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                Temp_T1.fecha_modificacion = DateTime.Now;
                                db.Entry(Temp_T1).State = EntityState.Modified;

                                db.SaveChanges();
                                return RedirectToAction("MovTicketEmpleado");
                            }
                            else
                                ModelState.AddModelError("Desde_Mayor", "El area especificada como \"desde\" debe ser menor que \"hasta\"");

                        }
                        else
                            ModelState.AddModelError("Traslado", "El usuario ya traslado esos tickets");
                    }
                    else
                        ModelState.AddModelError("Sin_Tickets", "El usuario no contiene ninguno de esos tickets");
                }
                else
                    ModelState.AddModelError("Sin_Existencia", "El encargado no cuenta con la cantidad Existente");
            }

            ViewBag.id_ticket_tipo = new SelectList(db.Tickets_Tipo.Where(p => p.activo && !p.eliminado), "id_ticket_tipo", "nombre", Primer_Movimiento.id_ticket_tipo);
            ViewBag.id_empleado = db.Empleado.Where(p => p.activo && !p.eliminado)
                    .Select(p => new SelectListItem
                    {
                        Text = p.id_empleado + " - " + p.primer_nombre + " " + p.primer_apellido,
                        Value = p.id_empleado.ToString()
                    });
            return View(Primer_Movimiento);
        }

        // GET: Tickets/Tickets_Movimiento/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Movimiento tickets_Movimiento = db.Tickets_Movimiento.Find(id);
            if (tickets_Movimiento == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", tickets_Movimiento.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", tickets_Movimiento.id_usuario_eliminacion);
            ViewBag.id_usuario = new SelectList(db.Usuarios, "id_usuario", "email", tickets_Movimiento.id_usuario);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", tickets_Movimiento.id_usuario_modificacion);
            ViewBag.id_empleado = new SelectList(db.Empleado, "id_empleado", "primer_nombre", tickets_Movimiento.id_empleado);
            ViewBag.id_ticket_tipo = new SelectList(db.Tickets_Tipo.Where(x=>x.activo && !x.eliminado && x.valor==10), "id_ticket_tipo", "nombre", tickets_Movimiento.id_ticket_tipo);
            ViewBag.id_ticket_tipo_movimiento = new SelectList(db.Tickets_Tipo_Movimiento.Where(x=>x.activo && !x.eliminado && x.id_ticket_tipo_movimiento == tickets_Movimiento.id_ticket_tipo_movimiento), "id_ticket_tipo_movimiento", "nombre", tickets_Movimiento.id_ticket_tipo_movimiento);
            return View(tickets_Movimiento);
        }

        // POST: Tickets/Tickets_Movimiento/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tickets_Movimiento tickets_Movimiento)
        {
            Tickets_Movimiento tMovEdit = db.Tickets_Movimiento.Find(tickets_Movimiento.id_ticket_movimiento);
            if (ModelState.IsValid)
            {
                db.Entry(tickets_Movimiento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", tickets_Movimiento.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", tickets_Movimiento.id_usuario_eliminacion);
            ViewBag.id_usuario = new SelectList(db.Usuarios, "id_usuario", "email", tickets_Movimiento.id_usuario);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", tickets_Movimiento.id_usuario_modificacion);
            ViewBag.id_empleado = new SelectList(db.Empleado, "id_empleado", "primer_nombre", tickets_Movimiento.id_empleado);
            ViewBag.id_ticket_tipo = new SelectList(db.Tickets_Tipo, "id_ticket_tipo", "nombre", tickets_Movimiento.id_ticket_tipo);
            ViewBag.id_ticket_tipo_movimiento = new SelectList(db.Tickets_Tipo_Movimiento, "id_ticket_tipo_movimiento", "nombre", tickets_Movimiento.id_ticket_tipo_movimiento);
            return View(tickets_Movimiento);
        }

        // GET: Tickets/Tickets_Movimiento/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Movimiento tickets_Movimiento = db.Tickets_Movimiento.Find(id);
            if (tickets_Movimiento == null)
            {
                return HttpNotFound();
            }
            return View(tickets_Movimiento);
        }

        // POST: Tickets/Tickets_Movimiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int id2 = id + 1;
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Tickets_Movimiento tickets_Movimiento1 = db.Tickets_Movimiento.Find(id);
            Tickets_Inventario ticketInventarioEncargadoUpdate = db.Tickets_Inventario.Where(x => x.activo && !x.eliminado && x.id_usuario == tickets_Movimiento1.id_usuario).SingleOrDefault();
            if (tickets_Movimiento1.id_empleado == null)
            {
                Tickets_Movimiento tickets_Movimiento2 = db.Tickets_Movimiento.Find(id2);
                //Tickets_Inventario ticketInventarioEncargadoUpdate = db.Tickets_Inventario.Where(x => x.activo && !x.eliminado && x.id_usuario == tickets_Movimiento1.id_usuario).SingleOrDefault();
                Tickets_Inventario ticketInventarioUpdate = db.Tickets_Inventario.Where(x => x.activo && !x.eliminado && x.id_usuario == tickets_Movimiento2.id_usuario).SingleOrDefault();

                tickets_Movimiento1.activo = false;
                tickets_Movimiento1.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                tickets_Movimiento1.fecha_eliminacion = DateTime.Now;
                tickets_Movimiento1.eliminado = true;
                db.Entry(tickets_Movimiento1).State = EntityState.Modified;

                tickets_Movimiento2.activo = false;
                tickets_Movimiento2.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                tickets_Movimiento2.fecha_eliminacion = DateTime.Now;
                tickets_Movimiento2.eliminado = true;
                db.Entry(tickets_Movimiento2).State = EntityState.Modified;

                ticketInventarioEncargadoUpdate.id_ticket_tipo = ticketInventarioEncargadoUpdate.id_ticket_tipo;
                ticketInventarioEncargadoUpdate.id_usuario = ticketInventarioEncargadoUpdate.id_usuario;
                ticketInventarioEncargadoUpdate.existencia = ticketInventarioEncargadoUpdate.existencia + tickets_Movimiento1.debe;
                ticketInventarioEncargadoUpdate.activo = true;
                ticketInventarioEncargadoUpdate.eliminado = false;
                ticketInventarioEncargadoUpdate.id_usuario_creacion = ticketInventarioEncargadoUpdate.id_usuario_creacion;
                ticketInventarioEncargadoUpdate.id_usuario_modificacion = ticketInventarioEncargadoUpdate.id_usuario_modificacion;
                ticketInventarioEncargadoUpdate.fecha_creacion = ticketInventarioEncargadoUpdate.fecha_creacion;
                ticketInventarioEncargadoUpdate.fecha_modificacion = DateTime.Now;
                db.Entry(ticketInventarioEncargadoUpdate).State = EntityState.Modified;

                ticketInventarioUpdate.id_ticket_tipo = ticketInventarioUpdate.id_ticket_tipo;
                ticketInventarioUpdate.id_usuario = ticketInventarioUpdate.id_usuario;
                ticketInventarioUpdate.existencia = ticketInventarioUpdate.existencia - tickets_Movimiento2.haber;
                ticketInventarioUpdate.activo = true;
                ticketInventarioUpdate.eliminado = false;
                ticketInventarioUpdate.id_usuario_creacion = ticketInventarioUpdate.id_usuario_creacion;
                ticketInventarioUpdate.id_usuario_modificacion = ticketInventarioUpdate.id_usuario_modificacion;
                ticketInventarioUpdate.fecha_creacion = ticketInventarioUpdate.fecha_creacion;
                ticketInventarioUpdate.fecha_modificacion = DateTime.Now;
                db.Entry(ticketInventarioUpdate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else {
                tickets_Movimiento1.activo = false;
                tickets_Movimiento1.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                tickets_Movimiento1.fecha_eliminacion = DateTime.Now;
                tickets_Movimiento1.eliminado = true;
                db.Entry(tickets_Movimiento1).State = EntityState.Modified;

                ticketInventarioEncargadoUpdate.id_ticket_tipo = ticketInventarioEncargadoUpdate.id_ticket_tipo;
                ticketInventarioEncargadoUpdate.id_usuario = ticketInventarioEncargadoUpdate.id_usuario;
                ticketInventarioEncargadoUpdate.existencia = ticketInventarioEncargadoUpdate.existencia + tickets_Movimiento1.debe;
                ticketInventarioEncargadoUpdate.activo = true;
                ticketInventarioEncargadoUpdate.eliminado = false;
                ticketInventarioEncargadoUpdate.id_usuario_creacion = ticketInventarioEncargadoUpdate.id_usuario_creacion;
                ticketInventarioEncargadoUpdate.id_usuario_modificacion = ticketInventarioEncargadoUpdate.id_usuario_modificacion;
                ticketInventarioEncargadoUpdate.fecha_creacion = ticketInventarioEncargadoUpdate.fecha_creacion;
                ticketInventarioEncargadoUpdate.fecha_modificacion = DateTime.Now;
                db.Entry(ticketInventarioEncargadoUpdate).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("MovTicketEmpleado");
            }
        }
        public FileStreamResult GetConstanciaTickets(int id)
        {

            Tickets_Movimiento ticket_movimiento = db.Tickets_Movimiento.SingleOrDefault(v => v.id_ticket_movimiento == id && !v.eliminado);
            if (ticket_movimiento == null)
            {
                return null;
            }
            string parametros = "&id_ticket_movimiento=" + id;
            string reporte = "rpt_Constancia_Entrega_Tickets";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Movimiento de Ticket - " + ticket_movimiento.id_ticket_movimiento +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult GetTicketsMovimientoEncargado(string initial_date, string final_date)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            int usuario = usuarioTO.usuario.id_usuario;
            //List<Tickets_Movimiento> tMovimientoEncargado = db.Tickets_Movimiento.Where(x => x.fecha_creacion >= initial_date && x.fecha_creacion <= final_date).ToList();
            //Tickets_Movimiento ticket_movimiento = db.Tickets_Movimiento.SingleOrDefault(v => v.id_ticket_movimiento == id && !v.eliminado);
            //if (tMovimientoEncargado.Count() < 0)
            //{
            //    return null;
            //}
            string parametros = "&usuario=" + usuario + "&initial_date=" + initial_date + "&final_date=" + final_date;
            string reporte = "rpt_Movimientos_Tickets_Encargado";
            //string parametros = "&id_ticket_movimiento=" + 6;
            //string reporte = "rpt_Constancia_Entrega_Tickets";
            Excel_Protal archivo_reporte = new Excel_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();

            //Response.Clear();
            Response.ContentType = "application/xls";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Movimientos de Tickets - Encargado.xls\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/xls");
        }

        public FileStreamResult GetTicketsMovimientoEncargadoEmpleado(string initial_date, string final_date)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            int usuario = usuarioTO.usuario.id_usuario;
            //List<Tickets_Movimiento> tMovimientoEncargado = db.Tickets_Movimiento.Where(x => x.fecha_creacion >= initial_date && x.fecha_creacion <= final_date).ToList();
            //Tickets_Movimiento ticket_movimiento = db.Tickets_Movimiento.SingleOrDefault(v => v.id_ticket_movimiento == id && !v.eliminado);
            //if (tMovimientoEncargado.Count() < 0)
            //{
            //    return null;
            //}
            string parametros = "&usuario=" + usuario + "&initial_date=" + initial_date + "&final_date=" + final_date;
            string reporte = "rpt_Movimientos_Tickets_Encargado_Empleado";
            //string parametros = "&id_ticket_movimiento=" + 6;
            //string reporte = "rpt_Constancia_Entrega_Tickets";
            Excel_Protal archivo_reporte = new Excel_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();

            //Response.Clear();
            Response.ContentType = "application/xls";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Movimientos de Tickets - Encargado - Empleado.xls\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/xls");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        private String Nombre_Completo(List<String> Nombres)
        {
            String Nombre_Retorno = "";
            for (int i = 0; i < Nombres.Count; i++)
            {
                if (i == Nombres.Count - 1)
                {
                    if (Nombres[i] != null)
                    {
                        Nombre_Retorno += Nombres[i];
                    }
                }
                else
                {
                    if (Nombres[i] != null)
                    {
                        Nombre_Retorno += Nombres[i] + " ";
                    }
                }

            }
            return Nombre_Retorno;
        }
    }
}
