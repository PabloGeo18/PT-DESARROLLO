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
using System.Transactions;
using System.Data.SqlClient;
using MVC2013.Src.Sdc.Reports; 

namespace MVC2013.Areas.Inventario.Controllers
{
    public class IngresosController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Ingresos
        public ActionResult Index()
        {
            List<Ingresos> ingresos = db.Ingresos.Where(x=>x.activo && !x.eliminado).OrderByDescending(x=>x.fecha_creacion).ToList(); //.Include(i => i.Usuarios).Include(i => i.Usuarios1).Include(i => i.Usuarios2).Include(i => i.Bodegas).Include(i => i.Ingreso_Estado).Where(i=> i.activo).Where(i=> !i.eliminado).OrderByDescending(i=>i.fecha_creacion);
            return View(ingresos);
        }
        public ActionResult Index_Autorizar()
        {
            var ingresos = db.Ingresos.Include(i => i.Usuarios).Include(i => i.Usuarios1).Include(i => i.Usuarios2).Include(i => i.Bodegas).Include(i => i.Ingreso_Estado).Where(i => i.activo).Where(i => !i.eliminado).Where(i => i.id_ingreso_estado==4);
            return View(ingresos.ToList());
        }
        // GET: Inventario/Ingresos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingresos ingresos = db.Ingresos.Find(id);
            if (ingresos == null)
            {
                return HttpNotFound();
            }
            return View(ingresos);
        }
        public ActionResult Details_Autorizar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingresos ingresos = db.Ingresos.Find(id);
            if (ingresos == null)
            {
                return HttpNotFound();
            }
            return View(ingresos);
        }

        // GET: Inventario/Ingresos/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado && x.id_bodega==1), "id_bodega", "descripcion");
            ViewBag.id_ingreso_estado = new SelectList(db.Ingreso_Estado.Where(x => x.activo && !x.eliminado), "id_ingreso_estado", "descripcion");
            return View();
        }

        // POST: Inventario/Ingresos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ingresos ingresos, DateTime fecha_factura_datetime)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                ingresos.id_ingreso_estado = 1;
                ingresos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                ingresos.fecha_creacion = DateTime.Now;
                ingresos.fecha_factura = fecha_factura_datetime;
                ingresos.activo = true;
                ingresos.eliminado = false;
                db.Ingresos.Add(ingresos);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = ingresos.id_ingreso });
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", ingresos.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", ingresos.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", ingresos.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas, "id_bodega", "descripcion", ingresos.id_bodega);
            ViewBag.id_ingreso_estado = new SelectList(db.Ingreso_Estado, "id_ingreso_estado", "descripcion", ingresos.id_ingreso_estado);
            return View(ingresos);
        }

        // GET: Inventario/Ingresos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingresos ingresos = db.Ingresos.Find(id);
            if (ingresos == null)
            {
                return HttpNotFound();
            }
            ViewBag.ingreso = ingresos.id_ingreso;
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", ingresos.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", ingresos.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", ingresos.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado), "id_bodega", "descripcion", ingresos.id_bodega);
            ViewBag.id_ingreso_estado = new SelectList(db.Ingreso_Estado.Where(x => x.activo && !x.eliminado), "id_ingreso_estado", "descripcion", ingresos.id_ingreso_estado);
            return View(ingresos);
        }

        // POST: Inventario/Ingresos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ingresos ingresos, DateTime fecha_factura_datetime)
        {
            if (ModelState.IsValid)
            {
                Ingresos ingresosEdit = db.Ingresos.Find(ingresos.id_ingreso);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                ingresosEdit.referencia = ingresos.referencia;
                ingresosEdit.fecha_factura = fecha_factura_datetime;
                ingresosEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                ingresosEdit.fecha_modificacion = DateTime.Now;
                ingresosEdit.activo = true;
                ingresosEdit.eliminado = false;
                db.Entry(ingresosEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", ingresos.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", ingresos.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", ingresos.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas, "id_bodega", "descripcion", ingresos.id_bodega);
            ViewBag.id_ingreso_estado = new SelectList(db.Ingreso_Estado, "id_ingreso_estado", "descripcion", ingresos.id_ingreso_estado);
            return View(ingresos);
        }

        // GET: Inventario/Ingresos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingresos ingresos = db.Ingresos.Find(id);
            if (ingresos == null)
            {
                return HttpNotFound();
            }
            return View(ingresos);
        }

        // POST: Inventario/Ingresos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ingresos ingresos = db.Ingresos.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            ingresos.id_ingreso_estado = 3;
            ingresos.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            ingresos.fecha_eliminacion = DateTime.Now;
            ingresos.eliminado = true;
            ingresos.activo = false;
            db.Entry(ingresos).State = EntityState.Modified;
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

        // GET: Inventario/Ingresos/add_arma/5
        public ActionResult add_arma(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingresos ingresos = db.Ingresos.Find(id);
            
            if (ingresos == null)
            {
                return HttpNotFound();
            }
            int r = Convert.ToInt32(id);
            ViewBag.id_ingreso = db.Ingresos.Find(id).id_ingreso;
            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado), "id_cliente", "nombre");
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_arma_estado = new SelectList(db.Arma_Estado.Where(x => x.activo && !x.eliminado), "id_arma_estado", "descripcion");
            ViewBag.id_arma_tipo = new SelectList(db.Arma_Tipo.Where(x => x.activo && !x.eliminado), "id_arma_tipo", "descripcion");
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado), "id_bodega", "descripcion");
            ViewBag.id_calibre = new SelectList(db.Calibres.Where(x => x.activo && !x.eliminado), "id_calibre", "descripcion");
            ViewBag.id_marca = new SelectList(db.Marca_Tipo.Where(x => x.activo && !x.eliminado && x.id_inventario_tipo==1).Select(x=> x.Marcas), "id_marca", "descripcion");
            ViewBag.id_modelo = new SelectList(db.Modelos.Where(x => x.activo && !x.eliminado), "id_modelo", "descripcion");
            ViewBag.id_proveedor = new SelectList(db.Proveedores.Where(x => x.activo && !x.eliminado), "id_proveedor", "descripcion"); 
            return View();
        }

        // POST: Inventario/Armas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult add_arma(int id, Armas armas, DateTime fecha_vencimiento_datetime)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Ingresos ingreso = db.Ingresos.Find(id);
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        armas.id_arma_estado = 1;
                        armas.id_arma_estado_policia = 1;
                        armas.id_bodega = ingreso.id_bodega;
                        armas.id_cliente = null;
                        armas.fecha_vencimiento = fecha_vencimiento_datetime;
                        armas.eliminado = false;
                        armas.autorizada = false;
                        armas.trans_autorizada = false;
                        armas.comprometida = false;
                        armas.activo = true;
                        armas.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        armas.fecha_creacion = DateTime.Now;
                        db.Armas.Add(armas);
                        db.SaveChanges();

                        Ingreso_Detalle indet = new Ingreso_Detalle();
                        indet.id_ingreso = id;
                        indet.id_arma = armas.id_arma;
                        indet.cantidad = 1;
                        indet.valor = armas.valor;
                        indet.activo = true;
                        indet.eliminado = false;
                        indet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        indet.fecha_creacion = DateTime.Now;
                        db.Ingreso_Detalle.Add(indet);
                        
                        
                        db.SaveChanges();
                        Ingresos ingresos = db.Ingresos.Find(id);
                        ingresos.id_ingreso_estado = 2;
                        ingresos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        ingresos.fecha_modificacion = DateTime.Now;
                        db.Entry(ingresos).State = EntityState.Modified;

                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id });
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id });
                    }
                }
            }

            return View(armas);
        }

        // GET: Inventario/Ingresos/add_arma/5
        public ActionResult add_articulo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingresos ingresos = db.Ingresos.Find(id);

            if (ingresos == null)
            {
                return HttpNotFound();
            }
            int r = Convert.ToInt32(id);
            ViewBag.id_ingreso = db.Ingresos.Find(id).id_ingreso;
            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.activo && !x.eliminado), "id_cliente", "nombre");
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_articulo_tipo = new SelectList(db.Articulo_Tipo.Where(x => x.activo && !x.eliminado), "id_articulo_tipo", "descripcion");
            ViewBag.id_bodega = new SelectList(db.Bodegas.Where(x => x.activo && !x.eliminado), "id_bodega", "descripcion");
            ViewBag.id_marca = new SelectList(db.Marca_Tipo.Where(x => x.activo && !x.eliminado && x.id_inventario_tipo == 2).Select(x => x.Marcas), "id_marca", "descripcion");
            ViewBag.id_proveedor = new SelectList(db.Proveedores.Where(x => x.activo && !x.eliminado), "id_proveedor", "descripcion");
            return View();
        }

        // POST: Inventario/Armas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult add_articulo(int id, [Bind(Include = "id_articulo,id_articulo_tipo,serie,id_marca,id_proveedor,costo,costo_venta,id_cliente,id_bodega,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Articulos articulos)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Ingresos ingreso = db.Ingresos.Find(id);
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        articulos.id_bodega = ingreso.id_bodega;
                        articulos.id_cliente = null;
                        articulos.eliminado = false;
                        articulos.activo = false;
                        articulos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        articulos.fecha_creacion = DateTime.Now;
                        db.Articulos.Add(articulos);

                        
                        Ingreso_Detalle indet = new Ingreso_Detalle();
                        indet.id_ingreso = id;
                        indet.id_articulo = articulos.id_articulo;
                        indet.cantidad = 1;
                        indet.valor = articulos.costo;
                        indet.activo = true;
                        indet.eliminado = false;
                        indet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        indet.fecha_creacion = DateTime.Now;
                        db.Ingreso_Detalle.Add(indet);
                        
                        db.SaveChanges();
                        Ingresos ingresos = db.Ingresos.Find(id);
                        ingresos.id_ingreso_estado = 2;
                        ingresos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        ingresos.fecha_modificacion = DateTime.Now;
                        db.Entry(ingresos).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id });
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id });
                    }
                }
            }

            return View(articulos);
        }


        // GET: Inventario/Ingresos/add_arma/5
        public ActionResult add_municiones(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingresos ingresos = db.Ingresos.Find(id);

            if (ingresos == null)
            {
                return HttpNotFound();
            }
            int r = Convert.ToInt32(id);
            ViewBag.id_ingreso = db.Ingresos.Find(id).id_ingreso;
            var municiones = db.Municiones.Include(m => m.Usuarios).Include(m => m.Usuarios1).Include(m => m.Usuarios2).Include(m => m.Bodega_Inventario_Municiones).Include(m => m.Calibres).Where(m => m.activo).Where(m=> m.eliminado==false).OrderBy(x=>x.id_municion);
            return View(municiones.ToList());
        }

        // GET: Inventario/Ingresos/add_arma/5
        public ActionResult add_municiones_new(int? id)
        {
            
            int r = Convert.ToInt32(id);
            ViewBag.id_ingreso = db.Ingresos.Find(id).id_ingreso;
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_municion = new SelectList(db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado), "id_municion", "id_municion");
            ViewBag.id_calibre = new SelectList(db.Calibres.Where(x => x.activo && !x.eliminado), "id_calibre", "descripcion");
            return View();
        }

        // POST: Inventario/Municiones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult add_municiones_new(int id, Municiones municiones)
        {

            int r = Convert.ToInt32(id);
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        municiones.eliminado = false;
                        municiones.activo = true;
                        municiones.autorizada = false;
                        municiones.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        municiones.fecha_creacion = DateTime.Now;
                        db.Municiones.Add(municiones);
                        
                        Ingreso_Detalle indet = new Ingreso_Detalle();
                        indet.id_ingreso = id;
                        indet.id_municion = municiones.id_municion;
                        indet.cantidad = municiones.existencia;
                        indet.valor = municiones.costo;
                        indet.activo = true;
                        indet.eliminado = false;
                        indet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        indet.fecha_creacion = DateTime.Now;
                        db.Ingreso_Detalle.Add(indet);

                        Ingresos ingresos = db.Ingresos.Find(id);
                        ingresos.id_ingreso_estado = 2;
                        ingresos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        ingresos.fecha_modificacion = DateTime.Now;
                        db.Entry(ingresos).State = EntityState.Modified;
                        db.SaveChanges();

                        Bodega_Inventario_Municiones bim = new Bodega_Inventario_Municiones();
                        bim.id_municion = municiones.id_municion;
                        bim.id_bodega = ingresos.id_bodega;
                        bim.comprometido = 0;
                        bim.existencia = 0;
                        bim.activo = true;
                        bim.eliminado = false;
                        bim.autorizada = false;
                        bim.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        bim.fecha_creacion = DateTime.Now;
                        db.Bodega_Inventario_Municiones.Add(bim);
                        db.SaveChanges();

                        municiones.existencia = 0;
                        db.Entry(municiones).State = EntityState.Modified;

                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id });
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id });
                    }
                }
                
            }
            return View(municiones);
        }

        // GET: Inventario/Ingresos/add_arma/5
        public ActionResult add_municiones_existencia(int? id_municion, int? id_ingreso)
        {
            if (id_municion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Municiones municiones = db.Municiones.Find(id_municion);
            if (municiones == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_ingreso = db.Ingresos.Find(id_ingreso).id_ingreso;
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", municiones.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", municiones.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", municiones.id_usuario_modificacion);
            ViewBag.id_municion = new SelectList(db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado), "id_municion", "id_municion", municiones.id_municion);
            ViewBag.id_calibre = new SelectList(db.Calibres.Where(x => x.activo && !x.eliminado), "id_calibre", "descripcion", municiones.id_calibre);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult add_municiones_existencia(int id_municion, int id_ingreso, [Bind(Include = "id_municion,id_calibre,descripcion,costo,costo_venta,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Municiones municiones)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Municiones municionesEdit = db.Municiones.Find(municiones.id_municion);
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        
                        Ingreso_Detalle indet = new Ingreso_Detalle();
                        indet.id_ingreso = id_ingreso;
                        indet.id_municion = municiones.id_municion;
                        indet.cantidad = municiones.existencia;
                        indet.valor = municionesEdit.costo;
                        indet.activo = true;
                        indet.eliminado = false;
                        indet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        indet.fecha_creacion = DateTime.Now;
                        db.Ingreso_Detalle.Add(indet);

                        Ingresos ingresos = db.Ingresos.Find(id_ingreso);
                        ingresos.id_ingreso_estado = 2;
                        ingresos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        ingresos.fecha_modificacion = DateTime.Now;
                        db.Entry(ingresos).State = EntityState.Modified;
                        db.SaveChanges();

                        var bim_original = db.Bodega_Inventario_Municiones.Where(x => x.id_bodega == ingresos.id_bodega).Where(y => y.id_municion == id_municion).Where(x=>x.activo).Where(x=>x.eliminado==false);

                        if (bim_original.Count() == 0)
                        {
                            Bodega_Inventario_Municiones bim = new Bodega_Inventario_Municiones();
                            bim.id_municion = municiones.id_municion;
                            bim.id_bodega = ingresos.id_bodega;
                            bim.existencia = 0;
                            bim.activo = true;
                            bim.eliminado = false;
                            bim.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                            bim.fecha_creacion = DateTime.Now;
                            db.Bodega_Inventario_Municiones.Add(bim);
                            db.SaveChanges();
                        }

                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_ingreso });
                        
                        
                        
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_ingreso });
                    }
                }

            }
            return View(municiones);
        }


        // GET: Inventario/Ingresos/add_arma/5
        public ActionResult add_uniformes(int? id_ingreso)
        {
            if (id_ingreso == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingresos ingresos = db.Ingresos.Find(id_ingreso);

            if (ingresos == null)
            {
                return HttpNotFound();
            }
            int r = Convert.ToInt32(id_ingreso);
            ViewBag.id_ingreso = db.Ingresos.Find(id_ingreso).id_ingreso;
            var uniformes = db.Uniformes.Include(u => u.Usuarios).Include(u => u.Usuarios1).Include(u => u.Usuarios2).Include(u => u.Bodega_Inventario_Uniformes).Include(u => u.Uniforme_Tipo).Where(u => u.activo).Where(u => u.eliminado == false);
            return View(uniformes.ToList());
        }

        // GET: Inventario/Ingresos/add_arma/5
        public ActionResult add_uniformes_new(int? id_ingreso)
        {

            int r = Convert.ToInt32(id_ingreso);
            ViewBag.id_ingreso = db.Ingresos.Find(id_ingreso).id_ingreso;
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_uniforme = new SelectList(db.Bodega_Inventario_Uniformes.Where(x => x.activo && !x.eliminado), "id_uniforme", "id_uniforme");
            ViewBag.id_uniforme_tipo = new SelectList(db.Uniforme_Tipo.Where(x => x.activo && !x.eliminado), "id_uniforme_tipo", "descripcion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult add_uniformes_new(int id_ingreso, Uniformes uniformes)
        {

            int r = Convert.ToInt32(id_ingreso);
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        uniformes.eliminado = false;
                        uniformes.activo = true;
                        uniformes.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        uniformes.fecha_creacion = DateTime.Now;
                        db.Uniformes.Add(uniformes);

                        

                        Ingreso_Detalle indet = new Ingreso_Detalle();
                        indet.id_ingreso = id_ingreso;
                        indet.id_uniforme = uniformes.id_uniforme;
                        indet.cantidad = uniformes.existencia;
                        indet.valor = uniformes.costo;
                        indet.activo = true;
                        indet.eliminado = false;
                        indet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        indet.fecha_creacion = DateTime.Now;
                        db.Ingreso_Detalle.Add(indet);

                        Ingresos ingresos = db.Ingresos.Find(id_ingreso);
                        ingresos.id_ingreso_estado = 2;
                        ingresos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        ingresos.fecha_modificacion = DateTime.Now;
                        db.Entry(ingresos).State = EntityState.Modified;
                        db.SaveChanges();

                        Bodega_Inventario_Uniformes biu = new Bodega_Inventario_Uniformes();
                        biu.id_uniforme = uniformes.id_uniforme;
                        biu.id_bodega = ingresos.id_bodega;
                        biu.existencia = 0;
                        biu.activo = true;
                        biu.eliminado = false;
                        biu.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        biu.fecha_creacion = DateTime.Now;
                        db.Bodega_Inventario_Uniformes.Add(biu);

                        uniformes.existencia = 0;
                        db.Entry(uniformes).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_ingreso });
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_ingreso });
                    }
                }

            }
            return View(uniformes);
        }

        // GET: Inventario/Ingresos/add_arma/5
        public ActionResult add_uniformes_existencia(int? id_uniforme, int? id_ingreso)
        {
            if (id_uniforme == null || id_ingreso == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Uniformes uniformes = db.Uniformes.Find(id_uniforme);
            if (uniformes == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_ingreso = db.Ingresos.Find(id_ingreso).id_ingreso;
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", uniformes.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", uniformes.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", uniformes.id_usuario_modificacion);
            ViewBag.id_uniforme = new SelectList(db.Bodega_Inventario_Uniformes.Where(x => x.activo && !x.eliminado), "id_uniforme", "id_uniforme", uniformes.id_uniforme);
            ViewBag.id_uniforme_tipo = new SelectList(db.Uniforme_Tipo.Where(x => x.activo && !x.eliminado), "id_uniforme_tipo", "descripcion", uniformes.id_uniforme_tipo);
            return View(uniformes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult add_uniformes_existencia(int id_uniforme, int id_ingreso, [Bind(Include = "id_uniforme,id_uniforme_tipo,descripcion,talla,costo,costo_venta,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Uniformes uniformes)
        {

            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Uniformes uniformesEdit = db.Uniformes.Find(uniformes.id_uniforme);
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        
                        Ingreso_Detalle indet = new Ingreso_Detalle();
                        indet.id_ingreso = id_ingreso;
                        indet.id_uniforme = uniformes.id_uniforme;
                        indet.cantidad = uniformes.existencia;
                        indet.valor = uniformesEdit.costo;
                        indet.activo = true;
                        indet.eliminado = false;
                        indet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        indet.fecha_creacion = DateTime.Now;
                        db.Ingreso_Detalle.Add(indet);

                        Ingresos ingresos = db.Ingresos.Find(id_ingreso);
                        ingresos.id_ingreso_estado = 2;
                        ingresos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        ingresos.fecha_modificacion = DateTime.Now;
                        db.Entry(ingresos).State = EntityState.Modified;
                        db.SaveChanges();

                        var biu_original = db.Bodega_Inventario_Uniformes.Where(x => x.id_bodega == ingresos.id_bodega).Where(y => y.id_uniforme == id_uniforme).Where(x => x.activo).Where(x => x.eliminado == false);

                        if (biu_original.Count() == 0)
                        {
                            Bodega_Inventario_Uniformes biu = new Bodega_Inventario_Uniformes();
                            biu.id_uniforme = uniformes.id_uniforme;
                            biu.id_bodega = ingresos.id_bodega;
                            biu.existencia = 0;
                            biu.activo = true;
                            biu.eliminado = false;
                            biu.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                            biu.fecha_creacion = DateTime.Now;
                            db.Bodega_Inventario_Uniformes.Add(biu);
                            db.SaveChanges();
                        }
                        
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_ingreso });



                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_ingreso });
                    }
                }

            }
            return View(uniformes);
        }

        // GET: Inventario/Ingresos/add_arma/5
        public ActionResult add_consumibles(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingresos ingresos = db.Ingresos.Find(id);

            if (ingresos == null)
            {
                return HttpNotFound();
            }
            int r = Convert.ToInt32(id);
            ViewBag.id_ingreso = db.Ingresos.Find(id).id_ingreso;
            var consumibles = db.Consumibles.Include(c => c.Usuarios).Include(c => c.Usuarios1).Include(c => c.Usuarios2).Include(c => c.Bodega_Inventario_Consumibles).Include(c => c.Consumible_Tipo).Where(c=> c.activo).Where(c=> c.eliminado==false);
            return View(consumibles.ToList());
        }

        // GET: Inventario/Ingresos/add_arma/5
        public ActionResult add_consumibles_new(int? id)
        {

            int r = Convert.ToInt32(id);
            ViewBag.id_ingreso = db.Ingresos.Find(id).id_ingreso;
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_consumible = new SelectList(db.Bodega_Inventario_Consumibles.Where(x => x.activo && !x.eliminado), "id_consumible", "id_consumible");
            ViewBag.id_consumible_tipo = new SelectList(db.Consumible_Tipo.Where(x => x.activo && !x.eliminado), "id_consumible_tipo", "descripcion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult add_consumibles_new(int id, [Bind(Include = "id_consumible,descripcion,costo,costo_venta,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_consumible_tipo")] Consumibles consumibles)
        {

            int r = Convert.ToInt32(id);
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        consumibles.eliminado = false;
                        consumibles.activo = true;
                        consumibles.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        consumibles.fecha_creacion = DateTime.Now;
                        db.Consumibles.Add(consumibles);

                        Ingreso_Detalle indet = new Ingreso_Detalle();
                        indet.id_ingreso = id;
                        indet.id_consumible = consumibles.id_consumible;
                        indet.cantidad = consumibles.existencia;
                        indet.valor = consumibles.costo;
                        indet.activo = true;
                        indet.eliminado = false;
                        indet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        indet.fecha_creacion = DateTime.Now;
                        db.Ingreso_Detalle.Add(indet);

                        Ingresos ingresos = db.Ingresos.Find(id);
                        ingresos.id_ingreso_estado = 2;
                        ingresos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        ingresos.fecha_modificacion = DateTime.Now;
                        db.Entry(ingresos).State = EntityState.Modified;
                        db.SaveChanges();

                        Bodega_Inventario_Consumibles bic = new Bodega_Inventario_Consumibles();
                        bic.id_consumible = consumibles.id_consumible;
                        bic.id_bodega = ingresos.id_bodega;
                        bic.existencia = 0;
                        bic.activo = true;
                        bic.eliminado = false;
                        bic.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        bic.fecha_creacion = DateTime.Now;
                        db.Bodega_Inventario_Consumibles.Add(bic);
                        consumibles.existencia = 0;
                        db.Entry(consumibles).State = EntityState.Modified;
                        
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id });
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id });
                    }
                }

            }
            return View(consumibles);
        }

        // GET: Inventario/Ingresos/add_arma/5
        public ActionResult add_consumibles_existencia(int? id_consumible, int? id_ingreso)
        {
            if (id_consumible == null || id_ingreso == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consumibles consumibles = db.Consumibles.Find(id_consumible);
            if (consumibles == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_ingreso = db.Ingresos.Find(id_ingreso).id_ingreso;
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", consumibles.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", consumibles.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", consumibles.id_usuario_modificacion);
            ViewBag.id_consumible = new SelectList(db.Bodega_Inventario_Consumibles.Where(x => x.activo && !x.eliminado), "id_consumible", "id_consumible", consumibles.id_consumible);
            ViewBag.id_consumible_tipo = new SelectList(db.Consumible_Tipo.Where(x => x.activo && !x.eliminado), "id_consumible_tipo", "descripcion", consumibles.id_consumible_tipo);
            return View(consumibles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult add_consumibles_existencia(int id_consumible, int id_ingreso, [Bind(Include = "id_consumible,descripcion,costo,costo_venta,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_consumible_tipo")] Consumibles consumibles)
        {

            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Consumibles consumiblesEdit = db.Consumibles.Find(consumibles.id_consumible);
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        
                        Ingreso_Detalle indet = new Ingreso_Detalle();
                        indet.id_ingreso = id_ingreso;
                        indet.id_consumible = consumibles.id_consumible;
                        indet.cantidad = consumibles.existencia;
                        indet.valor = consumiblesEdit.costo;
                        indet.activo = true;
                        indet.eliminado = false;
                        indet.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        indet.fecha_creacion = DateTime.Now;
                        db.Ingreso_Detalle.Add(indet);

                        Ingresos ingresos = db.Ingresos.Find(id_ingreso);
                        ingresos.id_ingreso_estado = 2;
                        ingresos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        ingresos.fecha_modificacion = DateTime.Now;
                        db.Entry(ingresos).State = EntityState.Modified;
                        db.SaveChanges();

                        var bic_original = db.Bodega_Inventario_Consumibles.Where(x => x.id_bodega == ingresos.id_bodega).Where(y => y.id_consumible == id_consumible).Where(x => x.activo).Where(x => x.eliminado == false);

                        if (bic_original.Count() == 0)
                        {
                            Bodega_Inventario_Consumibles bic = new Bodega_Inventario_Consumibles();
                            bic.id_consumible = consumibles.id_consumible;
                            bic.id_bodega = ingresos.id_bodega;
                            bic.existencia = 0;
                            bic.activo = true;
                            bic.eliminado = false;
                            bic.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                            bic.fecha_creacion = DateTime.Now;
                            db.Bodega_Inventario_Consumibles.Add(bic);
                            db.SaveChanges();
                        }

                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_ingreso });


                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_ingreso });
                    }
                }

            }
            return View(consumibles);
        }

        public ActionResult Reporte(int? id)
        {
            if (ModelState.IsValid)
            {
                PDF_Protal reporte = new PDF_Protal();

                return File("Test","Ingreso_"+id+DateTime.Now);
            }
            var ingresos = db.Ingresos.Include(i => i.Usuarios).Include(i => i.Usuarios1).Include(i => i.Usuarios2).Include(i => i.Bodegas).Include(i => i.Ingreso_Estado).Where(i => i.activo).Where(i => i.eliminado==false);
            return View(ingresos.ToList());
        }

        public ActionResult end_detalle(int? id)
        {
            if (ModelState.IsValid)
            {


                Ingresos ingresosEdit = db.Ingresos.Find(id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                ingresosEdit.id_ingreso_estado = 4;
                ingresosEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                ingresosEdit.fecha_modificacion = DateTime.Now;
                ingresosEdit.activo = true;
                ingresosEdit.eliminado = false;
                db.Entry(ingresosEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var ingresos = db.Ingresos.Include(i => i.Usuarios).Include(i => i.Usuarios1).Include(i => i.Usuarios2).Include(i => i.Bodegas).Include(i => i.Ingreso_Estado).Where(i => i.activo).Where(i => i.eliminado == false);
            return View(ingresos.ToList());
        }

        public ActionResult Aprobar(int? id)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Ingresos ingresosEdit = db.Ingresos.Find(id);
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                        ingresosEdit.id_ingreso_estado = 6;
                        ingresosEdit.id_usuario_autorizacion = usuarioTO.usuario.id_usuario;
                        ingresosEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        ingresosEdit.fecha_modificacion = DateTime.Now;
                        ingresosEdit.activo = true;
                        ingresosEdit.eliminado = false;
                        db.Entry(ingresosEdit).State = EntityState.Modified;

                        Transacciones_Inventario ti = new Transacciones_Inventario();
                        ti.activo = true;
                        ti.eliminado = false;
                        ti.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        ti.fecha_creacion = DateTime.Now;
                        db.Transacciones_Inventario.Add(ti);
                        //
                        db.SaveChanges();
                        foreach (Ingreso_Detalle ind_det in ingresosEdit.Ingreso_Detalle)
                        {
                            if (ind_det.activo && !ind_det.eliminado)
                            {
                                if (ind_det.id_arma != null)
                                {
                                    Armas arma = db.Armas.Single(x => !x.eliminado && x.activo && x.id_arma == ind_det.id_arma);
                                    Transaccion_Inventario_Detalle tid = new Transaccion_Inventario_Detalle();
                                    tid.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid.id_transaccion_inventario_tipo = 1;
                                    tid.cantidad = 1;
                                    tid.debe = arma.valor;
                                    tid.id_arma = arma.id_arma;
                                    tid.id_bodega = arma.id_bodega;
                                    tid.id_cliente = arma.id_cliente;

                                    tid.activo = true;
                                    tid.eliminado = false;
                                    tid.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid);
                                    arma.activo = true;
                                    arma.autorizada = true;
                                    db.Entry(arma).State = EntityState.Modified;

                                    //Creando registro en logs para el arma
                                    Logs_Movimientos_Arma lma = new Logs_Movimientos_Arma();
                                    lma.id_arma = arma.id_arma;
                                    lma.id_bodega_origen = ingresosEdit.id_bodega;
                                    lma.id_tipo_transaccion = Convert.ToInt32(Catalogos.InventarioTransacciones.Ingreso);
                                    lma.id_estado_transaccion = Convert.ToInt32(Catalogos.InventarioTransaccionEstado.Creado);
                                    lma.fecha_creacion = DateTime.Now;
                                    lma.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    lma.activo = true;
                                    lma.eliminado = false;
                                    db.Logs_Movimientos_Arma.Add(lma);

                                    db.SaveChanges();

                                }
                                if (ind_det.id_articulo != null)
                                {
                                    Articulos articulo = db.Articulos.Single(x => !x.eliminado && x.activo && x.id_articulo == ind_det.id_articulo);
                                    Transaccion_Inventario_Detalle tid = new Transaccion_Inventario_Detalle();
                                    tid.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid.id_transaccion_inventario_tipo = 1;
                                    tid.cantidad = 1;
                                    tid.debe = articulo.costo;
                                    tid.id_articulo = articulo.id_articulo;
                                    
                                    tid.id_bodega = articulo.id_bodega;
                                    tid.id_cliente = articulo.id_cliente;
                                    tid.activo = true;
                                    tid.eliminado = false;
                                    tid.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid);
                                    articulo.activo = true;
                                    db.Entry(articulo).State = EntityState.Modified;
                                    db.SaveChanges();

                                }
                                if (ind_det.id_consumible != null)
                                {
                                    Consumibles consumible = db.Consumibles.Single(x => !x.eliminado && x.activo && x.id_consumible == ind_det.id_consumible);
                                    Bodega_Inventario_Consumibles bic = db.Bodega_Inventario_Consumibles.Single(x => !x.eliminado && x.activo && x.id_consumible == ind_det.id_consumible && x.id_bodega == ingresosEdit.id_bodega);
                                    consumible.activo = true;
                                    consumible.existencia += Convert.ToInt32(ind_det.cantidad);
                                    consumible.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    consumible.fecha_modificacion = DateTime.Now;
                                    db.Entry(consumible).State = EntityState.Modified;

                                    bic.existencia += Convert.ToInt32(ind_det.cantidad);
                                    bic.activo = true;
                                    bic.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    bic.fecha_modificacion = DateTime.Now;
                                    db.Entry(bic).State = EntityState.Modified;

                                    Transaccion_Inventario_Detalle tid = new Transaccion_Inventario_Detalle();
                                    tid.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid.id_transaccion_inventario_tipo = 1;
                                    tid.cantidad = ind_det.cantidad;
                                    tid.debe = consumible.costo;
                                    tid.id_consumible = consumible.id_consumible;
                                    tid.id_bodega = ingresosEdit.id_bodega;
                                    tid.activo = true;
                                    tid.eliminado = false;
                                    tid.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid);
                                    db.SaveChanges();
                                }
                                if (ind_det.id_municion != null)
                                {
                                    Municiones municion = db.Municiones.Single(x => !x.eliminado && x.activo && x.id_municion == ind_det.id_municion);
                                    Bodega_Inventario_Municiones bim = db.Bodega_Inventario_Municiones.Single(x => !x.eliminado && x.activo && x.id_municion == ind_det.id_municion && x.id_bodega == ingresosEdit.id_bodega);
                                    municion.activo = true;
                                    municion.autorizada = true;
                                    municion.existencia += Convert.ToInt32(ind_det.cantidad);
                                    municion.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    municion.fecha_modificacion = DateTime.Now;
                                    db.Entry(municion).State = EntityState.Modified;

                                    if (!bim.autorizada)
                                    {
                                        //Creando registro en logs para las municiones
                                        Logs_Movimientos_Municion lmm = new Logs_Movimientos_Municion();
                                        lmm.id_bodega_inventario_municion = bim.id_bodega_inventario_municiones;
                                        lmm.id_bodega_origen = ingresosEdit.id_bodega;
                                        lmm.id_tipo_transaccion = Convert.ToInt32(Catalogos.InventarioTransacciones.Ingreso);
                                        lmm.id_estado_transaccion = Convert.ToInt32(Catalogos.InventarioTransaccionEstado.Creado);
                                        lmm.cantidad = ind_det.cantidad;
                                        lmm.fecha_creacion = DateTime.Now;
                                        lmm.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                        lmm.activo = true;
                                        lmm.eliminado = false;
                                        db.Logs_Movimientos_Municion.Add(lmm);
                                        db.SaveChanges();
                                    }
                                    else {
                                        //Creando registro en logs para las municiones
                                        Logs_Movimientos_Municion lmm = new Logs_Movimientos_Municion();
                                        lmm.id_bodega_inventario_municion = bim.id_bodega_inventario_municiones;
                                        lmm.id_bodega_origen = ingresosEdit.id_bodega;
                                        lmm.id_tipo_transaccion = Convert.ToInt32(Catalogos.InventarioTransacciones.Ingreso);
                                        lmm.id_estado_transaccion = Convert.ToInt32(Catalogos.InventarioTransaccionEstado.Actualizado);
                                        lmm.cantidad = ind_det.cantidad;
                                        lmm.fecha_creacion = DateTime.Now;
                                        lmm.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                        lmm.activo = true;
                                        lmm.eliminado = false;
                                        db.Logs_Movimientos_Municion.Add(lmm);
                                        db.SaveChanges();
                                    }

                                    bim.existencia += Convert.ToInt32(ind_det.cantidad);
                                    bim.activo = true;
                                    bim.autorizada = true;
                                    bim.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    bim.fecha_modificacion = DateTime.Now;
                                    db.Entry(bim).State = EntityState.Modified;

                                    Transaccion_Inventario_Detalle tid = new Transaccion_Inventario_Detalle();
                                    tid.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid.id_transaccion_inventario_tipo = 1;
                                    tid.cantidad = ind_det.cantidad;
                                    tid.debe = municion.costo;
                                    tid.id_municion = municion.id_municion;
                                    tid.id_bodega = ingresosEdit.id_bodega;
                                    tid.activo = true;
                                    tid.eliminado = false;
                                    tid.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid);
                                    db.SaveChanges();

                                }
                                if (ind_det.id_uniforme != null)
                                {
                                    Uniformes uniforme = db.Uniformes.Single(x => !x.eliminado && x.activo && x.id_uniforme == ind_det.id_uniforme);
                                    Bodega_Inventario_Uniformes biu = db.Bodega_Inventario_Uniformes.Single(x => !x.eliminado && x.activo && x.id_uniforme == ind_det.id_uniforme && x.id_bodega == ingresosEdit.id_bodega);
                                    uniforme.existencia += Convert.ToInt32(ind_det.cantidad);
                                    uniforme.activo = true;
                                    uniforme.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    uniforme.fecha_modificacion = DateTime.Now;
                                    db.Entry(uniforme).State = EntityState.Modified;

                                    biu.existencia += Convert.ToInt32(ind_det.cantidad);
                                    biu.activo = true;
                                    biu.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                                    biu.fecha_modificacion = DateTime.Now;
                                    db.Entry(biu).State = EntityState.Modified;

                                    Transaccion_Inventario_Detalle tid = new Transaccion_Inventario_Detalle();
                                    tid.id_transaccion_inventario = ti.id_transaccion_inventario;
                                    tid.id_transaccion_inventario_tipo = 1;
                                    tid.cantidad = ind_det.cantidad;
                                    tid.debe = uniforme.costo;
                                    tid.id_uniforme = uniforme.id_uniforme;
                                    tid.id_bodega = ingresosEdit.id_bodega;
                                    tid.activo = true;
                                    tid.eliminado = false;
                                    tid.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                    tid.fecha_creacion = DateTime.Now;
                                    db.Transaccion_Inventario_Detalle.Add(tid);
                                    db.SaveChanges();
                                }
                            }
                            
                        }
                        tran.Commit();
                        return RedirectToAction("Index_Autorizar");

                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Index_Autorizar");
                    }
                }
                
            }
            var ingresos = db.Ingresos.Include(i => i.Usuarios).Include(i => i.Usuarios1).Include(i => i.Usuarios2).Include(i => i.Bodegas).Include(i => i.Ingreso_Estado).Where(i => i.activo).Where(i => i.eliminado == false);
            return View(ingresos.ToList());
        }

        public ActionResult Rechazar(int? id)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Ingresos ingresosEdit = db.Ingresos.Find(id);
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                        ingresosEdit.id_ingreso_estado = 7;
                        ingresosEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        ingresosEdit.fecha_modificacion = DateTime.Now;
                        ingresosEdit.activo = true;
                        ingresosEdit.eliminado = false;
                        db.Entry(ingresosEdit).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Index_Autorizar");
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Index_Autorizar");
                    }
                }
                
            }
            var ingresos = db.Ingresos.Include(i => i.Usuarios).Include(i => i.Usuarios1).Include(i => i.Usuarios2).Include(i => i.Bodegas).Include(i => i.Ingreso_Estado).Where(i => i.activo).Where(i => i.eliminado == false);
            return View(ingresos.ToList());
        }

        public ActionResult del_detalle(int id_ingreso, int id_detalle)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Ingreso_Detalle indet = db.Ingreso_Detalle.Where(x=>x.activo && !x.eliminado && x.id_ingreso_detalle==id_detalle && x.id_ingreso==id_ingreso).Single();
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        indet.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                        indet.fecha_eliminacion = DateTime.Now;
                        indet.activo = false;
                        indet.eliminado = true;
                        db.Entry(indet).State = EntityState.Modified;
                        db.SaveChanges();

                        if (indet.id_arma != null)
                        { 
                            Armas armas = db.Armas.Where(x=> x.activo && !x.eliminado && x.id_arma==indet.id_arma ).Single();
                            armas.activo = false;
                            armas.eliminado = true;
                            armas.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                            armas.fecha_eliminacion = DateTime.Now;
                            db.Entry(armas).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        if (indet.id_articulo != null)
                        {
                            Articulos articulos = db.Articulos.Where(x => x.activo && !x.eliminado && x.id_articulo == indet.id_articulo).Single();
                            articulos.activo = false;
                            articulos.eliminado = true;
                            articulos.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                            articulos.fecha_eliminacion = DateTime.Now;
                            db.Entry(articulos).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        
                        tran.Commit();
                        return RedirectToAction("Edit", new { id = id_ingreso });
                    }
                    catch
                    {
                        tran.Rollback();
                        return RedirectToAction("Edit", new { id = id_ingreso });
                    }
                }

            }
            var ingresos = db.Ingresos.Include(i => i.Usuarios).Include(i => i.Usuarios1).Include(i => i.Usuarios2).Include(i => i.Bodegas).Include(i => i.Ingreso_Estado).Where(i => i.activo).Where(i => i.eliminado == false);
            return View(ingresos.ToList());
        }
    }
}
