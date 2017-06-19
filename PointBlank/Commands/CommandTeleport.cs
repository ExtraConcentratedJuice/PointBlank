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
    [PointBlankCommand("Teleport", 1)]
    internal class CommandTeleport : PointBlankCommand
    {
        #region Properties
        public override string[] DefaultCommands => new string[]
        {
            "tp",
            "Teleport"
        };

        public override string Help => "Teleports to a specific player or node";

        public override string Usage => Commands[0] + " <target/node> [player]";

        public override string DefaultPermission => "unturned.commands.admin.teleport";

        public override EAllowedServerState AllowedServerState => EAllowedServerState.RUNNING;
        #endregion

        public override void Execute(UnturnedPlayer executor, string[] args)
        {
            UnturnedPlayer ply;
            UnturnedPlayer pTarget;
            Node nTarget;

            if(args.Length < 2 || !UnturnedPlayer.TryGetPlayer(args[1], out ply))
            {
                if(executor == null)
                {
                    UnturnedChat.SendMessage(executor, "Invalid player to teleport!", ConsoleColor.Red);
                    return;
                }

                ply = executor;
            }

            if(UnturnedPlayer.TryGetPlayer(args[0], out pTarget))
            {
                ply.Teleport(pTarget.Player.transform.position);
                UnturnedChat.SendMessage(executor, ply.PlayerName + " teleported to " + pTarget.PlayerName, ConsoleColor.Green);
            }
            else
            {
                nTarget = LevelNodes.nodes.FirstOrDefault(a => a.type == ENodeType.LOCATION && NameTool.checkNames(args[0], ((LocationNode)a).name));

                if(nTarget == null)
                {
                    UnturnedChat.SendMessage(executor, "Invalid teleport location!", ConsoleColor.Red);
                    return;
                }

                ply.Teleport(nTarget.point);
                UnturnedChat.SendMessage(executor, ply.PlayerName + " teleported to " + ((LocationNode)nTarget).name);
            }
        }
    }
}