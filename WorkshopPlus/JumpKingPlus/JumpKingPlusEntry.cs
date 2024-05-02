using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EntityComponent;
using JumpKing;
using JumpKing.Level;
using JumpKing.Mods;
using JumpKing.Particles;
using JumpKing.Player;
using JumpKing.XnaWrappers;
using JumpKingPlus.BlockBehaviours;
using JumpKingPlus.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using static JumpKing.JKContentManager.Audio;

namespace JumpKingPlus
{
    [JumpKingMod("Phoenixx19.JumpKingPlus")]
    public class JumpKingPlusEntry
    {
        public static JKSound WarpTransition { get; private set; }

        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
#if DEBUG
            Debugger.Launch();
#endif
            // Register the block factory with the level manager, this way the 
            // level manager will know how to make our blocks when encountered in the level loading
            // code
            LevelManager.RegisterBlockFactory(new JumpKingPlusBlockFactory());
        }

        [OnLevelStart]
        public static void OnLevelStart()
        {
            // moved from BeforeLevelLoad() seems to fix whatever audio gibberish happened in here:
            // https://cdn.discordapp.com/attachments/1006113863248379996/1209132510148165692/20240219-1340-42.7501289.mp4
            string dllLocation = Path.GetDirectoryName(Assembly.GetAssembly(typeof(JumpKingPlusEntry)).Location);
            WarpTransition = new JKSound(Game1.instance.contentManager.Load<SoundEffect>($@"{dllLocation}\Content\warp_transition"), SoundType.SFX);
            WarpTransition.Volume = Player.JUMP_VOLUME * 0.75f;

            PlayerEntity player = EntityManager.instance.Find<PlayerEntity>();
            if (player != null)
            {
                player.m_body.RegisterBlockBehaviour<ThinSnowBlock>(new ThinSnowBlockBehaviour(player));
                player.m_body.RegisterBlockBehaviour<WarpBlock>(new WarpBlockBehaviour());
                player.m_body.RegisterBlockBehaviour<LegacyWarpBlock>(new LegacyWarpBlockBehaviour());
                player.m_body.RegisterBlockBehaviour<OneWayBlock>(new OneWayBlockBehaviour());
                player.m_body.RegisterBlockBehaviour<LowGravityBlock>(new LowGravityBlockBehaviour());

                // Register SFX for Thin Snow
                player.RegisterJumpSound<ThinSnowBlock>(Game1.instance.contentManager.audio.player.SnowJump);
                player.RegisterLandSound<ThinSnowBlock>(Game1.instance.contentManager.audio.player.SnowLand);
                player.RegisterFailSound<ThinSnowBlock>(Game1.instance.contentManager.audio.player.SnowSplat);

                // Register particle effects for Thin Snow
                // Not actually in the original JK+ so excluded here, but this is 
                // what it might look like
                /*
                player.RegisterLandParticleSpawningAction<ThinSnowBlock>(() =>
                {
                    Rectangle hitbox = player.m_body.GetHitbox();
                    int x = hitbox.Center.X;
                    int y = hitbox.Bottom;
                    SnowParticleEntity.ParticleSpawner.ForceCreateParticle(x, y, spriteIndex: 0);
                });
                */
            }
        }
    }
}
