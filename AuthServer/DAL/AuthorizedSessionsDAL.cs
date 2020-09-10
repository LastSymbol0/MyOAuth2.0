//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AuthServer.DAL
//{
//    public class AuthorizedSessionsDAL : IAuthorizedSessionsDAL
//    {
//        public async Task DeleteSession(string refreshToken)
//        {
//            Sessions.RemoveAll(i => i.RefreshToken == refreshToken);
//        }

//        public async Task<AuthorizedSession> GetSession(string refreshToken)
//        {
//            return Sessions.FirstOrDefault(i => i.RefreshToken == refreshToken);
//        }

//        public async Task PutSession(AuthorizedSession session)
//        {
//            var oldSession = Sessions.FirstOrDefault(i => i.Id == session.Id);

//            if (oldSession != null)
//            {
//                oldSession.RefreshToken = session.RefreshToken;
//                oldSession.AccessParameters = session.AccessParameters;
//            }
//            else
//            {
//                Sessions.Add(session);
//            }
//        }

//        private List<AuthorizedSession> Sessions = new List<AuthorizedSession>();
//    }
//}//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AuthServer.DAL
//{
//    public class AuthorizedSessionsDAL : IAuthorizedSessionsDAL
//    {
//        public async Task DeleteSession(string refreshToken)
//        {
//            Sessions.RemoveAll(i => i.RefreshToken == refreshToken);
//        }

//        public async Task<AuthorizedSession> GetSession(string refreshToken)
//        {
//            return Sessions.FirstOrDefault(i => i.RefreshToken == refreshToken);
//        }

//        public async Task PutSession(AuthorizedSession session)
//        {
//            var oldSession = Sessions.FirstOrDefault(i => i.Id == session.Id);

//            if (oldSession != null)
//            {
//                oldSession.RefreshToken = session.RefreshToken;
//                oldSession.AccessParameters = session.AccessParameters;
//            }
//            else
//            {
//                Sessions.Add(session);
//            }
//        }

//        private List<AuthorizedSession> Sessions = new List<AuthorizedSession>();
//    }
//}
