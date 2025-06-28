using Moongazing.Kernel.Application.Responses;

namespace Moongazing.Clinicly.Application.Features.Users.Commands.Delete;

public class DeleteUserResponse : IResponse
{
    public Guid Id { get; set; }
}