using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Moongazing.Kernel.CrossCuttingConcerns.Exceptions.HttpProblemDetails;

public class TimeoutExceptionProblemDetails : ProblemDetails
{
    public TimeoutExceptionProblemDetails(string detail)
    {
        Title = "Timeout Problem";
        Detail = detail;
        Status = StatusCodes.Status401Unauthorized;
        Type = "http://p8retrail.com/probs/timeout";
    }
}