using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC2013.Src.Comun
{
    public class CommonSelectTO
    {
                
        // Resumen:
        //     Obtiene o establece un valor que indica si la instancia de System.Web.WebPages.Html.SelectListItem
        //     está seleccionada.
        //
        // Devuelve:
        //     true si el elemento de la lista de selección está seleccionado; en caso contrario,
        //     false.
        public bool Selected { get; set; }
        //
        // Resumen:
        //     Obtiene o establece el texto que se usa para mostrar la instancia de System.Web.WebPages.Html.SelectListItem
        //     en una página web.
        //
        // Devuelve:
        //     El texto que se usa para mostrar el elemento de la lista de selección.
        public string Text { get; set; }
        //
        // Resumen:
        //     Obtiene o establece el valor del atributo HTML value del elemento HTML option
        //     asociado con la instancia de System.Web.WebPages.Html.SelectListItem.
        //
        // Devuelve:
        //     Valor del atributo HTML value asociado con el elemento de la lista de selección.
        public int Value { get; set; }

        public CommonSelectTO() {}

        public CommonSelectTO(int _value, string _text) {
            this.Value = _value;
            this.Text = _text;
        }

        public CommonSelectTO(int _value, string _text, bool _selected)
        {
            this.Value = _value;
            this.Text = _text;
            this.Selected = _selected;
        }

    }
}