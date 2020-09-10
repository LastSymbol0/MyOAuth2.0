using AuthServer.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Domain.AggregatesModel.SessionAggregate
{
    public interface ISessionRepository : IRepository<Session>
    {
        Task<bool> AddSession(Session session);

        Task<bool> OpenSession(int sessionId, string code);
        Task<bool> RefreshToken(int sessionId, string token);

        Session GetSessionById(int id);
        Session GetSessionByGrant(GrantType grantType, string grantValue);

        Task<bool> RemoveSession(int id);

        Task Update(Session session);
    }
}
