namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.DataProcessor.ExportDtos;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Xml.Serialization;

    public class Serializer
    {
       

        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {
            var meds=context.Medicines.Where(x=>(int)x.Category==medicineCategory&&x.Pharmacy.IsNonStop)
                .ToArray()
                .Select(x=>new MedicineExport
                {
                    Name=x.Name,
                    Price=$"{x.Price:F2}",
                    Pharmacy=new PharmacyExport()
                    { 
                        Name=x.Pharmacy.Name,
                        PhoneNumber=x.Pharmacy.PhoneNumber
                    },
                }).OrderBy(x=>x.Price).ThenBy(x=>x.Name).ToArray();

            return JsonConvert.SerializeObject(meds,Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        });
        }

        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {
            var patients=context.Patients.Where(x=>x.PatientsMedicines
            .Any(y=>y.Medicine.ProductionDate > Convert.ToDateTime(date, CultureInfo.InvariantCulture)))
                .ToArray()
                .Select(x=>new PatientExportDto() 
                {
                    Gender=x.Gender.ToString().ToLower(),
                    FullName=x.FullName,
                    AgeGroup=x.AgeGroup.ToString(),
                    Medicines=x.PatientsMedicines
                    .Where(y =>  y.Medicine.ProductionDate > Convert.ToDateTime(date, CultureInfo.InvariantCulture))
                    
                    .Select(y=> new MedicineExportDto()
                    {
                        Category=y.Medicine.Category.ToString().ToLower(),
                        Name=y.Medicine.Name,
                        Price=$"{y.Medicine.Price:F2}",
                        Producer=y.Medicine.Producer,
                        BestBefore=y.Medicine.ExpiryDate
                        .ToString("yyyy-MM-dd",CultureInfo.InvariantCulture),
                    }).ToArray()
                    .OrderByDescending(z => Convert.ToDateTime(z.BestBefore, CultureInfo.InvariantCulture))
                    .ThenBy(z=>decimal.Parse(z.Price))
                    .ToArray(),
                })
                .OrderByDescending(x=>x.Medicines.Count())
                .ThenBy(x=>x.FullName)
                .ToArray();


            return XMLSerializer(patients, "Patients");
        }

        private static string XMLSerializer<T>(T obj, string root)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(root));

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using StringWriter writer = new StringWriter();

            serializer.Serialize(writer, obj, ns);

            return writer.ToString().TrimEnd();
        }
    }
}
