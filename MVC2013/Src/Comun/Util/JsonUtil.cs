using MVC2013.Src.Comun.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MVC2013.Src.Comun.Util
{
    public class JsonUtil
    {


        public static HttpResponseMessage getJsonResponse(HttpRequestMessage request, Object obj)
        {
            return request.CreateResponse(HttpStatusCode.OK, obj);
        }

        public static HttpResponseMessage getJsonOkResponse(HttpRequestMessage request)
        {
            return request.CreateResponse(HttpStatusCode.OK, new JObject(new JProperty("message", "OK")));
        }


        public static HttpResponseMessage getJsonErrorResponse(HttpRequestMessage request, string error, HttpStatusCode code)
        {
            return request.CreateResponse(code, new JObject(new JProperty("message", error)));
        }

        public static HttpResponseMessage getJsonErrorResponse(HttpRequestMessage request, string error)
        {
            return request.CreateResponse(HttpStatusCode.BadRequest, new JObject( new JProperty("message", error)));
        }

        public static HttpResponseMessage getJsonForbiddenErrorResponse(HttpRequestMessage request, string error)
        {
            return request.CreateResponse(HttpStatusCode.Forbidden, new JObject(new JProperty("message", error)));
        }

        public static HttpResponseMessage getJsonInternalServerErrorResponse(HttpRequestMessage request, string error)
        {
            return request.CreateResponse(HttpStatusCode.InternalServerError, new JObject(new JProperty("message", error)));
        }

        public static HttpResponseMessage getJsonNotFoundErrorResponse(HttpRequestMessage request, string error)
        {
            return request.CreateResponse(HttpStatusCode.NotFound, new JObject(new JProperty("message", error)));
        }

        public static HttpResponseMessage getJsonUnauthorizedErrorResponse(HttpRequestMessage request)
        {
            return request.CreateResponse(HttpStatusCode.Unauthorized, new JObject(new JProperty("message", App_GlobalResources.Resources.usuario_no_autorizado_error)));
        }

    }
}