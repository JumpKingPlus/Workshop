using HarmonyLib;
using JumpKing;
using JumpKing.GameManager;
using JumpKing.MiscSystems.Achievements;
using JumpKing.Player;
using JumpKing.SaveThread;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpKingMultiplayer.Extensions;
using JumpKingMultiplayer.Models;
using JumpKingMultiplayer.Helpers;

namespace JumpKingMultiplayer.Extensions
{
    static class PlayerSpriteStateExtensions
    {
        public static ulong? GetLevelId()
        {
            if (AccessTools.Property("JumpKing.SaveThread.SaveLube:PlayerStatsAttemptSnapshot").GetValue(null) is PlayerStats stats)
            {
                return stats.steam_level_id;
            }
            return null;
        }

        public static int DifferenceWithPlayer(this Vector2 ghostPosition, BodyComp player)
        {
            return (int)((player.Position.Y * -1) - (ghostPosition.Y * -1));
        }

        public static TrackData FromPlayer(PlayerEntity player)
        {
            var pos = GameLoop.m_player.m_body.Position;
            var tPlayer = Traverse.Create(player);
            var m_flip = tPlayer.Field("m_flip").GetValue<SpriteEffects>();
            var m_sprite = tPlayer.Field("m_sprite").GetValue<Sprite>();
            return new TrackData
            {
                screenIndex1 = Camera.CurrentScreenIndex1,
                posX = pos.X,
                posY = pos.Y,
                colorIdx = ModEntry.Preferences.LobbySettings.PersonalColor,
                flip = m_flip == SpriteEffects.None 
                ? PlayerSpriteEffect.None 
                : m_flip == SpriteEffects.FlipHorizontally 
                    ? PlayerSpriteEffect.FlipH 
                    : PlayerSpriteEffect.FlipV,
                sprite = m_sprite.ToPlayerSpriteState(),
                levelId = GetLevelId(),
            };
        }

        public readonly static Type LayeredSpriteType = AccessTools.TypeByName("JumpKing.XnaWrappers.LayeredSprite");
        
        //public static StrippedSprite ToStrippedSprite(this Sprite sprite)
        //{
        //    if (sprite.GetType() == LayeredSpriteType)
        //    {
        //        var sprites = Traverse.Create(sprite).Property("Sprites").GetValue<List<Sprite>>();
        //        foreach (var spr in sprites)
        //        {

        //        }
        //    }
        //    // todo: most of the times it returns a {JumpKing.XnaWrappers.LayeredSprite}, use this to override color list?
        //    // https://community.monogame.net/t/most-efficient-way-to-combine-textures/16880
        //    return new StrippedSprite(sprite.texture.ToTextureRaw(sprite.source), sprite.source);
        //}

        //public static Sprite ToSprite(this StrippedSprite strip)
        //{
        //    return Sprite.CreateSprite(strip.TextureRaw.ToTexture2D(strip.Source));
        //}

        public static Sprite ToSprite(this PlayerSpriteState state)
        {
            switch (state)
            {
                case PlayerSpriteState.Idle: return Game1.instance.contentManager.playerSprites.idle;
                case PlayerSpriteState.JumpBounce: return Game1.instance.contentManager.playerSprites.jump_bounce;
                case PlayerSpriteState.JumpCharge: return Game1.instance.contentManager.playerSprites.jump_charge;
                case PlayerSpriteState.JumpFall: return Game1.instance.contentManager.playerSprites.jump_fall;
                case PlayerSpriteState.JumpSplat: return Game1.instance.contentManager.playerSprites.splat;
                case PlayerSpriteState.JumpUp: return Game1.instance.contentManager.playerSprites.jump_up;
                case PlayerSpriteState.LookUp: return Game1.instance.contentManager.playerSprites.look_up;
                case PlayerSpriteState.StretchOne: return Game1.instance.contentManager.playerSprites.stretch_one;
                case PlayerSpriteState.StretchSmear: return Game1.instance.contentManager.playerSprites.stretch_smear;
                case PlayerSpriteState.StretchTwo: return Game1.instance.contentManager.playerSprites.stretch_two;
                case PlayerSpriteState.WalkOne: return Game1.instance.contentManager.playerSprites.walk_one;
                case PlayerSpriteState.WalkSmear: return Game1.instance.contentManager.playerSprites.walk_smear;
                case PlayerSpriteState.WalkTwo: return Game1.instance.contentManager.playerSprites.walk_two;
            }
            return Game1.instance.contentManager.playerSprites.idle;
        }

        public static PlayerSpriteState ToPlayerSpriteState(this Sprite sprite)
        {
            // TODO: definitely needs some optimization 
            if (sprite == Game1.instance.contentManager.playerSprites.idle) { return PlayerSpriteState.Idle; }
            else if (sprite == Game1.instance.contentManager.playerSprites.jump_bounce) { return PlayerSpriteState.JumpBounce; }
            else if (sprite == Game1.instance.contentManager.playerSprites.jump_charge) { return PlayerSpriteState.JumpCharge; }
            else if (sprite == Game1.instance.contentManager.playerSprites.jump_fall) { return PlayerSpriteState.JumpFall; }
            else if (sprite == Game1.instance.contentManager.playerSprites.splat) { return PlayerSpriteState.JumpSplat; }
            else if (sprite == Game1.instance.contentManager.playerSprites.jump_up) { return PlayerSpriteState.JumpUp; }
            else if (sprite == Game1.instance.contentManager.playerSprites.look_up) { return PlayerSpriteState.LookUp; }
            else if (sprite == Game1.instance.contentManager.playerSprites.stretch_one) { return PlayerSpriteState.StretchOne; }
            else if (sprite == Game1.instance.contentManager.playerSprites.stretch_smear) { return PlayerSpriteState.StretchSmear; }
            else if (sprite == Game1.instance.contentManager.playerSprites.stretch_two) { return PlayerSpriteState.StretchTwo; }
            else if (sprite == Game1.instance.contentManager.playerSprites.walk_one) { return PlayerSpriteState.WalkOne; }
            else if (sprite == Game1.instance.contentManager.playerSprites.walk_smear) { return PlayerSpriteState.WalkSmear; }
            else if (sprite == Game1.instance.contentManager.playerSprites.walk_two) { return PlayerSpriteState.WalkTwo; }
            return PlayerSpriteState.Idle;
        }
    }
}
