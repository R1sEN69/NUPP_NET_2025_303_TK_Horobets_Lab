using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : BaseEntity
{
    private readonly ConcurrentDictionary<Guid, T> _data;
    private readonly string _filePath;
    private readonly SemaphoreSlim _fileAccessSemaphore = new SemaphoreSlim(1, 1); // Only one writer at a time
    private readonly object _collectionLock = new object(); // Lock for collection modifications (e.g., adding/removing)

    public CrudServiceAsync(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _data = new ConcurrentDictionary<Guid, T>();
        LoadDataFromFileAsync().Wait(); // Synchronously wait for initial load
    }

    private async Task LoadDataFromFileAsync()
    {
        if (!File.Exists(_filePath))
        {
            return;
        }

        await _fileAccessSemaphore.WaitAsync();
        try
        {
            string json = await File.ReadAllTextAsync(_filePath);
            var loadedData = JsonSerializer.Deserialize<List<T>>(json);
            if (loadedData != null)
            {
                foreach (var item in loadedData)
                {
                    _data.TryAdd(item.Id, item);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data from file: {ex.Message}");
        }
        finally
        {
            _fileAccessSemaphore.Release();
        }
    }

    public async Task<bool> CreateAsync(T element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));

        lock (_collectionLock) // Ensure thread-safe addition
        {
            if (_data.ContainsKey(element.Id))
            {
                return false; // Element with this ID already exists
            }
            return _data.TryAdd(element.Id, element);
        }
    }

    public Task<T> ReadAsync(Guid id)
    {
        _data.TryGetValue(id, out T element);
        return Task.FromResult(element);
    }

    public Task<IEnumerable<T>> ReadAllAsync()
    {
        return Task.FromResult<IEnumerable<T>>(_data.Values.ToList());
    }

    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than zero.");
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");

        var pagedData = _data.Values
                              .Skip((page - 1) * amount)
                              .Take(amount)
                              .ToList();
        return Task.FromResult<IEnumerable<T>>(pagedData);
    }

    public async Task<bool> UpdateAsync(T element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));

        lock (_collectionLock) // Ensure thread-safe update
        {
            if (_data.ContainsKey(element.Id))
            {
                _data[element.Id] = element; // Update the element
                return true;
            }
            return false; // Element not found
        }
    }

    public async Task<bool> RemoveAsync(T element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));

        lock (_collectionLock) // Ensure thread-safe removal
        {
            return _data.TryRemove(element.Id, out _);
        }
    }

    public async Task<bool> SaveAsync()
    {
        await _fileAccessSemaphore.WaitAsync(); // Ensure only one save operation at a time
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_data.Values.ToList(), options);
            await File.WriteAllTextAsync(_filePath, json);
            Console.WriteLine($"Data saved to {_filePath}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data to file: {ex.Message}");
            return false;
        }
        finally
        {
            _fileAccessSemaphore.Release();
        }
    }

    // Implementation of IEnumerable<T>
    public IEnumerator<T> GetEnumerator()
    {
        return _data.Values.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}