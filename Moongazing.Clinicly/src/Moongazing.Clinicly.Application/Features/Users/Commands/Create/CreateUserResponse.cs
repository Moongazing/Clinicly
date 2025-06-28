using Moongazing.Clinicly.Domain.Enums;
using Moongazing.Kernel.Application.Responses;


namespace Moongazing.Clinicly.Application.Features.Users.Commands.Create;

public class CreateUserResponse : IResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public UserStatus Status { get; set; } = default!;
}