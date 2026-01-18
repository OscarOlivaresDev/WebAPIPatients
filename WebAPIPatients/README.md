# 📌 WebAPIPatients

API REST para administrar pacientes utilizando **ASP.NET Core**, **Entity Framework Core** y **SQL Server**.

Permite realizar operaciones CRUD, actualizaciones parciales con JSON Patch, consultar pacientes por fecha de creación, consultas paginadas, filtros y documentación con Swagger.

---

## 🚀 Requisitos

Antes de empezar, necesitas:

✔ [.NET 8.0 SDK o superior]  
✔ SQL Server (puede ser Express o LocalDB)  
✔ SQL Server Management Studio (SSMS)  
✔ Visual Studio 2022 / VS Code

---

## Arquitectura de la API
La API está organizada en una arquitectura en capas para separar responsabilidades.
De la siguiente forma:

Controllers/     --> Controladores que definen los **endpoints** y reciben las solicitudes HTTP
DTOs/            --> Objetos de transferencia de datos (entrada y salida)
Models/          --> Entidad de dominio (clase que representa la tabla en base de datos)
Context/         --> DbContext de Entity Framework Core (acceso a la base de datos)
Program.cs       --> Configuración de servicios y middleware (arranque)

## Decisiones técnicas:
Se implementó Entity Framework Core para abstraer el acceso a datos, manejar migraciones y facilitar pruebas en memoria con xUnit.
Se implementó JSON Patch para actualizaciones parciales de recursos.
Se configuró Swagger para probar y documentar los endpoints de forma interactiva.
Se creó un procedimiento almacenado en SQL Server para demostrar interacción directa con la base de datos.

## Stored Procedure: GetPatientsCreatedAfter

Procedimiento almacenado para obtener pacientes que fueron creados después de una fecha específica.

### SP

```sql
CREATE PROCEDURE GetPatientsCreatedAfter
    @CreatedAfter DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PatientId,
        DocumentType,
        DocumentNumber,
        FirstName,
        LastName,
        BirthDate,
        PhoneNumber,
        Email,
        CreatedAt
    FROM Patients
    WHERE CreatedAt > @CreatedAfter
    ORDER BY CreatedAt ASC;
END;

## 📁 Clonar el repositorio

```bash
git clone https://github.com/OscarOlivaresDev/WebAPIPatients.git
cd WebAPIPatients