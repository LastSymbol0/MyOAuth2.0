//using AuthServer.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AuthServer.DAL
//{
//    public class InitSession
//    {
//        public string Id { get; set; } // ClientId + UserId

//        public DateTime ExpireIn { get; set; }

//        public AccessParameters AccessParameters { get; set; }

//        public string Code { get; set; }
//    }

//    public interface IInitSessionsDAL
//    {
//        public Task<InitSession> GetSession(string code);
//        public Task PutSession(InitSession session);
//        public Task DeleteSession(string code);
//    }
//}
