using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Management;

namespace GeneralService
{
    [RunInstaller(true)]
    public class GeneralServiceInstaller:Installer
    {
        private ServiceInstaller serviceInstaller;

        public GeneralServiceInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();
            serviceInstaller.Committed += new System.Configuration.Install.InstallEventHandler(GeneralServiceInstaller_Committed);

            //set the privileges
            processInstaller.Account = ServiceAccount.User;
            processInstaller.Username = Strings.USER_NAME;
            processInstaller.Password = Strings.USER_PASSWORD;

            serviceInstaller.DisplayName = Strings.SERVICE_NAME;
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = Strings.SERVICE_NAME;
            serviceInstaller.Description = Strings.SERVICE_DESCRIPTION;

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }

        private void GeneralServiceInstaller_Committed(object sender, InstallEventArgs e)
        {
            ConnectionOptions coOptions = new ConnectionOptions();
            coOptions.Impersonation = ImpersonationLevel.Impersonate;
            ManagementScope mgmtScope = new ManagementScope(@"root\CIMV2", coOptions);
            mgmtScope.Connect();
            ManagementObject wmiService;
            wmiService = new ManagementObject("Win32_Service.Name='" + serviceInstaller.ServiceName + "'");
            ManagementBaseObject InParam = wmiService.GetMethodParameters("Change");
            InParam["DesktopInteract"] = Strings.DESKTOP_INTERACT;
            ManagementBaseObject OutParam = wmiService.InvokeMethod("Change", InParam, null);
        }

    }
}
