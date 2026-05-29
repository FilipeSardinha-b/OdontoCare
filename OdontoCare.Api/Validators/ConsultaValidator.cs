using FluentValidation;
using OdontoCare.Api.DTOs;

namespace OdontoCare.Api.Validators;

public class ConsultaValidator : AbstractValidator<ConsultaCreateDto>
{
    public ConsultaValidator()
    {
        RuleFor(x => x.PacienteId)
            .NotEmpty().WithMessage("PacienteId é obrigatório.");

        RuleFor(x => x.Dentista)
            .NotEmpty().WithMessage("Nome do dentista é obrigatório.")
            .MinimumLength(3).WithMessage("Nome do dentista deve ter pelo menos 3 caracteres.");

        RuleFor(x => x.Especialidade)
            .NotEmpty().WithMessage("Especialidade é obrigatória.");

        RuleFor(x => x.DataConsulta)
            .NotEmpty().WithMessage("Data da consulta é obrigatória.")
            .GreaterThan(DateTime.Today.AddDays(-1)).WithMessage("Data da consulta não pode ser no passado.");

        RuleFor(x => x.Horario)
            .NotEmpty().WithMessage("Horário é obrigatório.")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Horário deve estar no formato HH:mm.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status é obrigatório.")
            .Must(s => new[] { "Agendada", "Confirmada", "Realizada", "Cancelada" }.Contains(s))
            .WithMessage("Status deve ser: Agendada, Confirmada, Realizada ou Cancelada.");

        RuleFor(x => x.Valor)
            .GreaterThanOrEqualTo(0).WithMessage("Valor não pode ser negativo.");
    }
}