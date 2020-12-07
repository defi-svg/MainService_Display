using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainService.Helpers
{
    public class Logger
    {
        /// <summary>
        /// Kreiranje i pisanje log file-a loklano na serveru
        /// </summary>
        /// <param name="method">naziv metode</param>
        /// <param name="ex">exception iz catch-a</param>
        public static void logError(string method, Exception ex)
        {
            try
            {
                string error;

                if (ex == null)
                {
                    error = DateTime.Now.ToString("dd.MM.yyyy. HH:mm:ss.fff") + " " + method;
                }
                else
                {
                    error = DateTime.Now.ToString("dd.MM.yyyy. HH:mm:ss.fff") + " " + method + " --- Exception:" + ex.Message;
                    if (ex.InnerException != null)
                    {
                        error += " inner ex:" + ex.InnerException.Message;

                        if (ex.InnerException.InnerException != null)
                            error += " inner_inner ex:" + ex.InnerException.InnerException.Message;

                        if (ex.InnerException.InnerException.InnerException != null)
                            error += " inner_inner_inner ex:" + ex.InnerException.InnerException.InnerException.Message;

                    }

                }

                string logFile = Environment.ExpandEnvironmentVariables(
                        @"%SystemDrive%\MainService\Log\"+ DateTime.Now.ToString("yyyy-MM-dd") + ".log");
                using (System.IO.TextWriter file = System.IO.File.AppendText(logFile))
                {
                    file.WriteLine(error);
                }
            }
            catch { } //samo neće upisati u log, ako bude došlo do greške
        }

        /// <summary>
        /// Kreiranje i pisanje log file-a loklano na serveru
        /// </summary>
        /// <param name="method">naziv metode</param>
        /// <param name="ex">opis greške</param>
        public static void logError(string method, string ex)
        {
            try
            {
                string error;

                if (ex == null)
                {
                    error = DateTime.Now.ToString("dd.MM.yyyy. HH:mm:ss.fff") + " " + method;
                }
                else
                {
                    error = DateTime.Now.ToString("dd.MM.yyyy. HH:mm:ss.fff") + " " + method + " --> :" + ex;


                }

                string logFile = Environment.ExpandEnvironmentVariables(
                        @"%SystemDrive%\MainService\Log\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");
                using (System.IO.TextWriter file = System.IO.File.AppendText(logFile))
                {
                    file.WriteLine(error);
                }
            }
            catch { }//samo neće upisati u log, ako bude došlo do greške
        }

    }

}
