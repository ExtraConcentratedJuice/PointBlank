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
    [PointBlankCommand("HideAdmins", 0)]
    internal class CommandHideAdmins : PointBlankCommand
    {
        #region Properties
        public override string[] DefaultCommands => new string[]
        {
            "hideadmins"
        };

        public override string Help => "Hides all admins on the server";

        public override string Usage => Commands[0];

        public override string DefaultPermission => "unturned.commands.server.hideadmins";

        public override EAllowedServerState AllowedServerState => EAllowedServerState.LOADING;
        #endregion

        public override void Execute(UnturnedPlayer executor, string[] args)
        {
            Provider.hideAdmins = true;
            UnturnedChat.SendMessage(executor, "Admins are hidden!", ConsoleColor.Green);
        }
    }
}
