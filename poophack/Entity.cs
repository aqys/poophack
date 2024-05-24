using System.Numerics;

namespace poophack
{
    public class Entity
    {
        public IntPtr address { get; set; }
        public int health { get; set; }
        public Vector3 origin { get; set; }
    }
}
