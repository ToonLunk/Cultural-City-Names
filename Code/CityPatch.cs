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
        public string ending { get; set;}
        public string type { get; set; }
        public string extension { get; set; }
    }

    [HarmonyPatch(typeof(City), "generateName")]
    internal class CityPatch
    {
        static bool Prefix(City __instance)
        {
            string infoMessage = "<INFO>Cultural City Names: ";

            // don't have to use try block here because a new city always has a kingdom
            var kingdom = (Kingdom)Reflection.GetField(__instance.GetType(), __instance, "kingdom");
            string kingdomName = kingdom.name.ToString();

            // have to declare outside then use a try block because sometimes a city is created without a culture (such as in a new world)
            string cultureName;
            try 
            {
                Culture culture = kingdom.getCulture();
                cultureName = culture.data.name.ToString();
            } catch
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

            //Debug.Log(string.Concat(new string[]
            //{
            //    infoMessage,
            //    "New village created in kingdom ",
            //    kingdomName,
            //    " with culture ",
            //    cultureName
            //}));

            string json = File.ReadAllText("./Mods/Cultural City Names/names.json");
            var cityTitles = JsonConvert.DeserializeObject<List<CityTitle>>(json);

            string suffix = "";
            string prefix = "";

            foreach(var cityTitle in cityTitles)
            {
                if (cultureName.EndsWith(cityTitle.ending))
                {
                    if (cityTitle.type == "prefix")
                    {
                        prefix = cityTitle.extension;
                    } else
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