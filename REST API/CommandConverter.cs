using NLog;
using Saritasa.JiraDayIssues.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Saritasa.JiraDayIssues.Infrastructure
{
    /// <summary>
    /// Offers the processing commands from user.
    /// </summary>
    public class CommandConverter
    {
        /// <summary>
        /// The smallest legth of command argument.
        /// </summary>
        private const int SmallestArgumentLegth = 1;

        /// <summary>
        /// Provides logging.
        /// </summary>
        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Dictionary of available commands.
        /// </summary>
        public Dictionary<string, PropertyInfo> Commands { get; } = new Dictionary<string, PropertyInfo>();

        /// <summary>
        /// String which marks the command.
        /// </summary>
        private readonly string commandStartsWith = "--";

        /// <summary>
        /// The symbol which assign the meaning.
        /// </summary>
        private readonly string commandAssignor = "=";

        /// <summary>
        /// The smallest length of commands.
        /// </summary>
        private int smallestCommandLength;

        /// <summary>
        /// Defines minimum length of command.
        /// </summary>
        /// <returns></returns>
        private int GetMinCommandLength()
        {
            var allUserProps = typeof(UserRequestData).GetProperties();
            var length = int.MaxValue;
            foreach (var prop in allUserProps)
            {
                if (length > prop.Name.Length)
                {
                    length = prop.Name.Length;
                }
            }
            return length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandConverter"/> class.
        /// </summary>
        public CommandConverter()
        {
            var allUserProps = typeof(UserRequestData).GetProperties();
            foreach (var prop in allUserProps)
            {
                Commands.Add(prop.Name, prop);
            }

            smallestCommandLength = GetMinCommandLength();
            logger.Debug("Initialized CommandConverter");
        }

        /// <summary>
        /// Ensures converting string into command and its argument.
        /// </summary>
        /// <param name="propName">String to convert.</param>
        /// <returns>Command and its argument.</returns>
        /// <exception cref="ArgumentException">Throws if string is not valid.</exception>
        public (PropertyInfo Field, string Argument) PropArgumentParser(string propName)
        {
            if (CommandIsValid(propName))
            {
                propName = propName.Substring(commandStartsWith.Length);
                var propAndArgument = propName.Split(commandAssignor);
                propName = propAndArgument[0];
                if (Commands.Keys.Contains(propName))
                {
                    var argument = propAndArgument[1];
                    return (Commands[propName], argument);
                }
            }

            throw new ArgumentException("Wrong command!", propName);
        }

        /// <summary>
        /// Checks if specified command is valid.
        /// </summary>
        /// <param name="command">Command to check.</param>
        private bool CommandIsValid(string command)
        {
            return !string.IsNullOrEmpty(command)
                && command.Length >= commandStartsWith.Length + commandAssignor.Length + smallestCommandLength + SmallestArgumentLegth
                && command.Contains(commandAssignor)
                && command.StartsWith(commandStartsWith);
        }
    }
}
