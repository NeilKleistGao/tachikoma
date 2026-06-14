using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Tachikoma {
  internal class ImGuiRenderer {
    private Game game;

    private int textureID = 0;
    private IntPtr? fontTextureID;
    private Dictionary<IntPtr, Texture2D> textures = new();
    private BasicEffect effect;
    private RasterizerState rasterizerState;

    private byte[] vertexData;
    private VertexBuffer vertexBuffer;
    private int vertexBufferSize;

    private byte[] indexData;
    private IndexBuffer indexBuffer;
    private int indexBufferSize;

    public ImGuiRenderer(Game game) {
      this.game = game;
      var context = ImGui.CreateContext();
      ImGui.SetCurrentContext(context);

      var io = ImGui.GetIO();
      int width = SystemInformation.WorkingArea.Size.Width; // TODO: refactor
      int height = SystemInformation.WorkingArea.Size.Height;
      io.DisplaySize = new System.Numerics.Vector2(width, height);

      rasterizerState = new RasterizerState() {
        CullMode = CullMode.None,
        DepthBias = 0,
        FillMode = FillMode.Solid,
        MultiSampleAntiAlias = false,
        ScissorTestEnable = true,
        SlopeScaleDepthBias = 0
      };

      SetupInput();
    }

    private void SetupInput() {
      var io = ImGui.GetIO();
      game.Window.TextInput += (s, c) => {
        if (c.Character == '\t') { return; }
        io.AddInputCharacter(c.Character);
      };
    }

    public void RebuildFontAltas() {
      var io = ImGui.GetIO();
      unsafe {
        io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out int bytesPerPixel);
        var pixels = new byte[width * height * bytesPerPixel];
        Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length);

        var texture2D = new Texture2D(game.GraphicsDevice, width, height, false, SurfaceFormat.Color);
        texture2D.SetData(pixels);

        if (fontTextureID.HasValue) {
          UnbindTexture(fontTextureID.Value);
        }
        fontTextureID = BindTexture(texture2D);
      }

      io.Fonts.SetTexID(fontTextureID.Value);
      io.Fonts.ClearTexData();
    }

    public IntPtr BindTexture(Texture2D texture) {
      var id = new IntPtr(textureID++);
      textures[id] = texture;
      return id;
    }

    public void UnbindTexture(IntPtr id) {
      textures.Remove(id);
    }

    public void StartLayout(GameTime gameTime) {
      ImGui.GetIO().DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
      // TODO: update input
      ImGui.NewFrame();
    }

    public void FinishLayout() {
      ImGui.Render();
      // TODO
    }

    protected Effect UpdateEffect(Texture2D texture) {
      var graphicsDevice = game.GraphicsDevice;
      effect = effect ?? new BasicEffect(graphicsDevice);

      var io = ImGui.GetIO();

      effect.World = Matrix.Identity;
      effect.View = Matrix.Identity;
      effect.Projection = Matrix.CreateOrthographicOffCenter(0f, io.DisplaySize.X, io.DisplaySize.Y, 0f, -1f, 1f);
      effect.TextureEnabled = true;
      effect.Texture = texture;
      effect.VertexColorEnabled = true;

      return effect;
    }

    private void RenderDrawData(ImDrawDataPtr drawData) {
      var graphicsDevice = game.GraphicsDevice;
      var lastViewport = graphicsDevice.Viewport;
      var lastScissorBox = graphicsDevice.ScissorRectangle;
      var lastRasterizer = graphicsDevice.RasterizerState;
      var lastDepthStencil = graphicsDevice.DepthStencilState;
      var lastBlendFactor = graphicsDevice.BlendFactor;
      var lastBlendState = graphicsDevice.BlendState;

      graphicsDevice.BlendFactor = Color.White;
      graphicsDevice.BlendState = BlendState.NonPremultiplied;
      graphicsDevice.RasterizerState = rasterizerState;
      graphicsDevice.RasterizerState = RasterizerState.CullNone;
      graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

      // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
      drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);

      // Setup projection
      graphicsDevice.Viewport = new Viewport(0, 0, graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight);

      UpdateBuffers(drawData);

      RenderCommandLists(drawData);

      // Restore modified state
      graphicsDevice.Viewport = lastViewport;
      graphicsDevice.ScissorRectangle = lastScissorBox;
      graphicsDevice.RasterizerState = lastRasterizer;
      graphicsDevice.DepthStencilState = lastDepthStencil;
      graphicsDevice.BlendState = lastBlendState;
      graphicsDevice.BlendFactor = lastBlendFactor;
    }

    private unsafe void UpdateBuffers(ImDrawDataPtr drawData) {
      if (drawData.TotalVtxCount == 0) {
        return;
      }

      var graphicsDevice = game.GraphicsDevice;

      // Expand buffers if we need more room
      if (drawData.TotalVtxCount > vertexBufferSize) {
        vertexBuffer?.Dispose();

        vertexBufferSize = (int)(drawData.TotalVtxCount * 1.5f);
        vertexBuffer = new VertexBuffer(graphicsDevice, DrawVertDeclaration.Declaration, vertexBufferSize, BufferUsage.None);
        vertexData = new byte[vertexBufferSize * DrawVertDeclaration.Size];
      }

      if (drawData.TotalIdxCount > indexBufferSize) {
        indexBuffer?.Dispose();

        indexBufferSize = (int)(drawData.TotalIdxCount * 1.5f);
        indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indexBufferSize, BufferUsage.None);
        indexData = new byte[indexBufferSize * sizeof(ushort)];
      }

      // Copy ImGui's vertices and indices to a set of managed byte arrays
      int vtxOffset = 0;
      int idxOffset = 0;

      for (int n = 0; n < drawData.CmdListsCount; n++) {
        ImDrawListPtr cmdList = drawData.CmdLists[n];

        fixed (void* vtxDstPtr = &vertexData[vtxOffset * DrawVertDeclaration.Size])
        fixed (void* idxDstPtr = &indexData[idxOffset * sizeof(ushort)]) {
          Buffer.MemoryCopy((void*)cmdList.VtxBuffer.Data, vtxDstPtr, vertexData.Length, cmdList.VtxBuffer.Size * DrawVertDeclaration.Size);
          Buffer.MemoryCopy((void*)cmdList.IdxBuffer.Data, idxDstPtr, indexData.Length, cmdList.IdxBuffer.Size * sizeof(ushort));
        }

        vtxOffset += cmdList.VtxBuffer.Size;
        idxOffset += cmdList.IdxBuffer.Size;
      }

      // Copy the managed byte arrays to the gpu vertex- and index buffers
      vertexBuffer.SetData(vertexData, 0, drawData.TotalVtxCount * DrawVertDeclaration.Size);
      indexBuffer.SetData(indexData, 0, drawData.TotalIdxCount * sizeof(ushort));
    }

    private unsafe void RenderCommandLists(ImDrawDataPtr drawData) {
      var graphicsDevice = game.GraphicsDevice;

      graphicsDevice.SetVertexBuffer(vertexBuffer);
      graphicsDevice.Indices = indexBuffer;

      int vtxOffset = 0;
      int idxOffset = 0;

      for (int n = 0; n < drawData.CmdListsCount; n++) {
        ImDrawListPtr cmdList = drawData.CmdLists[n];

        for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++) {
          ImDrawCmdPtr drawCmd = cmdList.CmdBuffer[cmdi];

          if (drawCmd.ElemCount == 0) {
            continue;
          }

          if (!textures.ContainsKey(drawCmd.TextureId)) {
            throw new InvalidOperationException($"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings");
          }

          graphicsDevice.ScissorRectangle = new Rectangle(
              (int)drawCmd.ClipRect.X,
              (int)drawCmd.ClipRect.Y,
              (int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
              (int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y)
          );

          var effect = UpdateEffect(textures[drawCmd.TextureId]);

          foreach (var pass in effect.CurrentTechnique.Passes) {
            pass.Apply();

#pragma warning disable CS0618 // // FNA does not expose an alternative method.
            graphicsDevice.DrawIndexedPrimitives(
                primitiveType: Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList,
                baseVertex: (int)drawCmd.VtxOffset + vtxOffset,
                minVertexIndex: 0,
                numVertices: cmdList.VtxBuffer.Size,
                startIndex: (int)drawCmd.IdxOffset + idxOffset,
                primitiveCount: (int)drawCmd.ElemCount / 3
            );
#pragma warning restore CS0618
          }
        }

        vtxOffset += cmdList.VtxBuffer.Size;
        idxOffset += cmdList.IdxBuffer.Size;
      }
    }
  }
}
