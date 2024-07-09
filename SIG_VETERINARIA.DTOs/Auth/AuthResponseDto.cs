using SIG_VETERINARIA.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIG_VETERINARIA.DTOs.Auth
{
    public class AuthResponseDto
    {
        public Boolean isSuccess {  get; set; }
        public UserDetailResponseDto User {  get; set; }
        public string Token { get; set; }
    }

    public class TokenResponseDto
    {
        public string Token { get; set; }
    }
}
