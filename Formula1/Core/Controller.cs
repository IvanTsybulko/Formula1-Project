using Formula1.Core.Contracts;
using Formula1.Models;
using Formula1.Models.Contracts;
using Formula1.Repositories;
using Formula1.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace Formula1.Core
{
    public class Controller : IController
    {
        public Controller()
        {
            pilotRepository = new PilotRepository();
            raceRepository = new RaceRepository();
            carRepository = new FormulaOneCarRepository();
        }

        //Repopsitories in wich the instances are stored
        private PilotRepository pilotRepository;
        private RaceRepository raceRepository;
        private FormulaOneCarRepository carRepository;

        //The function adds given car to given pilot if all the requiremesnts are met
        public string AddCarToPilot(string pilotName, string carModel)
        {
            //Finds the Pilot from the Pilot repository
            IPilot pilot = pilotRepository.FindByName(pilotName);
            //Finds the car from the car repository
            IFormulaOneCar car = carRepository.FindByName(carModel);

            if (pilot == null || pilot.Car != null)//if the pilot does not exist or they already have a car
            {
                throw new InvalidOperationException(String.Format(ExceptionMessages.PilotDoesNotExistOrHasCarErrorMessage, pilotName));
            }
            else if (car == null)//if the car does not exist
            {
                throw new NullReferenceException(String.Format(ExceptionMessages.CarDoesNotExistErrorMessage, carModel));
            }
            else
            {
                pilot.AddCar(car);
                carRepository.Remove(car);

                return String.Format(OutputMessages.SuccessfullyPilotToCar, pilotName, car.GetType().Name, carModel);
            }
        }

        //Adds a Pilot to the Collection of Pilots in the race if all the rquirements are met
        public string AddPilotToRace(string raceName, string pilotFullName)
        {
            //Find the instances from their repositories
            IPilot pilot = pilotRepository.FindByName(pilotFullName);
            IRace race = raceRepository.FindByName(raceName);

            List<IPilot> pilotsInRace = new List<IPilot>();

            if (race == null)//if the race does not exist
            {
                throw new NullReferenceException(String.Format(ExceptionMessages.RaceDoesNotExistErrorMessage, raceName));
            }
            else if (pilot == null || !pilot.CanRace || race.Pilots.Contains(pilot))//if the pilot does not exist or the pilot can not race or the pilot is already in the race
            {
                throw new InvalidOperationException(String.Format(ExceptionMessages.PilotDoesNotExistErrorMessage, pilotFullName));
            }
            else
            {
                race.AddPilot(pilot);
                return String.Format(OutputMessages.SuccessfullyAddPilotToRace, pilotFullName, raceName);
            }
        }

        //Creates a car and adds it to the car repo
        public string CreateCar(string type, string model, int horsepower, double engineDisplacement)
        {
            if (carRepository.FindByName(model) == null)//if there is no such car in the repo adds it
            {
                if (type == "Ferrari") //checks if the type is valid
                {
                    IFormulaOneCar car = new Ferrari(model,horsepower, engineDisplacement);
                    carRepository.Add(car);

                    return String.Format(OutputMessages.SuccessfullyCreateCar,type, model);
                }
                else if(type == "Williams")
                {
                    IFormulaOneCar car = new Williams(model, horsepower, engineDisplacement);
                    carRepository.Add(car);

                    return String.Format(OutputMessages.SuccessfullyCreateCar, type, model);
                }
                else
                {
                    throw new InvalidOperationException(String.Format(ExceptionMessages.InvalidTypeCar, type));
                }
            }
            //if the car exists
            throw new InvalidOperationException(String.Format(ExceptionMessages.CarExistErrorMessage, model));
        }
        //create a pilot and add it to the pilot repo
        public string CreatePilot(string fullName)
        {
            //if there is no such pilot creates one
            if (pilotRepository.FindByName(fullName) == null)
            {
                IPilot pilot = new Pilot(fullName);
                pilotRepository.Add(pilot);

                return String.Format(OutputMessages.SuccessfullyCreatePilot, fullName);
            }

            //if there is such pilot already
            throw new InvalidOperationException(String.Format(ExceptionMessages.PilotExistErrorMessage, fullName));
        }

        //create race and add it to the repo
        public string CreateRace(string raceName, int numberOfLaps)
        {
            if (raceRepository.FindByName(raceName) == null)//if there is no such race in the repo
            {
                IRace race = new Race(raceName,numberOfLaps);
                raceRepository.Add(race);

                return String.Format(OutputMessages.SuccessfullyCreateRace, raceName);
            }

            throw new InvalidOperationException(String.Format(ExceptionMessages.RaceExistErrorMessage, raceName));
        }

        public string PilotReport()//gives a report about the chosen pilot
        {
            List<IPilot> orderedPilots = pilotRepository.Models.OrderByDescending(p => p.NumberOfWins).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var pilot in orderedPilots)
            {
                sb.AppendLine(pilot.ToString());
            }

            return sb.ToString().Trim();
        }

        public string RaceReport() // gives a report about the choosen race
        {
            StringBuilder sb = new StringBuilder();

            foreach(var race in raceRepository.Models)
            {
                if (race.TookPlace)
                {
                    sb.AppendLine(race.RaceInfo());                }
            }

            return sb.ToString().Trim();
        }

        public string StartRace(string raceName)//Starts a reace and evaluates ranking
        {
            IRace race = raceRepository.FindByName(raceName);

            if (race == null)
            {
                throw new NullReferenceException(String.Format(ExceptionMessages.RaceDoesNotExistErrorMessage, raceName));
            }
            else if(race.Pilots.Count < 3)
            {
                throw new InvalidOperationException(String.Format(ExceptionMessages.InvalidRaceParticipants, raceName));

            }
            else if(race.TookPlace)
            {
                throw new InvalidOperationException(String.Format(ExceptionMessages.RaceTookPlaceErrorMessage, raceName));
            }
            else
            {
                List<IPilot> rankedPilots = race.Pilots.ToList();
                rankedPilots = rankedPilots.OrderByDescending(c => c.Car.RaceScoreCalculator(race.NumberOfLaps)).ToList();

                StringBuilder sb = new StringBuilder();

                race.TookPlace = true;
                rankedPilots[0].WinRace();

                sb.AppendLine($"Pilot {rankedPilots[0].FullName} wins the {raceName} race.");
                sb.AppendLine($"Pilot {rankedPilots[1].FullName} is second in the {raceName} race.");
                sb.AppendLine($"Pilot {rankedPilots[2].FullName} is third in the {raceName} race.");

                return sb.ToString().Trim();
            }
        }
    }
}
