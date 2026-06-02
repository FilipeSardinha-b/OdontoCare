using FluentAssertions;
using Moq;
using OdontoCare.Api.DTOs;
using OdontoCare.Api.Models;
using OdontoCare.Api.Repositories;
using OdontoCare.Api.Services;

namespace OdontoCare.Tests;

public class PacienteServiceTests
{
    private readonly Mock<IPacienteRepository> _repositoryMock;
    private readonly PacienteService _service;

    public PacienteServiceTests()
    {
        _repositoryMock = new Mock<IPacienteRepository>();
        _service = new PacienteService(_repositoryMock.Object);
    }

 
    [Fact]
    public async Task CriarAsync_DeveCriarPaciente_QuandoDadosValidos()
    {
        // Arrange
        var dto = new PacienteCreateDto
        {
            NomeCompleto = "João Silva",
            CPF = "12345678901",
            DataNascimento = new DateTime(1990, 1, 1),
            Telefone = "11999999999",
            Email = "joao@email.com",
            Endereco = "Rua das Flores, 123"
        };

        _repositoryMock
            .Setup(r => r.ObterPorCpfAsync(dto.CPF))
            .ReturnsAsync((Paciente?)null); // CPF não existe ainda

        _repositoryMock
            .Setup(r => r.CriarAsync(It.IsAny<Paciente>()))
            .ReturnsAsync((Paciente p) =>
            {
                p.Id = "abc123";
                return p;
            });

      
        var resultado = await _service.CriarAsync(dto);

     
        resultado.Should().NotBeNull();
        resultado.NomeCompleto.Should().Be("João Silva");
        resultado.CPF.Should().Be("12345678901");
        _repositoryMock.Verify(r => r.CriarAsync(It.IsAny<Paciente>()), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarPaciente_QuandoExiste()
    {
   
        var paciente = new Paciente
        {
            Id = "abc123",
            NomeCompleto = "Maria Souza",
            CPF = "98765432100",
            DataNascimento = new DateTime(1985, 5, 20),
            Telefone = "11888888888",
            Email = "maria@email.com",
            Endereco = "Av. Brasil, 456"
        };

        _repositoryMock
            .Setup(r => r.ObterPorIdAsync("abc123"))
            .ReturnsAsync(paciente);

        var resultado = await _service.ObterPorIdAsync("abc123");

      
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be("abc123");
        resultado.NomeCompleto.Should().Be("Maria Souza");
    }

   
    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoCpfJaExiste()
    {
        // Arrange
        var dto = new PacienteCreateDto
        {
            NomeCompleto = "Carlos Lima",
            CPF = "11111111111",
            DataNascimento = new DateTime(1980, 3, 15),
            Telefone = "11777777777",
            Email = "carlos@email.com",
            Endereco = "Rua A, 10"
        };

        // Simula que já existe um paciente com esse CPF
        _repositoryMock
            .Setup(r => r.ObterPorCpfAsync(dto.CPF))
            .ReturnsAsync(new Paciente { CPF = dto.CPF });

        // Act
        var acao = async () => await _service.CriarAsync(dto);

       
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Já existe um paciente com este CPF.");
    }

    
    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNull_QuandoNaoExiste()
    {
     
        _repositoryMock
            .Setup(r => r.ObterPorIdAsync("id_inexistente"))
            .ReturnsAsync((Paciente?)null);

     
        var resultado = await _service.ObterPorIdAsync("id_inexistente");

    
        resultado.Should().BeNull();
    }
}
