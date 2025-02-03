using JobManager.Domain.Dto;

namespace JobManager.Domain.Contracts;

public interface IUserRepository
{
    Task<UserDto?> GetUserByIdAsync(Guid userId);
}