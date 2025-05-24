using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class Program
{
    private static CrudServiceAsync<Bus> _busService;
    private static readonly string _busDataFilePath = "bus_data.json";

    // Примітиви синхронізації для демонстрації (окрім тих, що вже вбудовані в CrudServiceAsync)
    private static ManualResetEvent _manualResetEvent = new ManualResetEvent(false);
    private static Mutex _globalMutex = new Mutex(false, "MyGlobalCrudServiceMutex"); // For cross-process sync
    private static AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

    public static async Task Main(string[] args)
    {
        // Очистити файл даних перед стартом, щоб мати чисту колекцію
        if (File.Exists(_busDataFilePath))
        {
            File.Delete(_busDataFilePath);
        }

        _busService = new CrudServiceAsync<Bus>(_busDataFilePath);
        int numberOfBusesToCreate = 1000;

        Console.WriteLine($"Creating {numberOfBusesToCreate} buses asynchronously...");

        Stopwatch stopwatch = Stopwatch.StartNew();

        // Використання Parallel.ForEach для паралельного створення об'єктів
        var tasks = new List<Task>();
        Parallel.ForEach(Enumerable.Range(0, numberOfBusesToCreate), async i =>
        {
            Bus newBus = Bus.CreateNew();
            bool created = await _busService.CreateAsync(newBus);
            if (!created)
            {
                Console.WriteLine($"Failed to create bus with ID: {newBus.Id}");
            }
        });
        stopwatch.Stop();
        Console.WriteLine($"Finished creating {numberOfBusesToCreate} buses in {stopwatch.ElapsedMilliseconds} ms.");

        // Демонстрація використання примітивів синхронізації
        Console.WriteLine("\n--- Demonstrating Synchronization Primitives ---");

        // ManualResetEvent: Allow multiple threads to proceed once signaled
        Console.WriteLine("ManualResetEvent Demo:");
        Task.Run(() =>
        {
            Console.WriteLine("Thread 1: Waiting for ManualResetEvent...");
            _manualResetEvent.WaitOne(); // Blocks until signaled
            Console.WriteLine("Thread 1: ManualResetEvent signaled! Proceeding.");
        });
        Task.Run(() =>
        {
            Console.WriteLine("Thread 2: Waiting for ManualResetEvent...");
            _manualResetEvent.WaitOne(); // Blocks until signaled
            Console.WriteLine("Thread 2: ManualResetEvent signaled! Proceeding.");
        });
        Console.WriteLine("Main Thread: Signaling ManualResetEvent in 2 seconds...");
        await Task.Delay(2000);
        _manualResetEvent.Set(); // Signals both threads
        Console.WriteLine("Main Thread: ManualResetEvent signaled.");
        await Task.Delay(100); // Give threads time to print
        _manualResetEvent.Reset(); // Reset for future use

        // AutoResetEvent: Allows one thread to proceed, then automatically resets
        Console.WriteLine("\nAutoResetEvent Demo:");
        Task.Run(() =>
        {
            Console.WriteLine("Thread A: Waiting for AutoResetEvent...");
            _autoResetEvent.WaitOne(); // Blocks until signaled
            Console.WriteLine("Thread A: AutoResetEvent signaled! Proceeding.");
        });
        Task.Run(() =>
        {
            Console.WriteLine("Thread B: Waiting for AutoResetEvent...");
            _autoResetEvent.WaitOne(); // Blocks until signaled (will block until Thread A or another signal)
            Console.WriteLine("Thread B: AutoResetEvent signaled! Proceeding.");
        });
        Console.WriteLine("Main Thread: Signaling AutoResetEvent for Thread A in 2 seconds...");
        await Task.Delay(2000);
        _autoResetEvent.Set(); // Signals Thread A
        Console.WriteLine("Main Thread: AutoResetEvent signaled for Thread A.");
        await Task.Delay(1000); // Give Thread A time
        Console.WriteLine("Main Thread: Signaling AutoResetEvent for Thread B in 1 second...");
        await Task.Delay(1000);
        _autoResetEvent.Set(); // Signals Thread B
        Console.WriteLine("Main Thread: AutoResetEvent signaled for Thread B.");
        await Task.Delay(100); // Give threads time to print


        // Mutex: For cross-process synchronization (acquire and release)
        Console.WriteLine("\nMutex Demo (Simulating Cross-Process Lock):");
        bool hasMutex = false;
        try
        {
            // Try to acquire the mutex. If another process holds it, this will wait.
            hasMutex = _globalMutex.WaitOne(TimeSpan.FromSeconds(5));
            if (hasMutex)
            {
                Console.WriteLine("Main Thread: Acquired global Mutex. Performing some operation...");
                // Simulate work that needs exclusive access
                await Task.Delay(2000);
                Console.WriteLine("Main Thread: Mutex operation complete.");
            }
            else
            {
                Console.WriteLine("Main Thread: Could not acquire global Mutex within timeout.");
            }
        }
        finally
        {
            if (hasMutex)
            {
                _globalMutex.ReleaseMutex();
                Console.WriteLine("Main Thread: Released global Mutex.");
            }
        }


        Console.WriteLine("\n--- CRUD Operations & Data Analysis ---");

        // Збереження колекції у файл
        Console.WriteLine("Saving all buses to file...");
        bool saved = await _busService.SaveAsync();
        Console.WriteLine(saved ? "Buses saved successfully." : "Failed to save buses.");

        // Читання всіх об'єктів для аналізу
        IEnumerable<Bus> allBuses = await _busService.ReadAllAsync();
        Console.WriteLine($"Total buses in service: {allBuses.Count()}");

        // Знаходження мінімальних, максимальних та середніх значень
        if (allBuses.Any())
        {
            double minFuel = allBuses.Min(b => b.FuelConsumptionPer100Km);
            double maxFuel = allBuses.Max(b => b.FuelConsumptionPer100Km);
            double avgFuel = allBuses.Average(b => b.FuelConsumptionPer100Km);

            int minPassenger = allBuses.Min(b => b.PassengerCapacity);
            int maxPassenger = allBuses.Max(b => b.PassengerCapacity);
            double avgPassenger = allBuses.Average(b => b.PassengerCapacity);

            int minYear = allBuses.Min(b => b.Year);
            int maxYear = allBuses.Max(b => b.Year);
            double avgYear = allBuses.Average(b => b.Year);

            Console.WriteLine("\n--- Statistical Analysis ---");
            Console.WriteLine($"Fuel Consumption (L/100km): Min={minFuel:F2}, Max={maxFuel:F2}, Avg={avgFuel:F2}");
            Console.WriteLine($"Passenger Capacity: Min={minPassenger}, Max={maxPassenger}, Avg={avgPassenger:F2}");
            Console.WriteLine($"Year: Min={minYear}, Max={maxYear}, Avg={avgYear:F2}");
        }
        else
        {
            Console.WriteLine("No buses found for analysis.");
        }

        // Демонстрація пагінації
        Console.WriteLine("\n--- Pagination Demo ---");
        int pageSize = 10;
        int pageNumber = 1;
        IEnumerable<Bus> firstPage = await _busService.ReadAllAsync(pageNumber, pageSize);
        Console.WriteLine($"\nPage {pageNumber} (first {pageSize} buses):");
        foreach (var bus in firstPage)
        {
            Console.WriteLine(bus);
        }

        pageNumber = 2;
        IEnumerable<Bus> secondPage = await _busService.ReadAllAsync(pageNumber, pageSize);
        Console.WriteLine($"\nPage {pageNumber} (next {pageSize} buses):");
        foreach (var bus in secondPage)
        {
            Console.WriteLine(bus);
        }

        // Демонстрація Update
        Console.WriteLine("\n--- Update Demo ---");
        Bus busToUpdate = allBuses.FirstOrDefault();
        if (busToUpdate != null)
        {
            Console.WriteLine($"Original bus: {busToUpdate}");
            string originalModel = busToUpdate.Model;
            busToUpdate.Model = "Updated Model";
            bool updated = await _busService.UpdateAsync(busToUpdate);
            if (updated)
            {
                Console.WriteLine($"Bus updated successfully: {busToUpdate}");
            }
            else
            {
                Console.WriteLine("Failed to update bus.");
            }
            // Повернення до оригінального стану
            busToUpdate.Model = originalModel;
            await _busService.UpdateAsync(busToUpdate);
        }

        // Демонстрація Remove
        Console.WriteLine("\n--- Remove Demo ---");
        Bus busToRemove = allBuses.Skip(1).FirstOrDefault(); // Remove the second bus
        if (busToRemove != null)
        {
            Console.WriteLine($"Attempting to remove bus: {busToRemove.Id}");
            bool removed = await _busService.RemoveAsync(busToRemove);
            if (removed)
            {
                Console.WriteLine($"Bus {busToRemove.Id} removed successfully.");
                Console.WriteLine($"Total buses after removal: {(await _busService.ReadAllAsync()).Count()}");
            }
            else
            {
                Console.WriteLine($"Failed to remove bus {busToRemove.Id}.");
            }
        }

        // Збереження змін після оновлень та видалень
        await _busService.SaveAsync();

        Console.WriteLine("\nApplication finished.");
    }
}