using Mapster;
using MediatR;
using Moongazing.Clinicly.Application.Features.Users.Commands.Create;
using Moongazing.Clinicly.Application.Features.Users.Constants;
using Moongazing.Clinicly.Application.Features.Users.Rules;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Clinicly.Domain.Enums;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Application.Pipelines.Transaction;
using Moongazing.Kernel.Security.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moongazing.Clinicly.Application.Features.Users.Commands.Delete;

public class DeleteUserCommand : IRequest<DeleteUserResponse>,
    ILoggableRequest,
    IIntervalRequest,
    ITransactionalRequest,
    ICacheRemoverRequest,
    ISecuredRequest,
    IRateLimitedRequest
{

    public Guid Id { get; set; } = default!;

    public string[] Roles => [GeneralOperationClaims.Admin, GeneralOperationClaims.Delete, GeneralOperationClaims.Write];
    public bool BypassCache { get; }
    public string? CacheGroupKey => UsersMessages.SectionName;
    public string? CacheKey => null;
    public int Interval => 15;


    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
    {
        private readonly IUserRepository userRepository;
        private readonly UserBusinessRules userBusinessRules;
        public DeleteUserCommandHandler(IUserRepository userRepository,
                                        UserBusinessRules userBusinessRules)
        {
            this.userRepository = userRepository;
            this.userBusinessRules = userBusinessRules;
        }
        public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {

            UserEntity? user = await userRepository.GetAsync(predicate: x => x.Id == request.Id,
                                                             withDeleted: false,
                                                             cancellationToken: cancellationToken);

            await userBusinessRules.UserShouldBeExistsWhenSelected(user);

            await userRepository.DeleteAsync(user!, true, cancellationToken: cancellationToken);

            DeleteUserResponse response = user.Adapt<DeleteUserResponse>();

            return response;
        }
    }
}
