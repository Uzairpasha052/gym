using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class LoginResponseDto
{
        public int Id {get; set;}

        public int RoleId {get; set;} 

        public string FullName {get; set;} = string.Empty;

        public string Email {get; set;} = string.Empty;

        public string Token {get; set;} = string.Empty;

}