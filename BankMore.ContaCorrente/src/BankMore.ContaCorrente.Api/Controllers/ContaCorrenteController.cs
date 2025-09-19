using BankMore.ContaCorrente.Models.Requests;
using BankMore.ContaCorrente.Models.Services.ContaCorrente;
using BankMore.ContaCorrente.Services;
using BankMore.ContaCorrente.Services.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace BankMore.ContaCorrente.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContaCorrenteController : ControllerBase
{
    private readonly IContaCorrenteService contaCorrenteService;

    public ContaCorrenteController(IContaCorrenteService contaCorrenteService)
    {
        this.contaCorrenteService = contaCorrenteService;
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] CreateContaCorrenteRequest request)
    {
        try
        {
            return Ok(await contaCorrenteService.AddAsync(request));
        }
        catch (WorkException ex)
        {
            return BadRequest(new { ex.Message, Type = ex.ErrorType });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    [Authorize]
    [HttpDelete("inativar")]
    public async Task<IActionResult> InactivateAsync([FromBody] InativarContaRequest request)
    {
        try
        {
            var idConta = User.FindFirstValue("idConta");
            if (idConta == null)
            {
                return Forbid();
            }

            await contaCorrenteService.InactivateAsync(idConta, request.Senha);
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

    [Authorize]
    [HttpGet("Saldo")]
    public async Task<IActionResult> GetSaldo()
    {
        try
        {
            var usuarioContaClaim = User.FindFirstValue("idConta") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioContaClaim == null)
                return Forbid();

            var response = await contaCorrenteService.ConsultarSaldoAsync(usuarioContaClaim);
            return Ok(response);
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

    [Authorize]
    [HttpGet("{numeroConta}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetIdContaCorrente(int numeroConta)
    {
        try
        {
            var idContaCorrente = await contaCorrenteService.GetIdContaCorrente(numeroConta);
            return Ok(idContaCorrente);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
        }
    }
}
