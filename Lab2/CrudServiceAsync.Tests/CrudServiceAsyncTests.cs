using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;



namespace CrudServiceAsync.Tests
{
    
    public class TestBus : BaseEntity
    {
        public string Name { get; set; }
        public int NumberOfWheels { get; set; }
        public double AverageSpeed { get; set; }

        public static TestBus CreateNew()
        {
            var random = new Random();
            return new TestBus
            {
                Name = $"TestBus_{Guid.NewGuid().ToString().Substring(0, 4)}",
                NumberOfWheels = random.Next(4, 10),
                AverageSpeed = Math.Round(random.NextDouble() * (100.0 - 50.0) + 50.0, 2)
            };
        }
    }

    [TestFixture]
    public class CrudServiceAsyncTests
    {
        private string _testFilePath;
        private CrudServiceAsync<TestBus> _service;

        [SetUp] 
        public void Setup()
        {
       
            _testFilePath = Path.Combine(Path.GetTempPath(), $"test_data_{Guid.NewGuid()}.json");
            _service = new CrudServiceAsync<TestBus>(_testFilePath);
        }

        [TearDown] 
        public void Teardown()
        {
            
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [Test]
        public async Task CreateAsync_AddsElement_ReturnsTrue()
        {
            
            var bus = TestBus.CreateNew();

          
            bool result = await _service.CreateAsync(bus);


            Assert.IsTrue(result);
            var retrievedBus = await _service.ReadAsync(bus.Id);
            Assert.IsNotNull(retrievedBus);
            Assert.AreEqual(bus.Id, retrievedBus.Id);
            Assert.AreEqual(bus.Name, retrievedBus.Name);
        }

        [Test]
        public async Task CreateAsync_AddingExistingElement_ReturnsFalse()
        {
          
            var bus = TestBus.CreateNew();
            await _service.CreateAsync(bus);

           
            bool result = await _service.CreateAsync(bus);

          
            Assert.IsFalse(result);
            var allBuses = await _service.ReadAllAsync();
            Assert.AreEqual(1, allBuses.Count()); 
        }

        [Test]
        public async Task ReadAsync_ExistingElement_ReturnsElement()
        {
        
            var bus = TestBus.CreateNew();
            await _service.CreateAsync(bus);

           
            var retrievedBus = await _service.ReadAsync(bus.Id);

        
            Assert.IsNotNull(retrievedBus);
            Assert.AreEqual(bus.Id, retrievedBus.Id);
        }

        [Test]
        public async Task ReadAsync_NonExistingElement_ReturnsNull()
        {
           
            var nonExistingId = Guid.NewGuid();

            
            var retrievedBus = await _service.ReadAsync(nonExistingId);

          
            Assert.IsNull(retrievedBus);
        }

        [Test]
        public async Task ReadAllAsync_ReturnsAllElements()
        {
           
            await _service.CreateAsync(TestBus.CreateNew());
            await _service.CreateAsync(TestBus.CreateNew());
            await _service.CreateAsync(TestBus.CreateNew());

          
            var allBuses = await _service.ReadAllAsync();

          
            Assert.AreEqual(3, allBuses.Count());
        }

        [Test]
        public async Task ReadAllAsync_Pagination_ReturnsCorrectPage()
        {
           
            for (int i = 0; i < 25; i++)
            {
                await _service.CreateAsync(TestBus.CreateNew());
            }

         
            var page1 = await _service.ReadAllAsync(1, 10);
            var page2 = await _service.ReadAllAsync(2, 10);
            var page3 = await _service.ReadAllAsync(3, 10);

        
            Assert.AreEqual(10, page1.Count());
            Assert.AreEqual(10, page2.Count());
            Assert.AreEqual(5, page3.Count()); 
            Assert.IsEmpty(await _service.ReadAllAsync(4, 10)); 
        }

        [Test]
        public void ReadAllAsync_Pagination_InvalidPage_ThrowsArgumentOutOfRangeException()
        {
           
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _service.ReadAllAsync(0, 10));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _service.ReadAllAsync(-1, 10));
        }

        [Test]
        public void ReadAllAsync_Pagination_InvalidAmount_ThrowsArgumentOutOfRangeException()
        {
           
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _service.ReadAllAsync(1, 0));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _service.ReadAllAsync(1, -5));
        }


        [Test]
        public async Task UpdateAsync_ExistingElement_UpdatesAndReturnsTrue()
        {
           
            var bus = TestBus.CreateNew();
            await _service.CreateAsync(bus);

          
            bus.Name = "Updated Test Bus Name";
            bus.AverageSpeed = 99.9;
            bool result = await _service.UpdateAsync(bus);

           
            Assert.IsTrue(result);
            var updatedBus = await _service.ReadAsync(bus.Id);
            Assert.AreEqual("Updated Test Bus Name", updatedBus.Name);
            Assert.AreEqual(99.9, updatedBus.AverageSpeed);
        }

        [Test]
        public async Task UpdateAsync_NonExistingElement_ReturnsFalse()
        {
         
            var bus = TestBus.CreateNew(); 

       
            bool result = await _service.UpdateAsync(bus);

      
            Assert.IsFalse(result);
        }

        [Test]
        public async Task RemoveAsync_ExistingElement_RemovesAndReturnsTrue()
        {
       
            var bus = TestBus.CreateNew();
            await _service.CreateAsync(bus);

         
            bool result = await _service.RemoveAsync(bus);

         
            Assert.IsTrue(result);
            var retrievedBus = await _service.ReadAsync(bus.Id);
            Assert.IsNull(retrievedBus);
            Assert.AreEqual(0, (await _service.ReadAllAsync()).Count());
        }

        [Test]
        public async Task RemoveAsync_NonExistingElement_ReturnsFalse()
        {
           
            var bus = TestBus.CreateNew(); 

           
            bool result = await _service.RemoveAsync(bus);

        
            Assert.IsFalse(result);
        }

        [Test]
        public async Task SaveAsync_SavesDataToFile()
        {
            
            var bus1 = TestBus.CreateNew();
            var bus2 = TestBus.CreateNew();
            await _service.CreateAsync(bus1);
            await _service.CreateAsync(bus2);

      
            bool saved = await _service.SaveAsync();

         
            Assert.IsTrue(saved);
            Assert.IsTrue(File.Exists(_testFilePath));

           
            var newService = new CrudServiceAsync<TestBus>(_testFilePath);
            var loadedBuses = await newService.ReadAllAsync();
            Assert.AreEqual(2, loadedBuses.Count());
            Assert.Contains(bus1.Id, loadedBuses.Select(b => b.Id).ToList());
            Assert.Contains(bus2.Id, loadedBuses.Select(b => b.Id).ToList());
        }

        [Test]
        public async Task CrudServiceAsync_Constructor_LoadsExistingData()
        {
           
            var bus1 = TestBus.CreateNew();
            var bus2 = TestBus.CreateNew();
            await _service.CreateAsync(bus1);
            await _service.CreateAsync(bus2);
            await _service.SaveAsync();

            var newService = new CrudServiceAsync<TestBus>(_testFilePath);
            var loadedBuses = await newService.ReadAllAsync();

          
            Assert.AreEqual(2, loadedBuses.Count());
            Assert.Contains(bus1.Id, loadedBuses.Select(b => b.Id).ToList());
            Assert.Contains(bus2.Id, loadedBuses.Select(b => b.Id).ToList());
        }

        [Test]
        public async Task CrudServiceAsync_IsEnumerable()
        {
            
            var bus1 = TestBus.CreateNew();
            var bus2 = TestBus.CreateNew();
            await _service.CreateAsync(bus1);
            await _service.CreateAsync(bus2);

            
            var listFromEnumerable = new List<TestBus>();
            foreach (var bus in _service) 
            {
                listFromEnumerable.Add(bus);
            }

            
            Assert.AreEqual(2, listFromEnumerable.Count);
            Assert.Contains(bus1, listFromEnumerable);
            Assert.Contains(bus2, listFromEnumerable);
        }

        [Test]
        public async Task ThreadSafety_ConcurrentCreate_ShouldHandleManyElements()
        {
           
            int numberOfConcurrentOperations = 1000;
            var busesToCreate = new List<TestBus>();
            for (int i = 0; i < numberOfConcurrentOperations; i++)
            {
                busesToCreate.Add(TestBus.CreateNew());
            }

            
            var tasks = busesToCreate.Select(bus => _service.CreateAsync(bus)).ToList();
            await Task.WhenAll(tasks); 

            
            var allBuses = await _service.ReadAllAsync();
           
            Assert.AreEqual(numberOfConcurrentOperations, allBuses.Count());
            foreach (var bus in busesToCreate)
            {
                Assert.IsNotNull(await _service.ReadAsync(bus.Id));
            }
        }

        [Test]
        public async Task ThreadSafety_ConcurrentSaveOperations_OnlyOneSucceedsAtTime()
        {
           
            for (int i = 0; i < 50; i++)
            {
                await _service.CreateAsync(TestBus.CreateNew());
            }

            
            var tasks = new List<Task<bool>>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(_service.SaveAsync());
            }

            await Task.WhenAll(tasks);

        
            Assert.IsTrue(File.Exists(_testFilePath));
            var newService = new CrudServiceAsync<TestBus>(_testFilePath);
            Assert.AreEqual(50, (await newService.ReadAllAsync()).Count());
        }
    }
}