# The Engineless Game Engine.

A very simple ECS engine inspired by bevy.

The order of registration of the systems determine in which order
they will be executed.

What can a system query?
* `IECS`: this lets you create entities, resources and register systems
* `Res<T>`: a resource object with the field 'hit' which holds an instance
  of T. This acts like a singleton for the type T, but unlike singletons,
  there can be multiple instances of T if there are multiple 'Engine'
  objects at the same time.
* `Query<T>`: a query object with the field 'hits', this is a list
  of KeyValuePair where the Key is the id of the entity and the value
  is the component T. Basically this just queries all instances of a
  component T and their respective entity.
* `Query<(T, U)>`: the heart of an ECS, same as `Query<T>` but 'hits'
  contains tuples of components that share the same entity (the Key of the
  KeyValuePair).

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

ecs.AddSystem(Event.Update, SimpleSystem);
ecs.Start();
```

Note that components must be reference types if you wish to modify them
since the you will always get copies of value types.
A workaround is wrapping value types, say with `Wrapper<T>`.

The engine is currently not optimized for speed, it does not take
locality into consideration, and queries are of O(n) complexity (even
ones that query one component, this could probably be fixed).

There are few safety checks, for example, systems with faulty arguments
will be reported only when they are about to be used.


## Some design decisions
There was a separation of the interface of the engine, before and after
the engine is started, so that you say, cannot start it again (would make
no sense).

## Wish list / missing features
A way of adding / removing / reoredering systems.

"No engines?"

