using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2024.Shared.DTO
{
    public class UserTokenDTO
    {
        public string Token { get; set; } = null!;
        public DateTime Expiracion { get; set; }
    }
}