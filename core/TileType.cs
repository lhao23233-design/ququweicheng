using Godot;
using System.Collections.Generic;

namespace CoopDefense.Tiletypes
{
    public enum TileType
    {
        Empty,      // 空地
        Road,       // 可铺路
        Wall,       // 路障（墙）
        WeakWall,   // 可破坏的路障
        SpawnPoint, // 出怪口
        EndPoint    // 终点
    }

    public static class TileProperties
    {
        private static readonly Dictionary<TileType, bool> destructibleMap = new Dictionary<TileType, bool>
        {
            { TileType.Empty, false },
            { TileType.Road, false },
            { TileType.Wall, false },
            { TileType.WeakWall, true },
            { TileType.SpawnPoint, false },
            { TileType.EndPoint, false }
        };

        public static bool IsDestructible(TileType type)
        {
            return destructibleMap[type];
        }
    }
}
//枚举定义：空地、可铺路、路障（墙）、出怪口、终点等。
//属性字典：既然你说要给路障加“是否可破坏”的属性，这个类里应该有个映射表。比如：TileType.Wall 对应 IsDestructible = false，TileType.WeakWall 对应 IsDestructible = true。以后加新地形，直接在这里扩。