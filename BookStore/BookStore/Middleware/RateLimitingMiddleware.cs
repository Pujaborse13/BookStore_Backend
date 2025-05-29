using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace BookStore.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        // Track IP address and two time windows
        private static Dictionary<string, (DateTime shortTermStamp, int shortTermCount, DateTime longTermStamp, int longTermCount)> _clients
            = new Dictionary<string, (DateTime, int, DateTime, int)>();

        private static readonly object _lock = new object();

        private readonly int _shortTermLimit = 7;
        private readonly TimeSpan _shortTermWindow = TimeSpan.FromSeconds(1);

        private readonly int _longTermLimit = 20;
        private readonly TimeSpan _longTermWindow = TimeSpan.FromMinutes(1);

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress.ToString();

            lock (_lock)
            {
                if (_clients.ContainsKey(ip))
                {
                    var (shortStamp, shortCount, longStamp, longCount) = _clients[ip];

                    // Check short-term window (1 second)
                    if (DateTime.UtcNow - shortStamp < _shortTermWindow)
                        shortCount++;
                    else
                        (shortStamp, shortCount) = (DateTime.UtcNow, 1);

                    // Check long-term window (1 minute)
                    if (DateTime.UtcNow - longStamp < _longTermWindow)
                        longCount++;
                    else
                        (longStamp, longCount) = (DateTime.UtcNow, 1);

                    // Check if either limit exceeded
                    if (shortCount > _shortTermLimit || longCount > _longTermLimit)
                    {
                        context.Response.StatusCode = 429; // Too Many Requests
                        context.Response.ContentType = "application/json";
                        var response = new { message = "Too many requests. Please try again later." };
                        context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
                        return;
                    }

                    // Update record
                    _clients[ip] = (shortStamp, shortCount, longStamp, longCount);
                }
                else
                {
                    // First request from this IP
                    _clients[ip] = (DateTime.UtcNow, 1, DateTime.UtcNow, 1);
                }
            }

            await _next(context);
        }
    }

}

