using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerLayer.Interface
{
    public interface IJwtTokenManager
    {
        public string GenerateToken(string email, int userId, string role);

    }
}
