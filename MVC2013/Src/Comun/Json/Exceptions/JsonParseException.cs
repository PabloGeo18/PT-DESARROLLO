using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC2013.Src.Comun.Json.Exceptions
{
    public class JsonParseException: System.Exception
    {

        public JsonParseException(){}

        public JsonParseException(string message) : base(message){}

        public JsonParseException(string message, System.Exception inner) : base(message, inner) { }

    }
}