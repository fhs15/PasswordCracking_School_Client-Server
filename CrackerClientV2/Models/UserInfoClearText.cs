using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrackerClientV2.Models
{
    public class UserInfoClearText
    {
        public String UserName { get; set; }
        public String Password { get; set; }

        public UserInfoClearText(string username, string password)
        {
            if (username == null)
            {
                throw new ArgumentNullException("username");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            UserName = username;
            Password = password;
        }

        public override string ToString()
        {
            return UserName + ": " + Password;
        }
    }
}
