namespace Audiochan.Core.Common.Models
{
    public class PasswordSetting
    {
        public bool RequireUppercase { get; set; } = false;
        public bool RequireLowercase { get; set; } = false;
        public bool RequireNonAlphanumeric { get; set; } = false;
        public bool RequireDigit { get; set; } = false;
        public int RequireLength { get; set; } = 4;
    }
}