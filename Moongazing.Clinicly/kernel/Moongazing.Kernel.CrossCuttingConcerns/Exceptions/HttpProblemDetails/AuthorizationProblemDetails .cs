using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Moongazing.Kernel.CrossCuttingConcerns.Exceptions.HttpProblemDetails;

public class AuthorizationProblemDetails : ProblemDetails
{
    public AuthorizationProblemDetails(string detail)
    {
        Title = "Authorization Problem";
        Detail = detail;
        Status = StatusCodes.Status401Unauthorized;
        Type = "http://clinicly.com/probs/unauthorized";
    }
}
