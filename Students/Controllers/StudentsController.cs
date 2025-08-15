using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Students.Mappings;
using Students.Models;
using Students.Repositories;
using Students.Services;

namespace Students.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _repo;
        private readonly ILogger<StudentsController> _log;

        private readonly IStudentService _studentService;


        public StudentsController(
                IStudentRepository repo,
                ILogger<StudentsController> log,
                IStudentService studentService)
        {
            _repo = repo;
            _log = log;
            _studentService = studentService;
        }

        // GET: /api/students
        [HttpGet]
        public ActionResult<IEnumerable<StudentResponseDto>> GetAll()
            => Ok(_studentService.GetAll());

        // GET: /api/students/5
        [HttpGet("{id:int}", Name = "GetStudent")]
        public ActionResult<StudentResponseDto> GetById(int id)
            => _studentService.GetById(id) is { } s ? Ok(s) : NotFound();

        // POST: /api/students
        [HttpPost]
        public ActionResult<StudentRequestDto> Create(StudentRequestDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var student = _studentService.Create(dto);

            return CreatedAtRoute("GetStudent", new { id = student.Id }, student);
        }

        // PUT: /api/students/5
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, StudentRequestDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var success = _studentService.Update(id, dto);

            return success ? NoContent() : NotFound();
        }

        // DELETE: /api/students/5
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var success = _studentService.Delete(id);

            return success ? NoContent() : NotFound();
        }
    }
}
