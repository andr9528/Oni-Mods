using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Abstractions;
using UtilLibs;

namespace Utilities.Services
{
    public static class PatchService
    {
        private static ICollection<IPatchMe> GetImplementingClasses()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.GetInterfaces().Contains(typeof(IPatchMe))).Where(x => x.IsClass).Select(x =>
                    (IPatchMe) Activator.CreateInstance(x)).ToList();
        }

        public static void AddBuildingsToPlanScreen()
        {
            foreach (IPatchMe patchMe in GetImplementingClasses())
                InjectionMethods.AddBuildingToPlanScreenBehindNext(patchMe.GetPlanMenuCategory(), patchMe.GetId());
        }

        public static void AddBuildingToTechnology()
        {
            foreach (IPatchMe patchMe in GetImplementingClasses())
                InjectionMethods.AddBuildingToTechnology(patchMe.GetTechnology(), patchMe.GetId());
        }
    }
}