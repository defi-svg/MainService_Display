namespace MainService.Models
{
    public class DisplayRequestClass
    {
        public string text1 { get; set; }
        public string text2 { get; set; }
        public string font_name_1 { get; set; }
        public string font_name_2 { get; set; }
        public int r1 { get; set; }
        public int g1 { get; set; }
        public int b1 { get; set; }
        public int r2 { get; set; }
        public int g2 { get; set; }
        public int b2 { get; set; }
        public int delay { get; set; }
        public int bgR { get; set; }
        public int bgG { get; set; }
        public int bgB { get; set; }
        public bool scroll1 { get; set; }
        public bool scroll2 { get; set; }


    }


    public class TextRequestClass
    {
        public int CameraId { get; set; } 
        public string text1 { get; set; }
        public string text2 { get; set; }
        public string color1 { get; set; }
        public string color2 { get; set; }
    }
}