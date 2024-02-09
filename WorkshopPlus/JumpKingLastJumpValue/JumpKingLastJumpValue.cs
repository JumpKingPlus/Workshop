using BehaviorTree;
using HarmonyLib;
using JumpKing;
using JumpKing.GameManager;
using JumpKing.Mods;
using JumpKing.PauseMenu.BT;
using JumpKing.PauseMenu;
using JumpKing.Player;
using JumpKing.Util;
using Microsoft.Xna.Framework;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingLastJumpValue
{
    [JumpKingMod(IDENTIFIER)]
    public static class JumpKingLastJumpValue
    {
        const string IDENTIFIER = "Phoenixx19.LastJumpValue";
        const string HARMONY_IDENTIFIER = "Phoenixx19.LastJumpValue.Harmony";

        [BeforeLevelLoad]
        public static void OnLevelStart()
        {
#if DEBUG
            Debugger.Launch();
            Harmony.DEBUG = true;
#endif

            // setup harmony
            var harmony = new Harmony(HARMONY_IDENTIFIER);

            //// get jumpstate myrun method
            //var jumpstate = AccessTools.Method(
            //    "JumpKing.Player.JumpState:MyRun"
            //);
            //var draw = typeof(GameLoop).GetMethod(nameof(GameLoop.Draw));

            //// get method
            //var runMethod = typeof(JumpKingLastJumpValue).GetMethod(nameof(Run));
            //var drawMethod = typeof(JumpKingLastJumpValue).GetMethod(nameof(Draw));

            //// do the patching
            //harmony.Patch(jumpstate, postfix: new HarmonyMethod(runMethod));
            //harmony.Patch(draw, postfix: new HarmonyMethod(drawMethod));
            harmony.Patch(
                AccessTools.Method("JumpKing.PauseMenu.MenuFactory:CreateIngameOptions"),
                postfix: new HarmonyMethod(typeof(JumpKingLastJumpValue).GetMethod(nameof(Menu)))
            );

            harmony.PatchAll();
        }

        public static void Menu(ref MenuSelector __result)
        {
            // remove return button
            if (__result.Children[__result.Children.Length - 1].GetType() == typeof(IconButton))
            {
                var fieldReference = Traverse.Create(__result).Field("m_children");
                
                // get
                List<IBTnode> children = ((IBTnode[])fieldReference.GetValue()).ToList();
                
                // remove last child
                children[children.Count - 1].OnDispose();
                children.RemoveAt(children.Count - 1);
                
                // set
                fieldReference.SetValue(children.ToArray());
            }
            __result.AddChild<TextInfo>(
                new TextInfo("GOT EM!!", Color.Fuchsia)
            );
            __result.Initialize(true);
        }

        public static byte JumpFrames { get; internal set; }
        public static float JumpPercentage { get; internal set; }
    }
}
