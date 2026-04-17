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
            // Soporta >=, <=, >, <, ==, !=
            private static readonly string REGEX_CONDITION_OPERATORS = @"(>=|<=|>|<|==|!=)";
            private static readonly string[] AND_SEPARATOR = new string[] { "&&" };

            public static bool EvaluateCondition(string condition)
            {
                string[] subConditions = condition.Split(AND_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);

                foreach (string sub in subConditions)
                {
                    if (!EvaluateSingleCondition(sub.Trim()))
                        return false;
                }

                return true;
            }

            private static bool EvaluateSingleCondition(string condition)
            {
                string[] parts = Regex.Split(condition, REGEX_CONDITION_OPERATORS)
                                      .Select(p => p.Trim())
                                      .Where(p => !string.IsNullOrEmpty(p))
                                      .ToArray();

                if (parts.Length != 3)
                {
                    Debug.LogError($"Unsupported condition format: '{condition}'");
                    return false;
                }

                string variableName = parts[0];
                string op           = parts[1];
                string rawRight     = parts[2];

                if (!VariableStore.TryGetValue(variableName, out object value))
                {
                    Debug.LogError($"Variable not found in VariableStore: '{variableName}'");
                    return false;
                }

                return EvaluateExpression(value.ToString(), op, rawRight);
            }

            private delegate bool OperatorFunc<T>(T left, T right);

            private static readonly Dictionary<string, OperatorFunc<float>> floatOperators = new Dictionary<string, OperatorFunc<float>>()
            {
                {">=", (l, r) => l >= r},
                {"<=", (l, r) => l <= r},
                {">",  (l, r) => l >  r},
                {"<",  (l, r) => l <  r},
                {"==", (l, r) => l == r},
                {"!=", (l, r) => l != r},
            };

            private static readonly Dictionary<string, OperatorFunc<int>> intOperators = new Dictionary<string, OperatorFunc<int>>()
            {
                {">=", (l, r) => l >= r},
                {"<=", (l, r) => l <= r},
                {">",  (l, r) => l >  r},
                {"<",  (l, r) => l <  r},
                {"==", (l, r) => l == r},
                {"!=", (l, r) => l != r},
            };

            private static readonly Dictionary<string, OperatorFunc<bool>> boolOperators = new Dictionary<string, OperatorFunc<bool>>()
            {
                {"==", (l, r) => l == r},
                {"!=", (l, r) => l != r},
            };

            private static bool EvaluateExpression(string left, string op, string right)
            {
                if (int.TryParse(left, out int leftInt) && int.TryParse(right, out int rightInt))
                    return intOperators[op](leftInt, rightInt);

                if (float.TryParse(left, System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out float leftFloat) &&
                    float.TryParse(right, System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out float rightFloat))
                    return floatOperators[op](leftFloat, rightFloat);

                if (bool.TryParse(left, out bool leftBool) && bool.TryParse(right, out bool rightBool))
                    return boolOperators[op](leftBool, rightBool);

                throw new InvalidOperationException($"Cannot evaluate expression: '{left}' {op} '{right}'");
            }
        }
    }
}