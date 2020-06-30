using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CommandLinePlay
{
    internal class CommandLineProcessor
    {
        internal readonly Dictionary<string, ArgumentDescription> NamedArgList = new Dictionary<string, ArgumentDescription>(StringComparer.OrdinalIgnoreCase);
        internal readonly List<ArgumentDescription> UnNamedArgList = new List<ArgumentDescription>();
        public bool IsUsage = false;
        private int requiredUnNamedCount = 0;
        private CommandLineProcessor() { }
        static public CommandLineProcessor Create(params ArgumentDescription[] list)
        {
            CommandLineProcessor cmdLine = new CommandLineProcessor();
            foreach (var v in list)
            {
                if (v.ParameterType == ArgumentDescription.ParameterTypeEnum.UnNamed)
                {
                    if (v.Required)
                        cmdLine.requiredUnNamedCount++;
                    cmdLine.UnNamedArgList.Add(v);
                }
                else
                {
                    // Do not specify ? or --help here. These are hard coded as part of the user input.
                    // Only alphabet and numbers are allowed. Case insensitive.
                    Regex regex = new Regex("^[a-zA-Z0-9]+$");
                    if (!regex.IsMatch(v.Name))
                        throw new BadCodeException($"Invalid CommandLine parameter name: {v.Name}");
                    if (cmdLine.NamedArgList.ContainsKey(v.Name))
                        throw new BadCodeException($"The CommandLine parameter {v.Name} has already been added");
                    cmdLine.NamedArgList.Add(v.Name, v);
                }
            }
            if (cmdLine.UnNamedArgList.Count > 0)
            {
                bool hasNotRequired = false;
                foreach (var obj in cmdLine.UnNamedArgList)
                {
                    if (!obj.Required)
                    {
                        hasNotRequired = true;
                        continue;
                    }
                    if (hasNotRequired && obj.Required)
                        throw new BadCodeException("Declare all required UnNamed parameters before declaring parameters that are not required.");
                }
            }
            return cmdLine;
        }
        public void ProcessCommandLine(string[] args)
        {
            int unNamedIndex = 0;
            for (int i = 0; i < args.Length; i++)
            {
                string item = args[i];
                if (item == "-?" || item == "/?" || item == "--help")
                {
                    IsUsage = true;
                    return;
                }
                if (item[0] == '-' || item[0] == '/')
                {
                    if (item.Length == 1)
                        throw new UserInputException($"Cannot specify '{item[0]}' by itself. Please use a switch name");
                    string sw = item.Substring(1);
                    if (NamedArgList.ContainsKey(sw))
                    {
                        if (NamedArgList[sw].BoolValue != null || NamedArgList[sw].Value != null)
                        {
                            throw new UserInputException($"The {item} switch has already been declared");
                        }
                        if (NamedArgList[sw].ParameterType == ArgumentDescription.ParameterTypeEnum.NamedTrueFalse)
                        {
                            NamedArgList[sw].BoolValue = true;
                            continue;
                        }
                        if (NamedArgList[sw].ParameterType == ArgumentDescription.ParameterTypeEnum.NamedVariable &&
                                i == args.Length - 1)
                        {
                            throw new UserInputException($"This switch requires a value to be specified. e.g. {item} 'somevalue'");
                        }
                        i++;
                        NamedArgList[sw].Value = args[i];
                    }
                    else
                    {
                        throw new UserInputException($"The switch {item} is not a valid switch");
                    }
                }
                else
                {
                    if (unNamedIndex == UnNamedArgList.Count)
                    {
                        throw new UserInputException($"The item \"{item}\" exceeds the number of allowed values");
                    }
                    UnNamedArgList[unNamedIndex].Value = item;
                    unNamedIndex++;
                }
            }
            if (requiredUnNamedCount > unNamedIndex)
            {
                throw new UserInputException($"There must be {requiredUnNamedCount} parameters without switches");
            }
            foreach (var v in NamedArgList)
            {
                if (v.Value.Required && v.Value.ParameterType == ArgumentDescription.ParameterTypeEnum.NamedTrueFalse &&
                    v.Value.BoolValue == null)
                {
                    throw new UserInputException($"The required boolean switch -{v.Key} is not defined");
                }
                if (v.Value.Required && v.Value.ParameterType == ArgumentDescription.ParameterTypeEnum.NamedVariable &&
                    v.Value.Value == null)
                {
                    throw new UserInputException($"The required value switch -{v.Key} is not defined");
                }
            }
        }
    }

    internal class ArgumentDescription
    {
        public enum ParameterTypeEnum { NamedVariable, NamedTrueFalse, UnNamed };
        public ParameterTypeEnum ParameterType { get; private set; }
        public string Name { get; private set; }
        public bool Required { get; private set; }
        public string Value { get; set; }
        public Boolean? BoolValue { get; set; }
        public ArgumentDescription(ParameterTypeEnum ptype, Boolean required, string name = "")
        {
            ParameterType = ptype;
            Required = required;
            Name = name;
            BoolValue = null;
            Value = null;
        }
    }
    internal class BadCodeException : Exception
    {
        public BadCodeException() : base()
        {
        }
        public BadCodeException(string msg) : base(msg)
        {
        }
    }
    internal class UserInputException : Exception
    {
        public UserInputException() : base()
        {
        }
        public UserInputException(string msg) : base(msg)
        {
        }
    }
}
