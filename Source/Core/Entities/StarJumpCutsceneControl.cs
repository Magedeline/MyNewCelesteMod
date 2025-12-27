using System.Runtime.CompilerServices;
using DesoloZantas.Core.Core.Cutscenes;
// Ensure proper namespace for Player class
// Ensure proper namespace for core Celeste classes

// For CS08_StarJumpEnd cutscene

namespace DesoloZantas.Core.Core.Entities;
[CustomEntity("Ingeste/StarJumpControlCutscenes")]
[Tracked(true)]
public class StarJumpCutsceneControl : Entity
{
    public Level Level;
    public Random Random;
    public float MinY;
    public float MaxY;
    public float MinX;
    public float MaxX;
    public float CameraOffsetMarker;
    public float CameraOffsetTimer;
    public VirtualRenderTarget BlockFill;
    public const int RAY_COUNT = 100;
    public VertexPositionColor[] Vertices = new VertexPositionColor[600];
    public int VertexCount;
    public Color RayColor = Calc.HexToColor("a3ffff") * 0.25f; 
    public StarJumpCutsceneControl.Ray[] Rays = new StarJumpCutsceneControl.Ray[100];

    public StarJumpCutsceneControl() => this.InitBlockFill();

    public override void Added(Scene scene)
    {
        base.Added(scene);
        this.Level = this.SceneAs<Level>();
        this.MinY = (float)(this.Level.Bounds.Top + 80);
        this.MaxY = (float)(this.Level.Bounds.Top + 1800);
        this.MinX = (float)(this.Level.Bounds.Left + 80);
        this.MaxX = (float)(this.Level.Bounds.Right - 80);
        this.Level.Session.Audio.Music.Event = "event:/Ingeste/music/lvl8/starjump";
        this.Level.Session.Audio.Music.Layer(1, 1f);
        this.Level.Session.Audio.Music.Layer(2, 0.0f);
        this.Level.Session.Audio.Music.Layer(3, 0.0f);
        this.Level.Session.Audio.Music.Layer(4, 0.0f);
        this.Level.Session.Audio.Apply(false);
        this.Random = new Random(666);
        this.Add((Component)new BeforeRenderHook(new Action(this.BeforeRender)));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Update()
    {
        base.Update();
        global::Celeste.Player entity = this.Scene.Tracker.GetEntity<global::Celeste.Player>(); // Fully qualify Player class
        if (entity != null)
        {
            float centerY = entity.CenterY;
            this.Level.Session.Audio.Music.Layer(1, Calc.ClampedMap(centerY, this.MaxY, this.MinY, 1f, 0.0f));
            this.Level.Session.Audio.Music.Layer(2, Calc.ClampedMap(centerY, this.MaxY, this.MinY));
            this.Level.Session.Audio.Apply(false);
            
            // Trigger CS08_StarJumpEnd cutscene when player reaches the top
            if (centerY <= this.MinY && !this.Level.Session.GetFlag(CS08_StarJumpEnd.Flag))
            {
                // Find or create NPC for the cutscene
                NPC npcController = this.Scene.Tracker.GetEntity<NPC>();
                if (npcController == null)
                {
                    npcController = new NPC(entity.Position);
                    this.Scene.Add(npcController);
                }
                
                Vector2 playerStart = entity.Position;
                Vector2 cameraStart = this.Level.Camera.Position;
                this.Scene.Add(new CS08_StarJumpEnd(npcController, entity, playerStart, cameraStart));
            }
            
            if ((double)this.Level.CameraOffset.Y == -38.400001525878906)
            {
                if (entity.StateMachine.State != 19)
                {
                    this.CameraOffsetTimer += Engine.DeltaTime;
                    if ((double)this.CameraOffsetTimer >= 0.5)
                    {
                        this.CameraOffsetTimer = 0.0f;
                        this.Level.CameraOffset.Y = -12.8f;
                    }
                }
                else
                    this.CameraOffsetTimer = 0.0f;
            }
            else if (entity.StateMachine.State == 19)
            {
                this.CameraOffsetTimer += Engine.DeltaTime;
                if ((double)this.CameraOffsetTimer >= 0.10000000149011612)
                {
                    this.CameraOffsetTimer = 0.0f;
                    this.Level.CameraOffset.Y = -38.4f;
                }
            }
            else
                this.CameraOffsetTimer = 0.0f;
            this.CameraOffsetMarker = this.Level.Camera.Y;
        }
        else
        {
            this.Level.Session.Audio.Music.Layer(1, 1f);
            this.Level.Session.Audio.Music.Layer(2, 0.0f);
            this.Level.Session.Audio.Apply(false);
        }
        this.UpdateBlockFill();
    }

    public void InitBlockFill()
    {
        for (int index = 0; index < this.Rays.Length; ++index)
        {
            this.Rays[index].Reset();
            this.Rays[index].Percent = Calc.Random.NextFloat();
        }
    }

    public void UpdateBlockFill()
    {
        Level scene = this.Scene as Level;
        Vector2 vector = Calc.AngleToVector(-1.67079639f, 1f);
        Vector2 vector21 = new Vector2(-vector.Y, vector.X);
        int num1 = 0;
        for (int index1 = 0; index1 < this.Rays.Length; ++index1)
        {
            if ((double)this.Rays[index1].Percent >= 1.0)
                this.Rays[index1].Reset();
            this.Rays[index1].Percent += Engine.DeltaTime / this.Rays[index1].Duration;
            this.Rays[index1].Y += 8f * Engine.DeltaTime;
            float percent = this.Rays[index1].Percent;
            float x = this.Mod(this.Rays[index1].X - scene.Camera.X * 0.9f, 320f);
            float y = this.Mod(this.Rays[index1].Y - scene.Camera.Y * 0.7f, 580f) - 200f;
            float width = this.Rays[index1].Width;
            float length = this.Rays[index1].Length;
            Vector2 vector22 = new Vector2((float)(int)x, (float)(int)y);
            Color color = this.RayColor * Ease.CubeInOut(Calc.YoYo(percent));
            VertexPositionColor vertexPositionColor1 = new VertexPositionColor(new Vector3(vector22 + vector21 * width + vector * length, 0.0f), color);
            VertexPositionColor vertexPositionColor2 = new VertexPositionColor(new Vector3(vector22 - vector21 * width, 0.0f), color);
            VertexPositionColor vertexPositionColor3 = new VertexPositionColor(new Vector3(vector22 + vector21 * width, 0.0f), color);
            VertexPositionColor vertexPositionColor4 = new VertexPositionColor(new Vector3(vector22 - vector21 * width - vector * length, 0.0f), color);
            VertexPositionColor[] vertices1 = this.Vertices;
            int index2 = num1;
            int num2 = index2 + 1;
            VertexPositionColor vertexPositionColor5 = vertexPositionColor1;
            vertices1[index2] = vertexPositionColor5;
            VertexPositionColor[] vertices2 = this.Vertices;
            int index3 = num2;
            int num3 = index3 + 1;
            VertexPositionColor vertexPositionColor6 = vertexPositionColor2;
            vertices2[index3] = vertexPositionColor6;
            VertexPositionColor[] vertices3 = this.Vertices;
            int index4 = num3;
            int num4 = index4 + 1;
            VertexPositionColor vertexPositionColor7 = vertexPositionColor3;
            vertices3[index4] = vertexPositionColor7;
            VertexPositionColor[] vertices4 = this.Vertices;
            int index5 = num4;
            int num5 = index5 + 1;
            VertexPositionColor vertexPositionColor8 = vertexPositionColor2;
            vertices4[index5] = vertexPositionColor8;
            VertexPositionColor[] vertices5 = this.Vertices;
            int index6 = num5;
            int num6 = index6 + 1;
            VertexPositionColor vertexPositionColor9 = vertexPositionColor3;
            vertices5[index6] = vertexPositionColor9;
            VertexPositionColor[] vertices6 = this.Vertices;
            int index7 = num6;
            num1 = index7 + 1;
            VertexPositionColor vertexPositionColor10 = vertexPositionColor4;
            vertices6[index7] = vertexPositionColor10;
        }
        this.VertexCount = num1;
    }

    public void BeforeRender()
    {
        if (this.BlockFill == null)
            this.BlockFill = VirtualContent.CreateRenderTarget("block-fill", 320, 180);
        if (this.VertexCount <= 0)
            return;
        Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D)this.BlockFill);
        Engine.Graphics.GraphicsDevice.Clear(Color.Lerp(Color.Black, Color.LightSkyBlue, 0.3f));
        GFX.DrawVertices<VertexPositionColor>(Matrix.Identity, this.Vertices, this.VertexCount);
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
        if (this.BlockFill != null)
            this.BlockFill.Dispose();
        this.BlockFill = (VirtualRenderTarget)null;
    }

    public float Mod(float x, float m) => (x % m + m) % m;

    public struct Ray
    {
        public float X;
        public float Y;
        public float Percent;
        public float Duration;
        public float Width;
        public float Length;

        public void Reset()
        {
            this.Percent = 0.0f;
            this.X = Calc.Random.NextFloat(320f);
            this.Y = Calc.Random.NextFloat(580f);
            this.Duration = (float)(4.0 + (double)Calc.Random.NextFloat() * 8.0);
            this.Width = (float)Calc.Random.Next(8, 80);
            this.Length = (float)Calc.Random.Next(20, 200);
        }
    }
}




