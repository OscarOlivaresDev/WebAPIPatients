using Microsoft.AspNetCore.JsonPatch;
using WebAPIPatients.Models;

namespace WebAPIPatients.SwaggerExamples
{
    public class PatchPatientExample : Swashbuckle.AspNetCore.Filters.IExamplesProvider<JsonPatchDocument<Patient>>
    {
        public JsonPatchDocument<Patient> GetExamples()
        {
            var doc = new JsonPatchDocument<Patient>();
            doc.Replace(p => p.FirstName, "Leonardo");
            return doc;
        }
    }

}
