using Assets.Scripts.Models;
using Assets.Scripts.Models.Bloons.Behaviors;
using Assets.Scripts.Models.GenericBehaviors;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Simulation;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Display;
using Assets.Scripts.Unity.UI_New.InGame;
using Assets.Scripts.Unity.UI_New.InGame.Stats;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using CookieClicker;
using HarmonyLib;
using Il2CppSystem;
using JetBrains.Annotations;
using MelonLoader;
using System;
using System.Reflection;
using UnityEngine;
using String = Il2CppSystem.String;

[assembly: MelonInfo(typeof(CookieClicker.CookieClicker), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace CookieClicker;

public class CookieClicker : BloonsTD6Mod
{
    public static float time = 0;
    public static float timer = 0;
    public static int grandma = 0;
    public static int mines = 0;
    public static int factory = 0;
    public static int god = 0;
    public static bool menushop = false;
    public static bool menuinv = false;

    public override void OnApplicationStart()
    {

        //previous two lines are for debugging/finding names of assets

        assetBundle = AssetBundle.LoadFromMemory(ExtractResource("cookie.bundle"));// if using unityexplorer, there is an error, but everything still works
        ModHelper.Msg<CookieClicker>("Cookie loaded!");
    }
    public static void PlaySound(string name)
    {
        Game.instance.audioFactory.PlaySoundFromUnity(null, name, "FX", 1, 1);
    }

    public static AssetBundle assetBundle;


    private byte[] ExtractResource(String filename)
    {
        Assembly a = MelonAssembly.Assembly; // get the assembly
        return a.GetEmbeddedResource(filename).GetByteArray(); // get the embedded bundle as a raw file that unity can read
    }
    public override void OnMatchStart()
    {
        InGame.instance.SetCash(0);
        foreach (var bloon in InGame.instance.bridge.GetAllBloons())
        {
            bloon.GetBaseModel().GetBehavior<DistributeCashModel>().cash = 0f;
        }
        
        base.OnMatchStart();
    }
    public override void OnRoundEnd()
    {
        InGame.instance.AddCash(100);
        base.OnRoundEnd();
    }
    public override void OnUpdate()
    {
        if (InGame.instance != null && InGame.instance.bridge != null)
        {
            time += UnityEngine.Time.deltaTime;
            if (time > 1f)
            {
                time = 0;
                int money = 0;
                money += grandma;
                money += mines * 5;
                money += factory * 25;
                money += god * 1000;
                InGame.instance.AddCash(money);
            }

            if (InGame.instance != null && InGame.instance.bridge != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    InGame.instance.AddCash(1);

                }
            }
        }
        
    }
}
[HarmonyPatch(typeof(Simulation), "AddCash")]
public class NoCash
{
    [HarmonyPrefix]
    public static bool Prefix(ref double c, ref Simulation.CashSource source)
    {
        if (source == Simulation.CashSource.Normal)
        {
            c = 0f;
        }
        return true;
    }
}
[HarmonyPatch(typeof(RoundDisplay), nameof(RoundDisplay.OnUpdate))]
public sealed class Display
{
    [HarmonyPostfix]
    public static void Fix(ref RoundDisplay __instance)
    {
        
        __instance.text.text = $"{__instance.cachedRoundDisp}\n";
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (CookieClicker.timer < 1)
            {
                CookieClicker.timer = 20;
                CookieClicker.menuinv = false;
                switch (CookieClicker.menushop)
                {
                    case true:
                        CookieClicker.menushop = false;
                        break;
                    case false:
                        CookieClicker.menushop = true;
                        break;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.F1))
        {
            if (CookieClicker.timer < 1)
            {
                CookieClicker.timer = 20;
                CookieClicker.menushop = false;
                switch (CookieClicker.menuinv)
                {
                    case true:
                        CookieClicker.menuinv = false;
                        break;
                    case false:
                        CookieClicker.menuinv = true;
                        break;
                }
            }
        }
        else if (CookieClicker.timer > 0)
        {
            CookieClicker.timer -= 1;
        }
        if (CookieClicker.menushop == true)
        {
            if (CookieClicker.timer < 1)
            {
                if (Input.GetKey(KeyCode.F5))
                {
                    if (InGame.instance.GetCash() > 349)
                    {
                        CookieClicker.grandma += 1;
                        InGame.instance.AddCash(-350);
                        CookieClicker.timer = 10;
                    }
                }
                if (Input.GetKey(KeyCode.F6))
                {
                    if (InGame.instance.GetCash() > 1599)
                    {
                        CookieClicker.mines += 1;
                        InGame.instance.AddCash(-1600);
                        CookieClicker.timer = 10;
                    }
                }
                if (Input.GetKey(KeyCode.F7))
                {
                    if (InGame.instance.GetCash() > 6499)
                    {
                        CookieClicker.factory += 1;
                        InGame.instance.AddCash(-6500);
                        CookieClicker.timer = 10;
                    }
                }
                if (Input.GetKey(KeyCode.F8))
                {
                    if (InGame.instance.GetCash() > 99999)
                    {
                        CookieClicker.god += 1;
                        InGame.instance.AddCash(-100000);
                        CookieClicker.timer = 10;
                    }
                }
            }
            __instance.text.text += "F5 | Buy Cookie Grandma (1/cps): Cost 350" + "\n";
            __instance.text.text += "F6 | Buy Cookie Mine (5/cps): Cost 1600" + "\n";
            __instance.text.text += "F7 | Buy Cookie Factory (25/cps): Cost 6500" + "\n";
            __instance.text.text += "F8 | Buy Cookie ??? (???/cps): Cost 100000" + "\n";
            __instance.text.text += "F2 to toggle shop" + "\n";
        }
        else if (CookieClicker.menuinv == true)
        {
            __instance.text.text += "Cookie Grandmas owned: " + CookieClicker.grandma + "\n";
            __instance.text.text += "Cookie Mines owned: " + CookieClicker.mines + "\n";
            __instance.text.text += "Cookie Factorys owned: " + CookieClicker.factory + "\n";
            if (CookieClicker.god != 0)
            {
                __instance.text.text += "Cookie Gods owned: " + CookieClicker.god + "\n";
            }
            else
            {
                __instance.text.text += "Cookie ???s owned: ???" + "\n";
            }
            __instance.text.text += "F1 to toggle inventory" + "\n";
        }
        else
        {
            __instance.text.text += "F1 to toggle inventory" + "\n";
            __instance.text.text += "F2 to toggle shop" + "\n";
        }
    }
}
