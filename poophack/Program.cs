using ClickableTransparentOverlay;
using Swed64;
using System.Numerics;
using ImGuiNET;
using System.Runtime.InteropServices;

namespace poophack
{
    class Program : Overlay
    {
        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        Swed swed = new Swed("cs2");
        Offsets offsets = new Offsets();
        Entity localPlayer = new Entity();
        List<Entity> entities = new List<Entity>();
        List<Entity> enemyTeam = new List<Entity>();
        List<Entity> playerTeam = new List<Entity>();
        IntPtr client;

        // global colors
        Vector4 teamColor = new Vector4(0, 0, 1, 1); // blue
        Vector4 enemyColor = new Vector4(1, 0, 0, 1); // red
        Vector4 healthBarColor = new Vector4(0, 1, 0, 1); // green
        Vector4 healthTextColor = new Vector4(1, 1, 1, 1); // white

        // screen vars
        Vector2 windowLocation = new Vector2(0, 0);
        Vector2 windowSize = new Vector2(1920, 1080);
        Vector2 lineOrigin = new Vector2(1920 / 2, 1080); // middle of screen
        Vector2 windowCenter = new Vector2(1920 / 2, 1080 / 2);

        // imgui checkboxes
        bool enableEsp = true;

        bool enableTeamBox = false;
        bool enableTeamHealthBar = false;
        bool enableTeamLine = false;
        bool enabledTeamDot = false;
        bool enabledTeamDistance = false;

        bool enableEnemyBox = true;
        bool enableEnemyHealthBar = true;
        bool enableEnemyLine = false;
        bool enabledEnemyDot = false;
        bool enabledEnemyDistance = false;

        protected override void Render()
        {
            //ImGui.Begin("poophack");
            // render stuff here
        }

        void MainLogic()
        {
            client = swed.GetModuleBase("client.dll");
            
            while (true) // always run
            {
                
            }
        }

        void ReloadEntities()
        {
            entities.Clear(); // clear lists
            localPlayer.address = swed.ReadPointer(client, offsets.localPlayer); // set address so we can update
            UpdateEntity(localPlayer); // update
            UpdateEntities(); // update all entities
        }

        void UpdateEntities() // handle all other entities
        {
            for (int i = 0; i < 64; i++) // normally less than 64 entities
            {
                IntPtr tempEntityAddress = swed.ReadPointer(client, offsets.entityList + i * 0x08);
                if(tempEntityAddress == IntPtr.Zero)
                    continue; // skip if leading to address

                Entity entity = new Entity();
                entity.address = tempEntityAddress;
                UpdateEntity(entity);

                if (entity.health < 1 || entity.health > 100)
                    continue; // check if entity is dead or invalid

                if (!entities.Any(element => element.origin.X == entity.origin.X))
                {
                    entities.Add(entity);
                }
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