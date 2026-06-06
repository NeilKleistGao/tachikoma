using Microsoft.Xna.Framework.Graphics;

namespace Tachikoma {
  public class DesktopPetTachikoma : Game {
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

    private CanvasItem root = null;

    public DesktopPetTachikoma() {
      graphics = new GraphicsDeviceManager(this);
      graphics.IsFullScreen = true;
      graphics.HardwareModeSwitch = false;
      Content.RootDirectory = "Content";
      IsMouseVisible = true;
    }

    protected override void Initialize() {
      Window.IsBorderless = true;
      var hWnd = Window.Handle;
      WindowManager.SetColorKeyTransparent(hWnd, 0xFF00FF);
      WindowManager.SetTopMost(hWnd);

      root = new Tachikoma();
      root?.Initialize();
      base.Initialize();
    }

    protected override void LoadContent() {
      spriteBatch = new SpriteBatch(GraphicsDevice);
      root?.LoadContent(Content);
    }

    protected override void Update(GameTime gameTime) {
      base.Update(gameTime);
      root?.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
      GraphicsDevice.Clear(new Color(255, 0, 255));

      spriteBatch.Begin();
      root?.Draw(spriteBatch, gameTime);
      spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
