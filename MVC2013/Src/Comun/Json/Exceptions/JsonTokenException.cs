using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC2013.Src.Comun.Json.Exceptions
{
    public class JsonTokenException: System.Exception
    {

        public JsonTokenException(){}

        public JsonTokenException(string message) : base(message){}

        public JsonTokenException(string message, System.Exception inner) : base(message, inner) { }

    }
}