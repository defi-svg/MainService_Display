using MainService.Data;
using MainService.Helpers;
using MainService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MainService.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class DisplayController : ControllerBase
    {
        private readonly SqlConnector Connector;
        private readonly Parametars Parametars;

        public DisplayController(SqlConnector connector, IConfiguration Configuration)
        {
            Connector = connector;
            Parametars = Configuration.GetSection("Parametars").Get<Parametars>();

        }


        /// <param name="Request">
        /// Command 
        ///  <b>GetDisplay</b> -> Object.Type = int32 (0 = all, >0 GetDisplay) - sve informacije o displayu koji je vezan za Id kamere
        ///  <b>DisplayShowText</b> -> Object.Type=TextRequestClass 
        /// </param>
        // POST api/<controller>


        [HttpPost]
        public BaseResponseClass<object> Post([FromBody]Up<object> Request)
        {
            BaseResponseClass<bool> Prijava = null;

            using (Token t = new Token(Connector))
            {
                Prijava = t.CheckUser(Request.UserName, Request.Password);
            }


            if (!Prijava.Object)
            {
                return new BaseResponseClass<object>()
                {
                    ErrorId = 4,
                    ErrorDescription = "Invalid Token",
                    Object = new List<Models.PlateHistory> { }
                };

            }

            Logger.logError("DisplayController-> in", Newtonsoft.Json.JsonConvert.SerializeObject(Request));
            Logger.logError("DisplayController-> in", Convert.ToString(Request.Object));

            using (Data.Display t = new Data.Display(Connector))
            {
                try
                {
                    switch (Request.Command)
                    {
                        //podaci display-a 
                        case "GetDisplay":
                            return t.GetDisplay(Request.Object.GetType() == typeof(bool) ? -1 : Convert.ToInt32(Request.Object)).AsObject;
                            break;
                        //slanje poruke display-u za prikaz
                        case "DisplayShowText":
                            return t.SetTextDisplayAsync(Newtonsoft.Json.JsonConvert.DeserializeObject<Models.TextRequestClass>(Newtonsoft.Json.JsonConvert.SerializeObject(Request.Object))).Result.AsObject;
                            break;
                    }
                }
                catch (Exception ee)
                {
                    Logger.logError("DisplayController", ee);

                }
                return new BaseResponseClass<object>();
            }
        }
    }
}
