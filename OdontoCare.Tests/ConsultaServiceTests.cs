using FluentAssertions;
using Moq;
using OdontoCare.Api.DTOs;
using OdontoCare.Api.Models;
using OdontoCare.Api.Repositories;
using OdontoCare.Api.Services;

namespace OdontoCare.Tests;

public class ConsultaServiceTests
{
    private readonly Mock<IConsultaRepository> _consultaRepositoryMock;
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly ConsultaService _service;

    public ConsultaServiceTests()
    {
        _consultaRepositoryMock = new Mock<IConsultaRepository>();
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _service = new ConsultaService(
            _consultaRepositoryMock.Object,
            _pacienteRepositoryMock.Object);
    }

  
    [Fact]
    public async Task CriarAsync_DeveCriarConsulta_QuandoPacienteExiste()
    {
    
        var dto = new ConsultaCreateDto
        {
            PacienteId = "pac123",
            Dentista = "Dr. Roberto",
            Especialidade = "Ortodontia",
            DataConsulta = DateTime.Today.AddDays(1),
            Horario = "09:00",
            Status = "Agendada",
            Valor = 250.00m
        };

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorIdAsync("pac123"))
            .ReturnsAsync(new Paciente { Id = "pac123", NomeCompleto = "João Silva" });

        _consultaRepositoryMock
            .Setup(r => r.CriarAsync(It.IsAny<Consulta>()))
            .ReturnsAsync((Consulta c) =>
            {
                c.Id = "con123";
                return c;
            });

     
        var resultado = await _service.CriarAsync(dto);

    
        resultado.Should().NotBeNull();
        resultado.Dentista.Should().Be("Dr. Roberto");
        resultado.NomePaciente.Should().Be("João Silva");
        resultado.Valor.Should().Be(250.00m);
    }

    
    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarConsulta_QuandoExiste()
    {
      
        var consulta = new Consulta
        {
            Id = "con123",
            PacienteId = "pac123",
            Dentista = "Dra. Ana",
            Especialidade = "Clínico Geral",
            DataConsulta = DateTime.Today.AddDays(2),
            Horario = "14:00",
            Status = "Confirmada",
            Valor = 180.00m
        };

        _consultaRepositoryMock
            .Setup(r => r.ObterPorIdAsync("con123"))
            .ReturnsAsync(consulta);

        _pacienteRepositoryMock
            .Setup(r => r.ObterPorIdAsync("pac123"))
            .ReturnsAsync(new Paciente { Id = "pac123", NomeCompleto = "Maria Souza" });

      
        var resultado = await _service.ObterPorIdAsync("con123");

      
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be("con123");
        resultado.NomePaciente.Should().Be("Maria Souza");
    }

   
    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoPacienteNaoExiste()
    {
        // Arrange
        var dto = new ConsultaCreateDto
        {
            PacienteId = "pac_inexistente",
            Dentista = "Dr. Carlos",
            Especialidade = "Endodontia",
            DataConsulta = DateTime.Today.AddDays(1),
            Horario = "10:00",
            Status = "Agendada",
            Valor = 300.00m
        };

       
        _pacienteRepositoryMock
            .Setup(r => r.ObterPorIdAsync("pac_inexistente"))
            .ReturnsAsync((Paciente?)null);

       
        var acao = async () => await _service.CriarAsync(dto);

      
        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Paciente não encontrado.");
    }

 
    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNull_QuandoNaoExiste()
    {
     
        _consultaRepositoryMock
            .Setup(r => r.ObterPorIdAsync("id_inexistente"))
            .ReturnsAsync((Consulta?)null);

      
        var resultado = await _service.ObterPorIdAsync("id_inexistente");


        resultado.Should().BeNull();
    }
}