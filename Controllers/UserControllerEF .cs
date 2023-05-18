using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserControllerEF  : ControllerBase
    {
        EntityFramewordDataContext _entityFramewordDataContext;

        public UserControllerEF(IConfiguration configuration)
        {
            _entityFramewordDataContext = new EntityFramewordDataContext(configuration);
        }


        [HttpGet("GetUsersEF")]
        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> users = _entityFramewordDataContext.Users.ToList<User>();
            return users;
        }

        [HttpGet("GetSingleEF")]
        public User GetSingleUser(int userId)
        {
            User? userDb = _entityFramewordDataContext.Users
            .Where(user => user.UserId == userId )
            .FirstOrDefault<User>();

            if(userDb != null) 
            {
                return userDb;
            }

            throw new Exception("Could not retrieve user with given ID");
        }


        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
              User? userDb = _entityFramewordDataContext.Users
            .Where(user => user.UserId == user.UserId )
            .FirstOrDefault<User>();

            if(userDb != null) 
            {
               userDb.Active = user.Active;
               userDb.FirstName = user.FirstName;
               userDb.LastName = user.LastName;
               userDb.Email = user.Email;
               userDb.Gender = user.Gender;

               if(_entityFramewordDataContext.SaveChanges() > 0)
               {
                return Ok();

               }

               throw new Exception("Could not edit user");
            }

            throw new Exception("User does not exist");
        }


        [HttpPost("AddUser")]
        public IActionResult AddUser(UserDTO user)
        {
            User newUser = new User();

               newUser.Active = user.Active;
               newUser.FirstName = user.FirstName;
               newUser.LastName = user.LastName;
               newUser.Email = user.Email;
               newUser.Gender = user.Gender;

               _entityFramewordDataContext.Add(newUser);

               if(_entityFramewordDataContext.SaveChanges() > 0)
               {
                return Ok();

               }

               throw new Exception("Could not add new User");
        }
        

        [HttpDelete("Delete/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            User? userDb = _entityFramewordDataContext.Users
                .Where(user => user.UserId == user.UserId )
                .FirstOrDefault<User>();

            if(userDb != null)
            {
                _entityFramewordDataContext.Users.Remove(userDb);

                if(_entityFramewordDataContext.SaveChanges() > 0)
                {
                    return Ok();
                }

                throw new Exception("Could not delete user");
            }

            throw new Exception("Could not find user");
        }
    }
}