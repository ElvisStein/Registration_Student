using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using registration_api.Models;
using registration_api.Utils;

namespace registration_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly DataContext _context;

        private readonly string NotFoundMessage = "Estudante não encontrado.";

        public StudentController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentPublic>>> Get()
        {
            try
            {
                var data = await _context.Students.ToListAsync();

                List<StudentPublic> result = new List<StudentPublic>();
                foreach (var student in data)
                {
                    result.Add(new StudentPublic(student));
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{hash_id}")]
        public async Task<ActionResult<StudentPublic>> Get(string hash_id)
        {
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(x => x.HashId == hash_id);
                if (student == null)
                    return BadRequest(NotFoundMessage);

                return Ok(new StudentPublic(student));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet(), Route("FindByName")]
        public async Task<ActionResult<StudentPublic>> FindByName([FromHeader] String nome)
        {
            try
            {
                var data = await _context.Students.Where(x => x.Nome.ToUpper().Contains(nome.ToUpper())).ToListAsync();
                if (data == null)
                    return BadRequest(NotFoundMessage);

                List<StudentPublic> result = new List<StudentPublic>();
                foreach (var student in data)
                {
                    result.Add(new StudentPublic(student));
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]StudentPublic student)
        {
            try
            {

                int last_ra = await _context.Students.MaxAsync(x => x.RA) + 1;
                string hash_id = HashUtil.GetHash(string.Format("{0}{1}{2}", student.Nome, student.CPF, DateTime.Now));

                Student dbStudent = new Student()
                {
                    Nome = student.Nome,
                    CPF = student.CPF,
                    Email = student.Email,
                    HashId = hash_id,
                    RA = last_ra,
                };

                _context.Students.Add(dbStudent);
                if (!(await _context.SaveChangesAsync() > 0))
                    return BadRequest("Estudante não foi cadastrado.");
                else
                    return Ok("Estudante cadastrado com sucesso.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody]StudentPublic student)
        {
            try
            {
                var dbStudent = await _context.Students.FirstOrDefaultAsync(x => x.HashId == student.HashId);
                if (dbStudent == null)
                    return BadRequest(NotFoundMessage);

                dbStudent.Nome = student.Nome;
                student.CPF = student.CPF;
                dbStudent.Email = student.Email;

                _context.Students.Attach(dbStudent);
                if (!(await _context.SaveChangesAsync() > 0))
                    return BadRequest("Erro, estudante não foi atualizado.");

                return Ok("Estudante atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{hash_id}")]
        public async Task<ActionResult> Delete(string hash_id)
        {
            try
            {
                var dbStudent = await _context.Students.FirstOrDefaultAsync(x => x.HashId == hash_id);
                if (dbStudent == null)
                    return BadRequest(NotFoundMessage);

                _context.Students.Remove(dbStudent);
                if (!(await _context.SaveChangesAsync() > 0))
                    return NotFound("Erro, não foi possível excluir o estudante.");

                return Ok("Estudante excluído com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}