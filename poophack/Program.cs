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

        public RECT GetWindowRect(IntPtr hWnd)
        {
            RECT rect = new RECT();
            GetWindowRect(hWnd, out rect);
            return rect;
        }

        Swed swed = new Swed("cs2");
        Offsets offsets = new Offsets();
        ImDrawListPtr drawList;
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
        bool enableTeamDot = false;
        bool enableTeamDistance = false;

        bool enableEnemyBox = true;
        bool enableEnemyHealthBar = true;
        bool enableEnemyLine = false;
        bool enableEnemyDot = false;
        bool enableEnemyDistance = false;

        protected override void Render()
        {
            DrawMenu();
            DrawOverlay();
            // render stuff here
        }

        ViewMatrix ReadMatrix(IntPtr matrixAddress)
        {
            var viewMatrix = new ViewMatrix();
            var floatMatrix = swed.ReadMatrix(matrixAddress);

            viewMatrix.m11 = floatMatrix[0];
            viewMatrix.m12 = floatMatrix[1];
            viewMatrix.m13 = floatMatrix[2];
            viewMatrix.m14 = floatMatrix[3];

            viewMatrix.m21 = floatMatrix[4];
            viewMatrix.m22 = floatMatrix[5];
            viewMatrix.m23 = floatMatrix[6];
            viewMatrix.m24 = floatMatrix[7];

            viewMatrix.m31 = floatMatrix[8];
            viewMatrix.m32 = floatMatrix[9];
            viewMatrix.m33 = floatMatrix[10];
            viewMatrix.m34 = floatMatrix[11];

            viewMatrix.m41 = floatMatrix[12];
            viewMatrix.m42 = floatMatrix[13];
            viewMatrix.m43 = floatMatrix[14];
            viewMatrix.m44 = floatMatrix[15];

            return viewMatrix;
        }

        Vector2 WorldToScreen(ViewMatrix matrix, Vector3 pos, int width, int height)
        {
            Vector2 screenCoordinates = new Vector2();

            float screenW = (matrix.m41 * pos.X) + (matrix.m42 * pos.Y) + (matrix.m43 * pos.Z) + matrix.m44;

            if (screenW < 0.001f)
            {
                float screenX = (matrix.m11 * pos.X) + (matrix.m12 * pos.Y) + (matrix.m13 * pos.Z) + matrix.m14;

                float screenY = (matrix.m21 * pos.X) + (matrix.m22 * pos.Y) + (matrix.m23 * pos.Z) + matrix.m24;

                float camX = width / 2;
                float camY = height / 2;

                float X = camX + (camX * screenX / screenW);
                float Y = camY - (camY * screenY / screenW);

                screenCoordinates.X = X;
                screenCoordinates.Y = Y;
                return screenCoordinates;
            }
            else
            {
                return new Vector2(-99, -99);
            }
        }

        void DrawMenu()
        {
            ImGui.Begin("poophack");

            if (ImGui.BeginTabBar("Tabs"))
            {
                if (ImGui.BeginTabItem("General"))
                {
                    ImGui.Checkbox("Enable ESP", ref enableEsp);

                    ImGui.Checkbox("Team Box", ref enableTeamBox);
                    ImGui.Checkbox("Team Healthbar", ref enableTeamHealthBar);
                    ImGui.Checkbox("Team Line", ref enableTeamLine);
                    ImGui.Checkbox("Team Dot", ref enableTeamDot);
                    ImGui.Checkbox("Enemy Box", ref enableEnemyBox);
                    ImGui.Checkbox("Enemy Healthbar", ref enableEnemyHealthBar);
                    ImGui.Checkbox("Enemy Line", ref enableEnemyLine);
                    ImGui.Checkbox("Enemy Dot", ref enableEnemyDot);

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Colors"))
                {
                    ImGui.ColorEdit4("Team Color", ref teamColor);
                    ImGui.ColorEdit4("Enemy Color", ref enemyColor);
                }
            }
            ImGui.EndTabBar();
        }

        void DrawOverlay()
        {
            ImGui.SetNextWindowSize(windowSize);
            ImGui.SetNextWindowPos(windowLocation);
            ImGui.Begin("overlay", ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoBackground 
                | ImGuiWindowFlags.NoBringToFrontOnFocus 
                | ImGuiWindowFlags.NoMove 
                | ImGuiWindowFlags.NoInputs 
                | ImGuiWindowFlags.NoCollapse 
                | ImGuiWindowFlags.NoScrollWithMouse
                );
        }

        void MainLogic()
        {
            var window = GetWindowRect(swed.GetProcess().MainWindowHandle);
            windowLocation = new Vector2(window.left, window.top);
            windowSize = Vector2.Subtract(new Vector2(window.right, window.bottom), windowLocation);
            lineOrigin = new Vector2(windowLocation.X * windowLocation.X / 2, window.bottom);
            windowCenter = new Vector2(lineOrigin.X, window.bottom - windowSize.Y / 2);

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

                    if (entity.teamNum == localPlayer.teamNum)
                    {
                        playerTeam.Add(entity);
                    }
                    else
                    {
                        enemyTeam.Add(entity);
                    }
                }
            }
        }

        void UpdateEntity(Entity entity)
        {
            entity.health = swed.ReadInt(entity.address, offsets.health);
            entity.origin = swed.ReadVec(entity.address, offsets.origin);
            entity.teamNum = swed.ReadInt(entity.address, offsets.teamNum);
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