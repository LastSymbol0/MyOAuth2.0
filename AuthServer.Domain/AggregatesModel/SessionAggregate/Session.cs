using AuthServer.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthServer.Domain.AggregatesModel.SessionAggregate
{
    public class Session : Entity, IAggregateRoot
    {
        public SessionStatus Status { get; private set; }
        public virtual int ClientId { get; private set; }
        public int ResourceOwnerId { get; private set; }
        public AccessParameters AccessParameters { get; private set; }

        public GrantType ClientGrantType { get; private set; }
        public string ClientGrantValue { get; private set; }

        public DateTime ExpireIn
        {
            get => ClientGrantType == GrantType.refresh_token ? DateTime.MaxValue : _ExpireIn;
            set => _ExpireIn = value;
        }
        private DateTime _ExpireIn;

        public Session()
        {

        }

        public Session(int clientId,
                       int resourceOwnerId,
                       AccessParameters accessParameters)
        {
            const double AUTH_CODE_EXPIRATION_TIME = 30;

            ClientId = clientId;
            ResourceOwnerId = resourceOwnerId;
            AccessParameters = accessParameters ?? throw new ArgumentNullException();

            Id = int.Parse($"{ClientId}{ResourceOwnerId}");

            Status = SessionStatus.WaitingForClientAuthorization;
            ClientGrantType = GrantType.code;
            ClientGrantValue = GenerateAuthCode();
            ExpireIn = DateTime.UtcNow.AddSeconds(AUTH_CODE_EXPIRATION_TIME);
        }

        public virtual bool IsValid() => DateTime.Compare(ExpireIn, DateTime.UtcNow) > 0 && Status != SessionStatus.None && Status != SessionStatus.Closed;
        public virtual bool IsOpen() => Status == SessionStatus.Open;

        public virtual bool OpenSession(string code)
        {
            if (Status != SessionStatus.WaitingForClientAuthorization
                || DateTime.Compare(ExpireIn, DateTime.UtcNow) < 0
                || !ValidateAuthCode(code))
            {
                return false;
            }

            Status = SessionStatus.Open;
            ClientGrantType = GrantType.refresh_token;
            ClientGrantValue = GenerateRefreshToken();

            return true;
        }

        public bool CloseSession()
        {
            if (Status != SessionStatus.WaitingForClientAuthorization
                && Status != SessionStatus.Open)
            {
                return false;
            }

            ClientGrantType = GrantType.None;
            Status = SessionStatus.Closed;
            return true;
        }

        public virtual bool RefreshToken(string token)
        {
            if (!ValidateRefreshToken(token))
            {
                return false;
            }

            ClientGrantValue = GenerateRefreshToken();

            return true;
        }

        public bool ValidateGrant(GrantType type, string value)
        {
            switch (type)
            {
                case GrantType.code:
                    return ValidateAuthCode(value);
                case GrantType.refresh_token:
                    return ValidateRefreshToken(value);
            }
            return false;
        }

        private bool ValidateRefreshToken(string token)
        {
            if (ClientGrantType != GrantType.refresh_token
                || String.IsNullOrEmpty(token)
                || String.IsNullOrEmpty(ClientGrantValue))
            {
                return false;
            }

            return token == ClientGrantValue;
        }

        private bool ValidateAuthCode(string code)
        {
            if (ClientGrantType != GrantType.code
                || String.IsNullOrEmpty(code)
                || String.IsNullOrEmpty(ClientGrantValue))
            {
                return false;
            }

            return code == ClientGrantValue;
        }

        private string GenerateRefreshToken() => Utils.GenerateRandomString(32).Replace(' ', '0');
        private string GenerateAuthCode() => Utils.GenerateRandomInt().ToString();
    }
}
