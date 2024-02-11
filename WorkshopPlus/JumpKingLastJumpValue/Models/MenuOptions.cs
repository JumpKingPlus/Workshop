using BehaviorTree;
using HarmonyLib;
using JumpKing.PauseMenu.BT;
using JumpKing.PauseMenu;
using JumpKingLastJumpValue.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDrawable = JumpKing.Util.IDrawable;
using static JumpKing.Timer;

namespace JumpKingLastJumpValue.Models
{
    class MenuOptions
    {
        public MenuOptions(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method("JumpKing.PauseMenu.MenuFactory:CreateIngameOptions"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(MenuOptions), nameof(Menu)))
            );
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
        static void Menu(GuiFormat p_format, GuiFormat p_sub_format, ref MenuSelector __result, object __instance)
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

            menuSelector.AddChild(new ToggleLastJumpValue());
            menuSelector.AddChild(new LastJumpValueOption());
            menuSelector.Initialize(true);
            return menuSelector;
        }
    }
}
