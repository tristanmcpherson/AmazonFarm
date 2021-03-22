using System.Collections.Generic;

namespace AmazonFarm
{
    /// <summary>
    /// Hardcoded data, for now
    /// </summary>
    public class Data
    {
        static List<string> Nv3060 = new List<string> {
            "B08WM28PVH"
        };

        static List<string> Nv3070 = new List<string> {
            "B08LF1CWT2", "B08L8KC1J7", "B08L8L71SM"
        };

        static List<string> Nv3080 = new List<string> {
            "B08HH5WF97", "B08HR5SXPS", "B08HR55YB5"
        };

        static List<string> Amd6900XT = new List<string> {
            "B08QQFW9YS", "B08PHWJC8X", "B08PDQJVD9"
        };

        public static List<AsinGroup> asins = new List<AsinGroup> {
            new AsinGroup {
                Asins = Nv3060,
                MinPrice = 350,
                MaxPrice = 450
            },
            new AsinGroup {
                Asins = Nv3070,
                MinPrice = 600,
                MaxPrice = 850
            },
            new AsinGroup {
                Asins = Nv3080,
                MinPrice = 600,
                MaxPrice = 1000
            },
            new AsinGroup {
                Asins = Amd6900XT,
                MinPrice = 900,
                MaxPrice = 1100
            }
        };
    }
}
