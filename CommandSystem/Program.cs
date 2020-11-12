using System;
using System.Collections.Generic;

namespace CommandSystem
{
    class Program
    {
        static List<string> exitCommands = new List<string> { "exit", "quit", "stop", "shutdown", "cancel", "terminate" };

        static void Main(string[] args)
        {
            CommandManager.Awake(); // Simulating unity awake... really not perfect very hacky but what can I say it works for the testing <3 :D

            while (true)
            {
                Console.WriteLine("Enter Command!");
                string input = Console.ReadLine();

                if (exitCommands.Contains(input))
                {
                    break;
                }

                CommandManager.HandleInput(input);
            }
        }
    }

    static class CommandManager
    {
        public static List<CommandBase> commands = new List<CommandBase>();

        public static void Awake()
        {
            new TestCommandModule().RegisterCommands(commands);
        }

        public static void HandleInput(string input)
        {
            string[] properties = input.Split(' ');

            for (int i = 0; i < commands.Count; i++)
            {
                CommandBase commandBase = commands[i];

                if (properties[0] == commandBase.commandId)
                {
                    if (commands[i] is Command command)
                    {
                        command.Invoke();
                        return;
                    }
                    else if (commands[i] is Command<int> command1 && properties.Length > 1)
                    {
                        command1.Invoke(int.Parse(properties[1]));
                        return;
                    }
                    else
                    {
                        Console.WriteLine("command not found please try again...");
                    }
                }
            }
        }
    }

    public interface ICommandModule
    {
        void RegisterCommands(List<CommandBase> commands);
    }

    public class TestCommandModule : ICommandModule
    {
        public void RegisterCommands(List<CommandBase> commands)
        {
            var testCommand = new Command("test_command", "Test command.", "test_command", () =>
            {
                Console.WriteLine("Test command works!");
            });

            var testValueCommand = new Command<int>("set_value", "Value test command for generic inputs.", "set_value <value_ammount>", (x) =>
            {
                //Console.WriteLine(TEST_VALUE.commandDescription + " Value: " + x);
                Console.WriteLine(x);
            });

            commands.Add(testCommand);
            commands.Add(testValueCommand);
        }
    }

    public class CommandBase
    {
        private string _commandId;
        private string _commandDescription;
        private string _commandFormat;

        public string commandId { get { return _commandId; } }
        public string commandDescription { get { return _commandDescription; } }
        public string commandFormat { get { return _commandFormat; } }

        public CommandBase(string id, string description, string format)
        {
            _commandId = id;
            _commandDescription = description;
            _commandFormat = format;
        }
    }

    class Command : CommandBase
    {
        private Action command;

        public Command(string id, string description, string format, Action command) : base(id, description, format)
        {
            this.command = command;
        }

        public void Invoke()
        {
            command.Invoke();
        }
    }

    class Command<T1> : CommandBase
    {
        private Action<T1> command;

        public Command(string id, string description, string format, Action<T1> command) : base(id, description, format)
        {
            this.command = command;
        }

        public void Invoke(T1 value)
        {
            command.Invoke(value);
        }
    }
}
