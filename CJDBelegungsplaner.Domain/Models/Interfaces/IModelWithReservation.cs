namespace CJDBelegungsplaner.Domain.Models.Interfaces;

public interface IModelWithReservation<TModel>
{
    string Name { get; }

    public ICollection<TModel> Reservations { get; set; }
}
