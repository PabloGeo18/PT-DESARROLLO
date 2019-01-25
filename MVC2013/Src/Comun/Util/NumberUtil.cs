using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MVC2013.Src.Comun.Util
{
    public class NumberUtil
    {
        static int semilla;
        static Random rnd;

        public static string cadenaNumerosAleatoria(int cantidad)
        {
            // Un objeto StrngBuilder para mejorar el rendimiento
            // del manejo de cadenas, además indicando la longitud
            // el rendimiento es mejor todavía
            StringBuilder s = new StringBuilder(cantidad);

            Random obj = new Random();
            string posibles = "0123456789";
            int longitud = posibles.Length;
            char letra;
            //int longitudnuevacadena = 5;
            //string nuevacadena = "";
            for (int i = 0; i < cantidad; i++){
                letra = posibles[obj.Next(longitud)];
                //nuevacadena += letra.ToString();
                s.Append(letra);
            }
            return s.ToString();
        }
    }
}