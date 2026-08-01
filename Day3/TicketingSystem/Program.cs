using System;
using System.Collections.Generic;

namespace TicketingSystem
{
    class Passenger
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    class Train
    {
        public int TrainNumber { get; set; }
        public string TrainName { get; set; }
    }

    class Ticket
    {
        public int TicketId { get; set; }
        public Passenger PassengerDetails { get; set; }
        public Train TrainDetails { get; set; }
        public double Amount { get; set; }
    }

    class TicketingManager
    {
        private List<Ticket> bookedTickets = new List<Ticket>();

        public void BookTicket(Ticket ticket)
        {
            bookedTickets.Add(ticket);
            Console.WriteLine($"Ticket booked for {ticket.PassengerDetails.Name} on {ticket.TrainDetails.TrainName}");
        }

        // List the total amount collected
        public double GetTotalAmountCollected()
        {
            double total = 0;
            foreach (var ticket in bookedTickets)
            {
                total += ticket.Amount;
            }
            return total;
        }

        // List all the tickets for a passenger
        public List<Ticket> GetTicketsByPassenger(int passengerId)
        {
            List<Ticket> result = new List<Ticket>();
            foreach (var ticket in bookedTickets)
            {
                if (ticket.PassengerDetails.Id == passengerId)
                {
                    result.Add(ticket);
                }
            }
            return result;
        }

        // List all passenger for a train
        public List<Passenger> GetPassengersForTrain(int trainNumber)
        {
            List<Passenger> result = new List<Passenger>();
            foreach (var ticket in bookedTickets)
            {
                if (ticket.TrainDetails.TrainNumber == trainNumber)
                {
                    result.Add(ticket.PassengerDetails);
                }
            }
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TicketingManager tm = new TicketingManager();
            
            Passenger p1 = new Passenger { Id = 101, Name = "Rahul" };
            Passenger p2 = new Passenger { Id = 102, Name = "Aman" };

            Train t1 = new Train { TrainNumber = 12345, TrainName = "Rajdhani Express" };
            Train t2 = new Train { TrainNumber = 54321, TrainName = "Shatabdi Express" };

            tm.BookTicket(new Ticket { TicketId = 1, PassengerDetails = p1, TrainDetails = t1, Amount = 1500 });
            tm.BookTicket(new Ticket { TicketId = 2, PassengerDetails = p1, TrainDetails = t2, Amount = 800 });
            tm.BookTicket(new Ticket { TicketId = 3, PassengerDetails = p2, TrainDetails = t1, Amount = 1500 });

            Console.WriteLine($"\nTotal amount collected: Rs. {tm.GetTotalAmountCollected()}");

            Console.WriteLine($"\nTickets for passenger '{p1.Name}':");
            foreach(var t in tm.GetTicketsByPassenger(101))
            {
                Console.WriteLine($"- Ticket ID: {t.TicketId}, Train: {t.TrainDetails.TrainName}, Amount: {t.Amount}");
            }

            Console.WriteLine($"\nPassengers for train '{t1.TrainName}':");
            foreach(var p in tm.GetPassengersForTrain(12345))
            {
                Console.WriteLine($"- {p.Name}");
            }
        }
    }
}
