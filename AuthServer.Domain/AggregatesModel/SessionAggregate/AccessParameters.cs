using AuthServer.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AuthServer.Domain.AggregatesModel.SessionAggregate
{
    [Owned]
    public class ScopeAccess
    {
        public ScopeAccess() { }
        public ScopeAccess(string s) => FromString(s);

        public string ScopeName { get; set; }
        public bool HasAccess { get; set; }

        public override string ToString() => $"{ScopeName}:{HasAccess}";
        public void FromString(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException();
            }
            else if (!Regex.IsMatch(s, "^.*:(true|True|false|False)$"))
            {
                throw new FormatException();
            }

            int separatorIndex = s.IndexOf(':');

            ScopeName = s.Substring(0, separatorIndex);
            HasAccess = bool.Parse(s.Substring(separatorIndex + 1));
        }
    }

    [Owned]
    public class AccessParameters : IValueObject
    {
        public IList<ScopeAccess> Scopes { get; private set; } = new List<ScopeAccess>();

        public AccessParameters() { }
        public AccessParameters(IList<ScopeAccess> scopes)
        {
            Scopes = scopes;
        }

        public void AddScopeAccess(string scope, bool access = true)
        {
            if (Scopes.Any(i => i.ScopeName == scope))
            {
                Scopes.Where(i => i.ScopeName == scope).FirstOrDefault().HasAccess = access;
            }
            else
            {
                Scopes.Add(new ScopeAccess { ScopeName = scope, HasAccess = access });
            }
        }

        public bool HasAccess(string scope)
        {
            return Scopes.FirstOrDefault(i => i.ScopeName == scope)?.HasAccess ?? false;
        }
    }
}
