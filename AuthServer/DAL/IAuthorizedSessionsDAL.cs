//using AuthServer.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace AuthServer.DAL
//{
//    public class AuthorizedSession
//    {
//        public string Id { get; set; } // ClientId + UserId

//        public AccessParameters AccessParameters { get; set; }

//        public string RefreshToken { get; set; }
//    }

//    public interface IAuthorizedSessionsDAL
//    {
//        public Task<AuthorizedSession> GetSession(string refreshToken);
//        public Task PutSession(AuthorizedSession session);
//        public Task DeleteSession(string refreshToken);
//    }
//}
