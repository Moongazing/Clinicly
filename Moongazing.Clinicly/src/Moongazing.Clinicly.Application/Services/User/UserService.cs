using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;

namespace Moongazing.Clinicly.Application.Services.User;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<UserEntity?> GetByEmail(string email)
    {
        UserEntity? user = await userRepository.GetAsync(u => u.Email == email);
        return user;
    }

    public async Task<UserEntity> GetById(Guid id)
    {
        UserEntity? user = await userRepository.GetAsync(u => u.Id == id);
        return user!;
    }

    public async Task<UserEntity> Update(UserEntity user)
    {
        UserEntity updatedUser = await userRepository.UpdateAsync(user);
        return updatedUser;
    }
}
