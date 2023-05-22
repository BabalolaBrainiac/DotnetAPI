using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotnetAPI.Models;

namespace DotnetAPI.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        EntityFramewordDataContext _entityFramewordDataContext;

        public UserRepository(IConfiguration configuration, IMapper mapper)
        {
            _entityFramewordDataContext = new EntityFramewordDataContext(configuration);
        }

        public bool SaveChanges()
        {
            return _entityFramewordDataContext.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entity)
        {
            if (entity != null)
            {
                _entityFramewordDataContext.Add(entity);
            }

        }

        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> users = _entityFramewordDataContext.Users.ToList<User>();
            return users;
        }

        public User GetSingleUser(int userId)
        {
            User? user = _entityFramewordDataContext.Users
                .Where(u => u.UserId == userId)
                .FirstOrDefault<User>();

            if (user != null)
            {
                return user;
            }
            
            throw new Exception("Failed to Get User");
        }

        public void RemoveEntity<T>(T entity)
        {
            if(entity != null)
            {
                _entityFramewordDataContext.Remove(entity);
            }

        }

         public UserSalary GetSingleUserSalary(int userId)
        {
            UserSalary? userSalary = _entityFramewordDataContext.UserSalary
                .Where(u => u.UserId == userId)
                .FirstOrDefault<UserSalary>();

            if (userSalary != null)
            {
                return userSalary;
            }
            
            throw new Exception("Failed to Get User");
        }

        public UserJobInfo GetSingleUserJobInfo(int userId)
        {
            UserJobInfo? userJobInfo = _entityFramewordDataContext.UserJobInfo
                .Where(u => u.UserId == userId)
                .FirstOrDefault<UserJobInfo>();

            if (userJobInfo != null)
            {
                return userJobInfo;
            }
            
            throw new Exception("Failed to Get User");
        }
    }
}