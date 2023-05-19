using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetAPI.Models;

namespace DotnetAPI.Data.Repository
{
    public interface IUserRepository
    {
        public bool SaveChanges();

        public void AddEntity<T>(T entity);

        public void RemoveEntity<T>(T entity);

        public UserSalary GetSingleUserSalary(int userId);

        public UserJobInfo GetSingleUserJobInfo(int  userId);

        public User GetSingleUser(int userId);

        public IEnumerable<User> GetUsers(); 
    }
}