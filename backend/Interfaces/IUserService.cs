using backend.DTOs;
using backend.DTOs.Pagination;
using backend.Helper;

namespace backend.Interfaces;

public interface IUserService
{
    
    //Task<bool> Register(RegisterDto request);
    //Task<LoginResponseDto?> ValidateLogin(LoginDto request);

    Task<PagedResult<UserDto>> GetAllUsersAsync(PaginationRequest request);
    Task<UserDto?> UpdateUser(int id, UpdateUserDto dto);
    Task<bool> DeleteUser(int id);

}
