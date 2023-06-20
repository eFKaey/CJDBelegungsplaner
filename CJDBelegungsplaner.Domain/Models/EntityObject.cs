using System.ComponentModel.DataAnnotations;

namespace CJDBelegungsplaner.Domain.Models;

public abstract class EntityObject
{
    public int Id { get; set; }

    [Required]
    public DateTime Created { get; set; } = DateTime.Now;
}
