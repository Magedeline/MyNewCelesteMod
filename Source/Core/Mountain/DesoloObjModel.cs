#nullable enable
using System.Globalization;
using System.Runtime.CompilerServices;
using Celeste.Mod.Core;

namespace DesoloZantas.Core.Core.Mountain
{
    /// <summary>
    /// Custom ObjModel implementation for DesoloZantas mod.
    /// Handles loading and rendering 3D .obj models for custom mountain/overworld visuals.
    /// </summary>
    public class DesoloObjModel : IDisposable
    {
        /// <summary>
        /// Represents a named mesh within the model
        /// </summary>
        public class Mesh
        {
            public string Name = "";
            public DesoloObjModel? Model;
            public int VertexStart;
            public int VertexCount;
        }

        /// <summary>
        /// All meshes contained in this model
        /// </summary>
        public List<Mesh> Meshes = new List<Mesh>();

        /// <summary>
        /// GPU vertex buffer for rendering
        /// </summary>
        public VertexBuffer? Vertices;

        private VertexPositionTexture[] verts = Array.Empty<VertexPositionTexture>();
        private object? _Vertices_QueuedLoadLock;
        private ValueTask<VertexBuffer?> _Vertices_QueuedLoad;

        /// <summary>
        /// Whether this model has been disposed
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Number of vertices in the model
        /// </summary>
        public int VertexCount => verts?.Length ?? 0;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool ResetVertexBuffer()
        {
            if (Everest.Flags.IsHeadless)
            {
                return true;
            }

            if (Vertices != null && !Vertices.IsDisposed && !Vertices.GraphicsDevice.IsDisposed)
            {
                return false;
            }

            object? queuedLoadLock = _Vertices_QueuedLoadLock;
            if (queuedLoadLock != null)
            {
                lock (queuedLoadLock)
                {
                    if (_Vertices_QueuedLoadLock == null)
                    {
                        return true;
                    }
                    if (MainThreadHelper.IsMainThread)
                    {
                        _Vertices_QueuedLoadLock = null;
                    }
                }
                if (!MainThreadHelper.IsMainThread)
                {
                    _ = _Vertices_QueuedLoad.Result;
                    return true;
                }
            }

            if (((!CoreModule.Settings.ThreadedGL) ?? true) && !MainThreadHelper.IsMainThread && queuedLoadLock == null)
            {
                lock (queuedLoadLock = new object())
                {
                    _Vertices_QueuedLoadLock = queuedLoadLock;
                    _Vertices_QueuedLoad = MainThreadHelper.Schedule(delegate
                    {
                        lock (queuedLoadLock)
                        {
                            if (_Vertices_QueuedLoadLock == null)
                            {
                                return Vertices;
                            }
                            Vertices?.Dispose();
                            Vertices = new VertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexPositionTexture), verts.Length, BufferUsage.None);
                            Vertices.SetData(verts);
                            _Vertices_QueuedLoadLock = null;
                            return Vertices;
                        }
                    });
                }
                return true;
            }

            return CreateVertexBuffer();
        }

        /// <summary>
        /// Creates or recreates the vertex buffer on the GPU
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool CreateVertexBuffer()
        {
            if (Vertices == null || Vertices.IsDisposed || Vertices.GraphicsDevice.IsDisposed)
            {
                Vertices = new VertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexPositionTexture), verts.Length, BufferUsage.None);
                Vertices.SetData(verts);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reassigns vertex data to the GPU buffer
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ReassignVertices()
        {
            if (!ResetVertexBuffer())
            {
                Vertices?.SetData(verts);
            }
        }

        /// <summary>
        /// Draw the model using the specified effect
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Draw(Effect effect)
        {
            if (Vertices == null || verts.Length == 0)
                return;

            ResetVertexBuffer();
            Engine.Graphics.GraphicsDevice.SetVertexBuffer(Vertices);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Engine.Graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, Vertices!.VertexCount / 3);
            }
        }

        /// <summary>
        /// Draw a specific mesh from the model
        /// </summary>
        public void DrawMesh(Effect effect, int meshIndex)
        {
            if (Vertices == null || meshIndex < 0 || meshIndex >= Meshes.Count)
                return;

            var mesh = Meshes[meshIndex];
            if (mesh.VertexCount == 0)
                return;

            ResetVertexBuffer();
            Engine.Graphics.GraphicsDevice.SetVertexBuffer(Vertices);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Engine.Graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, mesh.VertexStart, mesh.VertexCount / 3);
            }
        }

        /// <summary>
        /// Draw a specific mesh by name
        /// </summary>
        public void DrawMesh(Effect effect, string meshName)
        {
            for (int i = 0; i < Meshes.Count; i++)
            {
                if (Meshes[i].Name == meshName)
                {
                    DrawMesh(effect, i);
                    return;
                }
            }
        }

        /// <summary>
        /// Dispose of GPU resources
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            Vertices?.Dispose();
            Vertices = null;
            Meshes.Clear();
            verts = Array.Empty<VertexPositionTexture>();
            IsDisposed = true;
        }

        /// <summary>
        /// Create an ObjModel from a file path
        /// Supports both .obj and .export (binary) formats
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static DesoloObjModel Create(string filename)
        {
            DesoloObjModel model = new DesoloObjModel();
            List<VertexPositionTexture> vertList = new List<VertexPositionTexture>();
            List<Vector3> positions = new List<Vector3>();
            List<Vector2> texCoords = new List<Vector2>();
            Mesh? currentMesh = null;

            // Check for pre-exported binary format first
            if (File.Exists(filename + ".export"))
            {
                using BinaryReader reader = new BinaryReader(File.OpenRead(filename + ".export"));
                int meshCount = reader.ReadInt32();

                for (int i = 0; i < meshCount; i++)
                {
                    if (currentMesh != null)
                    {
                        currentMesh.VertexCount = vertList.Count - currentMesh.VertexStart;
                    }

                    currentMesh = new Mesh
                    {
                        Name = reader.ReadString(),
                        VertexStart = vertList.Count,
                        Model = model
                    };
                    model.Meshes.Add(currentMesh);

                    // Read positions
                    int posCount = reader.ReadInt32();
                    for (int j = 0; j < posCount; j++)
                    {
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        positions.Add(new Vector3(x, y, z));
                    }

                    // Read texture coordinates
                    int texCount = reader.ReadInt32();
                    for (int k = 0; k < texCount; k++)
                    {
                        float u = reader.ReadSingle();
                        float v = reader.ReadSingle();
                        texCoords.Add(new Vector2(u, v));
                    }

                    // Read face indices
                    int faceCount = reader.ReadInt32();
                    for (int l = 0; l < faceCount; l++)
                    {
                        int posIndex = reader.ReadInt32() - 1;
                        int texIndex = reader.ReadInt32() - 1;
                        vertList.Add(new VertexPositionTexture
                        {
                            Position = positions[posIndex],
                            TextureCoordinate = texCoords[texIndex]
                        });
                    }
                }
            }
            else
            {
                // Parse standard .obj format
                using StreamReader reader = new StreamReader(filename);
                string? line;

                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0)
                        continue;

                    switch (parts[0])
                    {
                        case "o":
                        case "g": // Support both object and group names
                            if (currentMesh != null)
                            {
                                currentMesh.VertexCount = vertList.Count - currentMesh.VertexStart;
                            }
                            currentMesh = new Mesh
                            {
                                Name = parts.Length > 1 ? parts[1] : "unnamed",
                                VertexStart = vertList.Count,
                                Model = model
                            };
                            model.Meshes.Add(currentMesh);
                            break;

                        case "v":
                            if (parts.Length >= 4)
                            {
                                Vector3 pos = new Vector3(
                                    ParseFloat(parts[1]),
                                    ParseFloat(parts[2]),
                                    ParseFloat(parts[3])
                                );
                                positions.Add(pos);
                            }
                            break;

                        case "vt":
                            if (parts.Length >= 3)
                            {
                                Vector2 tex = new Vector2(
                                    ParseFloat(parts[1]),
                                    ParseFloat(parts[2])
                                );
                                texCoords.Add(tex);
                            }
                            break;

                        case "f":
                            // Handle triangles (and quads by triangulating)
                            ParseFace(parts, positions, texCoords, vertList);
                            break;
                    }
                }
            }

            if (currentMesh != null)
            {
                currentMesh.VertexCount = vertList.Count - currentMesh.VertexStart;
            }

            // If no meshes were defined, create a default one
            if (model.Meshes.Count == 0 && vertList.Count > 0)
            {
                model.Meshes.Add(new Mesh
                {
                    Name = "default",
                    VertexStart = 0,
                    VertexCount = vertList.Count,
                    Model = model
                });
            }

            model.verts = vertList.ToArray();
            model.ResetVertexBuffer();
            return model;
        }

        /// <summary>
        /// Create an ObjModel from a stream (for loading from mod packages)
        /// </summary>
        public static DesoloObjModel CreateFromStream(Stream stream, string filename)
        {
            if (Everest.Flags.IsHeadless)
            {
                return new DesoloObjModel();
            }

            DesoloObjModel model = new DesoloObjModel();
            List<VertexPositionTexture> vertList = new List<VertexPositionTexture>();
            List<Vector3> positions = new List<Vector3>();
            List<Vector2> texCoords = new List<Vector2>();
            Mesh? currentMesh = null;

            if (filename.EndsWith(".export"))
            {
                using BinaryReader reader = new BinaryReader(stream);
                int meshCount = reader.ReadInt32();

                for (int i = 0; i < meshCount; i++)
                {
                    if (currentMesh != null)
                    {
                        currentMesh.VertexCount = vertList.Count - currentMesh.VertexStart;
                    }

                    currentMesh = new Mesh
                    {
                        Name = reader.ReadString(),
                        VertexStart = vertList.Count,
                        Model = model
                    };
                    model.Meshes.Add(currentMesh);

                    int posCount = reader.ReadInt32();
                    for (int j = 0; j < posCount; j++)
                    {
                        positions.Add(new Vector3(
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle()
                        ));
                    }

                    int texCount = reader.ReadInt32();
                    for (int k = 0; k < texCount; k++)
                    {
                        texCoords.Add(new Vector2(
                            reader.ReadSingle(),
                            reader.ReadSingle()
                        ));
                    }

                    int faceCount = reader.ReadInt32();
                    for (int l = 0; l < faceCount; l++)
                    {
                        int posIndex = reader.ReadInt32() - 1;
                        int texIndex = reader.ReadInt32() - 1;
                        vertList.Add(new VertexPositionTexture
                        {
                            Position = positions[posIndex],
                            TextureCoordinate = texCoords[texIndex]
                        });
                    }
                }
            }
            else
            {
                using StreamReader reader = new StreamReader(stream);
                string? line;

                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0)
                        continue;

                    switch (parts[0])
                    {
                        case "o":
                        case "g":
                            if (currentMesh != null)
                            {
                                currentMesh.VertexCount = vertList.Count - currentMesh.VertexStart;
                            }
                            currentMesh = new Mesh
                            {
                                Name = parts.Length > 1 ? parts[1] : "unnamed",
                                VertexStart = vertList.Count,
                                Model = model
                            };
                            model.Meshes.Add(currentMesh);
                            break;

                        case "v":
                            if (parts.Length >= 4)
                            {
                                positions.Add(new Vector3(
                                    ParseFloat(parts[1]),
                                    ParseFloat(parts[2]),
                                    ParseFloat(parts[3])
                                ));
                            }
                            break;

                        case "vt":
                            if (parts.Length >= 3)
                            {
                                texCoords.Add(new Vector2(
                                    ParseFloat(parts[1]),
                                    ParseFloat(parts[2])
                                ));
                            }
                            break;

                        case "f":
                            ParseFace(parts, positions, texCoords, vertList);
                            break;
                    }
                }
            }

            if (currentMesh != null)
            {
                currentMesh.VertexCount = vertList.Count - currentMesh.VertexStart;
            }

            if (model.Meshes.Count == 0 && vertList.Count > 0)
            {
                model.Meshes.Add(new Mesh
                {
                    Name = "default",
                    VertexStart = 0,
                    VertexCount = vertList.Count,
                    Model = model
                });
            }

            model.verts = vertList.ToArray();
            model.ResetVertexBuffer();
            return model;
        }

        /// <summary>
        /// Load a model from the mod's content directory
        /// </summary>
        public static DesoloObjModel? LoadFromMod(string modPath)
        {
            try
            {
                // Try to find the model in the mod content
                string fullPath = Path.Combine("Mods", "DesoloZantas", modPath);
                
                if (Everest.Content.TryGet<ModAsset>(modPath, out var asset) && asset != null)
                {
                    using var stream = asset.Stream;
                    if (stream != null)
                    {
                        return CreateFromStream(stream, modPath);
                    }
                }

                IngesteLogger.Warn($"Could not find model at path: {modPath}");
                return null;
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Failed to load model from {modPath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Parse a float value using invariant culture
        /// </summary>
        private static float ParseFloat(string data)
        {
            return float.Parse(data, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parse a face line from .obj format
        /// Handles triangles and quads (by triangulating)
        /// </summary>
        private static void ParseFace(string[] parts, List<Vector3> positions, List<Vector2> texCoords, List<VertexPositionTexture> vertList)
        {
            List<VertexPositionTexture> faceVerts = new List<VertexPositionTexture>();

            for (int i = 1; i < parts.Length; i++)
            {
                VertexPositionTexture vert = default;
                string[] indices = parts[i].Split('/');

                // Position index
                if (indices.Length > 0 && indices[0].Length > 0)
                {
                    int posIndex = int.Parse(indices[0]) - 1;
                    if (posIndex >= 0 && posIndex < positions.Count)
                    {
                        vert.Position = positions[posIndex];
                    }
                }

                // Texture coordinate index
                if (indices.Length > 1 && indices[1].Length > 0)
                {
                    int texIndex = int.Parse(indices[1]) - 1;
                    if (texIndex >= 0 && texIndex < texCoords.Count)
                    {
                        vert.TextureCoordinate = texCoords[texIndex];
                    }
                }

                faceVerts.Add(vert);
            }

            // Triangulate the face
            // For triangles: just add the 3 vertices
            // For quads: split into 2 triangles (0,1,2) and (0,2,3)
            if (faceVerts.Count >= 3)
            {
                vertList.Add(faceVerts[0]);
                vertList.Add(faceVerts[1]);
                vertList.Add(faceVerts[2]);

                // Handle quads by adding second triangle
                if (faceVerts.Count >= 4)
                {
                    vertList.Add(faceVerts[0]);
                    vertList.Add(faceVerts[2]);
                    vertList.Add(faceVerts[3]);
                }

                // Handle n-gons (5+ vertices) by fan triangulation
                for (int i = 4; i < faceVerts.Count; i++)
                {
                    vertList.Add(faceVerts[0]);
                    vertList.Add(faceVerts[i - 1]);
                    vertList.Add(faceVerts[i]);
                }
            }
        }

        /// <summary>
        /// Get a mesh by name
        /// </summary>
        public Mesh? GetMesh(string name)
        {
            return Meshes.Find(m => m.Name == name);
        }

        /// <summary>
        /// Check if the model contains a mesh with the given name
        /// </summary>
        public bool HasMesh(string name)
        {
            return Meshes.Exists(m => m.Name == name);
        }
    }
}
