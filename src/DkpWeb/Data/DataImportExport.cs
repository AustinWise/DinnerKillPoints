using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DkpWeb.Data
{
    static class DataImportExport
    {
        public static void ExportData(ApplicationDbContext db, string path)
        {
            var options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                
            };
            string jsonString = JsonSerializer.Serialize(db, options);
            File.WriteAllText(path, jsonString);
        }
    }
}
