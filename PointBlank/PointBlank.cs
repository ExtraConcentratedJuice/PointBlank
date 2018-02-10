using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Net.Security;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Text;
using PointBlank.API;
using UnityEngine;
using PointBlank.Framework.Objects;
using PointBlank.Framework;
using PointBlank.API.DataManagment;
using PointBlank.API.Interfaces;
using PointBlank.Services.PluginManager;
using Newtonsoft.Json.Linq;
using PointBlank.API.Collections;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace PointBlank
{
    public class PointBlank : MonoBehaviour, IModuleNexus
    {
        #region Properties
        public static PointBlank Instance { get; private set; } // Self instance
        public static bool Enabled { get; private set; } // Is PointBlank running
        #endregion

        #region Initialization/Shutdown

        public void initialize() => Initialize();

        public void shutdown() => Shutdown();

        #endregion

        #region Loader Functions

        public void Initialize()
        {
            if (Instance != null && Enabled) // Don't run if already running
                return;

            Environment.ServerDirectory = $"{Directory.GetCurrentDirectory()}/Servers/{Dedicator.serverID}";

            if (!Directory.Exists(Environment.ServerDirectory))
                Directory.CreateDirectory(Environment.ServerDirectory);

            Console.WriteLine();
            PointBlankLogging.LogImportant("Loading " + PointBlankInfo.Name + " v" + PointBlankInfo.Version + "...");

            PointBlankLogging.Log();
            // Run required methods
            ApplyPatches();

            // Setup the runtime objects
            Environment.runtimeObjects.Add("Framework", new RuntimeObject(new GameObject("Framework")));
            Environment.runtimeObjects.Add("Extensions", new RuntimeObject(new GameObject("Extensions")));
            Environment.runtimeObjects.Add("Services", new RuntimeObject(new GameObject("Services")));
            Environment.runtimeObjects.Add("Plugins", new RuntimeObject(new GameObject("Plugins")));

            // Add the code objects
            Environment.runtimeObjects["Framework"].AddCodeObject<InterfaceManager>(); // Both the service manager and interface manager are important without them
            Environment.runtimeObjects["Framework"].AddCodeObject<ServiceManager>(); // the modloader won't be able to function properly making it as usefull as Rocket

            // Run the inits
            Environment.runtimeObjects["Framework"].GetCodeObject<InterfaceManager>().Init();
            Environment.runtimeObjects["Framework"].GetCodeObject<ServiceManager>().Init();

            // Run required methods
            RunRequirements();

            // Initialize
            Instance = this;
            Enabled = true;
#if !DEBUG
            Console.WriteLine("In debug mode!");
#endif
            PointBlankLogging.Log();
            PointBlankLogging.LogImportant("Loaded " + PointBlankInfo.Name + " v" + PointBlankInfo.Version + "!");
        }

        public void Shutdown()
        {
            PointBlankLogging.LogImportant("Shutting down " + PointBlankInfo.Name + " v" + PointBlankInfo.Version + "...");

            // Uninit
            Enabled = false;
            Instance = null;

            // Run the shutdowns
            Environment.runtimeObjects["Framework"].GetCodeObject<ServiceManager>().Shutdown();
            Environment.runtimeObjects["Framework"].GetCodeObject<InterfaceManager>().Shutdown();

            // Remove the runtime objects
            Environment.runtimeObjects["Framework"].RemoveCodeObject<ServiceManager>();
            Environment.runtimeObjects["Framework"].RemoveCodeObject<InterfaceManager>();

            // Remove the runtime objects
            Environment.runtimeObjects.Remove("Plugins");
            Environment.runtimeObjects.Remove("Services");
            Environment.runtimeObjects.Remove("Extensions");
            Environment.runtimeObjects.Remove("Framework");

            // Run the required functions
            RunRequirementsShutdown();

            PointBlankLogging.LogImportant("Shut down " + PointBlankInfo.Name + " v" + PointBlankInfo.Version + "!");
        }
        #endregion

        #region Functions
        private void ApplyPatches() => new I18N.West.CP1250();

        private void RunRequirements()
        {
            // Need to add something here
        }

        private void RunRequirementsShutdown()
        {
            Environment.Running = false;
            foreach(SQLData sql in Environment.SQLConnections.Where(a => a.Connected))
                sql.Disconnect();
        }
        #endregion

        #region Event Functions
        internal static bool ValidateCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors) => (errors == SslPolicyErrors.None);
        #endregion
    }
}
