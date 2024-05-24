using ClickableTransparentOverlay;
using Swed64;
using System.Numerics;

namespace poophack
{
    class Program : Overlay
    {
        Swed swed = new Swed("cs2.exe");
        Entity localPlayer = new Entity();
        List<Entity> entities = new List<Entity>();
        IntPtr client;

        protected override void Render()
        {
            // render stuff here
        }

        void MainLogic()
        {

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