using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Domain.AggregatesModel.SessionAggregate;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Infrastructure.Repository
{
    public class SessionRepository : ISessionRepository
    {
        public DbContext Context
        {
            get
            {
                return SessionsContext;
            }
        }

        private SessionsContext SessionsContext;

        public SessionRepository(SessionsContext sessionsContext)
        {
            SessionsContext = sessionsContext ?? throw new ArgumentNullException();
        }

        public async Task<bool> AddSession(Session session)
        {
            SessionsContext.Sessions.Add(session);

            await SessionsContext.SaveChangesAsync();

            return true;
        }

        public Session GetSessionById(int id)
        {
            return SessionsContext.Find<Session>(id);
        }

        public Session GetSessionByGrant(GrantType grantType, string grantValue)
        {
            return SessionsContext.Sessions
                .Where(i => i.ClientGrantType == grantType && i.ClientGrantValue == grantValue)
                .FirstOrDefault();
        }

        public async Task<bool> RemoveSession(int id)
        {
            SessionsContext.Remove(GetSessionById(id));

            await SessionsContext.SaveChangesAsync();

            return true;
        }

        public async Task Update(Session session)
        {
            SessionsContext.Entry(session).State = EntityState.Modified;

            await SessionsContext.SaveChangesAsync();
        }

        public async Task<bool> OpenSession(int sessionId, string code)
        {
            bool res = GetSessionById(sessionId)?.OpenSession(code) ?? false;

            await SessionsContext.SaveChangesAsync();

            return res;
        }

        public async Task<bool> RefreshToken(int sessionId, string token)
        {
            bool res = GetSessionById(sessionId)?.RefreshToken(token) ?? false;

            await SessionsContext.SaveChangesAsync();

            return res;
        }
    }
}
