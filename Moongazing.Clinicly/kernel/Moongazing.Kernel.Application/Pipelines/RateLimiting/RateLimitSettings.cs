namespace Moongazing.Kernel.Application.Pipelines.RateLimiting;

public class RateLimitSettings
    {
        public int Limit { get; set; } = 50;                
        public int PeriodInSeconds { get; set; } = 60;     

        public TimeSpan Period => TimeSpan.FromSeconds(PeriodInSeconds);
    }

