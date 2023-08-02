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

            //Appartment Mapper
            CreateMap<Appartment, GetAppartmentDto>();
            CreateMap<AddAppartmentDto, Appartment>();
            CreateMap<UpdateAppartmentDto, Appartment>();

            //Staff Mapper
            CreateMap<Staff, GetStaffDto>();
            CreateMap<AddStaffDto, Staff>();
            CreateMap<UpdateStaffDto, Staff>();

            //Order Mapper
            CreateMap<Order, OrderDto>();
            
            //Contact Mapper
            CreateMap<Contact, ContactDto>();
            CreateMap<ContactDto, Contact >();

            //Subject Mapper
            CreateMap<Subject, GetSubjectDto>();
            CreateMap<AddSubjectDto, Subject>();
            CreateMap<UpdateSubjectDto, Subject>();
        }
    }
}