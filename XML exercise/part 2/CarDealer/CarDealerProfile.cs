using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<ImportSupplyer, Supplier>();

            CreateMap<PartsDTO, Part>();

            CreateMap<CarDto, Car>();

            CreateMap<CustomerDTO, Customer>();

            CreateMap<SalesDTO,Sale>();

            CreateMap<Car, CarExportDTO>();

            CreateMap<Car, BWMExport>();

            CreateMap<Supplier, LocalSuppliersExport>()
                .ForMember(x => x.PartsCount, y => y.MapFrom(z => z.Parts.Count));
               
        }
    }
}
