using DesoloZantas.Core.Core;
using static DesoloZantas.Core.BossesHelper.Helpers.BossesHelperUtils;

namespace DesoloZantas.Core.BossesHelper.Entities
{
	internal static class HealthDisplays
	{
		private static IngesteModuleSession.HealthSystemData HealthData => IngesteModule.Session.healthData;

		public class HealthDisplay(Vector2 position, Vector2 barScale, Func<int> getHealth, Color color = default, bool isGlobal = false) : Entity
		{
			public readonly Sprite Frame = GFX.SpriteBank.TryCreate(HealthData.frameSprite);

			public readonly Vector2 BarScale = barScale;

			public readonly Func<int> GetHealth = getHealth;

			public readonly Color BaseColor = color;

			public readonly int MaxHealth = getHealth();

			public Color Color = color;

			public new bool Visible
			{
				get => IsVisible;
				set => ActiveVisibility = value;
			}

			protected virtual bool IsVisible
			{
				get => base.Visible;
				set => base.Visible = value;
			}

			private bool ActiveVisibility = true;

			public override void Added(Scene scene)
			{
				base.Added(scene);
				AddTag(Tags.HUD);
				this.ChangeTagState(Tags.Global, isGlobal);
				Position = position;
				if (Frame.Width > 0 && Frame.Height > 0)
				{
					Add(Frame);
				}
			}

			public override void Update()
			{
				base.Update();
				IsVisible = !Scene.Paused && ActiveVisibility;
			}
		}

		public class HealthIconList(Vector2 barPosition, Vector2 barScale, Func<int> getHealth, bool isGlobal, List<string> icons,
				List<string> createAnims, List<string> removeAnims, List<float> iconSeparations, bool removeIconOnDamage)
			: HealthDisplay(barPosition, barScale, getHealth, isGlobal: isGlobal)
		{
			private class HealthIcon : Sprite
			{
				private readonly string startAnim;

				private readonly string endAnim;

				public HealthIcon(Vector2 scale, Vector2 offset, string iconSprite, string startAnim, string endAnim)
					: base(GFX.Game, "")
				{
					this.startAnim = startAnim;
					this.endAnim = endAnim;
					GFX.SpriteBank.CreateOn(this, iconSprite);
					Scale = scale;
					Position = offset;
				}

				public IEnumerator DrawIcon()
				{
					return IconRoutine(startAnim);
				}

				public IEnumerator RemoveIcon(bool remove = false)
				{
					return IconRoutine(endAnim, remove);
				}

				private IEnumerator IconRoutine(string anim, bool remove = false)
				{
					yield return this.PlayAnim(anim);
					if (remove)
						RemoveSelf();
				}
			}

			private readonly Stack<HealthIcon> healthIcons = [];

			private readonly Stack<HealthIcon> toRemove = [];

			private readonly List<Vector2> iconSeparations = [Vector2.Zero, .. iconSeparations.ConvertAll(f => Vector2.UnitX * f)];

			private IEnumerable<HealthIcon> AllIcons => [.. healthIcons, .. toRemove];

			public int Count => healthIcons.Count;

			protected override bool IsVisible
			{
				get => AllIcons.Any(i => i.Visible);
				set
				{
					foreach (var icon in AllIcons)
						icon.Visible = value;
				}
			}

			public HealthIconList(bool global = false)
				: this(HealthData.healthBarPos, HealthData.healthIconScale, () => IngesteModule.Session.currentPlayerHealth,
					  global, SeparateList(HealthData.iconSprite), SeparateList(HealthData.startAnim),
					  SeparateList(HealthData.endAnim), SeparateFloatList(HealthData.iconSeparation), HealthData.removeOnDamage)
			{ }

			public HealthIconList(EntityData entityData, Vector2 barPosition, Vector2 barScale, Func<int> getHealth)
				: this(barPosition, barScale, getHealth, false, SeparateList(entityData.Attr("healthIcons")),
					  SeparateList(entityData.Attr("healthIconsCreateAnim")), SeparateList(entityData.Attr("healthIconsCreateAnim")),
					  SeparateFloatList(entityData.Attr("healthIconsSeparation")), entityData.Bool("removeOnDamage"))
			{ }

			public override void Awake(Scene scene)
			{
				base.Awake(scene);
				RefillHealth();
			}

			public void RefillHealth(int? upTo = null)
			{
				IncreaseHealth((upTo ?? MaxHealth) - Count);
			}

			public void IncreaseHealth(int amount = 1)
			{
				for (int i = 0; i < amount; i++)
				{
					if (!toRemove.TryPop(out HealthIcon healthIcon))
					{
						int index = Count;
						Vector2 offset = (healthIcons.TryPeek(out HealthIcon lastIcon) ? lastIcon.Position : Vector2.Zero)
							+ iconSeparations.ElementAtOrLast(index);

						Add(healthIcon = new(BarScale, offset, icons.ElementAtOrLast(index),
							createAnims.ElementAtOrLast(index), removeAnims.ElementAtOrLast(index)));
					}
					healthIcons.Push(healthIcon);
					healthIcon.DrawIcon();
				}
			}

			public void DecreaseHealth(int amount = 1)
			{
				for (int i = 0; i < amount; i++)
				{
					if (!healthIcons.TryPop(out HealthIcon removed))
					{
						Logger.Log("Health Render", "No Health Icon to remove");
						return;
					}
					removed.RemoveIcon(removeIconOnDamage).AsCoroutine(this);
					if (!removeIconOnDamage)
					{
						toRemove.Push(removed);
					}
				}
			}

			public void Clear()
			{
				healthIcons.Clear();
				toRemove.Clear();
			}
		}

		public class HealthNumber(Vector2 barPosition, Vector2 barScale, Func<int> bossHealth, Color color)
			: HealthDisplay(barPosition, barScale, bossHealth, color)
		{
			public override void Update()
			{
				base.Update();
				if (Color != BaseColor)
				{
					Color = Color.Lerp(Color, BaseColor, 0.1f);
				}
			}

			public override void Render()
			{
				ActiveFont.Draw($"{GetHealth()}", Position, new Vector2(0.5f, 0.5f), BarScale, Color);
				base.Render();
			}
		}

		public class HealthBar(Vector2 barPosition, Vector2 barScale, Func<int> bossHealth, Color color, Alignment barDir)
			: HealthDisplay(barPosition, barScale, bossHealth, color)
		{
			private readonly float leftEdge = barDir switch
			{
				Alignment.Left => barPosition.X - barScale.X,
				Alignment.Center => barPosition.Y - barScale.X / 2,
				_ => barPosition.X,
			};

			public override void Added(Scene scene)
			{
				base.Added(scene);
				Position.X = leftEdge;
				Collider = new Hitbox(BarScale.X, BarScale.Y);
			}

			public override void Update()
			{
				base.Update();
				if (Color != BaseColor)
				{
					Color = Color.Lerp(Color, BaseColor, 0.1f);
				}
				float maxWidth = BarScale.X;
				Collider.Width = maxWidth * GetHealth() / MaxHealth;
				Position.X = leftEdge + barDir switch
				{
					Alignment.Left => (maxWidth - Collider.Width),
					Alignment.Center => (maxWidth - Collider.Width) / 2,
					_ => 0
				};
			}

			public override void Render()
			{
				Draw.Rect(Collider, Color);
				base.Render();
			}
		}
	}
}




