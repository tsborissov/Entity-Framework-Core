using AutoMapper;
using CarDealer.Dtos.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<SupplierImportModel, Supplier>();
            this.CreateMap<PartImportModel, Part>();
            this.CreateMap<CustomerImportModel, Customer>();
            this.CreateMap<SaleImportModel, Sale>();
        }
    }
}
