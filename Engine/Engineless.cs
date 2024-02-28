using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Engineless {

    public enum Event {
        Startup,
        Update,
    }

    public abstract class Component { public int entity = -1; }
    // These empty classes are used to what resources to fetch
    public class Query<T> { List<T> hits; }
    
    // State class for the ECS
    public class Engine : IECS {
        private List<Delegate> updateSystems = new();
        private List<Delegate> startupSystems = new();
        private Dictionary<Type, List<Component>> allComponents = new();
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
                    Type[] queryTypes = typeArgument.GetGenericArguments();
                    if (queryTypes == null || queryTypes.Length == 0) {
                        // One argument
                        Console.WriteLine("One argument");
                        if (allComponents.ContainsKey(typeArgument)) {
                            systemArguments.Add(allComponents[typeArgument]);
                        } else {
                            continue;
                        }
                    } else {
                        // Two or more arguments
                        Console.WriteLine("Two argument");
                        // Only interested in the columns with the tuple types
                        List<List<Component>> cs = new();
                        foreach (Type t in queryTypes) {
                            if (allComponents.ContainsKey(t)) {
                                cs.Add(allComponents[t]);
                            } else {
                                continue;
                            }
                        }
                        //cs.GroupBy(c =>)
                        //var grouped = allComponents.GroupBy(c => c.Value);
                    }

                } else {
                    throw new InvalidOperationException("Systems cannot aquire anything other than IECS and Query");
                }
                
            }
            system.DynamicInvoke(systemArguments.ToArray());
        }

        public void AddEntity(List<Component> components) {
            Console.WriteLine("Adding Entity");
            foreach (Component c in components) {
                if (allComponents[c.GetType()] == null) {
                    // First component of this type
                    allComponents[c.GetType()] = new List<Component>();
                }
                c.entity = entityIndex;
                allComponents[c.GetType()].Add(c);
            }
            entityIndex += 1;
        }
    }

    // Resource passed to systems to provide functionality to create/remove
    // entities, components and systems
    public interface IECS {
        public void AddEntity(List<Component> components);
    }
}
