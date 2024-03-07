# The Engineless Game Engine

A very simple ECS engine heavily inspired by bevy.

Note that components must be reference types if you wish to modify them
since the you will always get copies of value types.
A workaround is wrapping value types in your own classes.

If you know how to solve this, tell me 😳 (the value types are boxed, so
the solution might not be that far off, maybe).

## Example
```cs
using Engineless;

void SimpleSystem(Query<(Wrapper<int>, String)> q) {
    foreach (var hit in q.hits) {
        // Retrieve the parts of the query
        var (x, y) = hit.Value;
        Console.WriteLine("Entity ID: " + hit.Key);
        Console.WriteLine("x: " + x.item + ", y: " + y);
        // Increment each query
        x.item += 1;
    }
}

Engine ecs = new();

ecs.AddEntity(new List<Object>() {
    new Wrapper<int>() { item = 0 },
    "Hi Mark!",
});

ecs
    .AddSystem(Event.Update, SimpleSystem)
    .Start();
```

## Some design decisions
There was a separation of the interface of the engine, before and after
the engine is started, so that you say, cannot start it again (would make
no sense).

"No engines?"

