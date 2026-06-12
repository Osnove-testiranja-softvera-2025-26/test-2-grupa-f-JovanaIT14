

using ApartmentAgencyApp.Exceptions;
using ApartmentAgencyApp.Fakes;
using ApartmentAgencyApp.Models;
using ApartmentAgencyApp.Services;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ApartmentAgencyApp.Test
{
    //example of Guid: 00000000-0000-0000-0000-000000000001

    [TestFixture]
    public class ApartmentAgencyServiceTest
    {
        private FakeApartmentService _fakeApartmentService;
        private FakeDateCalculationService _fakeDateCalculationService;
        private FakeReservationService _fakeReservationService;
        private ApartmentAgencyService _apartmentAgencyService;

        [SetUp]
        public void SetUp()
        {
            _fakeApartmentService = new FakeApartmentService(new List<Apartment>
        {
            new Apartment
            {
                Id = Guid.Parse(" 00000000-0000-0000-0000-000000000001"),
                Type = ApartmentType.BedOnly,
                NumberOfBeds = 5,
                PricePerNight = 7

            }

        });

            _fakeDateCalculationService = new FakeDateCalculationService(new RequestDaysInfo
            {
                NumberOfDays = 5,
                NumberOfSeasonDays = 5
            });

            _fakeReservationService = new FakeReservationService();

            _apartmentAgencyService = new ApartmentAgencyService(_fakeDateCalculationService, _fakeApartmentService, _fakeReservationService);
        }

        [Test]
        public void MakeApartmentReservation_Exception()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DateOfArrival = DateTime.Now,
                DateOfDeparture = DateTime.Now,
                DistanceFromTheBeach = 10,
                NumberOfBeds = 5,
                ApartmentType = ApartmentType.Studio
            };

            _fakeApartmentService = new FakeApartmentService(new List<Apartment>()); //pravim praznu listu jer exception baca kada je availableApartments.Count == 0
            _apartmentAgencyService = new ApartmentAgencyService(_fakeDateCalculationService, _fakeApartmentService, _fakeReservationService);

            Assert.Throws<NoAvailableApartmentsException>(
                () => _apartmentAgencyService.MakeApartmentReservation(request));

        }

        [Test]
        //BedOnly, distanca < 500 i brojKreveta >= 3
        public void MakeApartmentReservation_ComplexA()
        {

            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DateOfArrival = DateTime.Now,
                DateOfDeparture = DateTime.Now,
                DistanceFromTheBeach = 450,
                NumberOfBeds = 5,
                ApartmentType = ApartmentType.BedOnly
            };

            _apartmentAgencyService.MakeApartmentReservation(request);

            Assert.That(_fakeReservationService.MakeReservationInComplexCalled, Is.True);
            Assert.That(_fakeReservationService.CreatedReservation.ApartmentComplex, Is.EqualTo(ApartmentComplex.ComplexA));
        }

        [Test]
        //BedOnly, distanca < 500 i brojKreveta < 3
        public void MakeApartmentReservation_complexB_numOfBedsManji_Od3()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DateOfArrival = DateTime.Now,
                DateOfDeparture = DateTime.Now,
                DistanceFromTheBeach = 450,
                NumberOfBeds = 1,
                ApartmentType = ApartmentType.BedOnly
            };

            _apartmentAgencyService.MakeApartmentReservation(request);

            Assert.That(_fakeReservationService.MakeReservationInComplexCalled, Is.True);
            Assert.That(_fakeReservationService.CreatedReservation.ApartmentComplex, Is.EqualTo(ApartmentComplex.ComplexB));

        }

        //Studio, brojDanaVeciJednak 5, i numberOfSeasonDays < 2
        [Test]
        public void MakeApartmentReservation_complexB_numOfDays_Veci_Jednak5()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DateOfArrival = DateTime.Now,
                DateOfDeparture = DateTime.Now,
                DistanceFromTheBeach = 450,
                NumberOfBeds = 1,
                ApartmentType = ApartmentType.Studio
            };

            _fakeDateCalculationService = new FakeDateCalculationService(new RequestDaysInfo
            {
                NumberOfDays = 6,
                NumberOfSeasonDays = 1//namjero ne stavljam da je >2 da bi zbog NumberOfDays test prolazio
            });

            _apartmentAgencyService = new ApartmentAgencyService(_fakeDateCalculationService, _fakeApartmentService, _fakeReservationService);
            _apartmentAgencyService.MakeApartmentReservation(request);

            Assert.That(_fakeReservationService.MakeReservationInComplexCalled, Is.True);
            Assert.That(_fakeReservationService.CreatedReservation.ApartmentComplex, Is.EqualTo(ApartmentComplex.ComplexB));
        }

        [Test]
        //Studio, brojDana < 5, i numberOfSeasonDays > 2
        public void MakeApartmentReservation_complexB_numOfSeasonDays_VeciOd2()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DateOfArrival = DateTime.Now,
                DateOfDeparture = DateTime.Now,
                DistanceFromTheBeach = 450,
                NumberOfBeds = 1,
                ApartmentType = ApartmentType.Studio
            };

            _fakeDateCalculationService = new FakeDateCalculationService(new RequestDaysInfo
            {
                NumberOfDays = 2, //ovdje takodje ne stavljam da je >=5 da bi zbog NumberOfSeasonDays test prolazio
                NumberOfSeasonDays = 5
            });

            _apartmentAgencyService = new ApartmentAgencyService(_fakeDateCalculationService, _fakeApartmentService, _fakeReservationService);
            _apartmentAgencyService.MakeApartmentReservation(request);

            Assert.That(_fakeReservationService.MakeReservationInComplexCalled, Is.True);
            Assert.That(_fakeReservationService.CreatedReservation.ApartmentComplex, Is.EqualTo(ApartmentComplex.ComplexB));
        }

        [Test]
        public void MakeApartmentReservation_complexC()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DateOfArrival = DateTime.Now,
                DateOfDeparture = DateTime.Now,
                DistanceFromTheBeach = 450,
                NumberOfBeds = 1,
                ApartmentType = ApartmentType.Studio    
            };

            _fakeDateCalculationService = new FakeDateCalculationService(new RequestDaysInfo
            {
                NumberOfDays = 3, 
                NumberOfSeasonDays = 1 // bice complexC ako je NumberOfDays<5 i NumberOfSeasonDays < 3
            });

            _apartmentAgencyService = new ApartmentAgencyService(_fakeDateCalculationService, _fakeApartmentService, _fakeReservationService);
            _apartmentAgencyService.MakeApartmentReservation(request);

            Assert.That(_fakeReservationService.MakeReservationInComplexCalled, Is.True);
            Assert.That(_fakeReservationService.CreatedReservation.ApartmentComplex, Is.EqualTo(ApartmentComplex.ComplexC));
        }

        [Test]
        public void MakeApartmentReservation_complexD_kadaJe_StudioWithTerrace()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DateOfArrival = DateTime.Now,
                DateOfDeparture = DateTime.Now,
                DistanceFromTheBeach = 450,
                NumberOfBeds = 1,
                ApartmentType = ApartmentType.StudioWithTerrace //kad je studiowithterrace uvijek je complexD

            };
            _apartmentAgencyService.MakeApartmentReservation(request);

            Assert.That(_fakeReservationService.MakeReservationInComplexCalled, Is.True);
            Assert.That(_fakeReservationService.CreatedReservation.ApartmentComplex, Is.EqualTo(ApartmentComplex.ComplexD));

        }

        //nSubstitute test koji provjerava da li ce metoda MakeReservationInComplex biti pozvana i da li ce joj biti proslijedjen Reservation
        [Test]
        public void MakeApartmentReservation_ShouldCallMakeReservationInComplex()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DateOfArrival = DateTime.Now,
                DateOfDeparture = DateTime.Now,
                DistanceFromTheBeach = 450,
                NumberOfBeds = 1,
                ApartmentType = ApartmentType.StudioWithTerrace

            };

            IReservationService reservationService = Substitute.For<IReservationService>();

            _apartmentAgencyService = new ApartmentAgencyService(_fakeDateCalculationService, _fakeApartmentService, reservationService);
            _apartmentAgencyService.MakeApartmentReservation(request);

            reservationService.Received()
                .MakeReservationInComplex(Arg.Any<Reservation>());
        }

        //test za pict f3
        [TestCaseSource(typeof(PictParser), "GetTestCases", new object[] { "PictResults.txt" })]
        public void testZaPict(double distanceFromTheBeach, int percentOfPositiveReviews, ApartmentType apartmentType, bool renovatedInTheLastYear, ApartmentRank expectedResult)
        {
            ApartmentRank result = _apartmentAgencyService.CalculateApartmentRank(distanceFromTheBeach, percentOfPositiveReviews, apartmentType, renovatedInTheLastYear);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void MakeApartmentReservation_complexD_kadaJe_BedOnly_i_udaljenostJeVecaOd500()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.NewGuid(),
                DateOfArrival = DateTime.Now,
                DateOfDeparture = DateTime.Now.AddDays(1),
                DistanceFromTheBeach = 600,
                NumberOfBeds = 4,
                ApartmentType = ApartmentType.BedOnly
            };

            _apartmentAgencyService.MakeApartmentReservation(request);

            Assert.That(_fakeReservationService.MakeReservationInComplexCalled, Is.True);
            Assert.That(_fakeReservationService.CreatedReservation.ApartmentComplex,Is.EqualTo(ApartmentComplex.ComplexD));
        }

        [Test]
        public void MakeApartmentReservation_complexD_kadaJe_BedOnly_i_udaljenostJeTacno500()
        {
            ReservationRequest request = new ReservationRequest
            {
                Id = Guid.NewGuid(),
                DateOfArrival = DateTime.Now,
                DateOfDeparture = DateTime.Now.AddDays(1),
                DistanceFromTheBeach = 500,
                NumberOfBeds = 3,
                ApartmentType = ApartmentType.BedOnly
            };

            _apartmentAgencyService.MakeApartmentReservation(request);

            Assert.That(_fakeReservationService.MakeReservationInComplexCalled, Is.True);
            Assert.That(_fakeReservationService.CreatedReservation.ApartmentComplex,Is.EqualTo(ApartmentComplex.ComplexD));
        }


    }
}
