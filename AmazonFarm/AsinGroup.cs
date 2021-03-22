using System.Collections.Generic;

namespace AmazonFarm
{
    public struct AsinGroup
    {
        public List<string> Asins { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
    }
}
