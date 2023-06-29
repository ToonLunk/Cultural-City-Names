using System.IO.Pipes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NCMS;
using HarmonyLib;
using UnityEngine;


namespace CulturalCityNames
{
    [ModEntry]
    class Main : MonoBehaviour
    {
        const string ModName = "Cultural City Mod";
        void Awake()
        {
            PropertyInfo[] properties = typeof(City).GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                Debug.Log(prop);
            }
            Debug.Log($"Hello world from {ModName} by Toon Lunk!");
            Harmony harmony = new Harmony(ModName);
            harmony.PatchAll();
        }
    }
}