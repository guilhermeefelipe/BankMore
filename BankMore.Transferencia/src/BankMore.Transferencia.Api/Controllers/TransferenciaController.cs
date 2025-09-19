using BankMore.Transferencia.Models.Requests;
using BankMore.Transferencia.Models.Services.Transferencia;
using BankMore.Transferencia.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace BankMore.Transferencia.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransferenciaController : ControllerBase
{
    private readonly ITransferenciaService transferenciaService;

    public TransferenciaController(ITransferenciaService transferenciaService)
    {
        this.transferenciaService = transferenciaService;
    }

    [HttpPost]
    public async Task<IActionResult> EfetuarTransferenciaAsync([FromBody] TransferenciaRequest request)
    {
        try
        {
            var idContaClaim = User.FindFirstValue("idConta");
            if (idContaClaim == null)
                return Forbid();

            var numeroContaClaim = User.FindFirstValue("numero");

            int numeroContaOrigem = int.Parse(numeroContaClaim);

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            await transferenciaService.EfetuarTransferenciaAsync(request, numeroContaOrigem, idContaClaim, token);

            return NoContent();
        }
        catch (WorkException ex)
        {
            return BadRequest(new { ex.Message, Type = ex.ErrorType });
        }
        catch (SecurityTokenExpiredException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return StatusCode(500, new { Message = "Ocorreu um erro inesperado." });
        }
    }
}