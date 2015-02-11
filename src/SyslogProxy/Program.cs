namespace SyslogProxy
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ServiceProcess;

    using SimpleServices;

    [RunInstaller(true)]
    public class Program : SimpleServiceApplication
    {
        static void Main(string[] args)
        {
            new Service(args,
                   new List<IWindowsService> { new ProxyService() }.ToArray,
                   installationSettings: (serviceInstaller, serviceProcessInstaller) =>
                   {
                       serviceInstaller.ServiceName = "SyslogProxy";
                       serviceInstaller.Description = "A simple Syslog proxy for Seq.";
                       serviceInstaller.StartType = ServiceStartMode.Automatic;
                       serviceProcessInstaller.Account = ServiceAccount.LocalService;
                       serviceProcessInstaller.Installers.Add(new EventSourceInstaller());
                   },
                   configureContext: x => { x.Log = Logger.Information; })
           .Host();
        }
    }
}
