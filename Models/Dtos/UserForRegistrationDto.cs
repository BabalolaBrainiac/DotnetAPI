using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetAPI.Models.Dtos
{
     public partial class UserForRegistrationDto
    {
        public string Email {get; set;}

        public string Password {get; set;}

        public string ConfirmPassword {get; set;}

        public UserForRegistrationDto()
        {
            if(Email == null)
            {
                Email = "";
            }

              if(Password == null)
            {
                Password = "";
            }

              if(ConfirmPassword == null)
            {
                ConfirmPassword = "";
            }
        }
    
    }
    


}