using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Transport.Common.Services
{
    public class CrudService<T> : ICrudService<T> where T : class
    {

        private readonly Dictionary<Guid, T> _data;

        public CrudService()
        {
            _data = new Dictionary<Guid, T>();
        }

        public void Create(T element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element), "Елемент не може бути null.");
            }

            PropertyInfo idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null || idProperty.PropertyType != typeof(Guid))
            {
                throw new InvalidOperationException("Тип T повинен мати властивість 'Id' типу Guid.");
            }

            Guid id = (Guid)idProperty.GetValue(element);
            if (id == Guid.Empty)
            {
                id = Guid.NewGuid();
                idProperty.SetValue(element, id);
            }

            if (_data.ContainsKey(id))
            {
                throw new InvalidOperationException($"Елемент з ID {id} вже існує.");
            }

            _data.Add(id, element);
            Console.WriteLine($"[CRUD] Створено: {typeof(T).Name} з ID: {id}");
        }

        public T Read(Guid id)
        {
            _data.TryGetValue(id, out T element);
            Console.WriteLine($"[CRUD] Прочитано: {typeof(T).Name} з ID: {id} - {(element != null ? "Знайдено" : "Не знайдено")}");
            return element;
        }

        public IEnumerable<T> ReadAll()
        {
            Console.WriteLine($"[CRUD] Прочитано всі {typeof(T).Name} елементи.");
            return _data.Values.ToList();
        }

        public void Update(T element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element), "Елемент не може бути null.");
            }

            PropertyInfo idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null || idProperty.PropertyType != typeof(Guid))
            {
                throw new InvalidOperationException("Тип T повинен мати властивість 'Id' типу Guid.");
            }

            Guid id = (Guid)idProperty.GetValue(element);

            if (!_data.ContainsKey(id))
            {
                throw new KeyNotFoundException($"Елемент з ID {id} не знайдено для оновлення.");
            }

            _data[id] = element;
            Console.WriteLine($"[CRUD] Оновлено: {typeof(T).Name} з ID: {id}");
        }

        public void Remove(T element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element), "Елемент не може бути null.");
            }

            PropertyInfo idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null || idProperty.PropertyType != typeof(Guid))
            {
                throw new InvalidOperationException("Тип T повинен мати властивість 'Id' типу Guid.");
            }

            Guid id = (Guid)idProperty.GetValue(element);

            if (!_data.Remove(id))
            {
                throw new KeyNotFoundException($"Елемент з ID {id} не знайдено для видалення.");
            }
            Console.WriteLine($"[CRUD] Видалено: {typeof(T).Name} з ID: {id}");
        }
    }
}