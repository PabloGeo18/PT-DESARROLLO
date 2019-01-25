using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MVC2013.Src.Comun.Json
{
    [DataContract]
    class SimpleMessage
    {
        [DataMember]
        public string Message { get; set; }
    }
}
