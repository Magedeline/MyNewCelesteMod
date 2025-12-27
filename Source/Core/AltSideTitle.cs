namespace DesoloZantas.Core.Core {

	/// <summary>
	/// Displays the B-Side/Remix intro title card with artist and album information.
	/// Supports both custom dialog format and Everest standard format.
	/// </summary>
	public class AltSideTitle : Entity {

		// Text lines to display
		private readonly string[] textLines;
		
		// Fade values for each line (0.0 to 1.0)
		private readonly float[] fadeValues;
		
		// Horizontal offsets for slide animation
		private readonly float[] slideOffsets;
		
		// Global offset for continuous animation
		private float globalOffset;

		/// <summary>
		/// Creates a new AltSideTitle for the given session.
		/// </summary>
		/// <param name="session">The current game session</param>
		public AltSideTitle(Session session) : base() {
			// Get area data with safety check
			AreaData areaData = AreaData.Get(session);
			
			if (areaData == null || string.IsNullOrEmpty(areaData.SID)) {
				Logger.Log(LogLevel.Warn, "DesoloZantas", "AltSideTitle: AreaData or SID is null, using fallback text");
				textLines = ["Unknown Area"];
			} else {
				textLines = LoadTitleText(areaData);
			}
			
			// Initialize animation arrays
			fadeValues = new float[textLines.Length];
			slideOffsets = new float[textLines.Length];
			
			// Setup entity properties
			Tag = Tags.HUD;
			Visible = true;
		}

		/// <summary>
		/// Loads the title text from dialog, with fallback to standard Everest format.
		/// </summary>
		private static string[] LoadTitleText(AreaData areaData) {
			string dialogKey = areaData.SID.DialogKeyify();
			
			// Try custom format first: "{SID}_altsides_remix_intro"
			// Uses {break} to separate lines
			if (Dialog.Has(dialogKey + "_altsides_remix_intro")) {
				string[] lines = Dialog.Get(dialogKey + "_altsides_remix_intro").Split(
                    ["{break}"],
                    StringSplitOptions.RemoveEmptyEntries
				);
				
				if (lines.Length > 0) {
					return lines;
				}
			}
			
			// Fallback to standard Everest format
			return LoadStandardFormat(areaData, dialogKey);
		}

		/// <summary>
		/// Loads title text using standard Everest dialog keys.
		/// Format: [Area Name + Remix], [by Artist], [Album]
		/// </summary>
		private static string[] LoadStandardFormat(AreaData areaData, string dialogKey) {
			// Line 1: Area Name + Remix
			string areaName = GetDialogOrFallback(areaData.Name, areaData.Name, "");
			string remixText = GetDialogOrFallback(dialogKey + "_remix", "Remix", "Remix");
			string line1 = string.IsNullOrEmpty(areaName) 
				? remixText 
				: $"{areaName} {remixText}";
			
			// Line 2: by Artist
			string byPrefix = GetDialogOrFallback("remix_by", "by", "by");
			string artist = GetDialogOrFallback(dialogKey + "_remix_artist", "Unknown Artist", "Unknown Artist");
			string line2 = $"{byPrefix} {artist}";
			
			// Line 3: Album
			string album = GetDialogOrFallback(
				dialogKey + "_remix_album",
                GetDialogOrFallback("remix_album", "Remix Album", "Remix Album"),
				"Remix Album"
			);
			
			return [line1, line2, album];
		}

		/// <summary>
		/// Gets a dialog string with fallback support.
		/// </summary>
		private static string GetDialogOrFallback(string key, string defaultValue, string finalFallback) {
			if (Dialog.Has(key)) {
				return Dialog.Get(key);
			}
			return string.IsNullOrEmpty(defaultValue) ? finalFallback : defaultValue;
		}

		/// <summary>
		/// Animates the title text fading in with staggered timing.
		/// </summary>
		public IEnumerator EaseIn() {
			const float delayBetweenLines = 0.2f;
			const float fadeDuration = 1f;
			
			for (int i = 0; i < textLines.Length; i++) {
				Add(new Coroutine(FadeLine(i, 1f, fadeDuration)));
				yield return delayBetweenLines;
			}
			
			// Hold on screen
			yield return 1.6f;
		}

		/// <summary>
		/// Animates the title text fading out with staggered timing.
		/// </summary>
		public IEnumerator EaseOut() {
			const float delayBetweenLines = 0.2f;
			const float fadeDuration = 1f;
			
			for (int i = 0; i < textLines.Length; i++) {
				Add(new Coroutine(FadeLine(i, 0f, fadeDuration)));
				yield return delayBetweenLines;
			}
			
			// Wait for all to fade out
			yield return 1.6f;
		}

		/// <summary>
		/// Smoothly fades a single line of text in or out.
		/// </summary>
		/// <param name="lineIndex">Index of the line to fade</param>
		/// <param name="targetFade">Target fade value (0.0 to 1.0)</param>
		/// <param name="duration">Duration of the fade in seconds</param>
		private IEnumerator FadeLine(int lineIndex, float targetFade, float duration) {
			bool fadingOut = targetFade < 0.5f;
			
			while ((fadeValues[lineIndex] = Calc.Approach(fadeValues[lineIndex], targetFade, Engine.DeltaTime / duration)) != targetFade) {
				// Calculate slide offset based on fade progress
				float fadeProgress = fadingOut ? (1f - fadeValues[lineIndex]) : fadeValues[lineIndex];
				float slideAmount = Ease.CubeIn(1f - fadeProgress) * 32f;
				
				// Slide right when fading out, left when fading in
				slideOffsets[lineIndex] = fadingOut ? slideAmount : -slideAmount;
				
				yield return null;
			}
		}

		public override void Update() {
			base.Update();
			
			// Continuous horizontal drift animation
			globalOffset += Engine.DeltaTime * 32f;
		}

		public override void Render() {
			// Calculate base position (lower left area of screen)
			// Adjust vertical position based on number of lines to keep centered
			float verticalSpacing = 60f;
			float totalHeight = textLines.Length * verticalSpacing;
			float verticalOffset = MathHelper.Max(totalHeight - 180f, 0f);
			
			Vector2 basePosition = new(
				60f + globalOffset, 
				800f - verticalOffset
			);
			
			// Render each line
			for (int i = 0; i < textLines.Length; i++) {
				if (string.IsNullOrEmpty(textLines[i])) {
					continue;
				}
				
				Vector2 linePosition = basePosition + new Vector2(
					slideOffsets[i], 
					verticalSpacing * i
				);
				
				Color lineColor = Color.White * fadeValues[i];
				ActiveFont.Draw(textLines[i], linePosition, lineColor);
			}
		}
	}
}




