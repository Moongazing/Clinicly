using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Moongazing.Kernel.CrossCuttingConcerns.Exceptions.HttpProblemDetails;

public class RateLimitProblemDetails : ProblemDetails
{
    public RateLimitProblemDetails(string detail)
    {
        Title = "Rate Limiter Problem";
        Detail = detail;
        Status = StatusCodes.Status429TooManyRequests;
        Type = "http://clinicly.com/probs/tomanyrequests";
    }
}