using System.Collections.Concurrent;
using Students.Models;
using Students.Enums;

namespace Students.Repositories
{
    public class InMemoryStudentRepository : IStudentRepository
    {
        private readonly ConcurrentDictionary<int, Student> _store = new();
        private int _nextId = 1;
        private readonly object _idLock = new();

        public InMemoryStudentRepository()
        {
            // Seed data
            Insert(new Student {
                FirstName = "Ada", LastName = "Lovelace", Address = "1 Analytical Way",
                DateOfBirth = new DateTime(2008, 12, 10), Email = "ada@example.com",
                Phone = "321-555-0101", Grade = "12", EnrollmentStatus = EnrollmentStatus.Active
            });
            Insert(new Student {
                FirstName = "Alan", LastName = "Turing", Address = "23 Enigma Rd",
                DateOfBirth = new DateTime(2009, 6, 23), Email = "alan@example.com",
                Phone = "(321) 555-0102", Grade = "11", EnrollmentStatus = EnrollmentStatus.Active
            });
            Insert(new Student {
                FirstName = "Grace", LastName = "Hopper", Address = "99 Cobol Ct",
                DateOfBirth = new DateTime(2010, 12, 9),
                Grade = "K", EnrollmentStatus = EnrollmentStatus.Inactive
            });
        }

        private int NextId()
        {
            lock (_idLock) return _nextId++;
        }

        public IEnumerable<Student> GetAll() => _store.Values.OrderBy(s => s.Id);

        public Student? GetById(int id) => _store.TryGetValue(id, out var s) ? s : null;

        public Student Insert(Student student)
        {
            var id = NextId();
            student.Id = id;
            _store[id] = Clone(student);
            return GetById(id)!;
        }

        public bool Update(int id, Student student)
        {
            if (!_store.ContainsKey(id)) return false;
            student.Id = id;
            _store[id] = Clone(student);
            return true;
        }

        public bool Delete(int id) => _store.TryRemove(id, out _);

        private static Student Clone(Student s) => new Student
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Address = s.Address,
            DateOfBirth = s.DateOfBirth,
            Email = s.Email,
            Phone = s.Phone,
            Grade = s.Grade,
            EnrollmentStatus = s.EnrollmentStatus
        };
    }
}
