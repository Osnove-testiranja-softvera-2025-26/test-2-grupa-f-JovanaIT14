using ApartmentAgencyApp.Models;
using ApartmentAgencyApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentAgencyApp.Fakes
{
    public class FakeReservationService : IReservationService
    {
        public bool MakeReservationInComplexCalled;
        public Reservation CreatedReservation;

        public void MakeReservationInComplex(Reservation reservation)
        {
            MakeReservationInComplexCalled = true;
            CreatedReservation = reservation;
        }
    }
}
