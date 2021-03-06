﻿using System;
using System.Collections.Generic;
using System.Linq;
using Jabbot.Models;

namespace Jabbot.CommandSprockets
{
    public abstract class CommandSprocket : ICommandSprocket
    {
        public abstract IEnumerable<string> SupportedInitiators { get; }
        public abstract IEnumerable<string> SupportedCommands { get; }
        public abstract bool ExecuteCommand();
        public string Intitiator { get; protected set; }
        public string Command { get; protected set; }
        public string[] Arguments { get; protected set; }
        public ChatMessage Message { get; protected set; }
        public IBot Bot { get; protected set; }
        public bool HasArguments { get { return Arguments.Length > 0; } }

        public virtual bool MayHandle(string initiator, string command)
        {
            return SupportedInitiators.Any(i => i.Equals(initiator, StringComparison.OrdinalIgnoreCase)) &&
                SupportedCommands.Any(c => c.Equals(command, StringComparison.OrdinalIgnoreCase));
        }

        public virtual bool Handle(ChatMessage message, IBot bot)
        {
            try
            {
                string[] args = message.Content
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Intitiator = args.Length > 0 ? args[0] : string.Empty;
                Command = args.Length > 1 ? args[1] : string.Empty;
                Message = message;
                Bot = bot;

                if (MayHandle(Intitiator, Command))
                {
                    Arguments = args.Skip(2).ToArray();
                    return ExecuteCommand();
                }
            }
            catch (InvalidOperationException e)
            {
                Bot.PrivateReply(Message.Sender, e.GetBaseException().Message);
            }
            return false;
        }

        public bool Enabled { get; set; }
    }
}
