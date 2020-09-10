//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AuthServer.DAL
//{
//    public class InitSessionsDAL : IInitSessionsDAL
//    {
//        public async Task DeleteSession(string code)
//        {
//            InitSessions.RemoveAll(i => i.Code == code);
//        }

//        public async Task<InitSession> GetSession(string code)
//        {
//            return InitSessions.FirstOrDefault(i => i.Code == code);
//        }

//        public async Task PutSession(InitSession session)
//        {
//            var oldSession = InitSessions.FirstOrDefault(i => i.Id == session.Id);

//            if (oldSession != null)
//            {
//                oldSession.Code = session.Code;
//                oldSession.ExpireIn = session.ExpireIn;
//                oldSession.AccessParameters = session.AccessParameters;
//            }
//            else
//            {
//                InitSessions.Add(session);
//            }
//        }

//        private List<InitSession> InitSessions = new List<InitSession>();
//    }
//}
