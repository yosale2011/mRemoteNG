using System.IO;
using System.Runtime.Versioning;
using mRemoteNG.Config.Connections;
using mRemoteNG.Properties;

namespace mRemoteNG.App.Initialization
{
    [SupportedOSPlatform("windows")]
    public class CredsAndConsSetup
    {
        public void LoadCredsAndCons()
        {
            new SaveConnectionsOnEdit(Runtime.ConnectionsService);

            if (Properties.App.Default.FirstStart && !Properties.OptionsBackupPage.Default.LoadConsFromCustomLocation && !File.Exists(Runtime.ConnectionsService.GetStartupConnectionFileName()))
                Runtime.ConnectionsService.NewConnectionsFile(Runtime.ConnectionsService.GetStartupConnectionFileName());

            // The SQLite user-profile store is still maturing upstream and prompts for a
            // separate "Profile password" on every start; keep it opt-in so the master
            // password remains the only startup gate unless explicitly enabled.
            if (Properties.Settings.Default.EnableUserProfiles)
                Runtime.TryInitializeUserProfile();
            Runtime.LoadConnections();
        }
    }
}