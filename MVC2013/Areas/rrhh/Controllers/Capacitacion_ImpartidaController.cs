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
    public class Capacitacion_ImpartidaController : Controller
    {
        private AppEntities db = new AppEntities();

        public JsonResult ExisteEmpleado(int id_empleado)
        {
            object resultado = new object();
            Empleado empleado = db.Empleado.SingleOrDefault(e=>e.activo && e.id_empleado == id_empleado);
            if(empleado == null)
            {
                resultado = new { error = true };
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            resultado = new { error = false, id_empleado, nombre = empleado.primer_apellido + ", " + empleado.primer_nombre };
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        // GET: rrhh/Capacitacion_Impartida
        public ActionResult Index()
        {
            var capacitacion_Impartida = db.Capacitacion_Impartida.Include(c => c.Usuarios).Include(c => c.Usuarios1).Include(c => c.Usuarios2).Include(c => c.Usuarios3).Include(c => c.Usuarios4).Include(c => c.Usuarios5);
            return View(capacitacion_Impartida.ToList());
        }

        // GET: rrhh/Capacitacion_Impartida/Details/5
        public ActionResult Details(int? id, int id_academia)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Capacitacion_Impartida capacitacion_Impartida = db.Capacitacion_Impartida.Find(id);
            if (capacitacion_Impartida == null)
            {
                return HttpNotFound();
            }
            Curso_EmpleadoModel curso_empleado = new Curso_EmpleadoModel();
            curso_empleado.capacitacion_impartida = capacitacion_Impartida;
            curso_empleado.id_academia = id_academia;
            foreach (var item in db.Curso_Empleado.Where(e => !e.eliminado && e.id_capacitacion_impartida == id).GroupBy(e => e.id_empleado).Where(g => g.Any()).Select(g => new { id_empleado = g.Max(e => e.id_empleado), nombre = g.FirstOrDefault().Empleado.primer_nombre + " " + g.FirstOrDefault().Empleado.primer_apellido }))
            {
                curso_empleado.empleados.Add(item.id_empleado.ToString() + "|" + item.nombre );
            }

            return View(curso_empleado);
        }

        // GET: rrhh/Capacitacion_Impartida/Create
        public ActionResult Create(int id_academia)
        {
            Capacitacion_Impartir capacitacion_impartir = new Capacitacion_Impartir();
            var capacitacion = db.Capacitacion.Where(e => !e.eliminado && e.id_academia == id_academia).OrderBy(e => e.nombre).ToList();
            ViewBag.id_capacitacion = new SelectList(capacitacion, "id_capacitacion", "nombre");
            ViewBag.id_academia = id_academia;
            ViewBag.id_instructor = new SelectList(from s in db.Instructor where !s.eliminado select new { FullName = s.primer_nombre+ " " + s.primer_apellido},"id_instructor", "FullName");
            capacitacion_impartir.instructor = db.Instructor.Where(e => !e.eliminado).OrderBy(e => e.primer_apellido).ToList();
            capacitacion_impartir.cursos = db.Curso.Where(e => !e.eliminado);
            return View(capacitacion_impartir);
        }

        [HttpPost]
        public void GuardarGrupo(DateTime fecha_inicio, DateTime fecha_fin, int[] id_cursos, int[] id_empleados, int id_academia, int[] id_instructores)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        UsuarioTO usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        Capacitacion_Impartida capacitacion_impartida = new Capacitacion_Impartida();
                        capacitacion_impartida.activo = true;
                        capacitacion_impartida.eliminado = false;
                        capacitacion_impartida.fecha_inicio = fecha_inicio;
                        capacitacion_impartida.fecha_fin = fecha_fin;
                        capacitacion_impartida.fecha_creacion = DateTime.Now;
                        capacitacion_impartida.id_usuario_creacion = usuario.usuario.id_usuario;
                        capacitacion_impartida.id_academia = id_academia;
                        db.Capacitacion_Impartida.Add(capacitacion_impartida);
                        db.SaveChanges();

                        for (int i = 1; i < id_cursos.Count(); i++)
                        {
                            for (int j = 1; j < id_empleados.Count(); j++)
                            {
                                Curso_Empleado curso_empleado = new Curso_Empleado();
                                curso_empleado.activo = true;
                                curso_empleado.eliminado = false;
                                curso_empleado.id_empleado = id_empleados[j];
                                curso_empleado.id_curso = id_cursos[i];
                                curso_empleado.id_usuario_creacion = usuario.usuario.id_usuario;
                                curso_empleado.fecha_creacion = DateTime.Now;
                                curso_empleado.id_capacitacion_impartida = capacitacion_impartida.id_capacitacion_impartida;
                                curso_empleado.id_instructor = id_instructores[i];
                                db.Curso_Empleado.Add(curso_empleado);
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
        }

        //Obtener los cursos de una capacitación
        public JsonResult GetCursos(int id_capacitacion)
        {
            List<object> cursos = new List<object>();
            List<Capacitacion_Curso> cursos_por_capacitacion = db.Capacitacion_Curso.Where(e => !e.eliminado && e.id_capacitacion == id_capacitacion).OrderBy(e => e.Curso.nombre).ToList();
            foreach (var item in cursos_por_capacitacion)
            {
                cursos.Add(new { nombre = item.Curso.nombre, id_capacitacion_curso = item.id_capacitacion_curso, id_curso = item.id_curso });
            }
            //Obtener los instructores
            string select = "<select class=\"form-control\"><option value=\"0\">-</option>";
            foreach(var item in db.Instructor.Where(e=>!e.eliminado))
            {
                select += "<option value=\"" + item.id_instructor.ToString() +"\">" + item.primer_nombre + " " + item.primer_apellido + "</option>";
            }
            select += "</select>";
            var listas = new { curso = cursos, select_instructor = select };
            return Json(listas, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCursosEmpleado(int id_empleado, int id_capacitacion_impartida)
        {
            List<object> cursos_empleados = new List<object>();
            foreach (var curso_empleado in db.Curso_Empleado.Where(e=>!e.eliminado && e.id_empleado == id_empleado && e.id_capacitacion_impartida == id_capacitacion_impartida))
            {
                cursos_empleados.Add(new { id_ce = curso_empleado.id_curso_empleado, id_curso = curso_empleado.id_curso, nombre = curso_empleado.Curso.nombre, 
                    nota = curso_empleado.nota
                });
            }
            return Json(cursos_empleados, JsonRequestBehavior.AllowGet);
        }

        //Remover el curso del empleado en una capacitación
        [HttpPost]
        public void RemoverCurso(int id_curso_empleado)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Curso_Empleado curso_empleado = db.Curso_Empleado.Find(id_curso_empleado);
                    curso_empleado.eliminado = true;
                    curso_empleado.activo = false;
                    curso_empleado.fecha_eliminacion = DateTime.Now;
                    curso_empleado.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(curso_empleado).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }
        }

        [HttpPost]
        public ActionResult EliminarCurso(int id)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Curso_Empleado curso_empleado = db.Curso_Empleado.Find(id);
                    curso_empleado.eliminado = true;
                    curso_empleado.activo = false;
                    curso_empleado.fecha_eliminacion = DateTime.Now;
                    curso_empleado.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(curso_empleado).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return Json(new { msg = "", response = true });
                }
                catch
                {
                    tran.Rollback();
                    return Json(new { msg = "Cambios no guardados.", response = false });
                }
            }
        }


        //Guardar la nota del empleado en determinado curso
        [HttpPost]
        public void GuardarNota(int id_curso_empleado, int nota)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Curso_Empleado curso_empleado = db.Curso_Empleado.Find(id_curso_empleado);
                    curso_empleado.nota = nota;
                    curso_empleado.fecha_modificacion = DateTime.Now;
                    curso_empleado.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(curso_empleado).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }
        }

        //Subir el archivo con la calificación del curso
        [HttpPost]
        public void SubirNota(HttpPostedFileBase file, int id_curso_empleado)
        {
            if (file != null && file.ContentLength > 0)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        string name = file.FileName;
                        string extension = name.Substring(name.LastIndexOf('.'));
                        string ruta = "~/Archivos/Notas/" + id_curso_empleado.ToString() + extension;
                        file.SaveAs(Server.MapPath(ruta));
                        Curso_Empleado curso_empleado = db.Curso_Empleado.Find(id_curso_empleado);
                        Archivo archivo = db.Archivo.Find(curso_empleado.id_archivo);
                        if (archivo == null)
                        {
                            archivo = new Archivo();
                            archivo.activo = true; archivo.eliminado = false;
                            archivo.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                            archivo.ubicacion = ruta;
                            archivo.nombre = file.FileName;
                            archivo.fecha_creacion = DateTime.Now;
                            archivo.id_tipo_archivo = (int)Catalogos.Tipo_Archivo.Notas;
                            db.Archivo.Add(archivo);
                            db.SaveChanges();
                            curso_empleado.id_archivo = archivo.id_archivo;
                            db.Entry(curso_empleado).State = EntityState.Modified;
                            db.SaveChanges();
                            Archivo_Empleado archivo_empleado = new Archivo_Empleado();
                            archivo_empleado.id_empleado = curso_empleado.id_empleado;
                            archivo_empleado.id_archivo = archivo.id_archivo;
                            archivo_empleado.fecha_creacion = DateTime.Now;
                            archivo_empleado.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        }
                        else
                        {
                            archivo.nombre = file.FileName;
                            archivo.ubicacion = ruta;
                            archivo.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                            archivo.fecha_modificacion = DateTime.Now;
                            db.Entry(archivo).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
        }

        //Obtener los intructores
        public JsonResult GetInstructores()
        {
            //Obtener los instructores
            string select = "<select class=\"form-control\"><option value=\"0\">-</option>";
            foreach (var item in db.Instructor.Where(e => !e.eliminado))
            {
                select += "<option value=\"" + item.id_instructor.ToString() + "\">" + item.primer_nombre + " " + item.primer_apellido + "</option>";
            }
            select += "</select>";
            return Json(select, JsonRequestBehavior.AllowGet);
        }

        //Editar una capacitacion impartida
        [HttpPost]
        public ActionResult EditarCapacitacionImpartida(int id_academia, DateTime fecha_inicio, DateTime fecha_fin, int id_capacitacion_impartida)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Capacitacion_Impartida capacitacion_impartida = db.Capacitacion_Impartida.Find(id_capacitacion_impartida);
                        capacitacion_impartida.fecha_fin = fecha_fin;
                        capacitacion_impartida.fecha_inicio = fecha_inicio;
                        capacitacion_impartida.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        capacitacion_impartida.fecha_modificacion = DateTime.Now;
                        db.Entry(capacitacion_impartida).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        ModelState.AddModelError("", "Cambios no efectuados.");
                    }
                }
            }
            return RedirectToAction("Index", "Capacitacion", new { id = id_academia });
        }


        // GET: rrhh/Capacitacion_Impartida/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Capacitacion_Impartida capacitacion_Impartida = db.Capacitacion_Impartida.Find(id);
            if (capacitacion_Impartida == null)
            {
                return HttpNotFound();
            }
            return View(capacitacion_Impartida);
        }

        // POST: rrhh/Capacitacion_Impartida/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_capacitacion_impartida,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion,activo,eliminado,fecha_inicio,fecha_fin")] Capacitacion_Impartida capacitacion_Impartida)
        {
            if (ModelState.IsValid)
            {
                db.Entry(capacitacion_Impartida).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(capacitacion_Impartida);
        }

        // GET: rrhh/Capacitacion_Impartida/Delete/5
        public ActionResult Delete(int id, int id_academia)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Capacitacion_Impartida capacitacion_Impartida = db.Capacitacion_Impartida.Find(id);
                    capacitacion_Impartida.activo = false;
                    capacitacion_Impartida.eliminado = true;
                    capacitacion_Impartida.fecha_eliminacion = DateTime.Now;
                    capacitacion_Impartida.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(capacitacion_Impartida).State = EntityState.Modified;
                    db.SaveChanges();
                    foreach (var curso_empleado in db.Curso_Empleado.Where(e => !e.eliminado && e.id_capacitacion_impartida == id))
                    {
                        curso_empleado.activo = false;
                        curso_empleado.eliminado = true;
                        curso_empleado.fecha_eliminacion = DateTime.Now;
                        curso_empleado.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        db.Entry(curso_empleado).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    ModelState.AddModelError("", "Cambios no guardados.");
                }
            }
            return RedirectToAction("Index", "Capacitacion", new { id = id_academia });
        }

        [HttpPost]
        public ActionResult EliminarCapacitacionImpartida(int id)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Capacitacion_Impartida capacitacion_Impartida = db.Capacitacion_Impartida.Find(id);
                    capacitacion_Impartida.activo = false;
                    capacitacion_Impartida.eliminado = true;
                    capacitacion_Impartida.fecha_eliminacion = DateTime.Now;
                    capacitacion_Impartida.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(capacitacion_Impartida).State = EntityState.Modified;
                    db.SaveChanges();
                    foreach (var curso_empleado in db.Curso_Empleado.Where(e => !e.eliminado && e.id_capacitacion_impartida == id))
                    {
                        curso_empleado.activo = false;
                        curso_empleado.eliminado = true;
                        curso_empleado.fecha_eliminacion = DateTime.Now;
                        curso_empleado.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        db.Entry(curso_empleado).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    tran.Commit();
                    return Json(new { msg = "", response = true });
                }
                catch
                {
                    tran.Rollback();
                    return Json(new { msg = "Cambios no guardados.", response = false });
                }
            }
        }



        // POST: rrhh/Capacitacion_Impartida/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Capacitacion_Impartida capacitacion_Impartida = db.Capacitacion_Impartida.Find(id);
            db.Capacitacion_Impartida.Remove(capacitacion_Impartida);
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
