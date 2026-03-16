using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace COMMANDS
{
    public class CommandManager : MonoBehaviour
    {
        public static CommandManager Instance { get; private set; }
        private static Coroutine process = null;
        public static bool isRunningProcess => process != null;

        private CommandDatabase database;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                database = new CommandDatabase();

                Assembly assembly = Assembly.GetExecutingAssembly();
                Type[] extensionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CMD_DatabaseExtension))).ToArray();

                foreach(Type extension in extensionTypes)
                {
                    MethodInfo extendMethod = extension.GetMethod("Extend");
                    extendMethod.Invoke(null, new object[] {database}); //null because Extend is static method
                }
            }
            else
                DestroyImmediate(gameObject);
        }

        public Coroutine Execute(string commandName, params string[] args)
        {
            Delegate command = database.GetCommand(commandName);

            if(command == null)
                return null;
            
            return StartProcess(command, args);
        }

        private Coroutine StartProcess(Delegate command, string[] args)
        {
            StopCurentProcess();

            process = StartCoroutine(RunningProcess(command, args));

            return process;
        }

        private void StopCurentProcess()
        {
            if(process != null)
                StopCoroutine(process);
            
            process = null;
        }

        public void StopAllProcesses()
        {
            if(process != null)
                StopCoroutine(process);
            
            process = null;
        }

        private IEnumerator RunningProcess(Delegate command, string[] args)
        {
            yield return WaitingForProcessToComplete(command, args);

            process = null;
        }

        private IEnumerator WaitingForProcessToComplete(Delegate command, string[] args)
        {
            if(command is Action)
                command.DynamicInvoke();

            else if(command is Action<string>)
                command.DynamicInvoke(args.Length == 0 ? string.Empty : args[0]);

            else if(command is Action<string[]>)
                command.DynamicInvoke((object)args);

            else if(command is Func<IEnumerator>)
                yield return ((Func<IEnumerator>)command)();

            else if(command is Func<string, IEnumerator>)
                yield return ((Func<string, IEnumerator>)command)(args.Length == 0 ? string.Empty : args[0]);

            else if(command is Func<string[], IEnumerator>)
                yield return ((Func<string[], IEnumerator>)command)(args);
        }
    }
}