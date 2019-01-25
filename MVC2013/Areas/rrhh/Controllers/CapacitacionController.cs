using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Areas.rrhh.Models;
using MVC2013.Src.Seguridad.To;
using MVC2013.Src.Comun.Util;

namespace MVC2013.Areas.rrhh.Controllers
{
    public class CapacitacionController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Capacitacion
        public ActionResult Index(int id)
        {
            CapacitacionCursoModel capacitacion = new CapacitacionCursoModel();
            capacitacion.capacitacion = db.Capacitacion.Where(e => !e.eliminado && e.id_academia == id).OrderBy(e => e.nombre).ToList();
            capacitacion.capacitacion_curso = db.Capacitacion_Curso.Where(e => e.Capacitacion.id_academia == id && !e.eliminado).OrderBy(e => e.Curso.nombre).ToList();
            List<int> id_cursos = capacitacion.capacitacion_curso.Select(e => e.id_curso).ToList();
            capacitacion.curso_no_asignados = db.Curso.Where(e => !e.eliminado && !id_cursos.Contains(e.id_curso)).ToList();
            capacitacion.id_academia = id;
            capacitacion.cursos = db.Curso.Where(e => !e.eliminado).OrderBy(e => e.nombre);
            ViewBag.id_capacitacion = new SelectList(capacitacion.capacitacion, "id_capacitacion", "nombre");
            capacitacion.capacitacion_impartida = db.Capacitacion_Impartida.Where(e => !e.eliminado && e.activo);
            foreach (var item in capacitacion.capacitacion_impartida)
            {
                capacitacion.no_participantes.Add(db.Curso_Empleado.Where(e => !e.eliminado && e.activo && e.id_capacitacion_impartida == item.id_capacitacion_impartida).GroupBy(e=>e.id_empleado).Count());
            }
            return View(capacitacion);
        }

        public ActionResult List(int id_academia)
        {
            var capacitacion = db.Capacitacion.Where(e => !e.eliminado && e.id_academia == id_academia).OrderBy(e => e.nombre);
            ViewBag.id_academia = id_academia;
            return View(capacitacion.ToList());
        }

        // GET: rrhh/Capacitacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Capacitacion capacitacion = db.Capacitacion.Find(id);
            if (capacitacion == null)
            {
                return HttpNotFound();
            }
            return View(capacitacion);
        }

        // GET: rrhh/Capacitacion/Create
        public ActionResult Create()
        {
            return View();
        }

        //Crear curso
        [HttpPost]
        public ActionResult CrearCurso(string nombre_curso, int? id_capacitacion, int id_academia)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        Curso curso = new Curso();
                        curso.nombre = nombre_curso;
                        curso.fecha_creacion = DateTime.Now;
                        curso.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        curso.activo = true;
                        curso.eliminado = false;
                        db.Curso.Add(curso);
                        db.SaveChanges();
                        if (id_capacitacion != null)
                        {
                            Capacitacion_Curso capacitacion_curso = new Capacitacion_Curso();
                            capacitacion_curso.id_curso = curso.id_curso;
                            capacitacion_curso.id_capacitacion = (int)id_capacitacion;
                            capacitacion_curso.fecha_creacion = DateTime.Now;
                            capacitacion_curso.activo = true;
                            capacitacion_curso.eliminado = false;
                            capacitacion_curso.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                            db.Capacitacion_Curso.Add(capacitacion_curso);
                            db.SaveChanges();
                        }
                        tran.Commit();
                        return RedirectToAction("Index", new { id = id_academia });
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
            return RedirectToAction("Index", new { id = id_academia });
        }

        //Obtener los cursos de una capacitación
        public JsonResult GetCursos(int id_capacitacion)
        {
            var listas = new { cursos_categoria = new List<object>(), cursos = new List<object>()};

            List<object> cursos = new List<object>();
            List<Capacitacion_Curso> cursos_por_capacitacion = db.Capacitacion_Curso.Where(e => !e.eliminado && e.id_capacitacion == id_capacitacion).OrderBy(e => e.Curso.nombre).ToList();
            foreach (var item in cursos_por_capacitacion)
            {
                listas.cursos_categoria.Add(new { nombre = item.Curso.nombre, id_capacitacion_curso = item.id_capacitacion_curso, id_curso = item.id_curso });
            }
            List<int> id_cursos_por_capacitacion = cursos_por_capacitacion.Select(e=>e.id_curso).ToList();
            List<Curso> cursos_fuera = db.Curso.Where(e => !e.eliminado && !id_cursos_por_capacitacion.Contains(e.id_curso)).ToList();
            foreach (var item in cursos_fuera)
            {
                listas.cursos.Add(new { nombre = item.nombre, id_curso = item.id_curso });
            }
            return Json(listas, JsonRequestBehavior.AllowGet);
        }

        //id_edi_cap_cur = id_capacitacion_curso que se modifica                 t_edi_nom_cur = nombre_curso           id_ed_cur=id_curso
        public ActionResult EditarCurso(int id_academia, int? id_capacitacion, int? id_edi_cap_cur, string t_edi_nom_cur, int? id_ed_cur)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                //Si es nulo significa el curso no esta asignado a ninguna categoria
                if (id_edi_cap_cur == null)
                {
                    using (DbContextTransaction tran = db.Database.BeginTransaction())
                    {
                        try
                        {
                            Curso curso = db.Curso.Find(id_ed_cur);
                            curso.nombre = t_edi_nom_cur;
                            curso.fecha_modificacion = DateTime.Now;
                            curso.id_usuario_modificacion = usuario.usuario.id_usuario;
                            db.Entry(curso).State = EntityState.Modified;
                            db.SaveChanges();

                            //Si el id_capacitacion no es nula, quiere decir que se asignara el curso a una capacitacion
                            if (id_capacitacion != null)
                            {
                                Capacitacion_Curso capacitacion_curso = new Capacitacion_Curso();
                                capacitacion_curso.id_curso = curso.id_curso;
                                capacitacion_curso.id_capacitacion = (int)id_capacitacion;
                                capacitacion_curso.fecha_creacion = DateTime.Now;
                                capacitacion_curso.id_usuario_creacion = usuario.usuario.id_usuario;
                                capacitacion_curso.activo = true;
                                capacitacion_curso.eliminado = false;
                                db.Capacitacion_Curso.Add(capacitacion_curso);
                                db.SaveChanges();
                            }
                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            ModelState.AddModelError("", "Error. Los cambios no se han completado.");
                        }
                    }
                }
                else //Se esta modificando un curso asignado a una categoria
                {
                    using (DbContextTransaction tran = db.Database.BeginTransaction())
                    {
                        try
                        {
                            Capacitacion_Curso capacitacion_curso = db.Capacitacion_Curso.Find(id_edi_cap_cur);
                            Curso curso = db.Curso.Find(capacitacion_curso.id_curso);
                            curso.nombre = t_edi_nom_cur;
                            curso.fecha_modificacion = DateTime.Now;
                            curso.id_usuario_modificacion = usuario.usuario.id_usuario;
                            db.Entry(curso).State = EntityState.Modified;
                            db.SaveChanges();

                            if (id_capacitacion != null)
                            {
                                //Se elimi
                                capacitacion_curso.id_capacitacion = (int)id_capacitacion;
                                capacitacion_curso.fecha_modificacion = DateTime.Now;
                                capacitacion_curso.id_usuario_modificacion = usuario.usuario.id_usuario;
                                db.Entry(capacitacion_curso).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else //Si la capacitacion es nula quiere decir que el curso dejara de ser parte de la misma
                            {
                                //Se elimina el curso de la categoria
                                capacitacion_curso.fecha_eliminacion = DateTime.Now;
                                capacitacion_curso.id_usuario_eliminacion = usuario.usuario.id_usuario;
                                capacitacion_curso.activo = false;
                                capacitacion_curso.eliminado = true;
                                db.Entry(capacitacion_curso).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            ModelState.AddModelError("", "Error. Los cambios no se han completado.");
                        }
                    }
                }
            }
            return RedirectToAction("Index", new { id = id_academia });
 
        }
     
        // POST: rrhh/Capacitacion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult CrearCapacitacion(int id_academia, int[] id_cursos, string nombre_capacitacion)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Capacitacion capacitacion = new Capacitacion();
                        capacitacion.nombre = nombre_capacitacion;
                        capacitacion.activo = true;
                        capacitacion.eliminado = false;
                        capacitacion.id_academia = id_academia;
                        capacitacion.fecha_creacion = DateTime.Now;
                        UsuarioTO usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        capacitacion.id_usuario_creacion = usuario.usuario.id_usuario;
                        db.Capacitacion.Add(capacitacion);
                        db.SaveChanges();
                        if (id_cursos != null)
                        {

                            foreach (var curso in id_cursos)
                            {
                                Capacitacion_Curso capacitacion_curso = new Capacitacion_Curso();
                                capacitacion_curso.id_curso = curso;
                                capacitacion_curso.id_capacitacion = capacitacion.id_capacitacion;
                                capacitacion_curso.activo = true;
                                capacitacion_curso.eliminado = false;
                                capacitacion_curso.fecha_creacion = DateTime.Now;
                                capacitacion_curso.id_usuario_creacion = usuario.usuario.id_usuario;
                                db.Capacitacion_Curso.Add(capacitacion_curso);
                                db.SaveChanges();
                            }
                        }
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
            return RedirectToAction("Index", new { id = id_academia });
        }

        // GET: rrhh/Capacitacion/Edit/5
        public ActionResult Edit(int? id, int id_academia)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Capacitacion capacitacion = db.Capacitacion.Find(id);
            if (capacitacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_academia = id_academia;
            return View(capacitacion);
        }

        // POST: rrhh/Capacitacion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string nombre, int id_capacitacion, int id_academia)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        UsuarioTO usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        Capacitacion capacitacion = db.Capacitacion.Find(id_capacitacion);
                        capacitacion.fecha_modificacion = DateTime.Now;
                        capacitacion.nombre = nombre;
                        capacitacion.id_usuario_modificacion = usuario.usuario.id_usuario;
                        db.Entry(capacitacion).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch 
                    {
                        tran.Rollback();
                        ModelState.AddModelError("", "Error. Cambios no realizados.");
                    }
                    
                }
            }
            return RedirectToAction("List", new { id_academia = id_academia });
        }

        // GET: rrhh/Capacitacion/Delete/5
        public ActionResult Delete(int? id, int id_academia)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Capacitacion capacitacion = db.Capacitacion.Find(id);
            if (capacitacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_academia = id_academia;
            return View(capacitacion);
        }

        // POST: rrhh/Capacitacion/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id_capacitacion, int id_academia)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Capacitacion capacitacion = db.Capacitacion.Find(id_capacitacion);
                    capacitacion.activo = false;
                    capacitacion.eliminado = true;
                    UsuarioTO usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                    capacitacion.id_usuario_eliminacion = usuario.usuario.id_usuario;
                    capacitacion.fecha_eliminacion = DateTime.Now;
                    db.Entry(capacitacion).State = EntityState.Modified;
                    db.SaveChanges();
                    var capacitacion_curso = db.Capacitacion_Curso.Where(e => !e.eliminado && e.id_capacitacion == id_capacitacion);
                    foreach (var item in capacitacion_curso)
                    {
                        item.activo = false;
                        item.eliminado = true;
                        item.fecha_eliminacion = DateTime.Now;
                        item.id_usuario_eliminacion = usuario.usuario.id_usuario;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    ModelState.AddModelError("", "Error. Cambios no realizados.");
                }
            }
            return RedirectToAction("List", new { id_academia = id_academia });
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
