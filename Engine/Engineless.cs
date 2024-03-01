using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Engineless {

    public enum Event {
        Startup,
        Update,
    }

    public abstract class Component { }
    // These empty classes are used to what resources to fetch
    public class Query<T> { public IEnumerable<KeyValuePair<int, T>> hits; }
    
    // State class for the ECS
    public class Engine : IECS {
        private List<Delegate> updateSystems = new();
        private List<Delegate> startupSystems = new();
        private Dictionary<Type, Dictionary<int, Component>> allComponents = new();
        private int entityIndex = 0;
        private bool running = true;

        public Engine AddSystem(Event e, Delegate system) {
            if (e == Event.Startup) {
                startupSystems.Add(system);
            }
            if (e == Event.Update) {
                updateSystems.Add(system);
            }
            return this;
        }

        // Game loop
        public void Start() {
            Console.WriteLine("Starting Engine!");
            foreach (Delegate system in startupSystems) {
                HandleSystem(system);
            }
            // Start game loop
            while (running) {
                System.Threading.Thread.Sleep(5);
                foreach (Delegate system in updateSystems) {
                    HandleSystem(system);
                }
            }
        }

        private void HandleSystem(Delegate system) {
            // Example system:
            // "time" here is a "resource" in the form of a component
            // void MoveLeft(ECS ecs, Query<Time> time, Query<(Transform Transform, Color Color, Engineless.Entity Entity)> q)
            MethodInfo methodInfo = system.Method;
            ParameterInfo[] parameters = methodInfo.GetParameters();
            List<Object> systemArguments = new();
            foreach (ParameterInfo parameter in parameters) {
                Console.WriteLine("Type name: " + parameter.ParameterType.Name);
                if (parameter.ParameterType == typeof(IECS)) {
                    Console.WriteLine("Found IECS 🔥");
                    systemArguments.Add(this);
                } else if (parameter.ParameterType.Name.Substring(0, 5) == "Query") {
                    Console.WriteLine("About to process Query");
                    // Two cases:
                    // * One generic argument, provide that list directly
                    // * Tuple of arguments
                    // There is only one type argument of Query
                    Type typeArgument = parameter.ParameterType.GetGenericArguments()[0];

                    // Create the query object
                    var queryGenericType = typeof(Query<>);
                    var queryGenericTypeFulfilled = queryGenericType.MakeGenericType(typeArgument);
                    var queryInstance = Activator.CreateInstance(queryGenericTypeFulfilled);
                    Console.WriteLine("QueryInstance: " + queryInstance);
                    var fieldInfo = queryGenericTypeFulfilled.GetField("hits", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    Console.WriteLine("FieldInfo: " + fieldInfo);
                    Type[] queryTypes = typeArgument.GetGenericArguments();

                    if (queryTypes == null || queryTypes.Length == 0) {
                        // One argument
                        Console.WriteLine("One argument");
                        fieldInfo.SetValue(queryInstance,
                                allComponents.ContainsKey(typeArgument) ?
                                    allComponents[typeArgument] :
                                    RCreateEmptyList(typeArgument));
                        systemArguments.Add(queryInstance);
                    } else {
                        // Two or more arguments
                        Console.WriteLine("Two argument");
                        // Only interested in the columns with the tuple types
                        // All must be present
                        // Type needed to construct the tuples
                        List<(Type, Dictionary<int, Component>)> cs = new();
                        bool componentColumnsExist = true;
                        foreach (Type t in queryTypes) {
                            if (allComponents.ContainsKey(t)) {
                                cs.Add((t, allComponents[t]));
                            } else {
                                componentColumnsExist = false;
                            }
                        }
                        if (!componentColumnsExist) { 
                            Console.WriteLine("SUPER LIST: " + RCreateEmptyList(typeArgument));
                            fieldInfo.SetValue(queryInstance, RCreateEmptyList(
                                        typeof(KeyValuePair<,>).MakeGenericType(typeof(int), typeArgument)
                                        ));
                            systemArguments.Add(queryInstance);
                            continue;
                        }

                        var smallest = cs.Aggregate(cs[0],
                            (shortest, next) =>
                                next.Item2.Count < shortest.Item2.Count ? next : shortest);

                        // Use shortest to check for tuple matches
                        cs.Remove(smallest);
                        var restOfColumns = cs;
                        // List of tuples of the query
                        List<Object> queryResult = new();
                        foreach (var c in smallest.Item2) {
                            List<(Type, Object)> tupleComponents = new() { (smallest.Item1, c.Value) };
                            bool tupleComplete = true;
                            foreach (var column in restOfColumns) {
                                if (column.Item2.ContainsKey(c.Key)) {
                                    tupleComponents.Add((column.Item1, column.Item2[c.Key]));
                                } else {
                                    tupleComplete = false;
                                    break;
                                }
                            }
                            if (!tupleComplete) {
                                continue;
                            } 
                            // Construct the tuple to add to queryResult
                            queryResult.Add(new KeyValuePair<int, Object>(c.Key, RGetTuple(tupleComponents)));
                        }
                        fieldInfo.SetValue(queryInstance, queryResult);
                        systemArguments.Add(queryInstance);
                    }

                } else {
                    throw new InvalidOperationException("Systems cannot aquire anything other than IECS and Query");
                }
                
            }
            Console.WriteLine("SystemArguments:");
            foreach (var e in systemArguments) { Console.WriteLine("  * " + e); }
            // Create an object with the right type information
            
            system.DynamicInvoke(systemArguments.ToArray());
        }

        public void AddEntity(List<Component> components) {
            Console.WriteLine("Adding Entity");
            foreach (Component c in components) {
                if (!allComponents.ContainsKey(c.GetType())
                        || allComponents[c.GetType()] == null) {
                    // First component of this type
                    allComponents[c.GetType()] = new();
                }
                allComponents[c.GetType()].Add(entityIndex, c);
            }
            entityIndex += 1;
        }

        // Reflection helper methods ('R' is for reflection)
        private Object RGetTuple(List<(Type, Object)> input) {
            Type genericType = Type.GetType("System.Tuple`" + input.Count);
            Type[] typeArgs = input.Select(p => p.Item1).ToArray();
            Object[] valueArgs = input.Select(p => p.Item2).ToArray();
            Type specificType = genericType.MakeGenericType(typeArgs);
            object[] constructorArguments = valueArgs.Cast<object>().ToArray();
            return Activator.CreateInstance(specificType, constructorArguments);
        }

        private Object RCreateEmptyList(Type t) {
            return Activator.CreateInstance(typeof(List<>).MakeGenericType(t));
        }

    }

    // Resource passed to systems to provide functionality to create/remove
    // entities, components and systems
    public interface IECS {
        public void AddEntity(List<Component> components);
    }
}
