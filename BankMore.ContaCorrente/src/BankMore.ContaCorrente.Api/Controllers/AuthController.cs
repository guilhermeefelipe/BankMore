using BankMore.ContaCorrente.Models.Dto.Auth;
using BankMore.ContaCorrente.Models.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.ContaCorrente.Api.Controllers;

[ApiController]
[Route("api/")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var token = await authService.AuthenticateAsync(request.NumeroOuCpf, request.Senha);

            if (token is null)
            {
                return Unauthorized(new { Message = "Usuário ou senha inválidos.", Type = "USER_UNAUTHORIZED" });
            }

            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}