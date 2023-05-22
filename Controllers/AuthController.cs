using System.Text;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetAPI.Data;
using DotnetAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Data.SqlClient;
using System.Data;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        IConfiguration _config;

        DapperDataContext _dapper;

        public AuthController(IConfiguration configuration)
        {
            _config = configuration;
            _dapper = new DapperDataContext(configuration);

        }

        [HttpPost("Register")]
        public IActionResult RegisterUser(UserForRegistrationDto userForRgistrationDto) 
        {
            if(userForRgistrationDto.Password == userForRgistrationDto.ConfirmPassword)
            {

                string userExistsQuery = "SELECT Email from TutorialAppSchema.Auth WHERE " + 
                userForRgistrationDto.Email + "'";

                IEnumerable<string> existingUser = _dapper.LoadData<string>(userExistsQuery);

                if(existingUser.Count() == 0) 
                {

                    byte[] passwordSalt = new byte[128/8];

                    using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
                    {
                        randomNumberGenerator.GetNonZeroBytes(passwordSalt);
                    }

                   byte[] passwordHash = HashPassword(userForRgistrationDto.Password, passwordSalt);

                   string sqlAddAuthQuery = @"INSERT INTO TutorialAppSchema.Auth 
                   ([Email], [PasswordHash], [PasswordSalt]) VALUES 
                   ('" + userForRgistrationDto.Email + "' @PasswordHash, @PasswordSalt)";

                   List<SqlParameter> sqlParameters = new();

                   SqlParameter sqlPasswordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                   sqlPasswordSaltParameter.Value = passwordSalt;   

                   SqlParameter sqlPasswordHashParameter = new SqlParameter("PasswordHash", SqlDbType.VarBinary);
                   sqlPasswordHashParameter.Value = passwordHash;

                   sqlParameters.Add(sqlPasswordHashParameter);
                   sqlParameters.Add(sqlPasswordSaltParameter);

                   if(_dapper.ExecuteSqlWithParameters(sqlAddAuthQuery, sqlParameters))
                   {
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

            string sqlQuery = @"SELECT [PasswordHash], PasswordSalt FROM TutorialAppShema.Auth WHERE Email =  '" + userForLoginDto.Email + "'";

            UserForLoginConfirmationDto user = _dapper.LoadSingle<UserForLoginConfirmationDto>(sqlQuery);
            byte[] passwordHash = HashPassword(userForLoginDto.Password, user.PasswordSalt);

            for(int i = 0; i < passwordHash.Length; i++)
            {
                if(passwordHash[i] != user.PasswordHash[i])
                {
                    return StatusCode(401, "Password is incorrect. Enter correct password and try again");
                }
            }

            return Ok();

        }

        private byte[] HashPassword(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusKey = _config.GetSection("AppSettings:PasswordKey").Value 
                   + Convert.ToBase64String(passwordSalt);

                   return KeyDerivation.Pbkdf2(
                    password: password,
                    salt: Encoding.ASCII.GetBytes(passwordSaltPlusKey),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8
                   );
        }
    }
}