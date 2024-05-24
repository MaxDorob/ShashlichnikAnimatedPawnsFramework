using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shashlichnik
{
    [Verse.StaticConstructorOnStartup()]
    public static class StartUp
    {
        static StartUp()
        {
            new Harmony("Shashlichnik.AnimatedPawnsFramework").PatchAll();
        }
    }
}
