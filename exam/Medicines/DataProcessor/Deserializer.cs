namespace Medicines.DataProcessor
{
    using Castle.Core.Internal;
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";


        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var phramaciesDto = XMLDeserializer<PhramacyImportDto[]>(xmlString, "Pharmacies");

            List<Pharmacy> validPharmacies = new List<Pharmacy>();

            foreach (var phramacy in phramaciesDto)
            {
                if (!IsValid(phramacy))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var tempPharmacy = new Pharmacy()
                {
                    IsNonStop = bool.Parse(phramacy.IsNonStop),
                    Name = phramacy.Name,
                    PhoneNumber = phramacy.PhoneNumber,
                };

                foreach (var medicine in phramacy.Medicines)
                {
                    
                    if (medicine.ProductionDate.IsNullOrEmpty()|| medicine.ExpiryDate.IsNullOrEmpty())
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    DateTime productionDate = Convert.ToDateTime(medicine.ProductionDate, CultureInfo.InvariantCulture);
                    DateTime expiryDate = Convert.ToDateTime(medicine.ExpiryDate, CultureInfo.InvariantCulture);

                    if
                       (!IsValid(medicine) ||
                        productionDate >= expiryDate ||
                        tempPharmacy.Medicines
                        .Any(x => x.Name == medicine.Name && x.Producer ==      medicine.Producer))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    tempPharmacy.Medicines.Add(new Medicine()
                    {
                        Category = (Category)medicine.Category,
                        Name = medicine.Name,
                        Price = medicine.Price,
                        ProductionDate = productionDate,
                        ExpiryDate = expiryDate,
                        Producer = medicine.Producer,
                    });
                }

                sb.AppendLine(String.Format(SuccessfullyImportedPharmacy,tempPharmacy.Name, tempPharmacy.Medicines.Count));

                validPharmacies.Add(tempPharmacy);
            }

            context.AddRange(validPharmacies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

           var patientsDto= JsonConvert.DeserializeObject<PatientsImportDto[]>(jsonString);

            List<Patient> validPatients= new List<Patient>();

            foreach (var patient in patientsDto)
            {
                if (!IsValid(patient))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var tempPatient=new Patient() 
                {
                    FullName= patient.FullName,
                    AgeGroup=(AgeGroup)patient.AgeGroup,
                    Gender=(Gender)patient.Gender,
                };

                foreach (var medicine in patient.Medicines)
                {
                    if (tempPatient.PatientsMedicines.Any(x=>x.MedicineId==medicine))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    tempPatient.PatientsMedicines.Add(new PatientMedicine()
                    {
                        MedicineId = medicine,
                        Patient = tempPatient
                    });
                }
                sb.AppendLine(String.Format(SuccessfullyImportedPatient, tempPatient.FullName, tempPatient.PatientsMedicines.Count));

                validPatients.Add(tempPatient);
            }

            context.AddRange(validPatients);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        private static T XMLDeserializer<T>(string xmlString, string root)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(root));

            using TextReader reader = new StringReader(xmlString);

            return (T)serializer.Deserialize(reader);
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
