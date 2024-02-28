using System;
using System.Collections.Generic;
using Engineless;

// Test ECS
class ECSExample {

    void StartupSystem(IECS ecs) {
        ecs.AddEntity(new List<Component>());
    }

    void SuperSystem(Query<DuckAspects> ok) {
        
    }

    public void Example() {
        
        Engine engine = new();
        engine
            .AddSystem(Event.Startup, StartupSystem)
            .AddSystem(Event.Update, SuperSystem)
            .Start();
    }
}

class DuckAspects {
    public String greatQuack;
}

class InspirationalQuote {
    public String text;
}

