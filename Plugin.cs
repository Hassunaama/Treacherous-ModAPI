using BepInEx;
using BepInEx.Configuration;
using DiscordRPC;
using DiscordRPC.Logging;

using Treacherous_Modder.API;
using Treacherous_Modder.Misc;

namespace Treacherous_Modder
{
    [BepInPlugin("bepinex.hassunaama.mod.treacherousmodder", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private ConfigEntry<string> sessionKey;
        private ConfigEntry<bool> loggedIn;
        public DiscordRpcClient discordClient;

        private void Awake()
        {
            sessionKey = Config.Bind("Login",      // The section under which the option is shown
                                     "sessionKey",  // The key of the configuration option in the configuration file
                                     "", // The default value
                                     "The session key for logging in."); // Description of the option to show in the config file
            loggedIn = Config.Bind("Login",      // The section under which the option is shown
                                   "loggedIn",  // The key of the configuration option in the configuration file
                                   false, // The default value
                                   "The logged in config."); // Description of the option to show in the config file
            // Plugin startup logic
            Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is loaded!");
            /*
	            Create a Discord RPC Connection
	        */
            discordClient = new DiscordRpcClient("1032231288733700126");
            discordClient.Initialize();
            discordClient.SetPresence(new RichPresence
            {
                Details = "Currently on the menu.",
                Timestamps = Timestamps.Now,
                Buttons = new Button[]
            {
                new Button
                {
                    Label = "Play Treacherous Trials!",
                    Url = "https://gdcolon.itch.io/treacheroustrials"
                }
            },
                Assets = new Assets
                {
                    LargeImageKey = "icon",
                    LargeImageText = "The logo"
                }
            });
            /*
	            Create a Discord RPC Connection - End
	        */
        }

        void Update()
        {
            //Invoke all the events, such as OnPresenceUpdate
            discordClient.Invoke();
        }
    }
}
