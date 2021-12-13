using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LGuardaService.API.JWTAuth
{
    public interface IJWTAuthenticationManager
    {
        string Authenticate(string username);
    }
}
