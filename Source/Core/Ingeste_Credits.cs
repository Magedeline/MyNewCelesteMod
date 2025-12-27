namespace DesoloZantas.Core.Core
{
    public class IngesteCredits
    {
        public static string[] RemixersBSide = new string[6]
        {
            "Team BNP",
            "Nintendo, Warpstar.Inc, and Hal Lab",
            "Kyle Gamer Remix",
            "Cloud Jumper",
            "Toby Fox",
            "Maggy"
        };
        public static string[] RemixersCSide = new string[11]
        {
            "Kyle Gamer Remix",
            "Acid Notation",
            "NyxTheShield",
            "Nintendo, Warpstar.Inc, and Hal Lab",
            "Karu",
            "DJ-R",
            "TheMathewFlames",
            "Camellia Remix",
            "Osirois Music",
            "Toby Fox",
            "Maggy"
        };
        public static Color BorderColor = Color.Black;
        public const float CREDIT_SPACING = 64f;
        public const float AUTO_SCROLL_SPEED = 100f;
        public const float INPUT_SCROLL_SPEED = 600f;
        public const float SCROLL_RESUME_DELAY = 1f;
        public const float SCROLL_ACCELERATION = 1800f;
        private List<IngesteCreditsNode> credits;
        public float AutoScrollSpeedMultiplier = 1f;
        private float scrollSpeed = 100f;
        private float scroll;
        private float height;
        private float scrollDelay;
        private float scrollbarAlpha;
        private float alignment;
        private float scale;
        public float BottomTimer;
        public bool Enabled = true;
        public bool AllowInput = true;
        public static PixelFont Font;
        public static float FontSize;
        public static float LineHeight;

        private static List<IngesteCreditsNode> createCredits(bool title, bool polaroids)
        {
            List<IngesteCreditsNode> credits = new List<IngesteCreditsNode>();
            if (title)
                credits.Add(new Image(nameof(title), 320f));
            credits.AddRange(new List<IngesteCreditsNode>
            {
                new Role("Maddy Thorson", "Owner of Celestes"),
                new Role("EXOK", "Coroperation Company"),
                new Role("Toby Fox", "Sound and Music Owner", "Sprites and Content Owner"),
                new Role("sakurai masahiro", "Hal Labratory", "Warp Star.Inc", "and Nintendo Owner of Kirby Series"),
                new Role("ULTOK", "Original Classic Celeste Mod of Ingeste"),
                new Role("UnPseudo", "Kirby Skin Mod"),
                new Team("Prevoius Team", new string[3]
                {
                    "Jevil GrowTime",
                    "Moha aka Miha",
                    "MrCoy",
                },  "Small Team")
            });
            if (polaroids)
                credits.Add(new Image(GFX.Portraits, "credits/a", 64f, -0.05f));
            credits.AddRange(new List<IngesteCreditsNode>
            {
                new Role("Gabby DaRienzo", "3D Artist"),
                new Role("Sven Bergstr�m", "3D Lighting Artist")
            });
            credits.AddRange(new List<IngesteCreditsNode>
            {
                new Thanks("Writing Assistance", "Noel Berry", "Amora B.", "Greg Lobanov", "Lena Raine", "Nick Suttner"),
                new Thanks("Script Editor", "Nick Suttner"),
                new Thanks("Narrative Consulting", "Silverstring Media", "Claris Cyarron", "with Lucas JW Johnson", "and Tanya Kan")
            });
            credits.Add(new Thanks("Remixers", Credits.Remixers));
            credits.Add(new IngesteMultiCredit("Musical Performances", new IngesteMultiCredit.Section("Violin, Viola", "Michaela Nachtigall"), new IngesteMultiCredit.Section("Cello", "SungHa Hong"), new IngesteMultiCredit.Section("Drums", "Doug Perry")));
            if (polaroids)
                credits.Add(new Image(GFX.Portraits, "credits/b", 64f, 0.05f));
            credits.Add(new Thanks("Operations Manager", "Heidy Motta"));
            credits.Add(new Thanks("Porting", "Sickhead Games, LLC", "Ethan Lee"));
            credits.Add(new IngesteMultiCredit("Localization", new IngesteMultiCredit.Section("French, Italian, Korean, Russian,\nSimplified Chinese, Spanish,\nBrazilian Portuguese, German", "EDS Wordland, Ltd."), new IngesteMultiCredit.Section("Japanese", "8-4, Ltd", "Keiko Fukuichi", "Graeme Howard", "John Ricciardi"), new IngesteMultiCredit.Section("German, Additional Brazilian Portuguese", "Shloc, Ltd.", "Oli Chance", "Isabel Sterner", "Nadine Leonhardt"), new IngesteMultiCredit.Section("Additional Brazilian Portuguese", "Amora B.")));
            credits.Add(new Thanks("Contributors & Playtesters", "Nels Anderson", "Liam & Graeme Berry", "Tamara Bruketta", "Allan Defensor", "Grayson Evans", "Jada Gibbs", "Em Halberstadt", "Justin Jaffray", "Chevy Ray Johnston", "Will Lacerda", "Myriame Lachapelle", "Greg Lobanov", "Rafinha Martinelli", "Shane Neville", "Kyle Pulver", "Murphy Pyan", "Garret Randell", "Kevin Regamey", "Atlas Regaudie", "Stefano Strapazzon", "Nick Suttner", "Ryan Thorson", "Greg Wohlwend", "Justin Yngelmo", "baldjared", "zep", "DevilSquirrel", "Covert_Muffin", "buhbai", "Chaikitty", "Msushi", "TGH"));
            credits.Add(new Thanks("Community", "Speedrunners & Tool Assisted Speedrunners", "Everest Modding Community", "The Celeste Discord"));
            credits.Add(new Thanks("Special Thanks", "Fe Angelo", "Bruce Berry & Marilyn Firth", "Josephine Baird", "Liliane Carpinski", "Yvonne Hanson", "Katherine Elaine Jones", "Clint 'halfcoordinated' Lexa", "Greg Lobanov", "Gabi 'Platy' Madureira", "Rodrigo Monteiro", "Fernando Piovesan", "Paulo Szyszko Pita", "Zoe Si", "Julie, Richard, & Ryan Thorson", "Davey Wreden"));
            if (polaroids)
                credits.Add(new Image(GFX.Portraits, "credits/c", 64f, -0.05f));
            credits.Add(new Thanks("Production Kitties", "Jiji, Mr Satan, Peridot", "Azzy, Phil, Furiosa", "Fred, Bastion, Meredith", "Bobbin, Finn"));
            credits.AddRange(new List<IngesteCreditsNode>
            {
                new Image(GFX.Misc, "fmod"),
                new Image(GFX.Misc, "monogame"),
                new ImageRow(new Image(GFX.Misc, "fna"), new Image(GFX.Misc, "xna"))
            });
            credits.Add(new Break(540f));
            if (polaroids)
                credits.Add(new Image(GFX.Portraits, "credits/d", rotation: 0.05f, screenCenter: true));
            credits.Add(new Ending(Dialog.Clean("CREDITS_THANKYOU"), !polaroids));
            return credits;
        }

        public IngesteCredits(float alignment = 0.5f, float scale = 1f, bool haveTitle = true, bool havePolaroids = false)
        {
            this.alignment = alignment;
            this.scale = scale;
            credits = IngesteCredits.createCredits(haveTitle, havePolaroids);
            IngesteCredits.Font = Dialog.Languages["english"].Font;
            IngesteCredits.FontSize = Dialog.Languages["english"].FontFaceSize;
            IngesteCredits.LineHeight = IngesteCredits.Font.Get(IngesteCredits.FontSize).LineHeight;
            height = 0.0f;
            foreach (IngesteCreditsNode credit in credits)
                height += credit.Height(scale) + 64f * scale;
            height += 476f;
            if (!havePolaroids)
                return;
            height -= 280f;
        }

        public void Update()
        {
            if (Enabled)
            {
                scroll += scrollSpeed * Engine.DeltaTime * scale;
                if (scrollDelay <= 0.0)
                    scrollSpeed = Calc.Approach(scrollSpeed, 100f * AutoScrollSpeedMultiplier, 1800f * Engine.DeltaTime);
                else
                    scrollDelay -= Engine.DeltaTime;
                if (AllowInput)
                {
                    if (Input.MenuDown.Check)
                    {
                        scrollDelay = 1f;
                        scrollSpeed = Calc.Approach(scrollSpeed, 600f, 1800f * Engine.DeltaTime);
                    }
                    else if (Input.MenuUp.Check)
                    {
                        scrollDelay = 1f;
                        scrollSpeed = Calc.Approach(scrollSpeed, -600f, 1800f * Engine.DeltaTime);
                    }
                    else if (scrollDelay > 0.0)
                        scrollSpeed = Calc.Approach(scrollSpeed, 0.0f, 1800f * Engine.DeltaTime);
                }
                if (scroll < 0.0 || scroll > (double)height)
                    scrollSpeed = 0.0f;
                scroll = Calc.Clamp(scroll, 0.0f, height);
                if (scroll >= (double)height)
                    BottomTimer += Engine.DeltaTime;
                else
                    BottomTimer = 0.0f;
            }
            scrollbarAlpha = Calc.Approach(scrollbarAlpha, !Enabled || scrollDelay <= 0.0 ? 0.0f : 1f, Engine.DeltaTime * 2f);
        }

        public void Render(Vector2 position)
        {
            Vector2 position1 = position + new Vector2(0.0f, 1080f - scroll).Floor();
            foreach (IngesteCreditsNode credit in credits)
            {
                float num = credit.Height(scale);
                if (position1.Y > -(double)num && position1.Y < 1080.0)
                    credit.Render(position1, alignment, scale);
                position1.Y += num + 64f * scale;
            }
            if (scrollbarAlpha <= 0.0)
                return;
            int y = 64;
            int height1 = 1080 - y * 2;
            float height2 = height1 * (height1 / height);
            float num1 = (float)(scroll / (double)height * (height1 - (double)height2));
            Draw.Rect(1844f, y, 12f, height1, Color.White * 0.2f * scrollbarAlpha);
            Draw.Rect(1844f, y + num1, 12f, height2, Color.White * 0.5f * scrollbarAlpha);
        }

        private abstract class IngesteCreditsNode
        {
            public abstract void Render(Vector2 position, float alignment = 0.5f, float scale = 1f);

            public abstract float Height(float scale = 1f);
        }

        private class Role : IngesteCreditsNode
        {
            public const float NAME_SCALE = 1.8f;
            public const float ROLES_SCALE = 1f;
            public const float SPACING = 8f;
            public const float BOTTOM_SPACING = 64f;
            public static readonly Color NAME_COLOR = Color.White;
            public static readonly Color ROLES_COLOR = Color.White * 0.8f;
            public string Name;
            public string Roles;

            public Role(string name, params string[] roles)
            {
                Name = name;
                Roles = string.Join(", ", roles);
            }

            public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
            {
                Credits.Font.DrawOutline(Credits.FontSize, Name, position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 1.8f * scale, Role.NAME_COLOR, 2f, Credits.BorderColor);
                position.Y += (float)(Credits.LineHeight * 1.7999999523162842 + 8.0) * scale;
                Credits.Font.DrawOutline(Credits.FontSize, Roles, position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 1f * scale, Role.ROLES_COLOR, 2f, Credits.BorderColor);
            }

            public override float Height(float scale = 1f) => (float)(Credits.LineHeight * 2.7999999523162842 + 8.0 + 64.0) * scale;
        }

        private class Team : IngesteCreditsNode
        {
            public const float TEAM_SCALE = 1.4f;
            public static readonly Color TEAM_COLOR = Color.White;
            public string Name;
            public string[] Members;
            public string Roles;

            public Team(string name, string[] members, params string[] roles)
            {
                Name = name;
                Members = members;
                Roles = string.Join(", ", roles);
            }

            public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
            {
                Credits.Font.DrawOutline(Credits.FontSize, Name, position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 1.8f * scale, Role.NAME_COLOR, 2f, Credits.BorderColor);
                position.Y += (float)(Credits.LineHeight * 1.7999999523162842 + 8.0) * scale;
                for (int index = 0; index < Members.Length; ++index)
                {
                    Credits.Font.DrawOutline(Credits.FontSize, Members[index], position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 1.4f * scale, Team.TEAM_COLOR, 2f, Credits.BorderColor);
                    position.Y += Credits.LineHeight * 1.4f * scale;
                }
                Credits.Font.DrawOutline(Credits.FontSize, Roles, position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 1f * scale, Role.ROLES_COLOR, 2f, Credits.BorderColor);
            }

            public override float Height(float scale = 1f) => (float)(Credits.LineHeight * (1.7999999523162842 + Members.Length * 1.3999999761581421 + 1.0) + 8.0 + 64.0) * scale;
        }

        private class Thanks : IngesteCreditsNode
        {
            public const float TITLE_SCALE = 1.4f;
            public const float CREDITS_SCALE = 1.15f;
            public const float SPACING = 8f;
            public readonly Color TitleColor = Color.White;
            public readonly Color CreditsColor = Color.White * 0.8f;
            public int TopPadding;
            public string Title;
            public string[] Credits;
            private string[] linkedImages;

            public Thanks(string title, params string[] to)
                : this(0, title, to)
            {
            }

            public Thanks(int topPadding, string title, params string[] to)
            {
                TopPadding = topPadding;
                Title = title;
                Credits = to;
                linkedImages = new string[Credits.Length];
                for (int index = 0; index < linkedImages.Length; ++index)
                {
                    linkedImages[index] = null;
                    if (Credits[index].StartsWith("image:"))
                        linkedImages[index] = Credits[index].Substring(6);
                }
            }

            public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
            {
                position.Y += TopPadding * scale;
                Font.DrawOutline(FontSize, Title, position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 1.4f * scale, TitleColor, 2f, BorderColor);
                position.Y += (float)(LineHeight * 1.3999999761581421 + 8.0) * scale;
                for (int index = 0; index < Credits.Length; ++index)
                {
                    if (linkedImages[index] != null)
                    {
                        MTexture mtexture = GFX.Gui[linkedImages[index]];
                        mtexture.DrawJustified(position.Floor() + new Vector2(0.0f, -2f), new Vector2(alignment, 0.0f), BorderColor, 1.15f * scale, 0.0f);
                        mtexture.DrawJustified(position.Floor() + new Vector2(0.0f, 2f), new Vector2(alignment, 0.0f), BorderColor, 1.15f * scale, 0.0f);
                        mtexture.DrawJustified(position.Floor() + new Vector2(-2f, 0.0f), new Vector2(alignment, 0.0f), BorderColor, 1.15f * scale, 0.0f);
                        mtexture.DrawJustified(position.Floor() + new Vector2(2f, 0.0f), new Vector2(alignment, 0.0f), BorderColor, 1.15f * scale, 0.0f);
                        mtexture.DrawJustified(position.Floor(), new Vector2(alignment, 0.0f), CreditsColor, 1.15f * scale, 0.0f);
                        position.Y += mtexture.Height * 1.15f * scale;
                    }
                    else
                    {
                        Font.DrawOutline(FontSize, Credits[index], position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 1.15f * scale, CreditsColor, 2f, BorderColor);
                        position.Y += LineHeight * 1.15f * scale;
                    }
                }
            }

            public override float Height(float scale = 1f) => ((float)(LineHeight * (1.3999999761581421 + Credits.Length * 1.1499999761581421) + (Credits.Length != 0 ? 8.0 : 0.0)) + TopPadding) * scale;
        }

        private class IngesteMultiCredit(int topPadding, string title, IngesteMultiCredit.Section[] to)
            : IngesteCreditsNode
        {
            public const float TITLE_SCALE = 1.4f;
            public const float SUBTITLE_SCALE = 0.7f;
            public const float CREDITS_SCALE = 1.15f;
            public const float SPACING = 8f;
            public const float SECTION_SPACING = 32f;
            public readonly Color TitleColor = Color.White;
            public readonly Color SubtitleColor = Calc.HexToColor("a8a694");
            public readonly Color CreditsColor = Color.White * 0.8f;
            public int TopPadding = topPadding;
            public string Title = title;
            public Section[] Sections = to;

            public IngesteMultiCredit(string title, params Section[] to)
                : this(0, title, to)
            {
            }

            public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
            {
                position.Y += TopPadding * scale;
                Credits.Font.DrawOutline(Credits.FontSize, Title, position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 1.4f * scale, TitleColor, 2f, Credits.BorderColor);
                position.Y += (float)(Credits.LineHeight * 1.3999999761581421 + 8.0) * scale;
                for (int index1 = 0; index1 < Sections.Length; ++index1)
                {
                    Section section = Sections[index1];
                    string subtitle = section.Subtitle;
                    Credits.Font.DrawOutline(Credits.FontSize, section.Subtitle, position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 0.7f * scale, SubtitleColor, 2f, Credits.BorderColor);
                    position.Y += (float)(section.SubtitleLines * (double)Credits.LineHeight * 0.699999988079071) * scale;
                    for (int index2 = 0; index2 < section.Credits.Length; ++index2)
                    {
                        Credits.Font.DrawOutline(Credits.FontSize, section.Credits[index2], position.Floor(), new Vector2(alignment, 0.0f), Vector2.One * 1.15f * scale, CreditsColor, 2f, Credits.BorderColor);
                        position.Y += Credits.LineHeight * 1.15f * scale;
                    }
                    position.Y += 32f * scale;
                }
            }

            public override float Height(float scale = 1f)
            {
                float num = 0.0f + TopPadding + (float)(Credits.LineHeight * 1.3999999761581421 + 8.0);
                for (int index = 0; index < Sections.Length; ++index)
                    num = num + (float)(Sections[index].SubtitleLines * (double)Credits.LineHeight * 0.699999988079071) + Credits.LineHeight * 1.15f * Sections[index].Credits.Length;
                return (num + 32f * (Sections.Length - 1)) * scale;
            }

            public class Section
            {
                public string Subtitle;
                public int SubtitleLines;
                public string[] Credits;

                public Section(string subtitle, params string[] credits)
                {
                    Subtitle = subtitle.ToUpper();
                    SubtitleLines = subtitle.Split('\n').Length;
                    Credits = credits;
                }
            }
        }

        private class Ending : IngesteCreditsNode
        {
            public string Text;
            public bool Spacing;

            public Ending(string text, bool spacing)
            {
                Text = text;
                Spacing = spacing;
            }

            public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
            {
                if (Spacing)
                    position.Y += 540f;
                else
                    position.Y += (float)(ActiveFont.LineHeight * 1.5 * scale * 0.5);
                ActiveFont.DrawOutline(Text, new Vector2(960f, position.Y), new Vector2(0.5f, 0.5f), Vector2.One * 1.5f * scale, Color.White, 2f, Credits.BorderColor);
            }

            public override float Height(float scale = 1f) => Spacing ? 540f : ActiveFont.LineHeight * 1.5f * scale;
        }

        private class Image : IngesteCreditsNode
        {
            public Atlas Atlas;
            public string ImagePath;
            public float BottomPadding;
            public float Rotation;
            public bool ScreenCenter;

            public Image(string path, float bottomPadding = 0.0f)
                : this(GFX.Gui, path, bottomPadding)
            {
            }

            public Image(
                Atlas atlas,
                string path,
                float bottomPadding = 0.0f,
                float rotation = 0.0f,
                bool screenCenter = false)
            {
                Atlas = atlas;
                ImagePath = path;
                BottomPadding = bottomPadding;
                Rotation = rotation;
                ScreenCenter = screenCenter;
            }

            public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
            {
                MTexture atla = Atlas[ImagePath];
                Vector2 position1 = position + new Vector2(atla.Width * (0.5f - alignment), atla.Height * 0.5f) * scale;
                if (ScreenCenter)
                    position1.X = 960f;
                atla.DrawCentered(position1, Color.White, scale, Rotation);
            }

            public override float Height(float scale = 1f) => (Atlas[ImagePath].Height + BottomPadding) * scale;
        }

        private class ImageRow : IngesteCreditsNode
        {
            private Image[] images;

            public ImageRow(params Image[] images) => this.images = images;

            public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
            {
                float num1 = Height(scale);
                float num2 = 0.0f;
                foreach (Image image in images)
                    num2 += (image.Atlas[image.ImagePath].Width + 32) * scale;
                float num3 = num2 - 32f * scale;
                Vector2 vector2 = position - new Vector2(alignment * num3, 0.0f);
                foreach (Image image in images)
                {
                    image.Render(vector2 + new Vector2(0.0f, (float)((num1 - (double)image.Height(scale)) / 2.0)), 0.0f, scale);
                    vector2.X += (image.Atlas[image.ImagePath].Width + 32) * scale;
                }
            }

            public override float Height(float scale = 1f)
            {
                float num = 0.0f;
                foreach (Image image in images)
                {
                    if (image.Height(scale) > (double)num)
                        num = image.Height(scale);
                }
                return num;
            }
        }

        private class Break : IngesteCreditsNode
        {
            public float Size;

            public Break(float size = 64f) => Size = size;

            public override void Render(Vector2 position, float alignment = 0.5f, float scale = 1f)
            {
                position.Y += Size * scale;
            }

            public override float Height(float scale = 1f) => Size * scale;
        }
    }
}



