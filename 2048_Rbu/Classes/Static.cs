using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2048_Rbu.Elements.Indicators;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Readers;

namespace _2048_Rbu.Classes
{
    public static class Static
    {
        public enum ContainerItem
        {
            Additive1 = 1,
            Additive2,
            Silo1,
            Silo2,
            Water,
            Bunker1,
            Bunker2,
            Bunker3,
            Bunker4
        }

        public static string NumMech { get; set; } = "gi_NumMech";
        public static bool Link { get; set; }
        public static string Version { get; set; }
        public static string Copyright { get; set; }

        public static Dictionary<ContainerItem, string> СontainerNameDictionary = new Dictionary<ContainerItem, string>
        {
            {ContainerItem.Additive1, "Емкость Х/Д 1"},
            {ContainerItem.Additive2, "Емкость Х/Д 2"},
            {ContainerItem.Silo1, "Силос 1"},
            {ContainerItem.Silo2, "Силос 2"},
            {ContainerItem.Water, "Водопровод"},
            {ContainerItem.Bunker1, "Бункер 1"},
            {ContainerItem.Bunker2, "Бункер 2"},
            {ContainerItem.Bunker3, "Бункер 3"},
            {ContainerItem.Bunker4, "Бункер 4"}
        };

        public static Dictionary<long, ContainerItem> IdСontainerDictionary = new Dictionary<long, ContainerItem>
        {
            {8,ContainerItem.Additive1},
            {9,ContainerItem.Additive2},
            {5,ContainerItem.Silo1},
            {6,ContainerItem.Silo2},
            {7,ContainerItem.Water},
            {1,ContainerItem.Bunker1},
            {2,ContainerItem.Bunker2},
            {3,ContainerItem.Bunker3},
            {4,ContainerItem.Bunker4}
        };

        public static Dictionary<ContainerItem, string> СontainerMaterialDictionary = new Dictionary<ContainerItem, string>
        {
            {ContainerItem.Additive1, ""},
            {ContainerItem.Additive2, ""},
            {ContainerItem.Silo1, ""},
            {ContainerItem.Silo2, ""},
            {ContainerItem.Water, ""},
            {ContainerItem.Bunker1, ""},
            {ContainerItem.Bunker2, ""},
            {ContainerItem.Bunker3, ""},
            {ContainerItem.Bunker4, ""}
        }; //присовение в ElContainer (нужно будет переделать) 
    }
}
