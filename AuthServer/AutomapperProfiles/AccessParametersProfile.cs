using AuthServer.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Security.Claims;

namespace AuthServer.AutomapperProfiles
{
    public class AccessParametersProfile : Profile
    {
        public AccessParametersProfile()
        {
            CreateMap<AccessParameters, IEnumerable<Claim>>()
                .ConvertUsing(x =>
                    new List<Claim>
                    {
                        new Claim("Scope1Access", x.Scope1Access.ToString()),
                        new Claim("Scope2Access", x.Scope2Access.ToString()),
                        new Claim("Scope3Access", x.Scope3Access.ToString()),
                        new Claim("Scope4Access", x.Scope4Access.ToString())
                    });
        }
    }
}
