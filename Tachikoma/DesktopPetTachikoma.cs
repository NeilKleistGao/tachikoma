using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tachikoma {
  public class DesktopPetTachikoma : Game {
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

    // For Test
    private Texture2D texture;
    private Vector2 position;
    // ---

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

      base.Initialize();
    }

    protected override void LoadContent() {
      spriteBatch = new SpriteBatch(GraphicsDevice);

      texture = Content.Load<Texture2D>("icon");
      position = new Vector2(0, 0);
    }

    protected override void Update(GameTime gameTime) {
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
      GraphicsDevice.Clear(new Color(255, 0, 255));

      spriteBatch.Begin();
      spriteBatch.Draw(texture, position, Color.White);
      spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
