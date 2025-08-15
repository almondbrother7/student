using Students.Models;

namespace Students.Repositories
{
    public interface IStudentRepository
    {
        IEnumerable<Student> GetAll();
        Student? GetById(int id);
        Student Insert(Student student);   // assigns Id
        bool Update(int id, Student student);
        bool Delete(int id);
    }
}
