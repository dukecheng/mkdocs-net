using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Niusys.Docs.Core.Models
{
    public class TokenSettings
    {
        /// <summary>
        ///  The Issuer (iss) claim for generated tokens.
        /// </summary>
        public string Issuer { get; set; } = "MKDocs";

        /// <summary>
        /// The Audience (aud) claim for the generated tokens.
        /// </summary>
        public string Audience { get; set; } = "MKDocs";

        public int ExpirationMinutes { get; set; } = 60 * 8;

        /// <summary>
        /// The expiration time for the generated tokens.
        /// </summary>
        /// <remarks>The default is five minutes (300 seconds).</remarks>
        public TimeSpan Expiration => TimeSpan.FromMinutes(ExpirationMinutes);

        public string SigningKey { get; set; } = $"AC8E2B29-8DB3-493B-ADBF-DEE631209166";

        public SymmetricSecurityKey GetSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SigningKey));
        }

        public SigningCredentials GetSigningCredentials()
        {
            return new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256);
        }

        /// <summary>
        /// 是否验证Session有效性
        /// </summary>
        public bool IsVerifyTokenPerRequest { get; set; }

        /// <summary>
        /// CachedDeviceSession在Cache中缓存的时间
        /// </summary>
        public int SessionMemoryCacheSeconds { get; set; }
    }
}
