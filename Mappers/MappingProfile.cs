using AutoMapper;
using HackerNewsApi.DTOs;
using HackerNewsApi.Models;
using System;

namespace HackerNewsApi.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Story, StoryDto>()
            .ForMember(dest => dest.Uri, opt => opt.MapFrom(src => src.Url))
            .ForMember(dest => dest.PostedBy, opt => opt.MapFrom(src => src.By))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time)))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => (src.Kids != null ? src.Kids.Count : 0)));
    }
}