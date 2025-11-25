namespace USR5_Login.Entities
{
    public class Token
    {

        public string Access_Token { get; set; } = string.Empty;
        public string Refresh_Token { get; set; } = string.Empty;
        public DateTime Expires_In { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime RefreshTokenExpires { get; set; }   

    }
}
