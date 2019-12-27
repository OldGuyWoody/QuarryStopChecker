// Requires: QuarryLocks

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Quarry Stop Checker", "OldGuyWoody", "1.0.0")]
    [Description("Stop degenerates from turning off quarries they don't have access to")]
    public class QuarryStopChecker: RustPlugin
    {
        #region Fields
        private PluginConfig config;
        #endregion

        #region Hooks
        private void Init()
        {
            config = Config.ReadObject<PluginConfig>();
        }

        void OnQuarryToggled(MiningQuarry quarry)
        {
            if (quarry.IsEngineOn())
                return;

            var codelock = quarry.GetComponentInChildren<CodeLock>();
            if (codelock == null)
                return;

            var owner = covalence.Players.FindPlayerById(quarry.OwnerID.ToString())?.Name ?? "unknown";
            var position = quarry.ServerPosition;
            BaseEntity[] results = new BaseEntity[25];
            var by = BaseEntity.Query.Server.GetInSphere(position, 0.75f, results,
                entity =>
                {
                    BasePlayer player = entity as BasePlayer;
                    return player != null;
                });

            var near = "";
            var i = 0;
            var whitelisted = false;

            while (results[i] != null)
            {
                var player = (BasePlayer)results[i++];
                near += (player.displayName + " ");
                if (codelock.whitelistPlayers.Contains(player.userID))
                {
                    whitelisted = true;
                }

            }

            Puts("Quarry owned by " + owner + " at " + position + " turned off. Whitelisted player nearby? " + whitelisted + ". Nearby players: " + near);
            if (!whitelisted)
            {
                quarry.EngineSwitch(true);
                if (config.EnableChatOnCaught)
                {
                    foreach (var player in BasePlayer.activePlayerList)
                    {
                        Message(player, "NoStopWarn", near, owner);
                    }
                }
            }
        }
        #endregion
     
        #region Localization 1.1.1
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                {"NoStopWarn", "{0}tried to turn off quarry belonging to {1}"},
            }, this);
        }

        private void Message(BasePlayer player, string messageKey, params object[] args)
        {
            var message = GetMessage(messageKey, player.UserIDString, args);
            player.IPlayer.Message(message);
        }

        private string GetMessage(string messageKey, string playerID, params object[] args)
        {
            return string.Format(lang.GetMessage(messageKey, this, playerID), args);
        }
        #endregion

        #region Configuration
        private class PluginConfig
        {
            public bool EnableChatOnCaught;
        }

        protected override void LoadDefaultConfig()
        {
            Config.WriteObject(GetDefaultConfig(), true);
        }

        private PluginConfig GetDefaultConfig()
        {
            return new PluginConfig
            {
                EnableChatOnCaught = false
            };
        }
    }
    #endregion
}