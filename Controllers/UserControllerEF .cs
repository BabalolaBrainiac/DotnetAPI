using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Data.Repository;
using DotnetAPI.Models;
using DotnetAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserControllerEF  : ControllerBase
    {
        // EntityFramewordDataContext? _entityFramewordDataContext;

        IMapper _mapper;
        IUserRepository _userRepository;

        public UserControllerEF(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;

             _mapper = new Mapper(new MapperConfiguration(cfg =>{
            cfg.CreateMap<UserDTO, User>();
            cfg.CreateMap<UserSalary, UserSalary>();
            cfg.CreateMap<UserJobInfo, UserJobInfo>();
        }));

        }


        [HttpGet("GetUsersEF")]
        public IEnumerable<User> GetUsers()
        {
            // IEnumerable<User> users = _entityFramewordDataContext.Users.ToList<User>();
            IEnumerable<User> users = _userRepository.GetUsers();

            return users;
        }

        [HttpGet("GetSingleEF")]
        public User GetSingleUser(int userId)
        {
            User? userDb = _userRepository.GetSingleUser(userId);
            // .Where(user => user.UserId == userId )
            // .FirstOrDefault<User>();

            if(userDb != null) 
            {
                return userDb;
            }

            throw new Exception("Could not retrieve user with given ID");
        }


        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
              User userDb = _userRepository.GetSingleUser(user.UserId);

            if(userDb != null) 
            {
               userDb.Active = user.Active;
               userDb.FirstName = user.FirstName;
               userDb.LastName = user.LastName;
               userDb.Email = user.Email;
               userDb.Gender = user.Gender;


               if(_userRepository.SaveChanges())
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

               _userRepository.AddEntity<User>(newUser);


               if(_userRepository.SaveChanges())
               {
                return Ok();

               }

            //    _entityFramewordDataContext.Add(newUser);

            //    if(_entityFramewordDataContext.SaveChanges() > 0)
            //    {
            //     return Ok();

            //    }

               throw new Exception("Could not add new User");
        }
        

        [HttpDelete("Delete/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            User? userDb = _userRepository.GetSingleUser(userId);


            if(userDb != null)
            {
                // _entityFramewordDataContext.Users.Remove(userDb);

            _userRepository.RemoveEntity<User>(userDb);

                if(_userRepository.SaveChanges())
                {
                    return Ok();
                }

                throw new Exception("Could not delete user");
            }

            throw new Exception("Could not find user");
        }


        [HttpGet("UserSalary/{userId}")]
        public UserSalary GetUserSalaryEF(int userId)
        {
        return _userRepository.GetSingleUserSalary(userId);
        }

        [HttpPost("UserSalary")]
        public IActionResult PostUserSalaryEf(UserSalary userForInsert)
        {
            _userRepository.AddEntity<UserSalary>(userForInsert);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Adding UserSalary failed on save");
        }
        
        
        [HttpPut("UserSalary")]
        public IActionResult PutUserSalaryEf(UserSalary userForUpdate)
        {
            UserSalary? userToUpdate = _userRepository.GetSingleUserSalary(userForUpdate.UserId);

            if (userToUpdate != null)
            {
                _mapper.Map(userToUpdate, userForUpdate);
                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Updating UserSalary failed on save");
            }
            throw new Exception("Failed to find UserSalary to Update");
        }


    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUserSalaryEf(int userId)
    {
        UserSalary? userToDelete = _userRepository.GetSingleUserSalary(userId);

        if (userToDelete != null)
        {
            _userRepository.RemoveEntity<UserSalary>(userToDelete);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Deleting UserSalary failed on save");
        }
        throw new Exception("Failed to find UserSalary to delete");
    }

        [HttpGet("UserJobInfo/{userId}")]
    public UserJobInfo GetUserJobInfoEF(int userId)
    {
        return _userRepository.GetSingleUserJobInfo(userId);
    }

    [HttpPost("UserJobInfo")]
    public IActionResult PostUserJobInfoEf(UserJobInfo userForInsert)
    {
        _userRepository.AddEntity<UserJobInfo>(userForInsert);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        throw new Exception("Adding UserJobInfo failed on save");
    }


    [HttpPut("UserJobInfo")]
    public IActionResult PutUserJobInfoEf(UserJobInfo userForUpdate)
    {
        UserJobInfo? userToUpdate = _userRepository.GetSingleUserJobInfo(userForUpdate.UserId);

        if (userToUpdate != null)
        {
            _mapper.Map(userToUpdate, userForUpdate);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Updating UserJobInfo failed on save");
        }
        throw new Exception("Failed to find UserJobInfo to Update");
    }


    }
}