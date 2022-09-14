using System.ComponentModel.DataAnnotations;

namespace SuperHeroAPI
{
    public class Countries
    {
        [Key]
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string IOSCode { get; set; }


    }
}
