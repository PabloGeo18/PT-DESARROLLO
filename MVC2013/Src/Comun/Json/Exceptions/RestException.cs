using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC2013.Src.Comun.Json.Exceptions
{
    public class RestException: System.Exception
    {

        public RestException(){}

        public RestException(string message) : base(message){}

        public RestException(string message, System.Exception inner) : base(message, inner){}

    }
}