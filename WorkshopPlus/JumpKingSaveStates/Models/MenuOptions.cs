using BehaviorTree;
using HarmonyLib;
using JumpKing.PauseMenu.BT;
using JumpKing.PauseMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDrawable = JumpKing.Util.IDrawable;
using System.Threading.Tasks;
using EntityComponent.BT;
using JumpKing.Controller;
using JumpKing.MiscEntities.WorldItems.Inventory;
using JumpKing.MiscEntities.WorldItems;
using JumpKing.PauseMenu.BT.Actions.BindController;
using JumpKing;
using Microsoft.Xna.Framework.Graphics;
using LanguageJK;
using EntityComponent;
using JumpKing.Util;
using JumpKingSaveStates.Menu;
using JumpKing.PauseMenu.BT.Actions;
using Microsoft.Xna.Framework;

namespace JumpKingSaveStates.Models
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

            __result.AddChild(new TextButton("Savestates Bind Options", CreateSaveStatesBindControls(__instance)));
            __result.Initialize(true);
        }

        private static BTsimultaneous CreateSaveStatesBindControls(object __instance)
        {
            var _this = Traverse.Create(__instance);
            var _entity = _this.Field("m_entity").GetValue<Entity>();
            var gui_left = _this.Field("CONTROLS_GUI_FORMAT_LEFT").GetValue<GuiFormat>();
            var gui_right = _this.Field("CONTROLS_GUI_FORMAT_RIGHT").GetValue<GuiFormat>();

            MenuSelector menuSelector = new MenuSelector(gui_left);

            BTsimultaneous btsimultaneous = new BTsimultaneous(new IBTnode[0]);
            btsimultaneous.AddChild(menuSelector);
            MenuFactoryDrawables.Add(menuSelector);

            // left
            SpriteFont menuFontSmall = Game1.instance.contentManager.font.MenuFontSmall;
            menuSelector.AddChild<TextButton>(new TextButton(language.MENUFACTORY_INPUT_SCAN_FOR_DEVICES, new GetSlimDevices(), menuFontSmall));
            menuSelector.AddChild<SelectDevice>(new SelectDevice(_entity));
            int count = MenuFactoryDrawables.Count;
            IBTnode p_child = MakeBindController(0, _entity);
            IBTnode p_child2 = MakeBindController(1, _entity);
            menuSelector.AddChild<TextButton>(new TextButton(language.MENUFACTORY_INPUT_BIND_PRIMARY, p_child, menuFontSmall));
            menuSelector.AddChild<TextButton>(new TextButton(language.MENUFACTORY_INPUT_BIND_SECONDARY, p_child2, menuFontSmall));
            
            BTsequencor btsequencor = new BTsequencor();
            btsequencor.AddChild(new BindDefault(_entity));
            btsequencor.AddChild(new SetBBKeyNode<bool>(_entity, "BBKEY_UNSAVED_CHANGED", true));
            menuSelector.AddChild<TextButton>(new TextButton(language.MENUFACTORY_INPUT_DEFAULT, btsequencor, menuFontSmall));
            
            BTsequencor btsequencor2 = new BTsequencor();
            btsequencor2.AddChild(new SaveBind(_entity));
            btsequencor2.AddChild(new SetBBKeyNode<bool>(_entity, "BBKEY_UNSAVED_CHANGED", true));
            menuSelector.AddChild<SaveNotifier>(new SaveNotifier(_entity, new TextButton(language.MENUFACTORY_SAVE, btsequencor2, menuFontSmall)));
            
            BTsequencor btsequencor3 = new BTsequencor();
            btsequencor3.AddChild(new LoadBind(_entity));
            btsequencor3.AddChild(new SetBBKeyNode<bool>(_entity, "BBKEY_UNSAVED_CHANGED", true));
            menuSelector.AddChild<SaveNotifier>(new SaveNotifier(_entity, new TextButton(language.MENUFACTORY_LOAD, btsequencor3, menuFontSmall)));
            menuSelector.Initialize(true);
            menuSelector.GetBounds();

            // right
            DisplayFrame displayFrame = new DisplayFrame(gui_right, BTresult.Running);
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Up));
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Down));
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Left));
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Right));
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Jump));
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Confirm));
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Cancel));
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Pause));
            displayFrame.Initialize();
            MenuFactoryDrawables.Insert(count, displayFrame);
            
            
            btsimultaneous.AddChild(new StaticNode(displayFrame, BTresult.Failure));
            return btsimultaneous;
        }

        private static IBTnode MakeBindController(int p_order_index, Entity entity)
        {
            GuiFormat p_format = new GuiFormat
            {
                anchor_bounds = new Rectangle(0, 0, 480, 360),
                anchor = new Vector2(1f, 1f) / 2f,
                all_margin = 16,
                element_margin = 8,
                all_padding = 16
            };
            BindCatchSave p_child = new BindCatchSave(entity);
            BindCatchRevert p_child2 = new BindCatchRevert(entity);
            MenuSelector menuSelector = new MenuSelector(p_format);
            menuSelector.AllowEscape = false;
            MenuSelectorBack p_child3 = new MenuSelectorBack(menuSelector);
            BTsequencor btsequencor = new BTsequencor();
            btsequencor.AddChild(p_child2);
            btsequencor.AddChild(new SetBBKeyNode<bool>(entity, "BBKEY_UNSAVED_CHANGED", true));
            btsequencor.AddChild(p_child3);
            TimerAction timerAction = new TimerAction(language.MENUFACTORY_REVERTS_IN, 5, Color.Gray, btsequencor);
            menuSelector.AddChild<TextInfo>(new TextInfo(language.MENUFACTORY_KEEPCHANGES, Color.Gray));
            menuSelector.AddChild<TimerAction>(timerAction);
            menuSelector.AddChild<TextButton>(new TextButton(language.MENUFACTORY_NO, btsequencor));
            menuSelector.AddChild<TextButton>(new TextButton(language.MENUFACTORY_YES, p_child3));
            menuSelector.SetNodeForceRun(timerAction);
            menuSelector.Initialize(false);

            var drawables = MenuFactoryDrawables;
            drawables.Add(menuSelector);
            MenuFactoryDrawables = drawables;

            BTsequencor btsequencor2 = new BTsequencor();
            btsequencor2.AddChild(p_child);
            btsequencor2.AddChild(new WaitUntilNoMenuInput());
            btsequencor2.AddChild(MakeBindButtonMenu("Save Position", p_format, p_order_index, entity));
            btsequencor2.AddChild(MakeBindButtonMenu("Load Position", p_format, p_order_index, entity));
            btsequencor2.AddChild(new WaitUntilNoInput(entity));
            btsequencor2.AddChild(menuSelector);

            BTselector btselector = new BTselector(new IBTnode[0]);
            btselector.AddChild(btsequencor2);
            btselector.AddChild(new PlaySFX(Game1.instance.contentManager.audio.menu.MenuFail));
            return btselector;
        }

        private static BindButtonFrame MakeBindButtonMenu(string p_button, GuiFormat p_format, int p_order_index, Entity m_entity)
        {
            BTsequencor btsequencor = new BTsequencor();
            btsequencor.AddChild(new WaitUntilNoInput(m_entity));
            btsequencor.AddChild(new CustomBindButton(m_entity, p_button, p_order_index));
            BindButtonFrame bindButtonFrame = new BindButtonFrame(p_format, btsequencor);
            bindButtonFrame.AddChild<TextButton>(new TextButton(Util.ParseString(language.MENUFACTORY_PRESS_BUTTON, new object[]
            {
                p_button
            }), btsequencor));
            bindButtonFrame.Initialize();

            var draw = MenuFactoryDrawables;
            draw.Add(bindButtonFrame);
            MenuFactoryDrawables = draw;

            return bindButtonFrame;
        }
    }
}
