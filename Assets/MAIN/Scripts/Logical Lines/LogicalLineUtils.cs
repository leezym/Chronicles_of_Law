using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO.Pipes;
using System;

namespace DIALOGUE.LogicalLines
{
    public static class LogicalLineUtils
    {
        public static class Encapsulation
        {
            public struct EncapsulatedData
            {
                public List<string> lines;
                public int startingIndex;
                public int endingIndex;
            }

            private const char CHOICE_IDENTIFIER = '-';
            private const char ENCAPSULATION_START = '{';
            private const char ENCAPSULATION_END = '}';

            public static EncapsulatedData RipEncapsulationData(Conversation conversation, int startingIndex, bool ripHeaderAndEncapsulators = false, int parentStartingIndex = 0)
            {
                int encapsulationDepth = 0;
                EncapsulatedData data = new EncapsulatedData{ lines = new List<string>(), startingIndex = (startingIndex + parentStartingIndex), endingIndex = 0 };

                for(int i = startingIndex; i < conversation.Count; i++)
                {
                    string line = conversation.GetLines()[i];

                    if(ripHeaderAndEncapsulators || (encapsulationDepth > 0 && !IsEncapsulationEnd(line)))
                        data.lines.Add(line);

                    if(IsEncapsulationStart(line))
                    {
                        encapsulationDepth++;
                        continue;
                    }

                    if(IsEncapsulationEnd(line))
                    {
                        encapsulationDepth--;
                        if(encapsulationDepth == 0)
                        {
                            data.endingIndex = (i + parentStartingIndex);
                            break;
                        }
                    }
                }

                return data;
            }

            public static bool IsChoiceStart(string line) => line.Trim().StartsWith(CHOICE_IDENTIFIER);
            public static bool IsEncapsulationStart(string line) => line.Trim().StartsWith(ENCAPSULATION_START);
            public static bool IsEncapsulationEnd(string line) => line.Trim().StartsWith(ENCAPSULATION_END);
        }

        public static class Conditions
        {
            public static readonly string REGEX_CONDITION_OPERATORS = @"(>=|<)"; // Desarrollo para la única condición del juego
            public static bool EvaluateCondition(string condition)
            {
                string[] parts = Regex.Split(condition, REGEX_CONDITION_OPERATORS).Select(p => p.Trim()).ToArray();
                VariableStore.TryGetValue(parts[0], out object value);

                if(parts.Length == 3) //Detecta 3 partes en la condición (dos valores + un operador)
                    return EvaluateExpression(value.ToString(), parts[1], parts[2]);
                else
                {
                    Debug.LogError($"Unsupported condition format: {condition}");
                    return false;
                }
            }

            private delegate bool OperatorFunc<T>(T left, T right);
            
            private static Dictionary<string, OperatorFunc<float>> floatOperators = new Dictionary<string, OperatorFunc<float>>()
            {
                {">=", (left, right) => left >= right},
                {"<", (left, right) => left < right}
            };

            private static Dictionary<string, OperatorFunc<int>> intOperators = new Dictionary<string, OperatorFunc<int>>()
            {
                {">=", (left, right) => left >= right},
                {"<", (left, right) => left < right}
            };

            private static bool EvaluateExpression(string left, string op, string right)
            {
                if(float.TryParse(left, out float leftFloat) && float.TryParse(right, out float rightFloat))
                    return floatOperators[op](leftFloat, rightFloat);

                if(int.TryParse(left, out int leftInt) && int.TryParse(right, out int rightInt))
                    return intOperators[op](leftInt, rightInt);
                
                throw new InvalidOperationException($"Unsupported Operation: {op}");
            }
        }
    }
}