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
using JumpKing.SaveThread;
using JumpKing.GameManager;
using JumpKingSaveStates.Nodes;

namespace JumpKingSaveStates.Models
{
    class MenuOptions
    {
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

        internal static MenuSelector CreateTeleportList(GuiFormat p_format)
        {
            MenuSelector menuSelector = new MenuSelector(p_format);

            if (Game1.instance.contentManager.root == "Content")
            {
                if (EventFlagsSave.ContainsFlag(StoryEventFlags.StartedGhost))
                {
                    menuSelector.AddChild(new TextButton(language.LOCATION_PHILOSOPHERS_FOREST, new TeleportToLocationNode(16, -55714)));
                    menuSelector.AddChild(new TextButton("The Hole", new TeleportToLocationNode(32, -58434)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_BOG, new TeleportToLocationNode(79, -36074)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_MOULDING_MANOR, new TeleportToLocationNode(183, -38602)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_BUGSTALK, new TeleportToLocationNode(163, -41442)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_HOUSE_OF_NINE_LIVES, new TeleportToLocationNode(329, -43970)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_THE_PHANTOM_TOWER, new TeleportToLocationNode(333, -46530)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_HALTED_RUIN, new TeleportToLocationNode(204, -49794)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_THE_TOWER_OF_ANTUMBRA, new TeleportToLocationNode(292, -52626)));
                    menuSelector.AddChild(new TextButton("Ghost of the Babe's Screen", new TeleportToLocationNode(302, -54770)));
                }
                else if (EventFlagsSave.ContainsFlag(StoryEventFlags.StartedNBP))
                {
                    menuSelector.AddChild(new TextButton("Room of the Imp", new TeleportToLocationNode(171, -15570)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_BRIGHTCROWN_WOODS, new TeleportToLocationNode(377, -16274)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_COLOSSAL_DUNGEON, new TeleportToLocationNode(295, -18426)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_FALSE_KINGS_CASTLE, new TeleportToLocationNode(105, -20922)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_UNDERBURG, new TeleportToLocationNode(415, -22362)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_LOST_FRONTIER, new TeleportToLocationNode(242, -24898)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_HIDDEN_KINGDOM, new TeleportToLocationNode(360, -27402)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_BLACK_SANCTUM, new TeleportToLocationNode(210, -29578)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_DEEP_RUIN, new TeleportToLocationNode(170, -31786)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_THE_DARK_TOWER, new TeleportToLocationNode(288, -33554)));
                    menuSelector.AddChild(new TextButton("New Babe Plus' Screen", new TeleportToLocationNode(140, -35314)));
                }
                else
                {
                    menuSelector.AddChild(new TextButton(language.LOCATION_REDCROWN_WOODS, new TeleportToLocationNode(231, 302)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_COLOSSAL_DRAIN, new TeleportToLocationNode(251, -1498)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_FALSE_KINGS_KEEP, new TeleportToLocationNode(340, -3282)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_BARGAINBURG, new TeleportToLocationNode(150, -4738)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_GREAT_FRONTIER, new TeleportToLocationNode(222, -6594)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_WINDSWEPT_BLUFF, new TeleportToLocationNode(216, -8714)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_STORMWALL_PASS, new TeleportToLocationNode(223, -9074)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_CHAPEL_PERILOUS, new TeleportToLocationNode(426, -11202)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_BLUE_RUIN, new TeleportToLocationNode(410, -12658)));
                    menuSelector.AddChild(new TextButton(language.LOCATION_THE_TOWER, new TeleportToLocationNode(435, -13722)));
                    menuSelector.AddChild(new TextButton("Main Babe's Screen", new TeleportToLocationNode(150, -14802)));
                }
            }
            else
            {
                menuSelector.AddChild(new DoubleTextInfoButton(
                    new TextInfo("Work in progress!", Color.Gray),
                    new TextInfo("Custom teleports available in the future", Color.Gray, Game1.instance.contentManager.font.LocationFont),
                    new StaticNodeSimple(BTresult.Failure)
                ));
            }

            //MenuFactoryDrawables.Add(menuSelector);
            menuSelector.Initialize(true);
            return menuSelector;
        }

        internal static BTsimultaneous CreateSaveStatesBindControls(object __instance)
        {
            var _this = Traverse.Create(__instance);
            _drawables = _this.Field("m_drawables");

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
            btsequencor.AddChild(new CustomBindDefault(_entity));
            btsequencor.AddChild(new SetBBKeyNode<bool>(_entity, "BBKEY_UNSAVED_CHANGED", true));
            menuSelector.AddChild<TextButton>(new TextButton(language.MENUFACTORY_INPUT_DEFAULT, btsequencor, menuFontSmall));
            
            BTsequencor btsequencor2 = new BTsequencor();
            btsequencor2.AddChild(new CustomSaveBind(_entity));
            btsequencor2.AddChild(new SetBBKeyNode<bool>(_entity, "BBKEY_UNSAVED_CHANGED", true));
            menuSelector.AddChild<SaveNotifier>(new SaveNotifier(_entity, new TextButton(language.MENUFACTORY_SAVE, btsequencor2, menuFontSmall)));
            
            //BTsequencor btsequencor3 = new BTsequencor();
            //btsequencor3.AddChild(new LoadBind(_entity));
            //btsequencor3.AddChild(new SetBBKeyNode<bool>(_entity, "BBKEY_UNSAVED_CHANGED", true));
            //menuSelector.AddChild<SaveNotifier>(new SaveNotifier(_entity, new TextButton(language.MENUFACTORY_LOAD, btsequencor3, menuFontSmall)));
            
            menuSelector.Initialize(true);
            menuSelector.GetBounds();

            // right
            DisplayFrame displayFrame = new DisplayFrame(gui_right, BTresult.Running);
            displayFrame.AddChild<CustomBindDisplay>(new CustomBindDisplay(_entity, EBinding.SavePos));
            displayFrame.AddChild<CustomBindDisplay>(new CustomBindDisplay(_entity, EBinding.LoadPos));
            displayFrame.AddChild<CustomBindDisplay>(new CustomBindDisplay(_entity, EBinding.DeletePos));
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Down));
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Left));
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Right));
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Jump));
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Confirm));
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Cancel));
            //displayFrame.AddChild<DisplayBinding>(new DisplayBinding(_entity, JKpadButtons.Pause));
            displayFrame.Initialize();

            var drawables = MenuFactoryDrawables;
            drawables.Insert(count, displayFrame);
            MenuFactoryDrawables = drawables;
            
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

            // could be improved
            BindCatchSave p_child = new BindCatchSave(entity);
            CustomBindDefault p_child2 = new CustomBindDefault(entity);
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
            btsequencor2.AddChild(MakeBindButtonMenu(EBinding.SavePos, p_format, p_order_index, entity));
            btsequencor2.AddChild(MakeBindButtonMenu(EBinding.LoadPos, p_format, p_order_index, entity));
            btsequencor2.AddChild(MakeBindButtonMenu(EBinding.DeletePos, p_format, p_order_index, entity));
            btsequencor2.AddChild(new WaitUntilNoInput(entity));
            btsequencor2.AddChild(menuSelector);

            BTselector btselector = new BTselector(new IBTnode[0]);
            btselector.AddChild(btsequencor2);
            btselector.AddChild(new PlaySFX(Game1.instance.contentManager.audio.menu.MenuFail));
            return btselector;
        }

        private static BindButtonFrame MakeBindButtonMenu(EBinding p_button, GuiFormat p_format, int p_order_index, Entity m_entity)
        {
            BTsequencor btsequencor = new BTsequencor();
            btsequencor.AddChild(new WaitUntilNoInput(m_entity));
            btsequencor.AddChild(new CustomBindButton(m_entity, p_button, p_order_index));
            btsequencor.AddChild(new SetBBKeyNode<bool>(m_entity, "BBKEY_UNSAVED_CHANGED", true));
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
