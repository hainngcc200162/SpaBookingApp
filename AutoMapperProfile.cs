using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Product Mapper
            CreateMap<Product, GetProductDto>();
            CreateMap<AddProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            //Category Mapper
            CreateMap<Category, GetCategoryDto>();
            CreateMap<AddCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
            
            //Category Mapper
            CreateMap<Provision, GetProvisionDto>();
            CreateMap<AddProvisionDto, Provision>();
            CreateMap<UpdateProvisionDto, Provision>();
        }
    }
}