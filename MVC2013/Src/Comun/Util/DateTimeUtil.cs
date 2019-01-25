using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace MVC2013.Src.Comun.Util
{
    public class DateTimeUtil
    {


        public static List<CommonSelectTO> getDaysOfWeekList()
        {        
            List<CommonSelectTO> select = new List<CommonSelectTO>();            
            select.Add(new CommonSelectTO((int)DayOfWeek.Sunday, App_GlobalResources.Resources.domingo));
            select.Add(new CommonSelectTO((int)DayOfWeek.Monday, App_GlobalResources.Resources.lunes));
            select.Add(new CommonSelectTO((int)DayOfWeek.Tuesday, App_GlobalResources.Resources.martes));
            select.Add(new CommonSelectTO((int)DayOfWeek.Wednesday, App_GlobalResources.Resources.miercoles));
            select.Add(new CommonSelectTO((int)DayOfWeek.Thursday, App_GlobalResources.Resources.jueves));
            select.Add(new CommonSelectTO((int)DayOfWeek.Friday, App_GlobalResources.Resources.viernes));
            select.Add(new CommonSelectTO((int)DayOfWeek.Saturday, App_GlobalResources.Resources.sabado));
            return select;
        }

        public static List<CommonSelectTO> getDaysOfWeekList(IEnumerable<int> ids)
        {
            List<CommonSelectTO> select = new List<CommonSelectTO>();
            select.Add(new CommonSelectTO((int)DayOfWeek.Sunday, App_GlobalResources.Resources.domingo, true));
            select.Add(new CommonSelectTO((int)DayOfWeek.Monday, App_GlobalResources.Resources.lunes, ids.Any(x => x == (int)DayOfWeek.Monday)));
            select.Add(new CommonSelectTO((int)DayOfWeek.Tuesday, App_GlobalResources.Resources.martes, ids.Any(x => x == (int)DayOfWeek.Tuesday)));
            select.Add(new CommonSelectTO((int)DayOfWeek.Wednesday, App_GlobalResources.Resources.miercoles, ids.Any(x => x == (int)DayOfWeek.Wednesday)));
            select.Add(new CommonSelectTO((int)DayOfWeek.Thursday, App_GlobalResources.Resources.jueves, ids.Any(x => x == (int)DayOfWeek.Thursday)));
            select.Add(new CommonSelectTO((int)DayOfWeek.Friday, App_GlobalResources.Resources.viernes, ids.Any(x => x == (int)DayOfWeek.Friday)));
            select.Add(new CommonSelectTO((int)DayOfWeek.Saturday, App_GlobalResources.Resources.sabado, ids.Any(x => x == (int)DayOfWeek.Saturday)));
            return select;
        }


        public static List<SelectListItem> getDaysOfWeek()
        {

            List<SelectListItem> list = new List<SelectListItem> { 
                new SelectListItem { Text = App_GlobalResources.Resources.domingo, Value = (int)DayOfWeek.Sunday + "", Selected = false },
                new SelectListItem { Text = App_GlobalResources.Resources.lunes,  Value = (int)DayOfWeek.Monday + "", Selected = false },
                new SelectListItem { Text = App_GlobalResources.Resources.martes,  Value = (int)DayOfWeek.Tuesday + "", Selected = false },
                new SelectListItem { Text = App_GlobalResources.Resources.miercoles,  Value = (int)DayOfWeek.Wednesday + "", Selected = false },
                new SelectListItem { Text = App_GlobalResources.Resources.jueves,  Value = (int)DayOfWeek.Thursday + "", Selected = false },
                new SelectListItem { Text = App_GlobalResources.Resources.viernes,  Value = (int)DayOfWeek.Friday + "", Selected = false },
                new SelectListItem { Text = App_GlobalResources.Resources.sabado,  Value = (int)DayOfWeek.Saturday + "", Selected = false },
                };
            return list;

        }

        public static List<System.Web.Mvc.SelectListItem> getMonths(bool selectCurrent) 
        {
            List<System.Web.Mvc.SelectListItem> list = new List<System.Web.Mvc.SelectListItem> {
                new System.Web.Mvc.SelectListItem { Text = App_GlobalResources.Resources.enero, Value = "1", Selected = true },
                new System.Web.Mvc.SelectListItem { Text = App_GlobalResources.Resources.febrero, Value = "2", Selected = false },
                new System.Web.Mvc.SelectListItem { Text = App_GlobalResources.Resources.marzo, Value = "3", Selected = false },
                new System.Web.Mvc.SelectListItem { Text = App_GlobalResources.Resources.abril, Value = "4", Selected = false },
                new System.Web.Mvc.SelectListItem { Text = App_GlobalResources.Resources.mayo, Value = "5", Selected = false },
                new System.Web.Mvc.SelectListItem { Text = App_GlobalResources.Resources.junio, Value = "6", Selected = false },
                new System.Web.Mvc.SelectListItem { Text = App_GlobalResources.Resources.julio, Value = "7", Selected = false },
                new System.Web.Mvc.SelectListItem { Text = App_GlobalResources.Resources.agosto, Value = "8", Selected = false },
                new System.Web.Mvc.SelectListItem { Text = App_GlobalResources.Resources.septiembre, Value = "9", Selected = false },
                new System.Web.Mvc.SelectListItem { Text = App_GlobalResources.Resources.octubre, Value = "10", Selected = false },
                new System.Web.Mvc.SelectListItem { Text = App_GlobalResources.Resources.noviembre, Value = "11", Selected = false },
                new System.Web.Mvc.SelectListItem { Text = App_GlobalResources.Resources.diciembre, Value = "12", Selected = false }
            };
            if (selectCurrent) 
            {
                foreach (var item in list) 
                {
                    int month = DateTime.Now.Month;
                    if (int.Parse(item.Value) == month) 
                    {
                        item.Selected = true;
                        break;

                    }
                }
            }
            return list;
        }

        public static List<System.Web.Mvc.SelectListItem> getYears(int interval) 
        {
            int index = 1;
            int currentYear = DateTime.Now.Year;
            List<System.Web.Mvc.SelectListItem> list = new List<System.Web.Mvc.SelectListItem>();
            list.Add(new System.Web.Mvc.SelectListItem { Text = DateTime.Now.Year.ToString(), Value = DateTime.Now.Year.ToString(), Selected = true });
            while (index <= interval) 
            {
                list.Add(new System.Web.Mvc.SelectListItem { Text = (currentYear + index).ToString(), Value = (currentYear + index).ToString(), Selected = false });
                list.Add(new System.Web.Mvc.SelectListItem { Text = (currentYear - index).ToString(), Value = (currentYear - index).ToString(), Selected = false });
                index++;
            }
            return list.OrderBy(item => item.Value).ToList();
        }


        public static List<SelectListItem> getDaysOfWeek(IEnumerable<int> ids)
        {

            List<SelectListItem> list = new List<SelectListItem> { 
                new SelectListItem { Text = App_GlobalResources.Resources.domingo, Value = (int)DayOfWeek.Sunday + "", Selected = ids.Any(x => x == (int)DayOfWeek.Sunday) },
                new SelectListItem { Text = App_GlobalResources.Resources.lunes,  Value = (int)DayOfWeek.Monday + "", Selected = ids.Any(x => x == (int)DayOfWeek.Monday) },
                new SelectListItem { Text = App_GlobalResources.Resources.martes,  Value = (int)DayOfWeek.Tuesday + "", Selected = ids.Any(x => x == (int)DayOfWeek.Tuesday) },
                new SelectListItem { Text = App_GlobalResources.Resources.miercoles,  Value = (int)DayOfWeek.Wednesday + "", Selected = ids.Any(x => x == (int)DayOfWeek.Wednesday) },
                new SelectListItem { Text = App_GlobalResources.Resources.jueves,  Value = (int)DayOfWeek.Thursday + "", Selected = ids.Any(x => x == (int)DayOfWeek.Thursday) },
                new SelectListItem { Text = App_GlobalResources.Resources.viernes,  Value = (int)DayOfWeek.Friday + "", Selected = ids.Any(x => x == (int)DayOfWeek.Friday) },
                new SelectListItem { Text = App_GlobalResources.Resources.sabado,  Value = (int)DayOfWeek.Saturday + "", Selected = ids.Any(x => x == (int)DayOfWeek.Saturday) },
                };
            return list;

        }

        
  
        public static CommonSelectTO getCommonSelectDayByIdEnum(int id)
        {

            switch (id)
            {
                case (int)DayOfWeek.Sunday:
                    return new CommonSelectTO((int)DayOfWeek.Sunday, App_GlobalResources.Resources.domingo);
                case (int)DayOfWeek.Monday:
                    return new CommonSelectTO((int)DayOfWeek.Monday, App_GlobalResources.Resources.lunes);
                case (int)DayOfWeek.Tuesday:
                    return new CommonSelectTO((int)DayOfWeek.Tuesday, App_GlobalResources.Resources.martes);
                case (int)DayOfWeek.Wednesday:
                    return new CommonSelectTO((int)DayOfWeek.Wednesday, App_GlobalResources.Resources.miercoles);
                case (int)DayOfWeek.Thursday:
                    return new CommonSelectTO((int)DayOfWeek.Thursday, App_GlobalResources.Resources.jueves);
                case (int)DayOfWeek.Friday:
                    return new CommonSelectTO((int)DayOfWeek.Friday, App_GlobalResources.Resources.viernes);
                case (int)DayOfWeek.Saturday:
                    return new CommonSelectTO((int)DayOfWeek.Saturday, App_GlobalResources.Resources.sabado);
                default:
                    return new CommonSelectTO();
            }
        }


    }
}