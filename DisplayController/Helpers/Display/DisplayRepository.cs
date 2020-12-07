
using BusinessClasses.Models;
using MainService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Timers;

namespace MainService.Helpers.Display
{
    public class DisplayRepository : DisposableObject
    {
        Microsoft.AspNetCore.Http.IHttpContextAccessor cnt { get; set; }
        private static string _endpoint;
        public ServiceParametars Connection { get; set; }

        public DisplayRepository()
        {
        
        }
          


        public string UserId
        {
            get
            {
                if (cnt == null || cnt.HttpContext == null || cnt.HttpContext.User == null || cnt.HttpContext.User.Claims == null)
                {
                    return "";
                }
                return cnt.HttpContext.User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier).Value;
            }
        }


        //poziv Get za poslati dva reda text-a na display
        public async Task<BaseResponseClass<Boolean>> GetShowText(string Endpoint, string text1, string text2, string color1, string color2)
        {
            _endpoint = Endpoint;
            DisplayRequestClass disp = new DisplayRequestClass();
            object obj = null;

            BaseResponseClass<Boolean> rez = new BaseResponseClass<bool>()
            {
                ErrorDescription = "",
                ErrorId = 0,
                Object = false
            };

            if (Endpoint == "") //pošto starim display-em se ne komunicira preko servisa
            {
                rez.ErrorDescription = "Nije novi display";
                rez.ErrorId = 2;
                return rez;
            }

            //ovisno o boji R ili G podešavanje RGB vrijednosti
            switch (color1) 
            {
                case "R":
                    disp.r1 = 255;
                    disp.g1 = 0;
                    disp.b1 = 0;
                    break;
                case "G":
                    // parametar b2 i g2 su izmjenjeni b- je green a g je blue - greška u servisu
                    disp.r1 = 0;
                    disp.g1 = 0;
                    disp.b1 = 255;
                    break;
            }
            switch (color2)
            {
                case "R":
                    disp.r2 = 255;
                    disp.g2 = 0;
                    disp.b2 = 0;
                    break;
                case "G":
                    // parametar b2 i g2 su izmjenjeni b- je green a g je blue- greška u servisu
                    disp.r2 = 0;
                    disp.g2 = 0;
                    disp.b2 = 255;
                    break;
            }


            disp.text1 = text1;
            disp.text2 = text2;
            disp.font_name_1 = "6x13.bdf";
            disp.font_name_2 = "6x13.bdf";
            disp.scroll1 = false;
            disp.scroll2 = false;

            try
            {
                using (CommunicationHelper ch = new CommunicationHelper()) obj = await ch.GetFromApi(Endpoint, disp);
                rez.Object = true;

                return rez;
            }
            catch (Exception ex)
            {
                Logger.logError("GetShowText", ex);
                rez.ErrorDescription = ex.Message;
                rez.ErrorId = 2;
                return rez;
            }
        }

    }

}

