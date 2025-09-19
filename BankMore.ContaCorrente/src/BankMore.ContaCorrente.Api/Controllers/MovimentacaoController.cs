using BankMore.ContaCorrente.Models.Requests;
using BankMore.ContaCorrente.Models.Services.Movimentacao;
using BankMore.ContaCorrente.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;

namespace BankMore.ContaCorrente.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MovimentacaoController : ControllerBase
{
    private readonly IMovimentacaoService _movimentacaoService;

    public MovimentacaoController(IMovimentacaoService movimentacaoService)
    {
        _movimentacaoService = movimentacaoService;
    }

    [HttpPost]
    public async Task<IActionResult> MovimentarAsync([FromBody] MovimentacaoRequest request)
    {
        try
        {
            var usuarioContaClaim = User.FindFirstValue("idConta") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioContaClaim == null)
                return Forbid();

            int numeroContaLogado = int.Parse(User.FindFirstValue("numero").ToString() ?? "0");

            await _movimentacaoService.MovimentarAsync(request, numeroContaLogado);
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