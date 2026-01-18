using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using System;
using WebAPIPatients.Context;
using WebAPIPatients.DTOs;
using WebAPIPatients.Models;
using WebAPIPatients.SwaggerExamples;

namespace WebAPIPatients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatientController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Patient
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPersons(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null,
            [FromQuery] string? documentNumber = null)
        {
            var query = _context.Patients.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p =>
                    p.FirstName.Contains(name) || p.LastName.Contains(name));

            if (!string.IsNullOrEmpty(documentNumber))
                query = query.Where(p => p.DocumentNumber.Contains(documentNumber));

            var totalItems = await query.CountAsync();
            var patients = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Items = patients
            };

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = patients.Any()
                    ? "Pacientes obtenidos correctamente."
                    : "No se encontraron pacientes con esos criterios.",
                Data = result
            });
        }

        // GET: api/Patient/5
        [HttpGet("{PatientId}")]
        public async Task<ActionResult<Patient>> GetPerson(int PatientId)
        {
            var patient = await _context.Patients.FindAsync(PatientId);

            if (patient == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = $"No se encontró paciente con ID {PatientId}.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<Patient>
            {
                Success = true,
                Message = "Paciente obtenido correctamente.",
                Data = patient
            });
        }

        [HttpGet("createdAfter")]
        public async Task<IActionResult> GetPatientsCreatedAfter([FromQuery] DateTime date)
        {
            var patients = await _context.Patients
                .FromSqlRaw("EXEC GetPatientsCreatedAfter @p0", date)
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<Patient>>
            {
                Success = true,
                Message = $"Pacientes creados después de {date}.",
                Data = patients
            });
        }

        // POST: api/Patient
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPerson(PatientRequestDto dto)
        {
            // Validar duplicado
            var exists = await _context.Patients.AnyAsync(p =>
                p.DocumentType == dto.DocumentType &&
                p.DocumentNumber == dto.DocumentNumber);

            if (exists)
                return Conflict("Ya existe un paciente con ese tipo y número de documento.");

            var patient = new Patient
            {
                DocumentType = dto.DocumentType,
                DocumentNumber = dto.DocumentNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPerson),
                new { PatientId = patient.PatientId },
                new ApiResponse<Patient>
                {
                    Success = true,
                    Message = "Paciente creado exitosamente.",
                    Data = patient
                }
            );
            //return CreatedAtAction(
            //    nameof(GetPerson),
            //    new { PatientId = patient.PatientId },
            //    patient
            // );
        }

        // PUT: api/Patient/5
        [HttpPut("{PatientId}")]
        public async Task<IActionResult> PutPerson(int PatientId, PatientRequestDto dto)
        {
            var patient = await _context.Patients.FindAsync(PatientId);

            if (patient == null)
                return NotFound();

            patient.DocumentType = dto.DocumentType;
            patient.DocumentNumber = dto.DocumentNumber;
            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.BirthDate = dto.BirthDate;
            patient.PhoneNumber = dto.PhoneNumber;
            patient.Email = dto.Email;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Paciente actualizado correctamente.",
                data = patient
            });
        }

        [HttpPatch("{PatientId}")]
        [SwaggerRequestExample(typeof(JsonPatchDocument<Patient>), typeof(PatchPatientExample))]
        public async Task<IActionResult> PatchPerson(int PatientId, [FromBody] JsonPatchDocument<Patient> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var patient = await _context.Patients.FindAsync(PatientId);
            if (patient == null)
                return NotFound();

            patchDoc.ApplyTo(patient, ModelState);

            if (!TryValidateModel(patient))
                return ValidationProblem(ModelState);

            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Paciente actualizado correctamente.",
                data = patient
            });
        }

        // DELETE: api/Patient/5
        [HttpDelete("{PatientId}")]
        public async Task<IActionResult> DeletePerson(int PatientId)
        {
            var patient = await _context.Patients.FindAsync(PatientId);
            if (patient == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = $"No se encontró paciente con ID {PatientId}.",
                    Data = null
                });
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = $"Paciente con ID {PatientId} eliminado exitosamente.",
                Data = null
            });
        }

        //private bool PatientExists(int PatientId)
        //{
        //    return _context.Patients.Any(e => e.PatientId == PatientId);
        //}
    }
}
