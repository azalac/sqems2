using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EMS_Security
{
    public static class Authorization
    {
        public static class Roles
        {
            public const string Admin = "ADMIN";
            public const string Nurse = "NURSE";
            public const string Doctor = "DOCTOR";
        }

        public static void SetPrincipal(string name, string role)
        {
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(name), new string[] { role });
        }

        public static void EnforceRole(string role) // <-- enfore roles for certain actions
        {
            if (Thread.CurrentPrincipal == null || !Thread.CurrentPrincipal.IsInRole(role))
            {
                throw new SecurityException("Access denied, " + role + " has insufficient permissions.");
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = Roles.Admin)]    // <-- put on methods to set permissions for entire method
        public static void TestPrincipalPermissions()
        {
            // Thread.CurrentPrincipal.Identity.Name; // <-- gets users name for permissions
        }
        
    }
}
