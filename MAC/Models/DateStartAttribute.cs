using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MAC.Models
{
    public sealed class DateStartAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dateStart = (DateTime)value;
            // Meeting must start in the future time.
            //return (dateStart > DateTime.Now);
            return true;
        }
    }

    public sealed class DateEndAttribute : ValidationAttribute
    {
        public string DateStartProperty { get; set; }

        public override bool IsValid(object value)
        {
            // Get Value of the DateStart property
            string dateStartString = HttpContext.Current.Request[DateStartProperty];
            DateTime dateEnd = (DateTime)value;
            DateTime dateStart = dateStartString == null ? DateTime.Now : DateTime.Parse(dateStartString);

            //Obter data inicial por partes
            String startDayString = dateStart.Day.ToString(); int startDayInt = Convert.ToInt32(startDayString);
            String startMonthString = dateStart.Month.ToString(); int startMonthInt = Convert.ToInt32(startMonthString);
            String StartYearString = dateStart.Year.ToString(); int StartYearInt = Convert.ToInt32(StartYearString);

            //Obter data final por partes
            String endDayString = dateEnd.Day.ToString(); int endDayInt = Convert.ToInt32(endDayString);
            String endMonthString = dateEnd.Month.ToString(); int endMonthInt = Convert.ToInt32(endMonthString);
            String endYearString = dateEnd.Year.ToString(); int endYearInt = Convert.ToInt32(endYearString);

            if(dateStart < dateEnd)
            {
                if ((startDayInt >= 21 && startMonthInt == 09) || (startMonthInt > 09 && startMonthInt <= 12))//Se o dia e o mês da data inial for maior que 21 de Novembro
                {
                    //string FiscalYearInitial = startMonthString + "/" + startDayString + "/" + StartYearString; //(MM, dd, yyyy) - Ele vai guardar a data nessa variavel

                    if ((endDayInt <= 20 && endMonthInt == 09 && (endYearInt == (StartYearInt + 1))) //Se a data final tiver o dia menor igual a 20, e o mês igual ao mês de Novembro e o ano igual ao ano da data inicial + 1
                        || (endMonthInt < 9 && (endYearInt == (StartYearInt + 1))) //Se o a data final tiver o mês menor que Novembro e o ano igual ao ano da data inicial + 1
                        || (endMonthInt > 9 && endMonthInt <= 12 && (endYearInt == StartYearInt )) //Se a data final tiver um mês maior que Novembro e Menor que Dezembro, e o ano igual ao da data inicial
                        || (endDayInt > 21 && endMonthInt == 9 && (endYearInt == StartYearInt))) // Se a data final for maior que dia 21, mas o mês igual ao mês de Novembro e o ano igual ao da data inicial
                    {
                        // FiscalYearFinal = startMonthString + "/" + startDayString + "/" + StartYearString;
                        return true;
                    } 
                    else
                    {
                        return false;
                    }

                }
                else
                if (startDayInt >= 01 && startMonthInt >= 01)
                {
                    //string FiscalYearInitial = startMonthString + "/" + startDayString + "/" + StartYearString;

                    if ((endDayInt <= 20 && endMonthInt == 09 && (endYearInt == StartYearInt)) //Se a data final tiver o dia menor igual a 20, e o mês igual ao mês de Novembro e o ano igual ao ano da data inicial
                        || (endMonthInt < 9 && (endYearInt == StartYearInt))) //Se o a data final tiver o mês menor que Novembro e o ano igual ao ano da data inicial
                    {
                        // string FiscalYearFinal = startMonthString + "/" + startDayString + "/" + StartYearString;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                    
                
            }
            else
            {
                return false;
            }
            

        }
    }

}