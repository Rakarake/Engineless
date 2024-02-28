## SDL2
SDL Boilerplate based on: https://jsayers.dev/c-sdl-tutorial-part-2-creating-a-window/
To use the software renderer: SDL_RENDER_DRIVER=software
To enable wayland if detected: SDL_VIDEODRIVER="wayland,x11"

## OpenGL

## Some ECS design / implementation details
Systems should recieve their input through maps/dictionaries
    Schema:
Systems register their input "types" through maps
    Schema:
    
Components "types" should be identified by UID, static class member gotten
when inheriting Component. Strings are slow.


Extremely simple entity / component instantiation.
`Engineless.Start` handles systems that should be executed on startup, then
begins the game loop.

Components can be any type.
Systems can be any funciton.
This is achieved using rutime type reflection (System.Reflection).
Entities don't really exist, they are just a value-type wrapper
around an identifier.

```cs
void main() {
    Engineless.Engine e = new Engineless.Engine();
    e
        .AddSystem(Engineless.Events.Startup, Startup)
        .Start()
}

void Startup(ECS ecs) {
    Entity entityId = ecs.newEntity([
        Transform(1.7,2.1,3.0)
    ]);
    Color color = new Color(0.5, 0.2, 0.1);
    ecs.addComponent(entityId, color);
    ecs.removeComponent(entityId, typeof(Color));
    ecs.addSystem(MoveLeft);
}

// Query's generic argument can be any type, SHOULD also cover no generic arguments
void MoveLeft(ECS ecs, Res<Time> time, Query<(Transform Transform, Color Color, Engineless.Entity Entity)> q) {
    foreach (cs in q.l) {
        cs.Transform.x -= 0.5 * time.delta;
    }
    if (time.secondsSinceStart() >= 10.0) {
        ecs.removeSystem(MoveLeft);
        ecs.despawnEntity(q.Entity);
    }
}

// Extends here gives us the CID (Component Identifier)
class Transform extends Component {
    public Transform {}
    public Vec3 pos = Vec3(0, 0, 0);
}

class Color extends Component {
    public Color {}
    public Vec3 rgb = Vec3(0, 0, 0);
}

```

