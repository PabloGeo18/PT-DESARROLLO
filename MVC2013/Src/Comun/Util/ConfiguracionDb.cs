using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC2013.Models;
using System.ComponentModel;

namespace MVC2013.Src.Comun.Util
{
    public class ConfiguracionDb
    {
        //Get list of params
        public static List<string> Get(string key)
        {
            List<string> resultado = new List<string>();
            using (AppEntities _db = new AppEntities())
            {
                foreach (var item in _db.Parametros_Sistema.Where(p => p.clave == key))
                {
                    resultado.Add(item.valor);
                }
            }
            return resultado;
        }

        //Get list of params and convert to Type
        public static List<object> Get(string key, Type t) 
        {
            List<object> resultado = new List<object>();
            using (AppEntities _db = new AppEntities()) 
            {
                TypeConverter converter = TypeDescriptor.GetConverter(t);
                foreach (var item in _db.Parametros_Sistema.Where(p => p.clave == key)) 
                {
                    resultado.Add(converter.ConvertFrom(item.valor));
                }
            }
            return resultado;
        }

        //Get single param value
        public static string GetSingle(string key)
        {
            return Get(key).DefaultIfEmpty(null).SingleOrDefault();
        }

        //Get single param value or defaultValue
        public static string GetSingle(string key, string defaultValue)
        {
            return Get(key).DefaultIfEmpty(defaultValue).SingleOrDefault();
        }

        //Get single param value and convert to type
        public static object GetSingle(string key, Type t) 
        {
            return Get(key, t).DefaultIfEmpty(GetDefaultValue(t)).SingleOrDefault();
        }

        //Get single param value and convert to type or defaultValue
        public static object GetSingle(string key, Type t, object defaultValue)
        {
            return Get(key, t).DefaultIfEmpty(defaultValue).SingleOrDefault();
        }

        //Get first param value
        public static string GetFirst(string key)
        {
            return Get(key).DefaultIfEmpty(null).FirstOrDefault();
        }

        //Get first param value or defaultValue
        public static string GetFirst(string key, string defaultValue)
        {
            return Get(key).DefaultIfEmpty(defaultValue).FirstOrDefault();
        }

        //Get first param value and convert to type
        public static object GetFirst(string key, Type t)
        {
            return Get(key, t).DefaultIfEmpty(GetDefaultValue(t)).FirstOrDefault();
        }

        //Get first param value and convert to type or defaultValue
        public static object GetFirst(string key, Type t, object defaultValue)
        {
            return Get(key, t).DefaultIfEmpty(defaultValue).FirstOrDefault();
        }

        //Default value of type
        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}