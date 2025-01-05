using HarmonyLib;
using JumpKing.Level;
using JumpKing.MiscSystems.Achievements;
using JumpKing.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Reflection;

namespace JumpKingTAS
{
	public class FrameState {
		public Vector2 Position;
		public Vector2 Velocity;
		public Vector2 LastVelocity;
		public int Time;
		public float JumpTime;
		public int LastScreen;
		public int TimeStamp;
		public SpriteEffects Direction;
		public bool InWater;
		public bool Knocked;
		public bool OnGround;
		public bool OnIce;
		public bool OnSnow;
		public bool WindEnabled;
		public bool OnSand;
		public static FieldInfo AchievemementManager = 
			AccessTools.Field("JumpKing.MiscSystems.Achievements.AchievementManager:instance");

		public FrameState(PlayerEntity player) {
			SetValues(player);
		}
		public void SetValues(PlayerEntity player) {
			if (player != null) {
				BodyComp body = player.m_body;
				InWater = body.IsOnBlock(typeof(WaterBlock));
				Knocked = body.IsKnocked;
				OnGround = body.IsOnGround;
				OnIce = body.IsOnBlock(typeof(IceBlock));
				OnSnow = body.IsOnBlock(typeof(SnowBlock));
                OnSand = body.IsOnBlock(typeof(SandBlock));
                WindEnabled = LevelManager.CurrentScreen.WindEndabled;
				Position = body.Position;
				Velocity = body.Velocity;
				LastVelocity = body.LastVelocity;
				LastScreen = body.LastScreen;
				var tPlayer = Traverse.Create(player);
				Direction = tPlayer.Field("m_flip").GetValue<SpriteEffects>();
				TimeStamp = tPlayer.Field("m_time_stamp").GetValue<int>();
				JumpTime = tPlayer.Field("m_jump_state").Field("m_timer").GetValue<float>();

				if (AchievemementManager.GetValue(null) != null)
				{
					Time = Traverse.Create(AchievemementManager.GetValue(null))
                        .Field("m_all_time_stats")
						.Field("_ticks")
						.GetValue<int>();
                }
			}
		}
		public void UpdateBody(PlayerEntity player) {
			BodyComp body = player.m_body;

			var tBody = Traverse.Create(body);
            //body._is_in_water = InWater;
            tBody.Field("_knocked").SetValue(Knocked);
            tBody.Field("_is_on_ground").SetValue(OnGround);
			//body._is_on_ice = OnIce;
			//body._is_on_snow = OnSnow;
			//body._is_on_sand = OnSand;
			//body.m_wind_enabled = WindEnabled;
			body.Position = Position;
			body.Velocity = Velocity;
			tBody.Field("_last_velocity").SetValue(LastVelocity);
            //body.m_last_screen = LastScreen;

            var tPlayer = Traverse.Create(player);
			tPlayer.Field("m_flip").SetValue(Direction);
			tPlayer.Field("m_time_stamp").SetValue(TimeStamp);
			tPlayer.Field("m_jump_state").Field("m_timer").SetValue(JumpTime);

            var tAchievements = Traverse.Create(AchievemementManager.GetValue(null));

			// traverse cant properly set values (maybe related to structs?)
			var stats = tAchievements.Field("m_all_time_stats").GetValue<PlayerStats>();
			stats._ticks = Time;
			tAchievements.Field("m_all_time_stats").SetValue(stats);

			// while stepbacking with giant boots on
			// there has a chance to disable body component on air
			// then player can't move anymore
			if (!OnGround && !body.Enabled) {
				body.Enabled = true;
			}
        }
	}
}