using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BookPricesJob.Data.Entity;

public class ApiUserClaim : IdentityUserClaim<string> { }
