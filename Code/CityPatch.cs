using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using ReflectionUtility;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json;
using ai.behaviours;

namespace CulturalCityNames
{
    public class CityTitle
    {
        public string ending { get; set; }
        public string type { get; set; }
        public string extension { get; set; }
    }

    [HarmonyPatch(typeof(City), "generateName")]
    internal class CityPatch
    {
        static bool Prefix(City __instance)
        {
            string infoMessage = "<INFO>Cultural City Names: ";

            var kingdom = (Kingdom)Reflection.GetField(__instance.GetType(), __instance, "kingdom");
            string kingdomName = kingdom.name.ToString();

            string cultureName;
            try
            {
                Culture culture = kingdom.getCulture();
                cultureName = culture.data.name.ToString();
            }
            catch
            {
                Debug.Log(infoMessage + "No culture found for new city; must be a capital?");
                cultureName = "NONE";
            }

            string raceTemplateName;
            try
            {
                Race race = (Race)Reflection.GetField(__instance.GetType(), __instance, "race");
                raceTemplateName = race.name_template_city.ToString();
            }
            catch
            {
                Debug.Log(infoMessage + "No race found for new city!");
                raceTemplateName = "NONE";
            }

            string jsonPath = "./names.json"; // Assuming the JSON file is in the same directory as the mod's DLL.
            string json;

            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(jsonPath))
                {
                    json = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error reading names.json: " + e.Message);
                return false; // Return false to prevent mod from changing city name if JSON cannot be read.
            }

            var cityTitles = JsonConvert.DeserializeObject<List<CityTitle>>(json);

            string suffix = "";
            string prefix = "";

            foreach (var cityTitle in cityTitles)
            {
                if (cultureName.EndsWith(cityTitle.ending))
                {
                    if (cityTitle.type == "prefix")
                    {
                        prefix = cityTitle.extension;
                    }
                    else
                    {
                        suffix = cityTitle.extension;
                    }
                }
            }

            __instance.data.name = prefix + NameGenerator.getName(raceTemplateName, ActorGender.Male) + suffix;
            return false;
        }
    }
}