using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        DapperDataContext _dapperDataContext;
        public UserController(IConfiguration configuration) 
        {
            _dapperDataContext = new DapperDataContext(configuration);

        }

        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers() 
        {

            string sql = @"
            SELECT 
                [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            FROM TutorialAppSchema.Users";

            IEnumerable<User> users = _dapperDataContext.LoadData<User>(sql);
            return users;
            // return new string[] {"String1", "String2", testValue};

        }

        [HttpGet("GetSingleUser/{userId}")]
        public User getSingleUserById(int userId)
        {
            var sql = @"
            SELECT 
                [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            FROM TutorialAppSchema.Users
            WHERE UserId = " + userId.ToString();

            User user = _dapperDataContext.LoadSingle<User>(sql);
            return user;
            // return _dapperDataContext.LoadSingle<DateTime>("SELECT GETDATE()");
        }

        // [HttpPut("EditUser")]
        // public IActionResult EditUser()
        // {

        //     string sql = @"
        //     UPDATE TutorialAppSchema.Users
        //     SET 

        //     ";
        //     return Ok();
        // }

        [HttpPost]
        public IActionResult CreateUser()
        {
            return Ok();
        }

    }
}