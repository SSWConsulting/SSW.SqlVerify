using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SSW.HealthCheck;
using SSW.HealthCheck.Tests;


[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SSW.SqlVerify.HealthCheck.App_Start.HealthCheckConfig), "PreStart")]
namespace SSW.SqlVerify.HealthCheck.App_Start 
{
    public static class HealthCheckConfig 
    {
        public static void PreStart() 
        {
            // Add your start logic here
            RegisterTests();
        }
        public static void RegisterTests()
        {
            RegisterTests(HealthCheckService.Default);
        }
        public static void RegisterTests(HealthCheckService svc)
        {
            svc.Add(new NotDebugTest());
            svc.Add(new DbConnectionTest());
            svc.Setup<SSW.SqlVerify.HealthCheck.Hubs.HealthCheckHub>();
        }
    }
}