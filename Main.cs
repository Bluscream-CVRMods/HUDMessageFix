using ABI_RC.Core.Networking.Guardian;
using HarmonyLib;
using HUDMessageFix;
using MelonLoader;
using System;

[assembly: MelonInfo(typeof(HUDMessageFix.Main), Guh.Name, Guh.Version, Guh.Author, Guh.DownloadLink)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
namespace HUDMessageFix;
public static class Guh {
    public const string Name = "HUDMessageFix";
    public const string Author = "Bluscream";
    public const string Version = "1.0.0";
    public const string DownloadLink = "";
}
public static class Patches {
    public static string lasthudmsg = "";
    public static void Init(HarmonyLib.Harmony harmonyInstance) {
        Patch(harmonyInstance, "ViewDropTextImmediate");
        Patch(harmonyInstance, "ViewDropTextLong");
        Patch(harmonyInstance, "ViewDropTextLonger");
        PatchGuardian(harmonyInstance, "ViewGuardianDropText");
        //Patch(harmonyInstance, "ViewDropText", new Type[] { typeof(string), typeof(string), typeof(string) });
        //Patch(harmonyInstance, "ViewDropText", new Type[] { typeof(string), typeof(string) });
        MelonLogger.Msg("Harmony patches completed!");
    }
    public static void Patch(HarmonyLib.Harmony harmonyInstance, string methodName, Type[] types = null) {
        MelonLogger.Msg("Patching {0}", methodName);
        _ = types != null
            ? harmonyInstance.Patch(typeof(ABI_RC.Core.UI.CohtmlHud).GetMethod(methodName), prefix: new HarmonyMethod(typeof(Patches).GetMethod(methodName, types)))
            : harmonyInstance.Patch(typeof(ABI_RC.Core.UI.CohtmlHud).GetMethod(methodName), prefix: new HarmonyMethod(typeof(Patches).GetMethod(methodName)));
    }
    public static void PatchGuardian(HarmonyLib.Harmony harmonyInstance, string methodName, Type[] types = null) {
        MelonLogger.Msg("Patching {0}", methodName);
        _ = types != null
            ? harmonyInstance.Patch(typeof(GuardianMessaging).GetMethod(methodName), prefix: new HarmonyMethod(typeof(Patches).GetMethod(methodName, types)))
            : harmonyInstance.Patch(typeof(GuardianMessaging).GetMethod(methodName), prefix: new HarmonyMethod(typeof(Patches).GetMethod(methodName)));
    }

    public static bool ViewGuardianDropText(string cat = "", string headline = "", string small = "") {
        if ((bool)Main.LogHudMessagesSetting.BoxedValue) {
            MelonLogger.Msg("ViewGuardianDropText;{0};{1};{2}", cat, headline.Replace(";", ""), small.Replace(";", ""));
        }

        if ((bool)Main.IgnoreDuplicateMessagesSetting.BoxedValue && headline == lasthudmsg) {
            return false;
        }

        lasthudmsg = headline;
        return true;
    }
    public static bool ViewDropTextCaller(string method = "", string headline = "", string small = "", string cat = "") {
        if ((bool)Main.LogHudMessagesSetting.BoxedValue) {
            MelonLogger.Msg("{0};{1};{2};{3}", method, cat, headline.Replace(";", ""), small.Replace(";", ""));
        }

        if ((bool)Main.IgnoreDuplicateMessagesSetting.BoxedValue && headline == lasthudmsg) {
            return false;
        }

        lasthudmsg = headline;
        return true;
    }
    public static bool ViewDropTextImmediate(string cat = "", string headline = "", string small = "") {
        return ViewDropTextCaller("ViewDropTextImmediate", headline, small, cat);
    }

    public static bool ViewDropText(string headline = "", string small = "") {
        return ViewDropTextCaller("ViewDropText", headline, small);
    }

    public static bool ViewDropText(string cat = "", string headline = "", string small = "") {
        return ViewDropTextCaller("ViewDropText", headline, small, cat);
    }

    public static bool ViewDropTextLong(string cat = "", string headline = "", string small = "") {
        return ViewDropTextCaller("ViewDropTextLong", headline, small, cat);
    }

    public static bool ViewDropTextLonger(string cat = "", string headline = "", string small = "") {
        return ViewDropTextCaller("ViewDropTextLonger", headline, small, cat);
    }
}

public class Main : MelonMod {
    public static MelonPreferences_Entry LogHudMessagesSetting, IgnoreDuplicateMessagesSetting;

    public override void OnApplicationStart() {
        MelonPreferences_Category cat = MelonPreferences.CreateCategory(Guh.Name);
        IgnoreDuplicateMessagesSetting = cat.CreateEntry<bool>("Ignore Duplicates", true, null, "Ignore Duplicate HUD Messages");
        LogHudMessagesSetting = cat.CreateEntry<bool>("Log HUD Messages", false);

        Patches.Init(HarmonyInstance);
    }
}