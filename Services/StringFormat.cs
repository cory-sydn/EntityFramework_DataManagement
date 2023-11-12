using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

namespace ExcelToMSSQL.Services
{
    public static class StringFormat
    {
        public static string Clean(this string s)
        {
            StringBuilder sb = new StringBuilder(s);

            sb.Replace(",", "");
            sb.Replace("  ", "");
            sb.Replace(" ", "");
            sb.Replace("'", "");
            sb.Replace(".", "");
            sb.Replace("(", "");
            sb.Replace(")", "");

            return sb.ToString();
        }

        public static string CleanName(this string addr)
        {
            StringBuilder sb = new StringBuilder(addr);
            sb.Replace("'", "''");
            sb.Replace("*", " ");

            return sb.ToString();
        }

        public static decimal? CleanMoney(this string s)
        {
            StringBuilder sb = new StringBuilder();

            // Remove non-numeric characters and replace decimal separator
            // ENGLISH FORMAT
            // string numericValue = s.Replace(".", "").Replace(",", ".").Replace(" TL", "");
            // TURKISH FORMAT
            //string numericValue = s.Replace(".", "").Replace(" TL", "");

            // Parse the string as a decimal
            /* if (decimal.TryParse(numericValue, out decimal result))
             {
                 // Convert the decimal to the desired format and append to StringBuilder
                 sb.AppendFormat("{0:0.00}", result);
                 string formattedValue = sb.ToString();
                 Console.WriteLine(formattedValue);  // Output: 130641.00
             }
             else
             {
                 // Handle the case where the parsing failed
                 Console.WriteLine("Invalid money value format.");
             }     */
            decimal val;
            if (!decimal.TryParse(s.Replace(" TL", "").Replace(",", "").Replace(".", ""), NumberStyles.Number, CultureInfo.InvariantCulture, out val))
                return null;
            return val / 100;

        }
        
        public static DateTime? SortableDate(this string dateStr)
        {
            DateTime sortableDate;

            //var cultureInfo = CultureInfo.GetCultureInfo("en-US");

            //DateTime date = DateTime.ParseExact(dateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            //sortableDate = date.ToString("yyyy-MM-dd  h:mm:ss tt");

/*
            if (DateTime.TryParseExact(dateStr, "M/d/yyyy", CultureInfo.CreateSpecificCulture("en-US") , DateTimeStyles.None, out DateTime date))
                   {
                       //sortableDate = dateStr.ToString("yyyy-MM-dd");
                       sortableDate = Convert.ToDateTime(date.ToString("yyyy-MM-dd"));
                   }
                   else
                   {
                       Console.WriteLine("Invalid date format!");
                       sortableDate = DateTime.MinValue;
                   }            
           */
            //sortableDate = covertedDate.Replace(" 00:00:00", "" );
            
            // parse date string myself
            string[] splittedDate = dateStr.Split(' ')[0].Split('.');
            DateTime date1 = new DateTime(Int32.Parse(splittedDate[2]), Int32.Parse(splittedDate[1]), Int32.Parse(splittedDate[0]), 0, 0, 0);
            Console.WriteLine(date1);
            sortableDate = date1;
            
            return sortableDate;
        }

        public static string SortableDateTime(this string dateStr)
        {
            string sortableDate;

            //var cultureInfo = CultureInfo.GetCultureInfo("en-US");

            //DateTime date = DateTime.ParseExact(dateStr, "dd.MM.yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            //sortableDate = date.ToString("yyyy-MM-dd");


            if (DateTime.TryParseExact(dateStr, "M/d/yyyy h:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out DateTime date))
            {
                //sortableDate = dateStr.ToString("yyyy-MM-dd");
                sortableDate = date.ToString("yyyy-MM-dd h:mm:ss tt");
            }
            else
            {
                Console.WriteLine("Invalid date format!");
                sortableDate = "0000-00-00";
            }

            //sortableDate = covertedDate.Replace(" 00:00:00", "" );

            /* string[] splittedDate = dateStr.Split(' ')[0].Split('.');
             DateTime date1 = new DateTime(Int32.Parse(splittedDate[2]), Int32.Parse(splittedDate[1]), Int32.Parse(splittedDate[0]), 0, 0, 0);
             Console.WriteLine(date1);
             sortableDate = date1.ToString("s").Split('T')[0];
             */
            return sortableDate;
        }

        public static string PaidStatement(this string input)
        {
            string output = "";
            if(input == "Done" || input == "Paid")
            {
                output = "Certificate of Release";
            }
            else
            {
                output = input;
            }
            return output;
        }

        public static string FourDigitHour(this string timeString)
        {            
            string FourDigit = string.Empty;

            if (TimeSpan.TryParse(timeString, out TimeSpan time))
            {
                DateTime dateTime = DateTime.MinValue.Add(time);
                FourDigit = dateTime.ToString("HH:mm"); // Output: 08:00
            }
            else
            {
                Console.WriteLine("Invalid time format.");
            }
            return FourDigit;
        }

    }
}

