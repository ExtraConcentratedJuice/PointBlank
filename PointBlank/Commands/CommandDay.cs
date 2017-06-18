﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PointBlank.API.Commands;
using PointBlank.API.Unturned.Player;
using PointBlank.API.Unturned.Chat;
using SDG.Unturned;

namespace PointBlank.Commands
{
    [PointBlankCommand("Day", 0)]
    internal class CommandDay : PointBlankCommand
    {
        #region Properties
        public override string[] DefaultCommands => new string[]
        {
            "day"
        };

        public override string Help => "Sets the time to day";

        public override string Usage => Commands[0];

        public override string DefaultPermission => "unturned.commands.admin.day";

        public override EAllowedServerState AllowedServerState => EAllowedServerState.RUNNING;
        #endregion

        public override void Execute(UnturnedPlayer executor, string[] args)
        {
            if (Provider.isServer && Level.info.type == ELevelType.ARENA)
            {
                UnturnedChat.SendMessage(executor, "Can't set time on arena!", ConsoleColor.Red);
                return;
            }
            if (Provider.isServer && Level.info.type == ELevelType.HORDE)
            {
                UnturnedChat.SendMessage(executor, "Can't set time on horde!", ConsoleColor.Red);
                return;
            }

            LightingManager.time = (uint)(LightingManager.cycle * LevelLighting.transition);
            UnturnedChat.SendMessage(executor, "Time set to day!", ConsoleColor.Green);
        }
    }
}
