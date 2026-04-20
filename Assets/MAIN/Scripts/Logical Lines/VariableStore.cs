using System;
using System.Collections.Generic;

public class VariableStore
{
    public const string DEFAULT_DATABASE_NAME = "Default";
    public const string DEFAULT_VARIABLE_RELATIONAL_ID = ".";
    public class Database
    {
        public Database(string name)
        {
            this.name = name;
            variables = new Dictionary<string, Variable>();
        }

        public string name;
        public Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
    }

    public abstract class Variable
    {
        public abstract object Get();
        public abstract void Set(object vlaue);
    }

    public class Variable<T> : Variable
    {
        private T value;
        private Func<T> getter;
        private Action<T> setter;

        public Variable(T defaultValue = default, Func<T> getter = null, Action<T> setter = null)
        {
            value = defaultValue;

            if(getter == null)
                this.getter = () => value;
            else
                this.getter = getter;

            if(setter == null)
                this.setter = newValue => value = newValue;
            else
                this.setter = setter;            
        }

        public override object Get() => getter();

        public override void Set(object newValue) => setter((T)newValue);
    }

    private static Dictionary<string, Database> databases = new Dictionary<string, Database>() { {DEFAULT_DATABASE_NAME, new Database(DEFAULT_DATABASE_NAME)} };
    private static Database defaultDatabase => databases[DEFAULT_DATABASE_NAME];
    public static bool CreateDatabase(string name)
    {
        if(!databases.ContainsKey(name))
        {
            databases[name] = new Database(name);
            return true;
        }

        return false;
    }

    public static Database GetDatabase(string name)
    {
        if(name == string.Empty)
            return defaultDatabase;
        if(!databases.ContainsKey(name))
            CreateDatabase(name);
        
        return databases[name];
    }

    public static bool CreateVariable<T>(string name, T defaultValue, Func<T> getter = null, Action<T> setter = null)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);
        if(db.variables.ContainsKey(variableName))
            return false;
        
        db.variables[variableName] = new Variable<T>(defaultValue, getter, setter);
        return true;
    }

    public static bool TryGetValue(string name, out object variable)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if(!db.variables.ContainsKey(variableName))
        {
            variable = null;
            return false;
        }
        
        variable = db.variables[variableName].Get();
        return true;
    }

    public static bool TrySetValue<T>(string name, T value)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if(!db.variables.ContainsKey(variableName))
            return false;

        db.variables[variableName].Set(value);
        return true;
    }
    
    private static (string[], Database, string) ExtractInfo(string name)
    {
        string[] parts = name.Split(DEFAULT_VARIABLE_RELATIONAL_ID);
        Database db = parts.Length > 1 ? GetDatabase(parts[0]) : defaultDatabase;
        string variableName = parts.Length > 1 ? parts[1] : parts[0];

        return (parts, db, variableName);
    }

    public static void RemoveVariable(string name)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if(db.variables.ContainsKey(variableName))
            db.variables.Remove(variableName);        
    }

    public static void RemoveAllVariables()
    {
        databases.Clear();
        databases[DEFAULT_DATABASE_NAME] = new Database(DEFAULT_DATABASE_NAME);
    }
}
