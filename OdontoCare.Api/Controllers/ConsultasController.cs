using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OdontoCare.Api.DTOs;
using OdontoCare.Api.Services;

namespace OdontoCare.Api.Controllers;

[ApiController]
[Route("api/consultas")]
[Authorize]
public class ConsultasController : ControllerBase
{
    private readonly IConsultaService _service;

    public ConsultasController(IConsultaService service)
    {
        _service = service;
    }


    [HttpGet]
    public async Task<IActionResult> ObterTodos(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? status = null)
    {
        var resultado = await _service.ObterTodosAsync(pagina, tamanhoPagina, status);
        return Ok(resultado);
    }

  
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(string id)
    {
        var consulta = await _service.ObterPorIdAsync(id);
        if (consulta is null) return NotFound(new { mensagem = "Consulta não encontrada." });
        return Ok(consulta);
    }

   
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ConsultaCreateDto dto)
    {
        try
        {
            var criada = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

   
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Atualizar(string id, [FromBody] ConsultaCreateDto dto)
    {
        try
        {
            var atualizado = await _service.AtualizarAsync(id, dto);
            if (!atualizado) return NotFound(new { mensagem = "Consulta não encontrada." });
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

    /// <summary>Remove uma consulta</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Deletar(string id)
    {
        var deletado = await _service.DeletarAsync(id);
        if (!deletado) return NotFound(new { mensagem = "Consulta não encontrada." });
        return NoContent();
    }
}