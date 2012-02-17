using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolSource.Server.Management.Client
{
    public interface IManagementPreauthentication
    {
        User Validate(Caller caller);
        string DigestGenerateRequest(string realm);
        Caller DigestValidateResponse(string company, string method, string response);
        Caller GetUserByKey(string company, string type, string value);
    }
}
