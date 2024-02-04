using SDL2;

namespace HelloWorld
{
    class Hello {         
        // Global state 👻
        static IntPtr window;
        static IntPtr renderer;
        static bool running = true;

        static void Setup() 
        {
            // Initilizes SDL.
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine($"There was an issue initializing SDL. {SDL.SDL_GetError()}");
            }
        
            // Create a new window given a title, size, and passes it a flag indicating it should be shown.
            window = SDL.SDL_CreateWindow(
                "SDL .NET 6 Tutorial",
                SDL.SDL_WINDOWPOS_UNDEFINED, 
                SDL.SDL_WINDOWPOS_UNDEFINED, 
                640, 
                480, 
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
        
            if (window == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
            }
        
            // Creates a new SDL hardware renderer using the default graphics device with VSYNC enabled.
            renderer = SDL.SDL_CreateRenderer(
                window,
                -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
        
            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
            }
        }

        static void Cleanup() {
        }

        static void PollEvents() {
        }
        static void Render() {
            // Sets the color that the screen will be cleared with.
            SDL.SDL_SetRenderDrawColor(renderer, 135, 206, 235, 255);
        
            // Clears the current render surface.
            SDL.SDL_RenderClear(renderer);
        
            // Switches out the currently presented render surface with the one we just did work on.
            SDL.SDL_RenderPresent(renderer);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Initiating ... 🚂");
            Setup();
            while (running)
            {
                PollEvents();
                Render();
            }
            Cleanup();
        }
    }
}
