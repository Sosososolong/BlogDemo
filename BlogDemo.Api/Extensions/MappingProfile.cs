﻿using AutoMapper;
using BlogDemo.Core.Entities;
using BlogDemo.Infrastructure.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogDemo.Api.Extensions
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostResource>()
                .ForMember(dest=>dest.UpdateTime, opt => opt.MapFrom(src=>src.LastModified));
            CreateMap<PostResource, Post>();

            CreateMap<PostAddResource, Post>();

            CreateMap<PostUpdateResource, Post>();
            CreateMap<Post, PostUpdateResource>();
        }
    }
}
