namespace SuperHeroAPI
{
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ExpireDate { get; set; } 
    }
}
