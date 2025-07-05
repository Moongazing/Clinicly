using Moongazing.Clinicly.Domain.Entities;

namespace Moongazing.Clinicly.Application.Services.User;

public interface IUserService
{
    Task<UserEntity?> GetByEmail(string email);
    Task<UserEntity> GetById(Guid id);
    Task<UserEntity> Update(UserEntity user);
}