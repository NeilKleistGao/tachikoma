using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tachikoma {
  public class DesktopPetTachikoma : Game {
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

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
      WindowManager.SetTransparent(hWnd);

      base.Initialize();
    }

    protected override void LoadContent() {
      spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime) {
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
      GraphicsDevice.Clear(new Color(255, 0, 255));

      base.Draw(gameTime);
    }
  }
}
