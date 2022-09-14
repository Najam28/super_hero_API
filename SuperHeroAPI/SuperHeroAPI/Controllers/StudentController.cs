using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperHeroAPI.Data;
using SuperHeroAPI.Models;

namespace SuperHeroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly DataContext std_Context;

        public StudentController(DataContext std_context)
        {
            std_Context = std_context;
        }
        //Get (Read) Student Data
        [HttpGet]
        public async Task<ActionResult<List<Student>>> Get()
        {
            return Ok(await std_Context.students.ToListAsync());
        }
        //Get (Read) Student Data with id
        [HttpGet ("{id}")]
        public async Task<ActionResult<List<Student>>> Get_Std_id(int id )
        {
            var dbstudent = await std_Context.students.FindAsync(id);
            if (dbstudent == null)
                return BadRequest("Student not found");
            await std_Context.SaveChangesAsync();
            return Ok(await std_Context.students.ToListAsync());
        }
        //Post (Add) Student Data
        [HttpPost]
        public async Task<ActionResult<List<Student>>> Post(Student dbstudent)
        {
            std_Context.students.Add(dbstudent);
            await std_Context.SaveChangesAsync();
            return Ok(await std_Context.students.ToListAsync());
        }
        //Put (Update) Student Data
        [HttpPut]
        public async Task<ActionResult<List<Student>>> Put(Student request_student)
        {
            var dbstudent = await std_Context.students.FindAsync(request_student.Id);
            if (dbstudent == null)
                return BadRequest("Student not updated");
            dbstudent.Name = request_student.Name;
            dbstudent.Department = request_student.Department;
            dbstudent.RollNo = request_student.RollNo;
            await std_Context.SaveChangesAsync();
            return Ok(await std_Context.students.ToListAsync());
        }
        //Delete (Remove) Student Data
        [HttpDelete]
        public async Task<ActionResult<List<Student>>> Delete(int id)
        {
            var dbstudent = await std_Context.students.FindAsync(id);
            if (dbstudent == null)
                return BadRequest("Student not removed");
            std_Context.students.Remove(dbstudent);
            await std_Context.SaveChangesAsync();
            return Ok(await std_Context.students.ToListAsync());
        }
    }
}
