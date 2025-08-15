using System.Collections.Concurrent;
using System.Text.Json;
using Students.Models;

namespace Students.Repositories
{
    public class JsonFileStudentRepository : IStudentRepository
    {
        private readonly string _path;
        private readonly object _lock = new();
        private readonly ConcurrentDictionary<int, Student> _store = new();
        private int _nextId = 1;

        public JsonFileStudentRepository(IWebHostEnvironment env, IConfiguration config)
         {
            var rel = config["STUDENTS_PATH"] ?? "App_Data/students.json";
            _path = Path.IsPathRooted(rel) ? rel : Path.Combine(env.ContentRootPath, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            Load();
        }

        void Load()
        {
            Console.WriteLine($"[Students] JSON store: {_path}");

            if (!File.Exists(_path))
            {
                Save(); // create an empty file
                return;
            }

            var json = File.ReadAllText(_path);
            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            var items = JsonSerializer.Deserialize<List<Student>>(json, opts) ?? new();
            _store.Clear();

            // Repair missing/zero Ids so entries don't collide at 0
            var maxId = items.Where(s => s is not null && s.Id > 0)
                             .Select(s => s.Id)
                             .DefaultIfEmpty(0)
                             .Max();

            var next = maxId + 1;
            var repaired = false;

            foreach (var s in items)
            {
                if (s.Id <= 0) { s.Id = next++; repaired = true; }
                _store[s.Id] = Clone(s);
            }

            _nextId = next;

            if (repaired) Save();
        }

        void Save()
        {
            // snapshot and write atomically
            var list = _store.Values.OrderBy(s => s.Id).Select(Clone).ToList();
            var saveOpts = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var tmp = _path + ".tmp";
            File.WriteAllText(tmp, JsonSerializer.Serialize(list, saveOpts));
            if (File.Exists(_path)) File.Delete(_path);
            File.Move(tmp, _path);
        }
        

        public IEnumerable<Student> GetAll() => _store.Values.OrderBy(s => s.Id).Select(Clone).ToList();

        public Student? GetById(int id) => _store.TryGetValue(id, out var s) ? Clone(s) : null;

        public Student Insert(Student student)
        {
            ArgumentNullException.ThrowIfNull(student);
            lock (_lock)
            {
                var id = _nextId++;
                student.Id = id;
                _store[id] = Clone(student);
                Save();
                return Clone(student);
            }
        }

        public bool Update(int id, Student student)
        {
            ArgumentNullException.ThrowIfNull(student);
            lock (_lock)
            {
                if (!_store.ContainsKey(id)) return false;
                student.Id = id;           // route id is source of truth
                _store[id] = Clone(student);
                Save();
                return true;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock)
            {
                var ok = _store.TryRemove(id, out _);
                if (ok) Save();
                return ok;
            }
        }

        static Student Clone(Student s) => new Student
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Address = s.Address,
            DateOfBirth = s.DateOfBirth,
            Email = s.Email,
            Phone = s.Phone,
            Grade = s.Grade
        };
    }
}
