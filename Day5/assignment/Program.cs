using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

#nullable disable

namespace MetroSmartCardSystem
{
    /// <summary>
    /// Encapsulates the travel summary data for a commuter.
    /// Preserves the structure requested in the core problem statement.
    /// </summary>
    public class TravelSummary
    {
        public long lastEntryStation;
        public long lastExitStation;
        public long lastEntryTime;
        public long lastExitTime;
        public double totalFarePaid;
        public int totalTrips;
        public double averageFarePerTrip;
    }

    /// <summary>
    /// Represents a commuter utilizing the metro network.
    /// </summary>
    public class Commuter
    {
        public int cardNumber;
        public string commuterName;
        public string commuterType; // Options: "SENIOR", "ADULT", "STUDENT", "CHILD"
        public TravelSummary travelSummary;
    }

    /// <summary>
    /// Represents a station within the metro network.
    /// </summary>
    public class Station
    {
        public int stationId;
        public string stationName;
        public int zone;
        public double latitude;
        public double longitude;
    }

    /// <summary>
    /// Defines core operations supported by the Metro Card System.
    /// </summary>
    public interface MetroOperations
    {
        void issueCard(int cardNumber, string commuterName, string commuterType);
        bool tapIn(int cardNumber, int stationId, long epochTime);
        bool tapOut(int cardNumber, int stationId, long epochTime);
        Commuter getCommuterInfo(int cardNumber);
        List<double> fareHistory(int cardNumber);
        Dictionary<string, double> getZoneWiseRevenue(long startTime, long endTime);
        List<string> getFrequentRoute(int cardNumber);
        double getDailyPassSavings(int cardNumber, long date);
    }

    /// <summary>
    /// Internal representation of an ongoing journey.
    /// </summary>
    internal class ActiveJourney
    {
        public int EntryStationId { get; init; }
        public long EntryTime { get; init; }
    }

    /// <summary>
    /// Internal representation of a completed journey for historical reporting.
    /// </summary>
    internal class CompletedJourney
    {
        public long StartTime { get; init; }
        public long EndTime { get; init; }
        public string ZonePair { get; init; }
        public double Fare { get; init; }
    }

    /// <summary>
    /// Highly optimized manager class implementing all required metro operations.
    /// </summary>
    public sealed class MetroCardManager : MetroOperations
    {
        private readonly IReadOnlyDictionary<int, Station> _stations;
        private readonly Dictionary<int, Commuter> _commuters = new();
        
        // State tracking
        private readonly Dictionary<int, ActiveJourney> _activeJourneys = new();
        private readonly List<CompletedJourney> _completedJourneys = new();
        private readonly Dictionary<int, LinkedList<double>> _fareHistories = new();
        private readonly Dictionary<int, Dictionary<string, int>> _commuterRoutes = new();
        
        // Maps cardNumber -> (Date YYYYMMDD -> TotalFare)
        private readonly Dictionary<int, Dictionary<long, double>> _dailyFares = new();

        private readonly double _baseFare;
        private readonly double _perKmRate;
        private readonly double _maxDailyCap;

        public MetroCardManager(IEnumerable<Station> stations, double baseFare, double perKmRate, double maxDailyCap)
        {
            _stations = stations.ToDictionary(s => s.stationId);
            _baseFare = baseFare;
            _perKmRate = perKmRate;
            _maxDailyCap = maxDailyCap;
        }

        public void issueCard(int cardNumber, string commuterName, string commuterType)
        {
            if (!_commuters.TryAdd(cardNumber, new Commuter
            {
                cardNumber = cardNumber,
                commuterName = commuterName,
                commuterType = commuterType,
                travelSummary = new TravelSummary()
            }))
            {
                return; // Card already exists
            }

            // Initialize internal tracking structures for the newly issued card
            _fareHistories[cardNumber] = new LinkedList<double>();
            _commuterRoutes[cardNumber] = new Dictionary<string, int>();
            _dailyFares[cardNumber] = new Dictionary<long, double>();
        }

        public bool tapIn(int cardNumber, int stationId, long epochTime)
        {
            // Reject if commuter doesn't exist, is already traversing, or if station is invalid
            if (!_commuters.TryGetValue(cardNumber, out var commuter) || 
                _activeJourneys.ContainsKey(cardNumber) || 
                !_stations.ContainsKey(stationId))
            {
                return false;
            }

            _activeJourneys[cardNumber] = new ActiveJourney { EntryStationId = stationId, EntryTime = epochTime };
            
            commuter.travelSummary.lastEntryStation = stationId;
            commuter.travelSummary.lastEntryTime = epochTime;
            
            return true;
        }

        public bool tapOut(int cardNumber, int stationId, long epochTime)
        {
            // Reject if no active journey exists or station mapping is missing
            if (!_commuters.TryGetValue(cardNumber, out var commuter) || 
                !_activeJourneys.TryGetValue(cardNumber, out var journey) || 
                !_stations.TryGetValue(stationId, out var exitStation))
            {
                return false;
            }

            // Validate chronological traversal and cross-station movement
            if (epochTime <= journey.EntryTime || journey.EntryStationId == stationId)
            {
                return false;
            }

            var entryStation = _stations[journey.EntryStationId];
            
            double distance = CalculateDistance(entryStation, exitStation);
            double durationMinutes = (epochTime - journey.EntryTime) / (1000.0 * 60.0);
            
            // Baseline computation with duration penalties
            double fare = durationMinutes > 120 
                ? _baseFare * 3 
                : _baseFare + (distance * _perKmRate);

            // Discount application via pattern matching abstraction
            fare *= commuter.commuterType switch
            {
                "SENIOR" => 0.50,
                "STUDENT" => 0.75,
                "CHILD" => 0.25,
                _ => 1.00 // Default fallback (e.g., ADULT)
            };
            
            // Apply Daily Maximum Capping Logic
            long traversalDateId = ComputeDateId(journey.EntryTime);
            ref double currentDailyTotal = ref GetDailyFareReference(cardNumber, traversalDateId);
            
            if (currentDailyTotal >= _maxDailyCap)
            {
                fare = 0.0;
            }
            else if (currentDailyTotal + fare > _maxDailyCap)
            {
                fare = _maxDailyCap - currentDailyTotal;
            }
            
            // Execute state mutations safely
            currentDailyTotal += fare;
            MutateCommuterSummary(commuter.travelSummary, stationId, epochTime, fare);
            RecordHistoricalMetrics(cardNumber, entryStation, exitStation, journey.EntryTime, epochTime, fare);
            
            _activeJourneys.Remove(cardNumber);
            return true;
        }

        public Commuter getCommuterInfo(int cardNumber) => 
            _commuters.TryGetValue(cardNumber, out var commuter) ? commuter : null;

        public List<double> fareHistory(int cardNumber) => 
            _fareHistories.TryGetValue(cardNumber, out var history) 
                ? history.OrderByDescending(f => f).ToList() 
                : new List<double>();

        public Dictionary<string, double> getZoneWiseRevenue(long startTime, long endTime)
        {
            // Filters down completed journeys that occurred structurally inside the requested timespan
            return _completedJourneys
                .Where(j => j.StartTime >= startTime && j.EndTime <= endTime)
                .GroupBy(j => j.ZonePair)
                .Select(g => new { ZonePair = g.Key, TotalRevenue = g.Sum(x => x.Fare) })
                .Where(x => x.TotalRevenue > 0)
                .ToDictionary(x => x.ZonePair, x => x.TotalRevenue);
        }

        public List<string> getFrequentRoute(int cardNumber)
        {
            if (!_commuterRoutes.TryGetValue(cardNumber, out var routes)) return new List<string>();
            
            return routes
                .OrderByDescending(x => x.Value)
                .ThenBy(x => x.Key)
                .Take(3)
                .Select(x => x.Key)
                .ToList();
        }

        public double getDailyPassSavings(int cardNumber, long date)
        {
            if (!_dailyFares.TryGetValue(cardNumber, out var dailyMap) || !dailyMap.TryGetValue(date, out double actualFaresPaid))
            {
                return 0.0;
            }
            
            double dailyPassCost = _maxDailyCap * 0.8;
            double savings = actualFaresPaid - dailyPassCost;
            return Math.Max(0.0, savings);
        }

        // --- Private Utility Members ---

        /// <summary>
        /// Retrieves or creates a daily fare tracking reference map instance.
        /// </summary>
        private ref double GetDailyFareReference(int cardNumber, long dateId)
        {
            var map = _dailyFares[cardNumber];
            if (!map.ContainsKey(dateId)) map[dateId] = 0.0;
            
            // Using standard dictionary get, unfortunately ref returns for dict values requires Marshal/CollectionsMarshal in .NET 6+
            // To ensure compatibility, we return by value and the caller replaces it, but wait, ref return is requested.
            // For simplicity and avoiding compilation edge-cases with CollectionsMarshal, let's just simulate it structurally.
            return ref System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault(map, dateId, out _);
        }

        /// <summary>
        /// Updates the travel summary reference object.
        /// </summary>
        private static void MutateCommuterSummary(TravelSummary summary, int exitStation, long exitTime, double fare)
        {
            summary.lastExitStation = exitStation;
            summary.lastExitTime = exitTime;
            summary.totalFarePaid += fare;
            summary.totalTrips += 1;
            summary.averageFarePerTrip = summary.totalFarePaid / summary.totalTrips;
        }

        /// <summary>
        /// Orchestrates the persistence of historical metadata needed for analytical querying.
        /// </summary>
        private void RecordHistoricalMetrics(int cardNumber, Station entryStation, Station exitStation, long entryTime, long exitTime, double fare)
        {
            // Maintain localized deque bound to last 5 transactions
            var history = _fareHistories[cardNumber];
            history.AddLast(fare);
            if (history.Count > 5) history.RemoveFirst();

            // Store standardized routing format
            string route = $"{entryStation.stationName} to {exitStation.stationName}";
            if (!_commuterRoutes[cardNumber].ContainsKey(route)) _commuterRoutes[cardNumber][route] = 0;
            _commuterRoutes[cardNumber][route]++;

            // Append structured zone traversal sequence
            int z1 = Math.Min(entryStation.zone, exitStation.zone);
            int z2 = Math.Max(entryStation.zone, exitStation.zone);
            
            _completedJourneys.Add(new CompletedJourney
            {
                StartTime = entryTime,
                EndTime = exitTime,
                ZonePair = $"Zone{z1}-Zone{z2}",
                Fare = fare
            });
        }

        /// <summary>
        /// Haversine Formula for exact geodesic distances between stations.
        /// </summary>
        private static double CalculateDistance(Station s1, Station s2)
        {
            const double EarthRadiusKm = 6371.0;
            double lat1 = Math.PI * s1.latitude / 180.0;
            double lon1 = Math.PI * s1.longitude / 180.0;
            double lat2 = Math.PI * s2.latitude / 180.0;
            double lon2 = Math.PI * s2.longitude / 180.0;

            double dlat = lat2 - lat1;
            double dlon = lon2 - lon1;

            double a = Math.Pow(Math.Sin(dlat / 2.0), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Pow(Math.Sin(dlon / 2.0), 2);

            return 2 * EarthRadiusKm * Math.Asin(Math.Sqrt(a));
        }

        /// <summary>
        /// Converts generic MS unix timestamp representation into strict YYYYMMDD identity mapping.
        /// </summary>
        private static long ComputeDateId(long epochTime)
        {
            var date = DateTimeOffset.FromUnixTimeMilliseconds(epochTime).UtcDateTime;
            return date.Year * 10000L + date.Month * 100 + date.Day;
        }
    }

    /// <summary>
    /// Top-level application bootstrap and standard stream consumer.
    /// </summary>
    public static class Program
    {
        public static void Main()
        {
            // Initialize resilient standard stream processor
            string initialLine = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(initialLine)) return;

            var configArgs = ParseCommandTokens(initialLine);
            if (configArgs.Count < 4) return;
            
            int numRequests = int.Parse(configArgs[0]);
            double baseFare = double.Parse(configArgs[1], CultureInfo.InvariantCulture);
            double perKmRate = double.Parse(configArgs[2], CultureInfo.InvariantCulture);
            double maxDailyCap = double.Parse(configArgs[3], CultureInfo.InvariantCulture);

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int numStations)) return;

            var stationRegistry = new List<Station>(numStations);
            for (int i = 0; i < numStations; i++)
            {
                var parts = ParseCommandTokens(Console.ReadLine());
                if (parts.Count < 5) continue;
                
                int stationId = int.Parse(parts[0]);
                double lon = double.Parse(parts[^1], CultureInfo.InvariantCulture);
                double lat = double.Parse(parts[^2], CultureInfo.InvariantCulture);
                int zone = int.Parse(parts[^3]);
                string stationName = string.Join(" ", parts.GetRange(1, parts.Count - 4));
                
                stationRegistry.Add(new Station 
                { 
                    stationId = stationId, 
                    stationName = stationName, 
                    zone = zone, 
                    latitude = lat, 
                    longitude = lon 
                });
            }

            var manager = new MetroCardManager(stationRegistry, baseFare, perKmRate, maxDailyCap);

            // Execute operational streams
            for (int i = 0; i < numRequests; i++)
            {
                string line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                var payload = ParseCommandTokens(line);
                if (payload.Count == 0) continue;

                ProcessCommand(manager, payload);
            }
        }

        private static void ProcessCommand(MetroCardManager manager, IReadOnlyList<string> payload)
        {
            try
            {
                switch (payload[0])
                {
                    case "issueCard":
                        manager.issueCard(int.Parse(payload[1]), payload[2], payload[3]);
                        break;
                        
                    case "tapIn":
                        Console.WriteLine(manager.tapIn(
                            int.Parse(payload[1]), 
                            int.Parse(payload[2]), 
                            long.Parse(payload[3])).ToString().ToLowerInvariant());
                        break;
                        
                    case "tapOut":
                        Console.WriteLine(manager.tapOut(
                            int.Parse(payload[1]), 
                            int.Parse(payload[2]), 
                            long.Parse(payload[3])).ToString().ToLowerInvariant());
                        break;
                        
                    case "commuterInfo":
                        var commuter = manager.getCommuterInfo(int.Parse(payload[1]));
                        if (commuter?.travelSummary != null)
                        {
                            var ts = commuter.travelSummary;
                            Console.WriteLine(
                                $"{commuter.cardNumber} {commuter.commuterName} {commuter.commuterType} " +
                                $"{ts.lastEntryStation} {ts.lastExitStation} {ts.lastEntryTime} {ts.lastExitTime} " +
                                $"{ts.totalFarePaid.ToString("0.0#", CultureInfo.InvariantCulture)} " +
                                $"{ts.totalTrips} " +
                                $"{ts.averageFarePerTrip.ToString("0.0#", CultureInfo.InvariantCulture)}");
                        }
                        break;
                        
                    case "fareHistory":
                        var historicalFares = manager.fareHistory(int.Parse(payload[1]));
                        foreach (var fare in historicalFares)
                        {
                            Console.WriteLine(fare.ToString("0.0#", CultureInfo.InvariantCulture));
                        }
                        break;
                        
                    case "zoneRevenue":
                        var revenueMap = manager.getZoneWiseRevenue(long.Parse(payload[1]), long.Parse(payload[2]));
                        var optimalSort = revenueMap
                            .OrderByDescending(r => r.Value)
                            .ThenBy(r => r.Key);
                            
                        foreach (var segment in optimalSort)
                        {
                            Console.WriteLine($"{segment.Key}:{segment.Value.ToString("0.0#", CultureInfo.InvariantCulture)}");
                        }
                        break;
                        
                    case "frequentRoute":
                        var routes = manager.getFrequentRoute(int.Parse(payload[1]));
                        foreach (var rt in routes) Console.WriteLine(rt);
                        break;
                        
                    case "dailySavings":
                        double dailySavingsAccumulation = manager.getDailyPassSavings(
                            int.Parse(payload[1]), 
                            long.Parse(payload[2]));
                            
                        Console.WriteLine(dailySavingsAccumulation.ToString("0.0#", CultureInfo.InvariantCulture));
                        break;
                }
            }
            catch
            {
                // Mute failure blocks inside isolated command streams to ensure resilient loop continuation.
            }
        }

        /// <summary>
        /// Lexical token processor capable of extracting segmented shell-style arguments mapping string structures natively.
        /// </summary>
        private static List<string> ParseCommandTokens(string input)
        {
            var tokens = new List<string>();
            var tokenBuffer = new StringBuilder();
            bool inQuotes = false;

            foreach (char character in input)
            {
                if (character == '\"')
                {
                    inQuotes = !inQuotes;
                }
                else if (char.IsWhiteSpace(character) && !inQuotes)
                {
                    if (tokenBuffer.Length > 0)
                    {
                        tokens.Add(tokenBuffer.ToString());
                        tokenBuffer.Clear();
                    }
                }
                else
                {
                    tokenBuffer.Append(character);
                }
            }

            if (tokenBuffer.Length > 0)
            {
                tokens.Add(tokenBuffer.ToString());
            }

            return tokens;
        }
    }
}
