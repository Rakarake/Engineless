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

```cs
void main() {
    Engineless.Start([
        StartupSystem,
    ]);
}

void Startup(ECS ecs, Engineless.StartupComponent _) {
    ecs.
}

// Extends here gives us the CID (Component Identifier)
struct Transform extends Component {
    public Transform {}
    public Vec3 pos = Vec3(0, 0, 0);
}

```

