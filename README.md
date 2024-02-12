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

Classes Component and System only exists for readibility and some boilerplate,
this is not an endorsement to ues inhereitance.

```cs
import Engineless;

void main() {
    Engineless.Start([
        StartupSystem.Desc(),
    ]);
}

// Systems signal their arguments though 'Desc'
class Startup extends System {
    [int] Desc() {
        // Returns a list of the id:s of the components we are interested in
        // TODO does this work?
        return [ StartupComponent.CID ]
    }
    void Call(ECS ecs, Dictionary<int, Component> components) {
        // Have to cast component values
        Transform transform = Transform.NewFromPostion(Vec3(1,2,3));
        ecs.addNewEntity([transform]);
        // TODO add MoveLeft system
    }
}

class MoveLeft extends System {
    [int] Desc() {
        // Returns a list of the id:s of the components we are interested in
        return [ TransformComponent.CID ]
    }
    void Call(ECS ecs, Dictionary<int, Component> components) {
        Entity entity = new Entity();
        Transform transform = Transform.NewFromPostion(Vec3(1,2,3));
        entity.addComponent(transform);
    }
}

// Extends here gives us the CID (Component Identifier)
class Transform extends Component {
    public Vec3 pos = Vec3(0, 0, 0);
}

```

