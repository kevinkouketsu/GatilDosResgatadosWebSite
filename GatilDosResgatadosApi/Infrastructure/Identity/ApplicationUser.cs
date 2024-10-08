﻿using GatilDosResgatadosApi.Areas.Pets.Entities;
using Microsoft.AspNetCore.Identity;

namespace GatilDosResgatadosApi.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public virtual IList<Pet> Pets { get; set; } = default!;
}
