using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainService.Models
{
    public class Display
    {
        public int CameraId { get; set; }
        public bool IsEnterance { get; set; }
        public string IpAddress { get; set; }
        public int? IpPort { get; set; }
        public string DefaultText { get; set; }
        public bool Active { get; set; }
        public int Type { get; set; }
        public int DelayTextGreen { get; set; }
        public int DelayTextRed { get; set; }

    }

}
