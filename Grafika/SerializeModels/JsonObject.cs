using System;

namespace Grafika
{
    [Serializable()]
    class JsonObject
    {
        public string shapeType { get; set; }
        public double? x1 { get; set; }
        public double? y1 { get; set; }
        public double? x2 { get; set; }
        public double? y2 { get; set; }
        public double? width { get; set; }
        public double? height { get; set; }
        public double? r { get; set; }
    }
}
