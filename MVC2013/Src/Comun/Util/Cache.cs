using MVC2013.Models;
using MVC2013.Src.Seguridad.To;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC2013.Src.Comun.Util
{
    /* Sistema de cache para los usuarios logueados el cual contiene un diccionario con 
     * todos los objetos usuario con sus permisos respectivos para poder manejar el 
     * sistema de seguridad por roles
     */
    public class Cache
    {
        public static Dictionary<String, UsuarioTO> DiccionarioUsuariosLogueados = new Dictionary<string, UsuarioTO>();
    }
}