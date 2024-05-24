using System.Numerics;

namespace poophack
{
    public class Entity
    {
        public IntPtr address { get; set; }
        public int health { get; set; }
        public int teamNum { get; set; }
        public int jumpFlag { get; set; }
        public Vector3 origin { get; set; }
        public Vector3 abs { get; set; }
        public Vector2 originScreenPosition { get; set; }
        public Vector2 absScreenPosition { get; set; }
    }
}
