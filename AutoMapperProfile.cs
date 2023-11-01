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
            //Booking Mapper
            CreateMap<Booking, GetBookingDto>();
            CreateMap<AddBookingDto, Booking>();
            CreateMap<UpdateBookingDto, Booking>();

            //Product Mapper
            CreateMap<SpaProduct, GetSpaProductDto>();
            CreateMap<AddSpaProductDto, SpaProduct>();
            CreateMap<UpdateSpaProductDto, SpaProduct>();

            //Category Mapper
            CreateMap<Category, GetCategoryDto>();
            CreateMap<AddCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
            
            //Service Mapper
            CreateMap<Provision, GetProvisionDto>();
            CreateMap<AddProvisionDto, Provision>();
            CreateMap<UpdateProvisionDto, Provision>();

            //Department Mapper
            CreateMap<Department, GetDepartmentDto>();
            CreateMap<AddDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();

            //Staff Mapper
            CreateMap<Staff, GetStaffDto>();
            CreateMap<AddStaffDto, Staff>();
            CreateMap<UpdateStaffDto, Staff>();

            //Order Mapper
            CreateMap<Order, OrderDto>();
            
            //Contact Mapper
            CreateMap<Contact, GetContactDto>();
            CreateMap<AddContactDto, Contact>();
            CreateMap<UpdateContactDto, Contact>();

            //Subject Mapper
            CreateMap<Subject, GetSubjectDto>();
            CreateMap<AddSubjectDto, Subject>();
            CreateMap<UpdateSubjectDto, Subject>();

            
            

        }
    }
}