using System.Collections.Generic;

namespace AmazonFarm
{
    public class Config
    {
        public string Domain { get; set; } = "smile.amazon.com";
        public string Email { get; set; }
        public string Password { get; set; }
        public List<AsinGroup> AsinGroups { get; set; }
    }
}
