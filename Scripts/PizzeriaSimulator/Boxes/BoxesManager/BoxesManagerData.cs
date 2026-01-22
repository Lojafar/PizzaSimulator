using System;
using System.Collections.Generic;
namespace Game.PizzeriaSimulator.Boxes.Manager
{
    [Serializable]
    class BoxesManagerData
    {
        public readonly List<uint> ActiveIDs = new();
    }
}
