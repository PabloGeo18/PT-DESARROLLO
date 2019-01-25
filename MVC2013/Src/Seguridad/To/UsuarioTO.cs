using MVC2013.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC2013.Src.Seguridad.To
{
    public class UsuarioTO
    {
        public bool Valid { get; set; }
        public Usuarios usuario { get; set; }
        public string EmpresaDS { get; set; }
        public string IPAddress { get; set; }
        public string SessionId { get; set; }
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> DiccionarioAppAccesosLocal = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> DiccionarioAppAccesos {
            get { return DiccionarioAppAccesosLocal; }
            set { DiccionarioAppAccesosLocal = value; }
        }

        public void fillPermisions(string aplicacion, string controlador, string accion) {
            List<string> permisosDefault = new List<string> {
                "Mensaje"
            };
            if (DiccionarioAppAccesosLocal.ContainsKey(aplicacion))
            {
                // si contiene el controlador lo usa
                if (DiccionarioAppAccesosLocal[aplicacion].ContainsKey(controlador))
                {
                    //si no contiene la accion la agrega.
                    if (!DiccionarioAppAccesosLocal[aplicacion][controlador].ContainsKey(accion))
                    {
                        DiccionarioAppAccesosLocal[aplicacion][controlador].Add(accion, accion);
                    }
                    foreach (string accionDefault in permisosDefault) 
                    {
                        if (!DiccionarioAppAccesosLocal[aplicacion][controlador].ContainsKey(accionDefault))
                        {
                            DiccionarioAppAccesosLocal[aplicacion][controlador].Add(accionDefault, accionDefault);
                        }
                    }
                }
                //si no contiene el controlador lo crea
                else
                {
                    DiccionarioAppAccesosLocal[aplicacion].Add(controlador, new Dictionary<string, string>() { { accion, accion } });
                    foreach (string accionDefault in permisosDefault)
                    {
                        if (!DiccionarioAppAccesosLocal[aplicacion][controlador].ContainsKey(accionDefault))
                        {
                            DiccionarioAppAccesosLocal[aplicacion][controlador].Add(accionDefault, accionDefault);
                        }
                    }
                }
            }
            // si no contiene la aplicacion la crea
            else {
                DiccionarioAppAccesosLocal.Add(aplicacion, new Dictionary<string, Dictionary<string, string>> { { controlador, new Dictionary<string, string>() { { accion, accion } } } });
                foreach (string accionDefault in permisosDefault)
                {
                    if (!DiccionarioAppAccesosLocal[aplicacion][controlador].ContainsKey(accionDefault))
                    {
                        DiccionarioAppAccesosLocal[aplicacion][controlador].Add(accionDefault, accionDefault);
                    }
                }
            }   
        }

        public bool havePermissions(string aplicacion, string controlador, string accion)
        {
            bool havePermissions = false;
            if (DiccionarioAppAccesosLocal.ContainsKey(aplicacion)) {
                if (DiccionarioAppAccesosLocal[aplicacion].ContainsKey(controlador)) {
                    if (DiccionarioAppAccesosLocal[aplicacion][controlador].ContainsKey(accion)) {
                        havePermissions = true;
                    }
                }
            }
            return havePermissions;
        }

        public bool amenuhref(string aplicacion, string controlador, string accion)
        {
            bool havePermissions = true;
            return havePermissions;
        }


    }
}