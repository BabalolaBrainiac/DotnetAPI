using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Data;

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

        [HttpGet("users/{testValue}")]
        public string[] get(string testValue) 
        {
            return new string[] {"String1", "String2", testValue};

        }

        [HttpGet("TestDB")]
        public DateTime test()
        {
            return _dapperDataContext.LoadSingle<DateTime>("SELECT GETDATE()");
        }

    }
}