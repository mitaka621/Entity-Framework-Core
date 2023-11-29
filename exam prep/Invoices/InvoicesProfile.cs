﻿using AutoMapper;
using Invoices.Data.Models;
using Invoices.DataProcessor.ImportDto;

namespace Invoices
{
    public class InvoicesProfile : Profile
    {
        public InvoicesProfile()
        {
            CreateMap<AddressDTO, Address>();

            CreateMap<ImportClientDto, Client>();
        }
    }
}
