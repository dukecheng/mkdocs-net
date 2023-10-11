using AgileLabs;
using AgileLabs.AspNet.WebApis.Exceptions;
using AgileLabs.Securities;
using AgileLabs.Sessions;
using AgileLabs.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Niusys.Docs.Core.DataStores;
using Niusys.Docs.Core.Models;
using Niusys.Docs.Core.Projects;
using Niusys.Docs.Web.ApiModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Niusys.Docs.Web.ApiControllers;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IEncryptionService _encryptionService;
    private readonly IMapper _mapper;
    private readonly MkdocsDatabase _mkdocsDatabase;

    public AccountController(IEncryptionService encryptionService, IMapper mapper, MkdocsDatabase mkdocsDatabase)
    {
        _encryptionService = encryptionService;
        _mapper = mapper;
        _mkdocsDatabase = mkdocsDatabase;
    }

    [HttpPost, Route("login"), AllowAnonymous]

    public async Task<LoginResult> Login([FromBody] LoginRequest request, [FromServices] IRequestSession requestSession)
    {
        var userInfo = _mkdocsDatabase.Get<User>(x => x.Email == request.Email);
        if (userInfo == null)
        {
            throw new Exception("账户不存在");
        }

        if (userInfo.PasswordHash.IsNullOrEmpty())
        {
            // 如果未设置密码, 直接使用当前的密码
            userInfo.PasswordSalt = _encryptionService.CreateSaltKey(16);
            userInfo.PasswordHash = _encryptionService.CreatePasswordHash(request.Password, userInfo.PasswordSalt, "SHA256");
            _mkdocsDatabase.Update(userInfo);
        }

        string pwd = _encryptionService.CreatePasswordHash(request.Password, userInfo.PasswordSalt, "SHA256");

        if (userInfo.PasswordHash != pwd)
        {
            throw new ApiException("Wrong account or password");
        }



        var loginResult = _mapper.Map<LoginResult>(userInfo);
        var tokenInfo = await GenerateToken(GuidGenerator.GenerateDigitalUUID(), loginResult.Id, loginResult.Email, DateTime.Now);
        loginResult = _mapper.Map(tokenInfo, loginResult);
        return loginResult;
    }

    [HttpPost, Route("register"), AllowAnonymous]
    public async Task Register([FromBody] UserCreateRequest request)
    {
        var userInfo = _mkdocsDatabase.Get<User>(x => x.Email == request.Email);
        if (userInfo != null)
        {
            throw new Exception("账户已存在");
        }
        var worker = new Snowflake.Core.IdWorker(1, 1);

        userInfo = new User
        {
            Id = worker.NextId(),
            Email = request.Email,
            Password = request.Password,
        };
        userInfo.PasswordSalt = _encryptionService.CreateSaltKey(16);
        userInfo.PasswordHash = _encryptionService.CreatePasswordHash(request.Password, userInfo.PasswordSalt, "SHA256");
        _mkdocsDatabase.Insert(userInfo);
        await Task.CompletedTask;

    }
    #region Utils
    private async Task<TokenInfo> GenerateToken(string jwtid, long userId, string displayName, DateTime issued, Action<List<Claim>> claimHandler = null)
    {
        // Specifically add the jti (nonce), iat (issued timestamp), and sub (subject/user) claims.
        // You can add other claims here, if you want:
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name,displayName),
                new Claim(JwtRegisteredClaimNames.Jti, jwtid),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(issued).ToString(), ClaimValueTypes.Integer64),
            };

        if (claimHandler != null)
            claimHandler(claims);

        var tokenSettings = new TokenSettings();

        // Create the JWT and write it to a string
        var jwt = new JwtSecurityToken(
            issuer: tokenSettings.Issuer,
            audience: tokenSettings.Audience,
            claims: claims,
            notBefore: issued.AddMinutes(-1),
            expires: issued.Add(tokenSettings.Expiration),
            signingCredentials: tokenSettings.GetSigningCredentials());

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        var loginResult = new TokenInfo
        {
            Token = token,
            ExpiresIn = tokenSettings.ExpirationMinutes * 60,
        };

        //loginResult.Token = loginResult.Token.Insert(0, "Bearer ");

        return await Task.FromResult(loginResult);
    }

    /// <summary>
    /// Get this datetime as a Unix epoch timestamp (seconds since Jan 1, 1970, midnight UTC).
    /// </summary>
    /// <param name="date">The date to convert.</param>
    /// <returns>Seconds since Unix epoch.</returns>
    private static long ToUnixEpochDate(DateTime date)
    {
        return new DateTimeOffset(date).ToUniversalTime().ToUnixTimeSeconds();
    }
    #endregion

    [HttpGet]
    [Route("user_info")]
    public async Task<string> UserInfo()
    {
        await Task.CompletedTask;
        return string.Empty;
    }
}
