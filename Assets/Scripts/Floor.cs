using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Floor
{
    private List<Room> Rooms { get; }
    private Room Start { get; }
    private Room Shop { get; }
    private Room Boss { get; }
    private Room Treasure { get; }

    public Floor()
    {
        Start = new Room(4, 4);
        (Shop, Treasure, Boss, Rooms) = SpawnRooms(Start, 8);
        
    }

    private (Room shop, Room treasure, Room boss, List<Room> allRooms) SpawnRooms(Room start, int n)
    {
        var simpleRooms = new List<Room>();
        var shop = new Room(n);
        while (shop.Equals(start))
            shop = new Room(n);

        var treasure = new Room(n);
        while (treasure.Equals(start) || treasure.Equals(shop) || treasure.IsNeighboring(shop))
            treasure = new Room(n);

        var boss = new Room(n);
        while (boss.Equals(start) || boss.Equals(shop) || boss.Equals(treasure) ||
               boss.IsNear(start) || boss.IsNear(shop) || boss.IsNear(treasure))
            boss = new Room(n);

        var fake = new Room(n);
        while (fake.Equals(start) || fake.Equals(shop) || fake.Equals(treasure) || fake.Equals(boss) ||
               fake.IsNear(start) || fake.IsNear(shop) || fake.IsNear(treasure) || fake.IsNear(boss))
            fake = new Room(n);
        simpleRooms.AddRange(GetPath(start, fake));
        simpleRooms.AddRange(GetPath(start, boss));
        simpleRooms.AddRange(GetPath(start, shop));
        simpleRooms.AddRange(GetPath(start, treasure));
        simpleRooms.Add(fake);
        simpleRooms.Add(boss);
        simpleRooms.Add(shop);
        simpleRooms.Add(treasure);
        return (shop, treasure, boss, simpleRooms.Distinct().ToList());
    }

    private IEnumerable<Room> GetPath(Room r1, Room r2)
    {
        var result = new List<Room>();
        var dx = r1.X - r2.X;
        var dy = r1.Y - r2.Y;
        var k = Random.Range(0, 1);

        if ((dy != 0 && k == 0) || dx == 0)
        {
            switch (dy)
            {
                case < 0:
                    result.AddRange(AddRoomAndFindPath(r1, r2, 0, 1));
                    break;
                case > 0:
                    result.AddRange(AddRoomAndFindPath(r1, r2, 0, -1));
                    break;
            }
        }

        if ((dx != 0 && k == 1) || dy == 0)
        {
            switch (dx)
            {
                case < 0:
                    result.AddRange(AddRoomAndFindPath(r1, r2, 1, 0));
                    break;
                case > 0:
                    result.AddRange(AddRoomAndFindPath(r1, r2, -1, 0));
                    break;
            }
        }

        return result;
    }

    private IEnumerable<Room> AddRoomAndFindPath(Room r1, Room r2, int dx, int dy)
    {
        var result = new List<Room>();
        var room = new Room(r1.X + dx, r1.Y + dy);
        result.Add(room);
        result.AddRange(GetPath(room, r2));
        return result;
    }
}

public class BigFloor : Floor
{
}