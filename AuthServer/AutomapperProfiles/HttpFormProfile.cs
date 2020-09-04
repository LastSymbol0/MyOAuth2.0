using AuthServer.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.AutomapperProfiles
{
    public class HttpFormProfile : Profile
    {
        public HttpFormProfile()
        {
            CreateMap<IFormCollection, RequestTokenByCodeClientDTO>()
                .ForMember(
                    dest => dest.Code,
                    opt =>
                    {
                        opt.PreCondition(
                            source => source.Any(x => x.Key == "code"));
                        opt.MapFrom(
                            source => source.FirstOrDefault(x => x.Key == "code").Value);
                    })
                .ForMember(
                    dest => dest.RedirectUrl,
                    opt =>
                    {
                        opt.PreCondition(
                            source => source.Any(x => x.Key == "redirect_uri"));
                        opt.MapFrom(
                            source => source.FirstOrDefault(x => x.Key == "redirect_uri").Value);
                    });

            CreateMap<IFormCollection, RequestTokenRefreshClientDTO>()
                .ForMember(
                    dest => dest.RefreshToken,
                    opt =>
                    {
                        opt.PreCondition(
                            source => source.Any(x => x.Key == "refresh_token"));
                        opt.MapFrom(
                            source => source.FirstOrDefault(x => x.Key == "refresh_token").Value);
                    });
        }
    }
}
