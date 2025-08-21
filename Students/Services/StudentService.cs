using Students.Models;
using Students.Repositories;
using Students.Mappings;
using Students.Exceptions;

namespace Students.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repo;
        private readonly ILogger<StudentService> _log;

        public StudentService(IStudentRepository repo, ILogger<StudentService> log)
        {
            _repo = repo;
            _log = log;
        }

        public IEnumerable<StudentResponseDto> GetAll()
        {
            _log.LogInformation("GET GetAll");
           return _repo.GetAll().Select(s => s.ToResponseDto());
        }

        public StudentResponseDto? GetById(int id)
        {
            _log.LogInformation("GET /students/{Id}", id);
            return _repo.GetById(id)?.ToResponseDto();
        }

        public StudentResponseDto Create(StudentRequestDto dto)
        {
            _log.LogInformation("POST Create/{student}", dto);
            ThrowErrorIfDuplicateEmail(dto);

            var created = _repo.Insert(dto.ToEntity()); // repo assigns Id

            return created.ToResponseDto();
        }

        public bool Update(int id, StudentRequestDto dto)
        {
            _log.LogInformation("PUT Update/{student}", dto);

            // Look up the current record
            var current = _repo.GetAll().FirstOrDefault(s => s.Id == id);
            if (current is null) return false; // or throw NotFound if that's your pattern

            var newEmail = Normalize(dto.Email);
            var oldEmail = Normalize(current.Email);

            // Only check for dupes if the email changed, and exclude the current id
            if (!string.Equals(newEmail, oldEmail, StringComparison.Ordinal))
            {
                ThrowErrorIfDuplicateEmail(dto, excludeId: id);
            }

            return _repo.Update(id, dto.ToEntity(id)); // route id wins
        }

        private void ThrowErrorIfDuplicateEmail(StudentRequestDto dto, int? excludeId = null)
        {
            var email = Normalize(dto.Email);
            if (email is null) return;

            var clash = _repo.GetAll()
                .FirstOrDefault(s => Normalize(s.Email) == email &&
                                    (!excludeId.HasValue || s.Id != excludeId.Value));

            if (clash is not null)
                throw new DuplicateEmailException(dto.Email!, clash.Id);
        }

        public bool Delete(int id)
        {
            _log.LogInformation("DELETE {student ID}", id);
            return _repo.Delete(id);
        }

        public IEnumerable<StudentResponseDto> Search(StudentSearchRequest req)
        {
            // 1) normalize/sanitize
            var name = req.NameContains?.Trim();
            if (string.IsNullOrWhiteSpace(name)) name = null;

            // 2) pull everything once from repo (PoC) and filter in-memory
            var q = _repo.GetAll().AsEnumerable();

            if (req.Grade is { } g) q = q.Where(s => s.Grade == g);
            if (req.EnrollmentStatus is { } st) q = q.Where(s => s.EnrollmentStatus == st);
            if (name is not null)
                q = q.Where(s =>
                    (s.FirstName?.Contains(name, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (s.LastName?.Contains(name, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (s.Email?.Contains(name, StringComparison.OrdinalIgnoreCase) ?? false));

            // 3) sort
            q = (req.SortBy?.ToLowerInvariant()) switch
            {
                "firstname" => req.Desc ? q.OrderByDescending(s => s.FirstName) : q.OrderBy(s => s.FirstName),
                "lastname" => req.Desc ? q.OrderByDescending(s => s.LastName) : q.OrderBy(s => s.LastName),
                "grade" => req.Desc ? q.OrderByDescending(s => s.Grade) : q.OrderBy(s => s.Grade),
                _ => req.Desc ? q.OrderByDescending(s => s.Id) : q.OrderBy(s => s.Id),
            };

            // 4) page
            var skip = (req.Page - 1) * req.PageSize;
            q = q.Skip(skip).Take(req.PageSize);

            // 5) map to response DTOs
            return q.Select(s => new StudentResponseDto(
                s.Id, s.FirstName, s.LastName, s.Email, s.Grade, s.EnrollmentStatus
            )).ToList();
        }

        private static string? Normalize(string? e)
            => string.IsNullOrWhiteSpace(e) ? null : e.Trim().ToLowerInvariant();

    }
}
