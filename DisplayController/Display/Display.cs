using MainService.Helpers;
using MainService.Models;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using MainService.Helpers.Display;
using System.Timers;
using System.Threading.Tasks;

namespace MainService.Data
{
    public class Display : BaseData
    {
        private static Timer timer;
        private static TextRequestClass _textRequest;
        private static string _endpoint;

        public Display(SqlConnector connector) : base(connector)
        {
        }

        /// <summary>
        /// Slanje poruke na display
        /// </summary>
        /// <param name="txt">podaci o poruci potrebno za endpoint display-a</param>
        /// <returns>true/false ovisno da li je slanje usješno</returns>
        public async Task<BaseResponseClass<bool>> SetTextDisplayAsync(TextRequestClass txt)
        {
            string endpoint = "";
            int interval = 0;

            _textRequest = txt;

            BaseResponseClass<List<Models.Display>> listDisplay = new BaseResponseClass<List<Models.Display>>();
            listDisplay = GetDisplay(0);

            List<Models.Display> disp = new List<Models.Display>();
            disp = (List<Models.Display>)listDisplay.Object;

            Models.Display d = disp.FirstOrDefault(a => a.CameraId == Convert.ToInt32(txt.CameraId));

            if (d != null && d.Type == 1) //ako se radi o novom tipo display-a potrebno postaviti endpoint koji se poziva
            {
                endpoint = "https://" + d.IpAddress + ":" + d.IpPort + "/api/Examples/ScrollTwoLines";
            }
            else
            {
                endpoint = ""; //endpoint mora biti prazan za slanje na stari display
            }

            _endpoint = endpoint;

            //podešavanje textRequst koji se šalje za defaultni ekran
            //defaultni text ako ima | znači da ide u drugi red
            string[] defaultText = d.DefaultText.Split('|');
            _textRequest.text1 = "";
            _textRequest.text2 = "";

            if (defaultText.Length > 1)
            {
                for (int i = 0; i < defaultText.Length; i++)
                {
                    if (i == 0) _textRequest.text1 = defaultText[i];
                    else
                    {
                        _textRequest.text2 += defaultText[i] + " ";
                    }
                }
            }

            _textRequest.color1 = "G"; //komanda potrebno za slanje zelene boje na display
            _textRequest.color2 = "G";

            //koliko se drži poruka ovisno ako je dobra (G-green) ili loša (R- red)
            if (txt.color1 == "R" || txt.color2 == "R")
            {
                timer.Interval = interval = d.DelayTextRed;
              
            }
            if (txt.color1 == "G" || txt.color2 == "G")
            {
                timer.Interval = interval= d.DelayTextGreen;
            }

            DisplayRepository dispRep = new DisplayRepository();
            //slanje poruke na display sa informacijom
            await dispRep.GetShowText(endpoint, txt.text1, txt.text2, txt.color1, txt.color2);
            //trajanje prikaza poruke
            await Task.Delay(interval);
            //slanje defaultne poruke na display
            return await dispRep.GetShowText(_endpoint, _textRequest.text1, _textRequest.text2, _textRequest.color1, _textRequest.color2);

        }



        /// <summary>
        /// Get podataka display-a iz baze
        /// </summary>
        /// <param name="Id">null- svi, id- jedna specifičan</param>
        /// <returns>Popis display-a</returns>
        public BaseResponseClass<List<Models.Display>> GetDisplay(int? Id)
        {
            BaseResponseClass<List<Models.Display>> rez = new BaseResponseClass<List<Models.Display>>
            {
                Object = new List<Models.Display>(),
                ErrorDescription = "",
                ErrorId = 0

            };

            List<Models.Display> at = new List<Models.Display>();

            using (SqlConnection conn = Connector.Clone())
            {
                if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                SqlCommand cmd = new SqlCommand("GetDisplay", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CameraId", Id); //svaka kamera ima svoj odgovarajući display

                cmd.Prepare();
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Models.Display d = new Models.Display();

                            d.CameraId = Convert.ToInt32(reader["CameraId"]);
                            d.IsEnterance = Convert.ToBoolean(reader["IsEnterance"]);
                            d.IpAddress = reader["DisplayIpAddress"].ToString();
                            d.IpPort = Convert.ToInt32(reader["DisplayIpPort"]);
                            d.DefaultText = reader["DefaultDisplayText"].ToString();
                            d.Active = Convert.ToBoolean(reader["DisplayActive"]);
                            d.Type = Convert.ToInt32(reader["DisplayType"]);
                            d.DelayTextGreen = Convert.ToInt32(reader["TimeGreen"]);
                            d.DelayTextRed = Convert.ToInt32(reader["TimeRed"]);
                            at.Add(d);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Logger.logError("GetDisplay", ex.Message);
                    rez.ErrorId = 1;
                    rez.ErrorDescription = ex.Message;
                    rez.Object = at;
                    conn.Close();
                    return rez;
                }
                rez.Object = at;
                conn.Close();

            }
            return rez;

        }



    }
}
