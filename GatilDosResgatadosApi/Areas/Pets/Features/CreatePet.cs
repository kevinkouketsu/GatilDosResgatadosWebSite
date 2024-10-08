﻿using FastEndpoints;
using FluentValidation;
using GatilDosResgatadosApi.Areas.Common;
using GatilDosResgatadosApi.Areas.Pets.Entities;
using GatilDosResgatadosApi.Infrastructure;
using GatilDosResgatadosApi.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GatilDosResgatadosApi.Areas.Pets.Features;

public record CreatePetRequest : SignedInRequest
{
    public string Name { get; set; } = default!;
    public double? Weight { get; set; }
    public string Description { get; set; } = default!;
    public Gender Gender { get; set; }
    public IFormFile Avatar { get; set; } = default!;
}

public class CreatePetRequestValidator : Validator<CreatePetRequest>
{
    public CreatePetRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do pet não pode estar vazio");

        When(x => x.Avatar is not null, () =>
        {
            RuleFor(x => x.Avatar)
                .Must(x => x.Length < Pet.MaxAvatarSize).WithMessage("O tamanho máximo para o avatar é de 10mb")
                .Must(x => x.IsImage()).WithMessage("O avatar deve ser uma imagem válida");
        });

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("O gênero fornecido não é válido");
    }
}

public class CreatePet(ApplicationDbContext dbContext, ILogger<CreatePet> logger) : Endpoint<CreatePetRequest, Results<Created, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/api/pets/");
        AllowFileUploads();
    }

    public async override Task<Results<Created, ProblemHttpResult>> ExecuteAsync(CreatePetRequest req, CancellationToken ct)
    {
        byte[]? avatar = await req.Avatar.GetBytesAsync(ct);
        Pet pet = new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = req.Name,
            Description = req.Description,
            Gender = req.Gender,
            Weight = req.Weight,
            Avatar = avatar,
            CreatedById = req.UserId
        };

        try
        {
            await dbContext.Pets.AddAsync(pet, ct);
            await dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError("Could not create pet {pet}. Error: {ex}", pet.ToString(), ex);
            return TypedResults.Problem("Could not save to database");
        }

        // TODO: add link to the pet
        return TypedResults.Created();
    }
}
