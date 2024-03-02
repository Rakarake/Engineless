using System;
using System.Collections.Generic;
using Engineless;

// Test ECS
class ECSExample {

    void StartupSystem(IECS ecs) {
        Console.WriteLine("Starting it up with the startup system");
        ecs.AddEntity(new List<Component>() {
                    new DuckAspects() { greatQuack = "QUACK QUACK QUACK 🦜" },
                    new InspirationalQuote() { text = "I could eat an octorock!?" },
                });
    }

    void SuperSystem(Query<(DuckAspects, InspirationalQuote)> q) {
        foreach (var hit in q.hits) {
            var (duckAspects, inspirationalQuote) = hit.Value;
            Console.WriteLine("Quack: " + duckAspects.greatQuack);
            Console.WriteLine("Inspire me!: " + inspirationalQuote.text);
        }
    }

    public void Example() {
        
        Engine engine = new();
        engine
            .AddSystem(Event.Startup, StartupSystem)
            .AddSystem(Event.Update, SuperSystem)
            .Start();
    }
}

class DuckAspects : Component {
    public String greatQuack;
}

class InspirationalQuote : Component {
    public String text;
}

