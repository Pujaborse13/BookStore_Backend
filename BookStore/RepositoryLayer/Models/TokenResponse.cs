using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Models
{
    public class TokenResponse
    {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public string FullName { get; set; }

    }
}
