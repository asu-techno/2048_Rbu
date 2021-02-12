using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using _2048_Rbu.Elements.Indicators;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Readers;
using Newtonsoft.Json;

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

        public static Dictionary<ContainerItem, string> СontainerNameDictionary = GetСontainerNameDictionary();
        private static Dictionary<ContainerItem, string> GetСontainerNameDictionary()
        {
            try
            {
                if (false)
                {
                    return new Dictionary<ContainerItem, string>
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
                }
                else
                {
                    using StreamReader r = new StreamReader("Data\\ContainerDictionary\\СontainerName.json", Encoding.GetEncoding("windows-1251"));
                    string json = r.ReadToEnd();
                    return JsonConvert.DeserializeObject<Dictionary<ContainerItem, string>>(json);
                }
            }
            catch (Exception ex)
            {
                Service.GetInstance().GetLogger().Error(ex);
                MessageBox.Show("Проблемы с парсингом СontainerName.json.");
                return new Dictionary<ContainerItem, string>
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
            }
        }

        public static Dictionary<ContainerItem,long> IdСontainerDictionary = GetIdСontainerDictionary();
        private static Dictionary<ContainerItem, long> GetIdСontainerDictionary()
        {
            try
            {
                using StreamReader r = new StreamReader("Data\\ContainerDictionary\\IdСontainer.json");
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<Dictionary<ContainerItem, long>>(json);
            }
            catch (Exception ex)
            {
                Service.GetInstance().GetLogger().Error(ex);
                MessageBox.Show("Проблемы с парсингом IdСontainer.json.");
                return new Dictionary<ContainerItem, long>
                {
                    {ContainerItem.Additive1,8},
                    {ContainerItem.Additive2,9},
                    {ContainerItem.Silo1,5},
                    {ContainerItem.Silo2,6},
                    {ContainerItem.Water,7},
                    {ContainerItem.Bunker1,1},
                    {ContainerItem.Bunker2,2},
                    {ContainerItem.Bunker3,3},
                    {ContainerItem.Bunker4,4}
                };
            }
        }

        public static Dictionary<ContainerItem, string> СontainerMaterialDictionary = GetСontainerMaterialDictionary();
        private static Dictionary<ContainerItem, string> GetСontainerMaterialDictionary()
        {
            try
            {
                if (false)
                {
                    return new Dictionary<ContainerItem, string>
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
                    };
                }
                else
                {
                    using StreamReader r = new StreamReader("Data\\ContainerDictionary\\СontainerMaterial.json");
                    string json = r.ReadToEnd();
                    return JsonConvert.DeserializeObject<Dictionary<ContainerItem, string>>(json);
                }
            }
            catch (Exception ex)
            {
                Service.GetInstance().GetLogger().Error(ex);
                MessageBox.Show("Проблемы с парсингом СontainerMaterial.json.");
                return new Dictionary<ContainerItem, string>
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
                };//присовение в ElContainer (нужно будет переделать) 
            }
        }
    }
}
