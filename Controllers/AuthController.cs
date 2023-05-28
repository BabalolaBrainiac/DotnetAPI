using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Models.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static System.DateTime;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly DapperDataContext _dapper;

    public AuthController(IConfiguration configuration)
    {
        _config = configuration;
        _dapper = new DapperDataContext(configuration);
    }

    [HttpPost("Register")]
    public IActionResult RegisterUser(UserForRegistrationDto userForRgistrationDto)
    {
        Console.WriteLine("Gets here");
        if (userForRgistrationDto.Password == userForRgistrationDto.ConfirmPassword)
        {
            var userExistsQuery = @"SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" +
                                  userForRgistrationDto.Email + "'";

            var existingUser = _dapper.LoadData<string>(userExistsQuery);

            Console.WriteLine(existingUser);

            if (existingUser.Count() == 0)
            {
                var passwordSalt = new byte[128 / 8];

                using (var randomNumberGenerator = RandomNumberGenerator.Create())
                {
                    randomNumberGenerator.GetNonZeroBytes(passwordSalt);
                }

                var passwordHash = HashPassword(userForRgistrationDto.Password, passwordSalt);


                // string sqlAddAuth = @"
                //     INSERT INTO TutorialAppSchema.Auth  ([Email],
                //     [PasswordHash],
                //     [PasswordSalt]) VALUES ('" + userForRegistration.Email +
                //     "', @PasswordHash, @PasswordSalt)";

                // List<SqlParameter> sqlParameters = new List<SqlParameter>();

                // SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                // passwordSaltParameter.Value = passwordSalt;

                // SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                // passwordHashParameter.Value = passwordHash;

                // sqlParameters.Add(passwordSaltParameter);
                // sqlParameters.Add(passwordHashParameter);

                var sqlAddAuthQuery =
                    @"INSERT INTO TutorialAppSchema.Auth ([Email], [PasswordHash], [PasswordSalt]) VALUES ('" +
                    userForRgistrationDto.Email + "', @PasswordHash, @PasswordSalt)";

                var sqlParameters = new List<SqlParameter>();

                var sqlPasswordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                sqlPasswordSaltParameter.Value = passwordSalt;

                var sqlPasswordHashParameter = new SqlParameter("PasswordHash", SqlDbType.VarBinary);
                sqlPasswordHashParameter.Value = passwordHash;

                sqlParameters.Add(sqlPasswordHashParameter);
                sqlParameters.Add(sqlPasswordSaltParameter);

                if (_dapper.ExecuteSqlWithParameters(sqlAddAuthQuery, sqlParameters))
                {
                    var sqlAddUser = @"
                            INSERT INTO TutorialAppSchema.Users(
                                [FirstName],
                                [LastName],
                                [Email],
                                [Gender],
                                [Active]
                            ) VALUES (" +
                                     "'" + userForRgistrationDto.FirstName +
                                     "', '" + userForRgistrationDto.LastName +
                                     "', '" + userForRgistrationDto.Email +
                                     "', '" + userForRgistrationDto.Gender +
                                     "', 0)";
                    if (_dapper.ExecuteSql(sqlAddUser)) return Ok();
                    return Ok();
                }

                throw new Exception("Failed to register User");
            }

            throw new Exception("User is already registered, kindly login");
        }

        throw new Exception("Passwords do not match");
    }

    [HttpPost("Login")]
    public IActionResult LoginUser(UserForLoginDto userForLoginDto)
    {
        var loggedInUser = new User
        {
            Email = "test@email.com",
            FirstName = "Babalola Opeyemi",
            LastName = "Daniel",
            Active = true,
            Gender = "Male",
            UserId = 5
        };

        // string sqlQuery = @"SELECT [PasswordHash], PasswordSalt FROM TutorialAppShema.Auth WHERE Email =  '" + userForLoginDto.Email + "'";
        //
        // UserForLoginConfirmationDto user = _dapper.LoadSingle<UserForLoginConfirmationDto>(sqlQuery);
        // byte[] passwordHash = HashPassword(userForLoginDto.Password, user.PasswordSalt);
        //
        // for(int i = 0; i < passwordHash.Length; i++)
        // {
        //     if(passwordHash[i] != user.PasswordHash[i])
        //     {
        //         return StatusCode(401, "Password is incorrect. Enter correct password and try again");
        //     }
        // }

        if (userForLoginDto.Email == loggedInUser.Email)
            return Ok(new Dictionary<string, string>
            {
                { "token", CreateJwtToken(loggedInUser.UserId) }
            });


        throw new Exception("Invalid Email Used");
    }

    private byte[] HashPassword(string password, byte[] passwordSalt)
    {
        var passwordSaltPlusKey = _config.GetSection("AppSettings:PasswordKey").Value
                                  + Convert.ToBase64String(passwordSalt);

        return KeyDerivation.Pbkdf2(
            password,
            Encoding.ASCII.GetBytes(passwordSaltPlusKey),
            KeyDerivationPrf.HMACSHA256,
            100000,
            256 / 8
        );
    }

    private string CreateJwtToken(int userId)
    {
        Claim[] claim =
        {
            new("userId", userId.ToString())
        };

        var tokenKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config.GetSection("Appsettings:TokenKey").Value));

        var signingCredentials = new SigningCredentials(
            tokenKey,
            SecurityAlgorithms.HmacSha512
        );

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claim),
            SigningCredentials = signingCredentials,
            Expires = Now.AddDays(1)
        };

        var handler = new JwtSecurityTokenHandler();

        var token = handler.CreateToken(securityTokenDescriptor);

        return handler.WriteToken(token);
    }
}