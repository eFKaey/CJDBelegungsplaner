﻿using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Models.Interfaces;

namespace CJDBelegungsplaner.WPF.Intermediate;

public class ReservationCellContainerOfTModelOfTReservation<TModel, TReservation> : ReservationCellContainer
    where TModel : EntityObject, IModelWithReservation<TReservation>
    where TReservation : Reservation
{
    public override TModel Entity { get; }

    public override TReservation Reservation { get; }

    public ReservationCellContainerOfTModelOfTReservation(DataColumnWeek dataColumnWeek, int row, int column, TModel entity, TReservation reservation, ReservationCellContainer? relatedContainer)
        : base(dataColumnWeek, row, column, relatedContainer)
    {
        Entity = entity;
        Reservation = reservation;
    }
}
