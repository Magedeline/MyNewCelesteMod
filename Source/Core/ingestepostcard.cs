namespace DesoloZantas.Core.Core;

public class Postcard : Entity
{
    public const float TEXT_SCALE = 0.7f;
    public MTexture Postcard1;
    public VirtualRenderTarget Target; 
    public FancyText.Text Text;
    public float Alpha = 1f;
    public float Scale = 1f;
    public float Rotation;
    public float ButtonEase;
    public string SfxEventIn;
    public string SfxEventOut;
    public Coroutine easeButtonIn1;
         private const float offset_y = 200f;
     private const float text_scale_factor = 0.7f;
     private const int text_padding = 120;


    public Postcard(string msg, int area)
        : this(msg, "event:/ui/main/postcard_ch" + (object)area + "_in", "event:/ui/main/postcard_ch" + (object)area + "_out")
    {
    }

    public Postcard(string msg, string sfxEventIn, string sfxEventOut)
    {
        this.Visible = false;
     this.Tag = Tags.HUD;
        this.SfxEventIn = sfxEventIn;
        this.SfxEventOut = sfxEventOut;
        this.Postcard1 = GFX.Gui[nameof(Postcard1)];
        this.Text = FancyText.Parse(msg, (int)((double)(this.Postcard1.Width - 120) / 0.699999988079071), -1, defaultColor: new Color?(Color.Black * 0.6f));
    }

    public IEnumerator DisplayRoutine()
    {
        yield return (object)this.EaseIn();
        yield return (object)0.75f;
        while (!Input.MenuConfirm.Pressed)
            yield return (object)null;
        Audio.Play("event:/ui/main/button_lowkey");
        yield return (object)this.EaseOut();
        yield return (object)1.2f;
    }

    public IEnumerator EaseIn()
    {
        Postcard postcard = this;
        Audio.Play(postcard.SfxEventIn);
        Vector2 vector2 = new Vector2((float)Engine.Width, (float)Engine.Height) / 2f;
        Vector2 from = vector2 + new Vector2(0.0f, 200f);
        Vector2 to = vector2;
        float rFrom = -0.1f;
        float rTo = 0.05f;
        postcard.Visible = true;
        for (float p = 0.0f; (double)p < 1.0; p += Engine.DeltaTime * 0.8f)
        {
            postcard.Position = from + (to - from) * Ease.CubeOut(p);
            postcard.Alpha = Ease.CubeOut(p);
            postcard.Rotation = rFrom + (rTo - rFrom) * Ease.CubeOut(p);
            yield return (object)null;
        }
        postcard.Add((Component)(postcard.easeButtonIn1 = new Coroutine(postcard.EaseButtonIn())));
    }

     public IEnumerator EaseButtonIn()
    {
        yield return (object)0.75f;
        while ((double)(this.ButtonEase += Engine.DeltaTime * 2f) < 1.0)
            yield return (object)null;
    }

    public IEnumerator EaseOut()
    {
        Postcard postcard = this;
        Audio.Play(postcard.SfxEventOut);
        if (postcard.easeButtonIn1 != null)
            postcard.easeButtonIn1.RemoveSelf();
        Vector2 from = postcard.Position;
        Vector2 to = new Vector2((float)Engine.Width, (float)Engine.Height) / 2f + new Vector2(0.0f, -200f);
        float rFrom = postcard.Rotation;
        float rTo = postcard.Rotation + 0.1f;
        for (float p = 0.0f; (double)p < 1.0; p += Engine.DeltaTime)
        {
            postcard.Position = from + (to - from) * Ease.CubeIn(p);
            postcard.Alpha = 1f - Ease.CubeIn(p);
            postcard.Rotation = rFrom + (rTo - rFrom) * Ease.CubeIn(p);
            postcard.ButtonEase = Calc.Approach(postcard.ButtonEase, 0.0f, Engine.DeltaTime * 8f);
            yield return (object)null;
        }
        postcard.Alpha = 0.0f;
        postcard.RemoveSelf();
    }
    
    public void BeforeRender()
    {
        if (this.Target == null)
            this.Target = VirtualContent.CreateRenderTarget("postcard", this.Postcard1.Width, this.Postcard1.Height);
        Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D)this.Target);
        Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
        Draw.SpriteBatch.Begin();
        string text = Dialog.Clean("FILE_DEFAULT");
        if (SaveData.Instance != null && Dialog.Language.CanDisplay(SaveData.Instance.Name))
            text = SaveData.Instance.Name;
        this.Postcard1.Draw(Vector2.Zero);
        ActiveFont.Draw(text, new Vector2(115f, 30f), Vector2.Zero, Vector2.One * 0.9f, Color.Black * 0.7f);
        this.Text.DrawJustifyPerLine(new Vector2((float)this.Postcard1.Width, (float)this.Postcard1.Height) / 2f + new Vector2(0.0f, 40f), new Vector2(0.5f, 0.5f), Vector2.One * 0.7f, 1f);
        Draw.SpriteBatch.End();
    }

    public override void Render()
    {
        if (this.Target != null)
            Draw.SpriteBatch.Draw((Texture2D)(RenderTarget2D)this.Target, this.Position, new Rectangle?(this.Target.Bounds), Color.White * this.Alpha, this.Rotation, new Vector2((float)this.Target.Width, (float)this.Target.Height) / 2f, this.Scale, SpriteEffects.None, 0.0f);
        if ((double)this.ButtonEase <= 0.0)
            return;
        Input.GuiButton(Input.MenuConfirm, Input.PrefixMode.Latest, "controls/keyboard/oemquestion").DrawCentered(new Vector2((float)(Engine.Width - 120), (float)(Engine.Height - 100) - 20f * Ease.CubeOut(this.ButtonEase)), Color.White * Ease.CubeOut(this.ButtonEase));
    }

    public override void Removed(Scene scene)
    {
        this.Dispose();
        base.Removed(scene);
    }

    public override void SceneEnd(Scene scene)
    {
        this.Dispose();
        base.SceneEnd(scene);
    }

    public void Dispose()
    {
        if (this.Target != null)
            this.Target.Dispose();
        this.Target = (VirtualRenderTarget)null;
    }

     public Postcard(string msg) : this(msg, "event:/ui/main/postcard_csides_in", "event:/ui/main/postcard_csides_out")
     {
         Visible = true;
         Tag = Tags.HUD;
         Postcard1 = GFX.Gui[nameof(Postcard1)];
         Text = FancyText.Parse(msg, (int)((double)(Postcard1.Width - text_padding) / text_scale_factor), -1,
             defaultColor: new Color?(Color.Black * 0.6f));
     }

    public Postcard(string msg, string soundId)
    {
        if (string.IsNullOrEmpty(soundId)) 
            soundId = "csides";
        string str1;
        if (soundId.StartsWith("event:/"))
            str1 = soundId;
        else if (soundId == "dside")
        {
            str1 = "event:/new_content/ui/postcard_dside";
        }
        this.GetFlag = "UnlockedDSide";
        string str2;
        if (soundId.StartsWith("event:/"))
            str2= soundId;
        else if (soundId == "rmxside")
        {
            str2 = "event:/new_content/ui/postcard_rmxside";
        }
        this.GetFlag = "UnlockedRMXSide";
        string str3;
        if (soundId.StartsWith("event:/"))
            str3 = soundId;
        else if (soundId == "variants")
        {
            str3 = "event:/new_content/ui/postcard_variants";
        }
        else
        {
            string str4 = "event:/ui/main/postcard_";
            if (int.TryParse(soundId, out int _))
                str4 += "ch";
            str3 = str4 + soundId;
        }
        this.SfxEventIn = str3 + "_in";
        this.SfxEventOut = str3 + "_out";
        this.Visible = false;
        this.Tag = Tags.HUD;
        this.Postcard1 = GFX.Gui[nameof(Postcard1)];
        this.Text = FancyText.Parse(msg, (int)((double)(this.Postcard1.Width - 120) / 0.699999988079071), -1, defaultColor: new Color?(Color.Black * 0.6f));
    }

    public string GetFlag { get; }
}



