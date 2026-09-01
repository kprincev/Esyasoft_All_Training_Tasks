using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace MDMSApi.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("Authorization header missing"));
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();

                var token = authHeader.Substring("Basic ".Length).Trim();

                var credentialBytes = Convert.FromBase64String(token);

                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');

                var username = credentials[0];
                var password = credentials[1];

                // Read from appsettings.json
                var configUser = _configuration["BasicAuth:Username"];
                var configPass = _configuration["BasicAuth:Password"];

                if (username != configUser || password != configPass)
                {
                    return Task.FromResult(
                        AuthenticateResult.Fail("Invalid username or password"));
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, username)
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);

                var principal = new ClaimsPrincipal(identity);

                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(
                    AuthenticateResult.Success(ticket));
            }
            catch
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("Authentication failed"));
            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            Response.ContentType = "application/json";

            await Response.WriteAsync(
                "{\"message\":\"Invalid username or password\"}");
        }
    }
}