﻿/*
 * Copyright 2010-2011 Bastian Eicher
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Common.Utils;
using NDesk.Options;
using ZeroInstall.Commands.Properties;
using ZeroInstall.Injector;
using ZeroInstall.Injector.Solver;

namespace ZeroInstall.Commands
{
    /// <summary>
    /// Handles the creation of <see cref="CommandBase"/> instances for handling of commands like "0install COMMAND [OPTIONS]".
    /// </summary>
    [CLSCompliant(false)]
    public static class CommandSwitch
    {
        /// <summary>
        /// Creates a set of all available commands.
        /// </summary>
        /// <param name="handler">A callback object used when the the user needs to be asked questions or is to be informed about progress.</param>
        /// <param name="policy">Combines configuration and resources used to solve dependencies and download implementations.</param>
        internal static IEnumerable<CommandBase> GetAvailableCommands(IHandler handler, Policy policy)
        {
            // ToDo: Replace this with something reflection-based
            var solver = SolverProvider.Default;
            return new CommandBase[] { new Selection(handler, policy, solver), new Download(handler, policy, solver), new Update(handler, policy, solver), new Run(handler, policy, solver), new Import(handler, policy), new List(handler, policy), new Config(handler, policy), new AddFeed(handler, policy), new RemoveFeed(handler, policy), new ListFeeds(handler, policy) };
        }

        /// <summary>
        /// Determines the command name specified in the command-line arguments.
        /// </summary>
        /// <param name="commandName">The <see cref="CommandBase.Name"/> to look for; may be <see langword="null"/>.</param>
        /// <param name="handler">A callback object used when the the user needs to be asked questions or is to be informed about progress.</param>
        /// <returns>The requested <see cref="CommandBase"/> or <see cref="DefaultCommand"/> if <paramref name="commandName"/> was <see langword="null"/>.</returns>
        /// <exception cref="OptionException">Thrown if <paramref name="commandName"/> is an unknown command.</exception>
        private static CommandBase GetCommand(string commandName, IHandler handler)
        {
            var policy = Policy.CreateDefault();

            if (commandName == null) return new DefaultCommand(handler, policy);

            CommandBase command = null;
            foreach (var possibleCommand in GetAvailableCommands(handler, policy))
            {
                if (StringUtils.Compare(possibleCommand.Name, commandName))
                {
                    command = possibleCommand;
                    break;
                }
            }

            if (command == null) throw new OptionException(Resources.UnknownCommand, "CommandBase");
            return command;
        }

        /// <summary>
        /// Parses command-line arguments, automatically selecting the correct <see cref="CommandBase"/>.
        /// </summary>
        /// <param name="args">The command-line arguments to be parsed.</param>
        /// <param name="handler">A callback object used when the the user needs to be asked questions or is to be informed about progress.</param>
        /// <returns>The newly created <see cref="CommandBase"/> after <see cref="CommandBase.Parse"/> has been called.</returns>
        /// <exception cref="OptionException">Thrown if <paramref name="args"/> contains unknown options or specified an unknown command.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the underlying filesystem of the user profile can not store file-changed times accurate to the second.</exception>
        /// <exception cref="IOException">Thrown if a problem occurred while creating a directory.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if creating a directory is not permitted.</exception>
        /// <exception cref="InvalidInterfaceIDException">Thrown when trying to set an invalid interface ID.</exception>
        public static CommandBase CreateAndParse(IEnumerable<string> args, IHandler handler)
        {
            #region Sanity checks
            if (args == null) throw new ArgumentNullException("args");            
            if (handler == null) throw new ArgumentNullException("handler");
            #endregion

            var arguments = new LinkedList<String>(args);
            CommandBase command = GetCommand(GetCommandName(arguments), handler);

            command.Parse(arguments);
            return command;
        }

        /// <summary>
        /// Determines the command name specified in the command-line arguments.
        /// </summary>
        /// <param name="arguments">The command-line arguments to search for a command name. If a command is found it is removed from the collection.</param>
        /// <returns>The name of the command that was found or <see langword="null"/> if none was specified.</returns>
        private static string GetCommandName(ICollection<string> arguments)
        {
            #region Sanity checks
            if (arguments == null) throw new ArgumentNullException("arguments");            
            #endregion

            string commandName = null;
            foreach (var argument in arguments)
            {
                if (!argument.StartsWith("-") && !argument.StartsWith("/"))
                {
                    commandName = argument;
                    arguments.Remove(argument);
                    break;
                }
            }

            return commandName;
        }
    }
}
