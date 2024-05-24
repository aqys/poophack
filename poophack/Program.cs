using ClickableTransparentOverlay;
using Swed64;
using System.Numerics;

namespace poophack
{
    class Program : Overlay
    {
        Swed swed = new Swed("cs2.exe");
        Offsets offsets = new Offsets();
        Entity localPlayer = new Entity();
        List<Entity> entities = new List<Entity>();
        IntPtr client;

        protected override void Render()
        {
            // render stuff here
        }

        void MainLogic()
        {
            client = swed.GetModuleBase("client.dll");
            
            while (true) // always run
            {
                entities.Clear(); // clear lists
                localPlayer.address = swed.ReadPointer(client, offsets.localPlayer); // set address so we can update
                UpdateEntity(localPlayer); // update

                Console.WriteLine($"localPlayer health -> {localPlayer.health}");
            }
        }

        void UpdateEntity(Entity entity)
        {
            entity.health = swed.ReadInt(entity.address, offsets.health);
            entity.origin = swed.ReadVec(entity.address, offsets.origin);
        }

        static void Main(string[] args)
        {
            // logic methods & stuff here
            Program program = new Program();
            program.Start().Wait();

            Thread mainLogicThread = new Thread(program.MainLogic) { IsBackground = true }; // logic thread
            mainLogicThread.Start();
        }
    }
}