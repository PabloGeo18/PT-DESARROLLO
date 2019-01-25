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

namespace MVC2013.Areas.Comercializacion.Controllers
{
    public class Fases_CotizacionController : Controller
    {
        private Protal_webEntities db = new Protal_webEntities();
        private AppEntities ddb = new AppEntities();
        List<int> rolesConPermisoAVerProcesoCot = new List<int> { 25, 26, 27, 28, 29 };
        // GET: Comercializacion/Fases_Cotizacion
        public ActionResult Index(int? id)
        {  
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            int idUsuario = usuarioTO.usuario.id_usuario;
            Usuarios user = ddb.Usuarios.Find(id);
            var rol = usuarioTO.usuario.Roles.Select(x => x.id_rol);
            Pt_Cotizaciones cotizacion = db.Pt_Cotizaciones.Find(id);
            ViewBag.nombreCot = cotizacion.ccot_nombre_proyecto;
            ViewBag.cotizacion = cotizacion;
            List<Pt_Fases_Cotizacion> fasesCotizaciones = db.Pt_Fases_Cotizacion.Where(fc => fc.cfas_ccot_id == id).ToList();
            ViewBag.fases = fasesCotizaciones;
            ViewBag.id = id;
            var pt_Fases_Cotizacion = db.Pt_Fases_Cotizacion.Include(p => p.Pt_Cotizaciones);
            return View(pt_Fases_Cotizacion.ToList());
        }

        // GET: Comercializacion/Fases_Cotizacion/Details/5
        public ActionResult Details(int? id, int? cotid)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Fases_Cotizacion pt_Fases_Cotizacion = db.Pt_Fases_Cotizacion.Find(id);
            if (pt_Fases_Cotizacion == null)
            {
                return HttpNotFound();
            }
            Pt_Cotizaciones cotizacion = db.Pt_Cotizaciones.Find(cotid);
            ViewBag.cotizacion = cotizacion;
            ViewBag.cotid = cotid;
            return View(pt_Fases_Cotizacion);
        }

        // GET: Comercializacion/Fases_Cotizacion/Create
        public ActionResult Create(int? id)
        {
            ViewBag.conut = db.Pt_Fases_Cotizacion.Where(fc => fc.cfas_ccot_id == id && fc.activo && !fc.eliminado).Count();
            ViewBag.id = id;
            ViewBag.cfas_cefa_id = new SelectList(db.Pt_Estados_Fase.Where(ef => ef.activo && !ef.eliminado), "cefa_id", "cefa_descripcion");
            ViewBag.cfas_ccot_id = new SelectList(db.Pt_Cotizaciones.Where(c=>c.ccot_id==id), "ccot_id", "ccot_nombre_proyecto");
            return View();
        }

        // POST: Comercializacion/Fases_Cotizacion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pt_Fases_Cotizacion fasesCotizacion)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                fasesCotizacion.cfas_rev_ven = true;
                fasesCotizacion.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                fasesCotizacion.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                fasesCotizacion.fecha_creacion = DateTime.Now;
                fasesCotizacion.activo = true;
                fasesCotizacion.eliminado = false;
                db.Pt_Fases_Cotizacion.Add(fasesCotizacion);
                db.SaveChanges();
                return RedirectToAction("Index/"+fasesCotizacion.cfas_ccot_id);
            }
            ViewBag.cfas_cefa_id = new SelectList(db.Pt_Estados_Fase.Where(ef => ef.activo && !ef.eliminado), "cefa_id", "cefa_descripcion", fasesCotizacion.cfas_cefa_id);
            ViewBag.cfas_ccot_id = new SelectList(db.Pt_Cotizaciones, "ccot_id", "ccot_nombre_proyecto", fasesCotizacion.cfas_ccot_id);
            return View(fasesCotizacion);
        }

        // GET: Comercializacion/Fases_Cotizacion/Edit/5
        public ActionResult Edit(int? id, int? cotid)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Fases_Cotizacion pt_Fases_Cotizacion = db.Pt_Fases_Cotizacion.Find(id);
            if (pt_Fases_Cotizacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.fechaEntrega = Convert.ToDateTime(pt_Fases_Cotizacion.cfas_fecha_entrega).ToString("yyyy-MM-dd");
            ViewBag.cotid = cotid;
            ViewBag.cfas_cefa_id = new SelectList(db.Pt_Estados_Fase.Where(ef => ef.activo && !ef.eliminado), "cefa_id", "cefa_descripcion");
            ViewBag.cfas_ccot_id = new SelectList(db.Pt_Cotizaciones.Where(c => c.ccot_id == cotid), "ccot_id", "ccot_nombre_proyecto", pt_Fases_Cotizacion.cfas_ccot_id);
            return View(pt_Fases_Cotizacion);
        }

        // POST: Comercializacion/Fases_Cotizacion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pt_Fases_Cotizacion fasesCotizacion)
        {
            if (ModelState.IsValid)
            {
                Pt_Fases_Cotizacion fasescotizacionEdit = db.Pt_Fases_Cotizacion.Find(fasesCotizacion.cfas_id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                fasescotizacionEdit.cfas_nombre = fasesCotizacion.cfas_nombre;
                fasescotizacionEdit.cfas_descripcion = fasesCotizacion.cfas_descripcion;
                fasescotizacionEdit.cfas_fecha_entrega = fasesCotizacion.cfas_fecha_entrega;
                fasescotizacionEdit.cfas_cefa_id = fasesCotizacion.cfas_cefa_id;
                fasescotizacionEdit.cfas_ccot_id = fasesCotizacion.cfas_ccot_id;
                fasescotizacionEdit.activo = true;
                fasescotizacionEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                fasescotizacionEdit.fecha_modificacion = DateTime.Now;
                fasescotizacionEdit.eliminado = false;
                db.Entry(fasescotizacionEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index/"+fasesCotizacion.cfas_ccot_id);
            }
            ViewBag.cfas_cefa_id = new SelectList(db.Pt_Estados_Fase.Where(ef => ef.activo && !ef.eliminado), "cefa_id", "cefa_descripcion", fasesCotizacion.cfas_cefa_id);
            ViewBag.cfas_ccot_id = new SelectList(db.Pt_Cotizaciones, "ccot_id", "ccot_nombre_proyecto", fasesCotizacion.cfas_ccot_id);
            return View(fasesCotizacion);
        }

        //POST: Comercializacion/Fases_Cotizacion/EnviarAOperaciones
        public ActionResult EnviarRevisionOperaciones(int id)
        {
            Pt_Fases_Cotizacion fasescotizacionEdit = db.Pt_Fases_Cotizacion.Find(id);
            Pt_Fases_Cotizacion newFaseCotizacion = new Pt_Fases_Cotizacion();
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            List<Pt_Tmp_Propuesta_Fase_Puesto> confPropuesta = db.Pt_Tmp_Propuesta_Fase_Puesto.Where(x => x.activo && !x.eliminado && x.ctpf_cfas_id == id).ToList();
            List<Pt_Tmp_Cotizacion_Fase_Insumos> confInsumoList = db.Pt_Tmp_Cotizacion_Fase_Insumos.Where(x => x.activo && !x.eliminado && x.ctpfi_cfas_id == id).ToList();
            List<Pt_Tmp_Cotizacion_Fase_Insumos_New> confInsumoNew = db.Pt_Tmp_Cotizacion_Fase_Insumos_New.Where(x => x.activo && !x.eliminado && x.ctpfin_cfas_id == id).ToList();
            Pt_Tmp_Cotizacion_Fase_Margen_Conf confMargen = db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Where(x => x.activo && !x.eliminado && x.ctcfm_cfas_id == id).SingleOrDefault();
            if (ModelState.IsValid)
            {
                //creando cotización como histórica
                fasescotizacionEdit.cfas_nombre = fasescotizacionEdit.cfas_nombre;
                fasescotizacionEdit.cfas_descripcion = fasescotizacionEdit.cfas_descripcion;
                fasescotizacionEdit.cfas_fecha_entrega = fasescotizacionEdit.cfas_fecha_entrega;
                fasescotizacionEdit.cfas_cefa_id = fasescotizacionEdit.cfas_cefa_id;
                fasescotizacionEdit.cfas_ccot_id = fasescotizacionEdit.cfas_ccot_id;
                fasescotizacionEdit.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                fasescotizacionEdit.historico = true;
                db.Entry(fasescotizacionEdit).State = EntityState.Modified;
                db.SaveChanges();
                //es nueva cotización, con los datos anteriores
                newFaseCotizacion.cfas_nombre = fasescotizacionEdit.cfas_nombre;
                newFaseCotizacion.cfas_descripcion = fasescotizacionEdit.cfas_descripcion;
                newFaseCotizacion.cfas_fecha_entrega = fasescotizacionEdit.cfas_fecha_entrega;
                newFaseCotizacion.cfas_cefa_id = fasescotizacionEdit.cfas_cefa_id;
                newFaseCotizacion.cfas_ccot_id = fasescotizacionEdit.cfas_ccot_id;
                newFaseCotizacion.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.cfas_rev_ven = false;
                newFaseCotizacion.cfas_rev_ope = true;
                newFaseCotizacion.activo = true;
                newFaseCotizacion.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.fecha_creacion = DateTime.Now;
                newFaseCotizacion.eliminado = false;
                db.Pt_Fases_Cotizacion.Add(newFaseCotizacion);
                db.SaveChanges();
                foreach (var newConfPropuesta in confPropuesta) {
                    Pt_Tmp_Propuesta_Fase_Puesto newConf = new Pt_Tmp_Propuesta_Fase_Puesto();
                    newConf.ctpf_cpue_id = newConfPropuesta.ctpf_cpue_id;
                    newConf.ctpf_cfas_id = newFaseCotizacion.cfas_id;
                    newConf.ctpf_personal = newConfPropuesta.ctpf_personal;
                    newConf.ctpf_facConIVA = 0;
                    newConf.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConf.fecha_creacion = DateTime.Now;
                    newConf.activo = true;
                    newConf.eliminado = false;
                    db.Pt_Tmp_Propuesta_Fase_Puesto.Add(newConf);
                    db.SaveChanges();
                }
                foreach (var newInsumosList in confInsumoList)
                {
                    Pt_Tmp_Cotizacion_Fase_Insumos newConfInsumoList = new Pt_Tmp_Cotizacion_Fase_Insumos();
                    newConfInsumoList.ctpfi_cins_id = newInsumosList.ctpfi_cins_id;
                    newConfInsumoList.ctpfi_cantidad = newInsumosList.ctpfi_cantidad;
                    newConfInsumoList.ctpfi_cfas_id = newFaseCotizacion.cfas_id;
                    newConfInsumoList.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConfInsumoList.fecha_creacion = DateTime.Now;
                    newConfInsumoList.activo = true;
                    newConfInsumoList.eliminado = false;
                    db.Pt_Tmp_Cotizacion_Fase_Insumos.Add(newConfInsumoList);
                    db.SaveChanges();
                }
                foreach (var newInsumosNew in confInsumoNew)
                {
                    Pt_Tmp_Cotizacion_Fase_Insumos_New newConfInsumoNew = new Pt_Tmp_Cotizacion_Fase_Insumos_New();
                    newConfInsumoNew.ctpfin_descripcion = newInsumosNew.ctpfin_descripcion;
                    newConfInsumoNew.ctpfin_precio_costo = newInsumosNew.ctpfin_precio_costo;
                    newConfInsumoNew.ctpfin_precio_venta = newInsumosNew.ctpfin_precio_venta;
                    newConfInsumoNew.ctpfin_talla = newInsumosNew.ctpfin_talla;
                    newConfInsumoNew.ctpfin_es_uniforme = newInsumosNew.ctpfin_es_uniforme;
                    newConfInsumoNew.ctpfin_depreciacion = newInsumosNew.ctpfin_depreciacion;
                    newConfInsumoNew.ctpfin_porcentaje_depreciacion = newInsumosNew.ctpfin_porcentaje_depreciacion;
                    newConfInsumoNew.ctpfin_cantidad = newInsumosNew.ctpfin_cantidad;
                    newConfInsumoNew.ctpfin_cfas_id = newFaseCotizacion.cfas_id;
                    newConfInsumoNew.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConfInsumoNew.fecha_creacion = DateTime.Now;
                    newConfInsumoNew.activo = true;
                    newConfInsumoNew.eliminado = false;
                    db.Pt_Tmp_Cotizacion_Fase_Insumos_New.Add(newConfInsumoNew);
                    db.SaveChanges();
                }
                Pt_Tmp_Cotizacion_Fase_Margen_Conf newConfMargen = new Pt_Tmp_Cotizacion_Fase_Margen_Conf();
                newConfMargen.ctcfm_cfas_id = newFaseCotizacion.cfas_id; ;
                newConfMargen.ctcfm_margen = confMargen.ctcfm_margen;
                newConfMargen.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                newConfMargen.fecha_creacion = DateTime.Now;
                newConfMargen.activo = true;
                newConfMargen.eliminado = false;
                db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Add(newConfMargen);
                db.SaveChanges();
                return RedirectToAction("Index/" + newFaseCotizacion.cfas_ccot_id);
            }
            ViewBag.cfas_cefa_id = new SelectList(db.Pt_Estados_Fase.Where(ef => ef.activo && !ef.eliminado), "cefa_id", "cefa_descripcion", newFaseCotizacion.cfas_cefa_id);
            ViewBag.cfas_ccot_id = new SelectList(db.Pt_Cotizaciones, "ccot_id", "ccot_nombre_proyecto", newFaseCotizacion.cfas_ccot_id);
            return View(newFaseCotizacion);
        }

        public ActionResult EnviarRevisionVentas(int id)
        {
            Pt_Fases_Cotizacion fasescotizacionEdit = db.Pt_Fases_Cotizacion.Find(id);
            Pt_Fases_Cotizacion newFaseCotizacion = new Pt_Fases_Cotizacion();
            List<Pt_Tmp_Propuesta_Fase_Puesto> confPropuesta = db.Pt_Tmp_Propuesta_Fase_Puesto.Where(x => x.activo && !x.eliminado && x.ctpf_cfas_id == id).ToList();
            List<Pt_Tmp_Cotizacion_Fase_Insumos> confInsumoList = db.Pt_Tmp_Cotizacion_Fase_Insumos.Where(x => x.activo && !x.eliminado && x.ctpfi_cfas_id == id).ToList();
            List<Pt_Tmp_Cotizacion_Fase_Insumos_New> confInsumoNew = db.Pt_Tmp_Cotizacion_Fase_Insumos_New.Where(x => x.activo && !x.eliminado && x.ctpfin_cfas_id == id).ToList();
            Pt_Tmp_Cotizacion_Fase_Margen_Conf confMargen = db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Where(x => x.activo && !x.eliminado && x.ctcfm_cfas_id == id).SingleOrDefault();
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            if (ModelState.IsValid)
            {
                //creando cotización como histórica
                fasescotizacionEdit.cfas_nombre = fasescotizacionEdit.cfas_nombre;
                fasescotizacionEdit.cfas_descripcion = fasescotizacionEdit.cfas_descripcion;
                fasescotizacionEdit.cfas_fecha_entrega = fasescotizacionEdit.cfas_fecha_entrega;
                fasescotizacionEdit.cfas_cefa_id = fasescotizacionEdit.cfas_cefa_id;
                fasescotizacionEdit.cfas_ccot_id = fasescotizacionEdit.cfas_ccot_id;
                fasescotizacionEdit.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                fasescotizacionEdit.historico = true;
                db.Entry(fasescotizacionEdit).State = EntityState.Modified;
                db.SaveChanges();
                //es nueva cotización, con los datos anteriores
                newFaseCotizacion.cfas_nombre = fasescotizacionEdit.cfas_nombre;
                newFaseCotizacion.cfas_descripcion = fasescotizacionEdit.cfas_descripcion;
                newFaseCotizacion.cfas_fecha_entrega = fasescotizacionEdit.cfas_fecha_entrega;
                newFaseCotizacion.cfas_cefa_id = fasescotizacionEdit.cfas_cefa_id;
                newFaseCotizacion.cfas_ccot_id = fasescotizacionEdit.cfas_ccot_id;
                newFaseCotizacion.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.cfas_rev_ven = true;
                newFaseCotizacion.cfas_rev_ope = false;
                newFaseCotizacion.activo = true;
                newFaseCotizacion.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.fecha_creacion = DateTime.Now;
                newFaseCotizacion.eliminado = false;
                db.Pt_Fases_Cotizacion.Add(newFaseCotizacion);
                db.SaveChanges();
                foreach (var newConfPropuesta in confPropuesta)
                {
                    Pt_Tmp_Propuesta_Fase_Puesto newConf = new Pt_Tmp_Propuesta_Fase_Puesto();
                    newConf.ctpf_cpue_id = newConfPropuesta.ctpf_cpue_id;
                    newConf.ctpf_cfas_id = newFaseCotizacion.cfas_id;
                    newConf.ctpf_personal = newConfPropuesta.ctpf_personal;
                    newConf.ctpf_facConIVA = 0;
                    newConf.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConf.fecha_creacion = DateTime.Now;
                    newConf.activo = true;
                    newConf.eliminado = false;
                    db.Pt_Tmp_Propuesta_Fase_Puesto.Add(newConf);
                    db.SaveChanges();
                }
                foreach (var newInsumosList in confInsumoList)
                {
                    Pt_Tmp_Cotizacion_Fase_Insumos newConfInsumoList = new Pt_Tmp_Cotizacion_Fase_Insumos();
                    newConfInsumoList.ctpfi_cins_id = newInsumosList.ctpfi_cins_id;
                    newConfInsumoList.ctpfi_cantidad = newInsumosList.ctpfi_cantidad;
                    newConfInsumoList.ctpfi_cfas_id = newFaseCotizacion.cfas_id;
                    newConfInsumoList.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConfInsumoList.fecha_creacion = DateTime.Now;
                    newConfInsumoList.activo = true;
                    newConfInsumoList.eliminado = false;
                    db.Pt_Tmp_Cotizacion_Fase_Insumos.Add(newConfInsumoList);
                    db.SaveChanges();
                }
                foreach (var newInsumosNew in confInsumoNew)
                {
                    Pt_Tmp_Cotizacion_Fase_Insumos_New newConfInsumoNew = new Pt_Tmp_Cotizacion_Fase_Insumos_New();
                    newConfInsumoNew.ctpfin_descripcion = newInsumosNew.ctpfin_descripcion;
                    newConfInsumoNew.ctpfin_precio_costo = newInsumosNew.ctpfin_precio_costo;
                    newConfInsumoNew.ctpfin_precio_venta = newInsumosNew.ctpfin_precio_venta;
                    newConfInsumoNew.ctpfin_talla = newInsumosNew.ctpfin_talla;
                    newConfInsumoNew.ctpfin_es_uniforme = newInsumosNew.ctpfin_es_uniforme;
                    newConfInsumoNew.ctpfin_depreciacion = newInsumosNew.ctpfin_depreciacion;
                    newConfInsumoNew.ctpfin_porcentaje_depreciacion = newInsumosNew.ctpfin_porcentaje_depreciacion;
                    newConfInsumoNew.ctpfin_cantidad = newInsumosNew.ctpfin_cantidad;
                    newConfInsumoNew.ctpfin_cfas_id = newFaseCotizacion.cfas_id;
                    newConfInsumoNew.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConfInsumoNew.fecha_creacion = DateTime.Now;
                    newConfInsumoNew.activo = true;
                    newConfInsumoNew.eliminado = false;
                    db.Pt_Tmp_Cotizacion_Fase_Insumos_New.Add(newConfInsumoNew);
                    db.SaveChanges();
                }
                Pt_Tmp_Cotizacion_Fase_Margen_Conf newConfMargen = new Pt_Tmp_Cotizacion_Fase_Margen_Conf();
                newConfMargen.ctcfm_cfas_id = newFaseCotizacion.cfas_id; ;
                newConfMargen.ctcfm_margen = confMargen.ctcfm_margen;
                newConfMargen.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                newConfMargen.fecha_creacion = DateTime.Now;
                newConfMargen.activo = true;
                newConfMargen.eliminado = false;
                db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Add(newConfMargen);
                db.SaveChanges();
                return RedirectToAction("Index/" + fasescotizacionEdit.cfas_ccot_id);
            }
            ViewBag.cfas_cefa_id = new SelectList(db.Pt_Estados_Fase.Where(ef => ef.activo && !ef.eliminado), "cefa_id", "cefa_descripcion", fasescotizacionEdit.cfas_cefa_id);
            ViewBag.cfas_ccot_id = new SelectList(db.Pt_Cotizaciones, "ccot_id", "ccot_nombre_proyecto", fasescotizacionEdit.cfas_ccot_id);
            return View(fasescotizacionEdit);
        }

        public ActionResult EnviarRevisionGerenteG(int id)
        {
            Pt_Fases_Cotizacion fasescotizacionEdit = db.Pt_Fases_Cotizacion.Find(id);
            Pt_Fases_Cotizacion newFaseCotizacion = new Pt_Fases_Cotizacion();
            List<Pt_Tmp_Propuesta_Fase_Puesto> confPropuesta = db.Pt_Tmp_Propuesta_Fase_Puesto.Where(x => x.activo && !x.eliminado && x.ctpf_cfas_id == id).ToList();
            List<Pt_Tmp_Cotizacion_Fase_Insumos> confInsumoList = db.Pt_Tmp_Cotizacion_Fase_Insumos.Where(x => x.activo && !x.eliminado && x.ctpfi_cfas_id == id).ToList();
            List<Pt_Tmp_Cotizacion_Fase_Insumos_New> confInsumoNew = db.Pt_Tmp_Cotizacion_Fase_Insumos_New.Where(x => x.activo && !x.eliminado && x.ctpfin_cfas_id == id).ToList();
            Pt_Tmp_Cotizacion_Fase_Margen_Conf confMargen = db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Where(x => x.activo && !x.eliminado && x.ctcfm_cfas_id == id).SingleOrDefault();
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            if (ModelState.IsValid)
            {
                //creando cotización como histórica
                fasescotizacionEdit.cfas_nombre = fasescotizacionEdit.cfas_nombre;
                fasescotizacionEdit.cfas_descripcion = fasescotizacionEdit.cfas_descripcion;
                fasescotizacionEdit.cfas_fecha_entrega = fasescotizacionEdit.cfas_fecha_entrega;
                fasescotizacionEdit.cfas_cefa_id = fasescotizacionEdit.cfas_cefa_id;
                fasescotizacionEdit.cfas_ccot_id = fasescotizacionEdit.cfas_ccot_id;
                fasescotizacionEdit.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                fasescotizacionEdit.historico = true;
                db.Entry(fasescotizacionEdit).State = EntityState.Modified;
                db.SaveChanges();
                //es nueva cotización, con los datos anteriores
                newFaseCotizacion.cfas_nombre = fasescotizacionEdit.cfas_nombre;
                newFaseCotizacion.cfas_descripcion = fasescotizacionEdit.cfas_descripcion;
                newFaseCotizacion.cfas_fecha_entrega = fasescotizacionEdit.cfas_fecha_entrega;
                newFaseCotizacion.cfas_cefa_id = fasescotizacionEdit.cfas_cefa_id;
                newFaseCotizacion.cfas_ccot_id = fasescotizacionEdit.cfas_ccot_id;
                newFaseCotizacion.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.cfas_rev_gge = false;
                newFaseCotizacion.cfas_rev_ven = false;
                newFaseCotizacion.cfas_rev_gge = true;
                newFaseCotizacion.activo = true;
                newFaseCotizacion.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.fecha_creacion = DateTime.Now;
                newFaseCotizacion.eliminado = false;
                db.Pt_Fases_Cotizacion.Add(newFaseCotizacion);
                db.SaveChanges();
                foreach (var newConfPropuesta in confPropuesta)
                {
                    Pt_Tmp_Propuesta_Fase_Puesto newConf = new Pt_Tmp_Propuesta_Fase_Puesto();
                    newConf.ctpf_cpue_id = newConfPropuesta.ctpf_cpue_id;
                    newConf.ctpf_cfas_id = newFaseCotizacion.cfas_id;
                    newConf.ctpf_personal = newConfPropuesta.ctpf_personal;
                    newConf.ctpf_facConIVA = 0;
                    newConf.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConf.fecha_creacion = DateTime.Now;
                    newConf.activo = true;
                    newConf.eliminado = false;
                    db.Pt_Tmp_Propuesta_Fase_Puesto.Add(newConf);
                    db.SaveChanges();
                }
                foreach (var newInsumosList in confInsumoList)
                {
                    Pt_Tmp_Cotizacion_Fase_Insumos newConfInsumoList = new Pt_Tmp_Cotizacion_Fase_Insumos();
                    newConfInsumoList.ctpfi_cins_id = newInsumosList.ctpfi_cins_id;
                    newConfInsumoList.ctpfi_cantidad = newInsumosList.ctpfi_cantidad;
                    newConfInsumoList.ctpfi_cfas_id = newFaseCotizacion.cfas_id;
                    newConfInsumoList.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConfInsumoList.fecha_creacion = DateTime.Now;
                    newConfInsumoList.activo = true;
                    newConfInsumoList.eliminado = false;
                    db.Pt_Tmp_Cotizacion_Fase_Insumos.Add(newConfInsumoList);
                    db.SaveChanges();
                }
                foreach (var newInsumosNew in confInsumoNew)
                {
                    Pt_Tmp_Cotizacion_Fase_Insumos_New newConfInsumoNew = new Pt_Tmp_Cotizacion_Fase_Insumos_New();
                    newConfInsumoNew.ctpfin_descripcion = newInsumosNew.ctpfin_descripcion;
                    newConfInsumoNew.ctpfin_precio_costo = newInsumosNew.ctpfin_precio_costo;
                    newConfInsumoNew.ctpfin_precio_venta = newInsumosNew.ctpfin_precio_venta;
                    newConfInsumoNew.ctpfin_talla = newInsumosNew.ctpfin_talla;
                    newConfInsumoNew.ctpfin_es_uniforme = newInsumosNew.ctpfin_es_uniforme;
                    newConfInsumoNew.ctpfin_depreciacion = newInsumosNew.ctpfin_depreciacion;
                    newConfInsumoNew.ctpfin_porcentaje_depreciacion = newInsumosNew.ctpfin_porcentaje_depreciacion;
                    newConfInsumoNew.ctpfin_cantidad = newInsumosNew.ctpfin_cantidad;
                    newConfInsumoNew.ctpfin_cfas_id = newFaseCotizacion.cfas_id;
                    newConfInsumoNew.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConfInsumoNew.fecha_creacion = DateTime.Now;
                    newConfInsumoNew.activo = true;
                    newConfInsumoNew.eliminado = false;
                    db.Pt_Tmp_Cotizacion_Fase_Insumos_New.Add(newConfInsumoNew);
                    db.SaveChanges();
                }
                Pt_Tmp_Cotizacion_Fase_Margen_Conf newConfMargen = new Pt_Tmp_Cotizacion_Fase_Margen_Conf();
                newConfMargen.ctcfm_cfas_id = newFaseCotizacion.cfas_id; ;
                newConfMargen.ctcfm_margen = confMargen.ctcfm_margen;
                newConfMargen.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                newConfMargen.fecha_creacion = DateTime.Now;
                newConfMargen.activo = true;
                newConfMargen.eliminado = false;
                db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Add(newConfMargen);
                db.SaveChanges();
                return RedirectToAction("Index/" + fasescotizacionEdit.cfas_ccot_id);
            }
            ViewBag.cfas_cefa_id = new SelectList(db.Pt_Estados_Fase.Where(ef => ef.activo && !ef.eliminado), "cefa_id", "cefa_descripcion", fasescotizacionEdit.cfas_cefa_id);
            ViewBag.cfas_ccot_id = new SelectList(db.Pt_Cotizaciones, "ccot_id", "ccot_nombre_proyecto", fasescotizacionEdit.cfas_ccot_id);
            return View(fasescotizacionEdit);
        }

        public ActionResult RechazoGerente(int id)
        {
            Pt_Fases_Cotizacion fasescotizacionEdit = db.Pt_Fases_Cotizacion.Find(id);
            Pt_Fases_Cotizacion newFaseCotizacion = new Pt_Fases_Cotizacion();
            List<Pt_Tmp_Propuesta_Fase_Puesto> confPropuesta = db.Pt_Tmp_Propuesta_Fase_Puesto.Where(x => x.activo && !x.eliminado && x.ctpf_cfas_id == id).ToList();
            List<Pt_Tmp_Cotizacion_Fase_Insumos> confInsumoList = db.Pt_Tmp_Cotizacion_Fase_Insumos.Where(x => x.activo && !x.eliminado && x.ctpfi_cfas_id == id).ToList();
            List<Pt_Tmp_Cotizacion_Fase_Insumos_New> confInsumoNew = db.Pt_Tmp_Cotizacion_Fase_Insumos_New.Where(x => x.activo && !x.eliminado && x.ctpfin_cfas_id == id).ToList();
            Pt_Tmp_Cotizacion_Fase_Margen_Conf confMargen = db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Where(x => x.activo && !x.eliminado && x.ctcfm_cfas_id == id).SingleOrDefault();
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            if (ModelState.IsValid)
            {
                //creando cotización como histórica
                fasescotizacionEdit.cfas_nombre = fasescotizacionEdit.cfas_nombre;
                fasescotizacionEdit.cfas_descripcion = fasescotizacionEdit.cfas_descripcion;
                fasescotizacionEdit.cfas_fecha_entrega = fasescotizacionEdit.cfas_fecha_entrega;
                fasescotizacionEdit.cfas_cefa_id = fasescotizacionEdit.cfas_cefa_id;
                fasescotizacionEdit.cfas_ccot_id = fasescotizacionEdit.cfas_ccot_id;
                fasescotizacionEdit.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                fasescotizacionEdit.historico = true;
                db.Entry(fasescotizacionEdit).State = EntityState.Modified;
                db.SaveChanges();
                //es nueva cotización, con los datos anteriores
                newFaseCotizacion.cfas_nombre = fasescotizacionEdit.cfas_nombre;
                newFaseCotizacion.cfas_descripcion = fasescotizacionEdit.cfas_descripcion;
                newFaseCotizacion.cfas_fecha_entrega = fasescotizacionEdit.cfas_fecha_entrega;
                newFaseCotizacion.cfas_cefa_id = fasescotizacionEdit.cfas_cefa_id;
                newFaseCotizacion.cfas_ccot_id = fasescotizacionEdit.cfas_ccot_id;
                newFaseCotizacion.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.cfas_rev_gge = false;
                newFaseCotizacion.cfas_rev_ven = false;
                newFaseCotizacion.cfas_rc_gge = true;
                newFaseCotizacion.activo = true;
                newFaseCotizacion.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.fecha_creacion = DateTime.Now;
                newFaseCotizacion.eliminado = false;
                db.Pt_Fases_Cotizacion.Add(newFaseCotizacion);
                db.SaveChanges();
                foreach (var newConfPropuesta in confPropuesta)
                {
                    Pt_Tmp_Propuesta_Fase_Puesto newConf = new Pt_Tmp_Propuesta_Fase_Puesto();
                    newConf.ctpf_cpue_id = newConfPropuesta.ctpf_cpue_id;
                    newConf.ctpf_cfas_id = newFaseCotizacion.cfas_id;
                    newConf.ctpf_personal = newConfPropuesta.ctpf_personal;
                    newConf.ctpf_facConIVA = 0;
                    newConf.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConf.fecha_creacion = DateTime.Now;
                    newConf.activo = true;
                    newConf.eliminado = false;
                    db.Pt_Tmp_Propuesta_Fase_Puesto.Add(newConf);
                    db.SaveChanges();
                }
                foreach (var newInsumosList in confInsumoList)
                {
                    Pt_Tmp_Cotizacion_Fase_Insumos newConfInsumoList = new Pt_Tmp_Cotizacion_Fase_Insumos();
                    newConfInsumoList.ctpfi_cins_id = newInsumosList.ctpfi_cins_id;
                    newConfInsumoList.ctpfi_cantidad = newInsumosList.ctpfi_cantidad;
                    newConfInsumoList.ctpfi_cfas_id = newFaseCotizacion.cfas_id;
                    newConfInsumoList.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConfInsumoList.fecha_creacion = DateTime.Now;
                    newConfInsumoList.activo = true;
                    newConfInsumoList.eliminado = false;
                    db.Pt_Tmp_Cotizacion_Fase_Insumos.Add(newConfInsumoList);
                    db.SaveChanges();
                }
                foreach (var newInsumosNew in confInsumoNew)
                {
                    Pt_Tmp_Cotizacion_Fase_Insumos_New newConfInsumoNew = new Pt_Tmp_Cotizacion_Fase_Insumos_New();
                    newConfInsumoNew.ctpfin_descripcion = newInsumosNew.ctpfin_descripcion;
                    newConfInsumoNew.ctpfin_precio_costo = newInsumosNew.ctpfin_precio_costo;
                    newConfInsumoNew.ctpfin_precio_venta = newInsumosNew.ctpfin_precio_venta;
                    newConfInsumoNew.ctpfin_talla = newInsumosNew.ctpfin_talla;
                    newConfInsumoNew.ctpfin_es_uniforme = newInsumosNew.ctpfin_es_uniforme;
                    newConfInsumoNew.ctpfin_depreciacion = newInsumosNew.ctpfin_depreciacion;
                    newConfInsumoNew.ctpfin_porcentaje_depreciacion = newInsumosNew.ctpfin_porcentaje_depreciacion;
                    newConfInsumoNew.ctpfin_cantidad = newInsumosNew.ctpfin_cantidad;
                    newConfInsumoNew.ctpfin_cfas_id = newFaseCotizacion.cfas_id;
                    newConfInsumoNew.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConfInsumoNew.fecha_creacion = DateTime.Now;
                    newConfInsumoNew.activo = true;
                    newConfInsumoNew.eliminado = false;
                    db.Pt_Tmp_Cotizacion_Fase_Insumos_New.Add(newConfInsumoNew);
                    db.SaveChanges();
                }
                Pt_Tmp_Cotizacion_Fase_Margen_Conf newConfMargen = new Pt_Tmp_Cotizacion_Fase_Margen_Conf();
                newConfMargen.ctcfm_cfas_id = newFaseCotizacion.cfas_id; ;
                newConfMargen.ctcfm_margen = confMargen.ctcfm_margen;
                newConfMargen.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                newConfMargen.fecha_creacion = DateTime.Now;
                newConfMargen.activo = true;
                newConfMargen.eliminado = false;
                db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Add(newConfMargen);
                db.SaveChanges();
                return RedirectToAction("Index/" + fasescotizacionEdit.cfas_ccot_id);
            }
            ViewBag.cfas_cefa_id = new SelectList(db.Pt_Estados_Fase.Where(ef => ef.activo && !ef.eliminado), "cefa_id", "cefa_descripcion", fasescotizacionEdit.cfas_cefa_id);
            ViewBag.cfas_ccot_id = new SelectList(db.Pt_Cotizaciones, "ccot_id", "ccot_nombre_proyecto", fasescotizacionEdit.cfas_ccot_id);
            return View(fasescotizacionEdit);
        }

        public ActionResult AprobadoGerente(int id)
        {
            Pt_Fases_Cotizacion fasescotizacionEdit = db.Pt_Fases_Cotizacion.Find(id);
            Pt_Fases_Cotizacion newFaseCotizacion = new Pt_Fases_Cotizacion();
            List<Pt_Tmp_Propuesta_Fase_Puesto> confPropuesta = db.Pt_Tmp_Propuesta_Fase_Puesto.Where(x => x.activo && !x.eliminado && x.ctpf_cfas_id == id).ToList();
            List<Pt_Tmp_Cotizacion_Fase_Insumos> confInsumoList = db.Pt_Tmp_Cotizacion_Fase_Insumos.Where(x => x.activo && !x.eliminado && x.ctpfi_cfas_id == id).ToList();
            List<Pt_Tmp_Cotizacion_Fase_Insumos_New> confInsumoNew = db.Pt_Tmp_Cotizacion_Fase_Insumos_New.Where(x => x.activo && !x.eliminado && x.ctpfin_cfas_id == id).ToList();
            Pt_Tmp_Cotizacion_Fase_Margen_Conf confMargen = db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Where(x => x.activo && !x.eliminado && x.ctcfm_cfas_id == id).SingleOrDefault();
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            if (ModelState.IsValid)
            {
                //creando cotización como histórica
                fasescotizacionEdit.cfas_nombre = fasescotizacionEdit.cfas_nombre;
                fasescotizacionEdit.cfas_descripcion = fasescotizacionEdit.cfas_descripcion;
                fasescotizacionEdit.cfas_fecha_entrega = fasescotizacionEdit.cfas_fecha_entrega;
                fasescotizacionEdit.cfas_cefa_id = fasescotizacionEdit.cfas_cefa_id;
                fasescotizacionEdit.cfas_ccot_id = fasescotizacionEdit.cfas_ccot_id;
                fasescotizacionEdit.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                fasescotizacionEdit.historico = true;
                db.Entry(fasescotizacionEdit).State = EntityState.Modified;
                db.SaveChanges();
                //es nueva cotización, con los datos anteriores
                newFaseCotizacion.cfas_nombre = fasescotizacionEdit.cfas_nombre;
                newFaseCotizacion.cfas_descripcion = fasescotizacionEdit.cfas_descripcion;
                newFaseCotizacion.cfas_fecha_entrega = fasescotizacionEdit.cfas_fecha_entrega;
                newFaseCotizacion.cfas_cefa_id = fasescotizacionEdit.cfas_cefa_id;
                newFaseCotizacion.cfas_ccot_id = fasescotizacionEdit.cfas_ccot_id;
                newFaseCotizacion.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.cfas_rev_gge = false;
                newFaseCotizacion.cfas_rev_ven = true;
                newFaseCotizacion.cfas_ok_gge = true;
                newFaseCotizacion.activo = true;
                newFaseCotizacion.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.fecha_creacion = DateTime.Now;
                newFaseCotizacion.eliminado = false;
                db.Pt_Fases_Cotizacion.Add(newFaseCotizacion);
                db.SaveChanges();
                foreach (var newConfPropuesta in confPropuesta)
                {
                    Pt_Tmp_Propuesta_Fase_Puesto newConf = new Pt_Tmp_Propuesta_Fase_Puesto();
                    newConf.ctpf_cpue_id = newConfPropuesta.ctpf_cpue_id;
                    newConf.ctpf_cfas_id = newFaseCotizacion.cfas_id;
                    newConf.ctpf_personal = newConfPropuesta.ctpf_personal;
                    newConf.ctpf_facConIVA = 0;
                    newConf.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConf.fecha_creacion = DateTime.Now;
                    newConf.activo = true;
                    newConf.eliminado = false;
                    db.Pt_Tmp_Propuesta_Fase_Puesto.Add(newConf);
                    db.SaveChanges();
                }
                foreach (var newInsumosList in confInsumoList)
                {
                    Pt_Tmp_Cotizacion_Fase_Insumos newConfInsumoList = new Pt_Tmp_Cotizacion_Fase_Insumos();
                    newConfInsumoList.ctpfi_cins_id = newInsumosList.ctpfi_cins_id;
                    newConfInsumoList.ctpfi_cantidad = newInsumosList.ctpfi_cantidad;
                    newConfInsumoList.ctpfi_cfas_id = newFaseCotizacion.cfas_id;
                    newConfInsumoList.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConfInsumoList.fecha_creacion = DateTime.Now;
                    newConfInsumoList.activo = true;
                    newConfInsumoList.eliminado = false;
                    db.Pt_Tmp_Cotizacion_Fase_Insumos.Add(newConfInsumoList);
                    db.SaveChanges();
                }
                foreach (var newInsumosNew in confInsumoNew)
                {
                    Pt_Tmp_Cotizacion_Fase_Insumos_New newConfInsumoNew = new Pt_Tmp_Cotizacion_Fase_Insumos_New();
                    newConfInsumoNew.ctpfin_descripcion = newInsumosNew.ctpfin_descripcion;
                    newConfInsumoNew.ctpfin_precio_costo = newInsumosNew.ctpfin_precio_costo;
                    newConfInsumoNew.ctpfin_precio_venta = newInsumosNew.ctpfin_precio_venta;
                    newConfInsumoNew.ctpfin_talla = newInsumosNew.ctpfin_talla;
                    newConfInsumoNew.ctpfin_es_uniforme = newInsumosNew.ctpfin_es_uniforme;
                    newConfInsumoNew.ctpfin_depreciacion = newInsumosNew.ctpfin_depreciacion;
                    newConfInsumoNew.ctpfin_porcentaje_depreciacion = newInsumosNew.ctpfin_porcentaje_depreciacion;
                    newConfInsumoNew.ctpfin_cantidad = newInsumosNew.ctpfin_cantidad;
                    newConfInsumoNew.ctpfin_cfas_id = newFaseCotizacion.cfas_id;
                    newConfInsumoNew.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConfInsumoNew.fecha_creacion = DateTime.Now;
                    newConfInsumoNew.activo = true;
                    newConfInsumoNew.eliminado = false;
                    db.Pt_Tmp_Cotizacion_Fase_Insumos_New.Add(newConfInsumoNew);
                    db.SaveChanges();
                }
                Pt_Tmp_Cotizacion_Fase_Margen_Conf newConfMargen = new Pt_Tmp_Cotizacion_Fase_Margen_Conf();
                newConfMargen.ctcfm_cfas_id = newFaseCotizacion.cfas_id; ;
                newConfMargen.ctcfm_margen = confMargen.ctcfm_margen;
                newConfMargen.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                newConfMargen.fecha_creacion = DateTime.Now;
                newConfMargen.activo = true;
                newConfMargen.eliminado = false;
                db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Add(newConfMargen);
                db.SaveChanges();
                return RedirectToAction("Index/" + fasescotizacionEdit.cfas_ccot_id);
            }
            ViewBag.cfas_cefa_id = new SelectList(db.Pt_Estados_Fase.Where(ef => ef.activo && !ef.eliminado), "cefa_id", "cefa_descripcion", fasescotizacionEdit.cfas_cefa_id);
            ViewBag.cfas_ccot_id = new SelectList(db.Pt_Cotizaciones, "ccot_id", "ccot_nombre_proyecto", fasescotizacionEdit.cfas_ccot_id);
            return View(fasescotizacionEdit);
        }

        public ActionResult AprobadoVentas(int id)
        {
            Pt_Fases_Cotizacion fasescotizacionEdit = db.Pt_Fases_Cotizacion.Find(id);
            Pt_Fases_Cotizacion newFaseCotizacion = new Pt_Fases_Cotizacion();
            List<Pt_Tmp_Propuesta_Fase_Puesto> confPropuesta = db.Pt_Tmp_Propuesta_Fase_Puesto.Where(x => x.activo && !x.eliminado && x.ctpf_cfas_id == id).ToList();
            List<Pt_Tmp_Cotizacion_Fase_Insumos> confInsumoList = db.Pt_Tmp_Cotizacion_Fase_Insumos.Where(x => x.activo && !x.eliminado && x.ctpfi_cfas_id == id).ToList();
            List<Pt_Tmp_Cotizacion_Fase_Insumos_New> confInsumoNew = db.Pt_Tmp_Cotizacion_Fase_Insumos_New.Where(x => x.activo && !x.eliminado && x.ctpfin_cfas_id == id).ToList();
            Pt_Tmp_Cotizacion_Fase_Margen_Conf confMargen = db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Where(x => x.activo && !x.eliminado && x.ctcfm_cfas_id == id).SingleOrDefault();
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            if (ModelState.IsValid)
            {
                //creando cotización como histórica
                fasescotizacionEdit.cfas_nombre = fasescotizacionEdit.cfas_nombre;
                fasescotizacionEdit.cfas_descripcion = fasescotizacionEdit.cfas_descripcion;
                fasescotizacionEdit.cfas_fecha_entrega = fasescotizacionEdit.cfas_fecha_entrega;
                fasescotizacionEdit.cfas_cefa_id = fasescotizacionEdit.cfas_cefa_id;
                fasescotizacionEdit.cfas_ccot_id = fasescotizacionEdit.cfas_ccot_id;
                fasescotizacionEdit.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                fasescotizacionEdit.historico = true;
                db.Entry(fasescotizacionEdit).State = EntityState.Modified;
                db.SaveChanges();
                //es nueva cotización, con los datos anteriores
                newFaseCotizacion.cfas_nombre = fasescotizacionEdit.cfas_nombre;
                newFaseCotizacion.cfas_descripcion = fasescotizacionEdit.cfas_descripcion;
                newFaseCotizacion.cfas_fecha_entrega = fasescotizacionEdit.cfas_fecha_entrega;
                newFaseCotizacion.cfas_cefa_id = fasescotizacionEdit.cfas_cefa_id;
                newFaseCotizacion.cfas_ccot_id = fasescotizacionEdit.cfas_ccot_id;
                newFaseCotizacion.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.cfas_ok_gge = true;
                newFaseCotizacion.cfas_rev_ven = false;
                newFaseCotizacion.cfas_ok_ven = true;
                newFaseCotizacion.activo = true;
                newFaseCotizacion.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.cfas_id_usuario_ven = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                newFaseCotizacion.fecha_creacion = DateTime.Now;
                newFaseCotizacion.eliminado = false;
                db.Pt_Fases_Cotizacion.Add(newFaseCotizacion);
                db.SaveChanges();
                foreach (var newConfPropuesta in confPropuesta)
                {
                    Pt_Tmp_Propuesta_Fase_Puesto newConf = new Pt_Tmp_Propuesta_Fase_Puesto();
                    newConf.ctpf_cpue_id = newConfPropuesta.ctpf_cpue_id;
                    newConf.ctpf_cfas_id = newFaseCotizacion.cfas_id;
                    newConf.ctpf_personal = newConfPropuesta.ctpf_personal;
                    newConf.ctpf_facConIVA = 0;
                    newConf.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConf.fecha_creacion = DateTime.Now;
                    newConf.activo = true;
                    newConf.eliminado = false;
                    db.Pt_Tmp_Propuesta_Fase_Puesto.Add(newConf);
                    db.SaveChanges();
                }
                foreach (var newInsumosList in confInsumoList)
                {
                    Pt_Tmp_Cotizacion_Fase_Insumos newConfInsumoList = new Pt_Tmp_Cotizacion_Fase_Insumos();
                    newConfInsumoList.ctpfi_cins_id = newInsumosList.ctpfi_cins_id;
                    newConfInsumoList.ctpfi_cantidad = newInsumosList.ctpfi_cantidad;
                    newConfInsumoList.ctpfi_cfas_id = newFaseCotizacion.cfas_id;
                    newConfInsumoList.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConfInsumoList.fecha_creacion = DateTime.Now;
                    newConfInsumoList.activo = true;
                    newConfInsumoList.eliminado = false;
                    db.Pt_Tmp_Cotizacion_Fase_Insumos.Add(newConfInsumoList);
                    db.SaveChanges();
                }
                foreach (var newInsumosNew in confInsumoNew)
                {
                    Pt_Tmp_Cotizacion_Fase_Insumos_New newConfInsumoNew = new Pt_Tmp_Cotizacion_Fase_Insumos_New();
                    newConfInsumoNew.ctpfin_descripcion = newInsumosNew.ctpfin_descripcion;
                    newConfInsumoNew.ctpfin_precio_costo = newInsumosNew.ctpfin_precio_costo;
                    newConfInsumoNew.ctpfin_precio_venta = newInsumosNew.ctpfin_precio_venta;
                    newConfInsumoNew.ctpfin_talla = newInsumosNew.ctpfin_talla;
                    newConfInsumoNew.ctpfin_es_uniforme = newInsumosNew.ctpfin_es_uniforme;
                    newConfInsumoNew.ctpfin_depreciacion = newInsumosNew.ctpfin_depreciacion;
                    newConfInsumoNew.ctpfin_porcentaje_depreciacion = newInsumosNew.ctpfin_porcentaje_depreciacion;
                    newConfInsumoNew.ctpfin_cantidad = newInsumosNew.ctpfin_cantidad;
                    newConfInsumoNew.ctpfin_cfas_id = newFaseCotizacion.cfas_id;
                    newConfInsumoNew.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    newConfInsumoNew.fecha_creacion = DateTime.Now;
                    newConfInsumoNew.activo = true;
                    newConfInsumoNew.eliminado = false;
                    db.Pt_Tmp_Cotizacion_Fase_Insumos_New.Add(newConfInsumoNew);
                    db.SaveChanges();
                }
                Pt_Tmp_Cotizacion_Fase_Margen_Conf newConfMargen = new Pt_Tmp_Cotizacion_Fase_Margen_Conf();
                newConfMargen.ctcfm_cfas_id = newFaseCotizacion.cfas_id; ;
                newConfMargen.ctcfm_margen = confMargen.ctcfm_margen;
                newConfMargen.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                newConfMargen.fecha_creacion = DateTime.Now;
                newConfMargen.activo = true;
                newConfMargen.eliminado = false;
                db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Add(newConfMargen);
                db.SaveChanges();
                return RedirectToAction("Index/" + fasescotizacionEdit.cfas_ccot_id);
            }
            ViewBag.cfas_cefa_id = new SelectList(db.Pt_Estados_Fase.Where(ef => ef.activo && !ef.eliminado), "cefa_id", "cefa_descripcion", fasescotizacionEdit.cfas_cefa_id);
            ViewBag.cfas_ccot_id = new SelectList(db.Pt_Cotizaciones, "ccot_id", "ccot_nombre_proyecto", fasescotizacionEdit.cfas_ccot_id);
            return View(fasescotizacionEdit);
        }

        // GET: Comercializacion/Fases_Cotizacion/Delete/5
        public ActionResult Delete(int? id, int? cotid)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Fases_Cotizacion pt_Fases_Cotizacion = db.Pt_Fases_Cotizacion.Find(id);
            if (pt_Fases_Cotizacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.cotid = cotid;
            return View(pt_Fases_Cotizacion);
        }

        // POST: Comercializacion/Fases_Cotizacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int cotid)
        {
            Pt_Fases_Cotizacion fasesCotizacion = db.Pt_Fases_Cotizacion.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            fasesCotizacion.activo = false;
            fasesCotizacion.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            fasesCotizacion.fecha_eliminacion = DateTime.Now;
            fasesCotizacion.eliminado = true;
            db.Entry(fasesCotizacion).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index/"+cotid);
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
