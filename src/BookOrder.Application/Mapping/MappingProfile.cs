using AutoMapper;
using BookOrder.Application.DTO;
using BookOrder.Domain;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookOrder.Application.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<BookApiResponse, Order>().ConvertUsing<BookApiResponseToOrderConverter>();
            CreateMap<BookApiResponse, CancelRequest>().ConvertUsing<BookApiResponseToCancelRequestConverter>();
        }
    }
}
