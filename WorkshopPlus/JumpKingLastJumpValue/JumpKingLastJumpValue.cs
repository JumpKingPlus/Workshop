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
using IDrawable = JumpKing.Util.IDrawable;
using static JumpKing.Timer;
using JumpKingLastJumpValue.Menu;

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

        private static Traverse _drawables;
        public static List<IDrawable> MenuFactoryDrawables
        {
            get => _drawables.GetValue<List<IDrawable>>();
            set => _drawables.SetValue(value);
        }

        /// <summary>
        /// Remove return button that most <see cref="MenuSelector"/> adds before returning.
        /// </summary>
        /// <param name="__result">Reference to MenuSelector returned.</param>
        private static void RemoveLastChild(ref MenuSelector __result)
        {
            Traverse fieldReference = Traverse.Create(__result).Field("m_children");

            // get
            List<IBTnode> children = fieldReference.GetValue<IBTnode[]>().ToList();

            // remove last child
            children[children.Count - 1].OnDispose();
            children.RemoveAt(children.Count - 1);

            // set
            fieldReference.SetValue(children.ToArray());
        }


        /// <summary>
        /// Patches menu to add additional option(s).
        /// </summary>
        public static void Menu(GuiFormat p_format, GuiFormat p_sub_format, ref MenuSelector __result, object __instance)
        {
            _drawables = Traverse.Create(__instance).Field("m_drawables");

            // remove return button
            if (__result.Children[__result.Children.Length - 1].GetType() == typeof(IconButton))
            {
                RemoveLastChild(ref __result);
            }

            __result.AddChild(new TextButton("Jump% Options", CreateJumpOptions(p_sub_format)));
            __result.Initialize(true);
        }


        /// <summary>
        /// Creates options menu for Jump%
        /// </summary>
        /// <param name="p_format"></param>
        /// <returns></returns>
        private static MenuSelector CreateJumpOptions(GuiFormat p_format)
        {
            MenuSelector menuSelector = new MenuSelector(p_format);

            List<JumpKing.Util.IDrawable> drawable = MenuFactoryDrawables;
            drawable.Add(menuSelector);
            MenuFactoryDrawables = drawable;

            //menuSelector.AddChild(new TextInfo("got em 2", Color.Fuchsia));
            menuSelector.AddChild(new ToggleLastJumpValue());
            menuSelector.AddChild(new LastJumpValueOption());
            menuSelector.Initialize(true);
            return menuSelector;
        }

        public static int JumpFrames { get; internal set; }
        public static float JumpPercentage { get; internal set; }

        public static bool IsEnabled { get; internal set; } = true;
        public static ELastJumpDisplayType DisplayType { get; internal set; }
    }
}
