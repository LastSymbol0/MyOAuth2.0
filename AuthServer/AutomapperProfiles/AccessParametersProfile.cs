//using AuthServer.Models;
using AuthServer.Domain.AggregatesModel.SessionAggregate;
using AutoMapper;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using AuthServer.Models;

namespace AuthServer.AutomapperProfiles
{
    public class AccessParametersProfile : Profile
    {
        public AccessParametersProfile()
        {
            CreateMap<AccessParametersDTO, AccessParameters>()
                .ConvertUsing(i =>
                new AccessParameters(new List<ScopeAccess>
                {
                    new ScopeAccess{ ScopeName = "Scope1Access", HasAccess = i.Scope1Access },
                    new ScopeAccess{ ScopeName = "Scope2Access", HasAccess = i.Scope2Access },
                    new ScopeAccess{ ScopeName = "Scope3Access", HasAccess = i.Scope3Access },
                    new ScopeAccess{ ScopeName = "Scope4Access", HasAccess = i.Scope4Access }
                }));

            CreateMap<AccessParameters, IEnumerable<Claim>>()
                .ConvertUsing(x => x.Scopes.Select(i => new Claim(i.ScopeName, i.HasAccess.ToString())));
        }
    }
}
