using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC2013.Src.Comun.Exception
{
    public class ControlledException : System.Exception
    {
        public ControlledException(){}

        public ControlledException(string message) : base(message){}

        public ControlledException(string message, System.Exception inner) : base(message, inner) { }
    }
}