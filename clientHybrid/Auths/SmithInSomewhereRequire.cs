using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace clientHybrid.Auths
{
    public class SmithInSomewhereRequire:IAuthorizationRequirement
    {
        public SmithInSomewhereRequire()
        {

        }
    }
}
