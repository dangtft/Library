using System.ComponentModel.DataAnnotations;

namespace ConsumeWebAPI.DTOs
{
    public class RegisterRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string[] Roles { get; set; }
    }
}
