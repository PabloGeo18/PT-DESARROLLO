using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC2013.Src.Comun.To
{
    public class Arbol
    {
        private static int UniqueId = 0;
        const string View = "~/Areas/SalaConteo/Views/Shared/_ArbolPartial.cshtml";

        public static int GetUniqueId(){
            UniqueId = UniqueId + 1;
            return UniqueId;
        }

        public static Dictionary<string, string> Vistas = new Dictionary<string, string> {
            { "Paquetes",  "~/Areas/SalaConteo/Views/Shared/_ArbolPaquetesPartial.cshtml"}
        };

        public List<ArbolTo> Children = new List<ArbolTo>();
        public string GetType;
    }
}