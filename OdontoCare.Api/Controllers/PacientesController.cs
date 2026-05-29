using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoCare.Api.DTOs;
using OdontoCare.Api.Services;

namespace OdontoCare.Api.Controllers;

[ApiController]
[Route("api/pacientes")]
[Authorize] // Todos os endpoints exigem autenticação
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _service;

    public PacientesController(IPacienteService service)
    {
        _service = service;
    }

 
    [HttpGet]
    public async Task<IActionResult> ObterTodos(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? nome = null)
    {
        var resultado = await _service.ObterTodosAsync(pagina, tamanhoPagina, nome);
        return Ok(resultado);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(string id)
    {
        var paciente = await _service.ObterPorIdAsync(id);
        if (paciente is null) return NotFound(new { mensagem = "Paciente não encontrado." });
        return Ok(paciente);
    }


    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] PacienteCreateDto dto)
    {
        try
        {
            var criado = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")] // Apenas admin pode editar
    public async Task<IActionResult> Atualizar(string id, [FromBody] PacienteCreateDto dto)
    {
        var atualizado = await _service.AtualizarAsync(id, dto);
        if (!atualizado) return NotFound(new { mensagem = "Paciente não encontrado." });
        return NoContent();
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")] // Apenas admin pode deletar
    public async Task<IActionResult> Deletar(string id)
    {
        var deletado = await _service.DeletarAsync(id);
        if (!deletado) return NotFound(new { mensagem = "Paciente não encontrado." });
        return NoContent();
    }
}