using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;

namespace Tachikoma {
  public class DesktopPetTachikoma : Game {
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private ContextMenuStrip trayMenu;
    private NotifyIcon trayIcon;
    private ImGuiRenderer imGuiRenderer;

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
      WindowManager.SetToolWindow(hWnd);
      InitializeTray();

      imGuiRenderer = new ImGuiRenderer(this);

      root = new Tachikoma();
      root?.Initialize();
      base.Initialize();
    }

    private void InitializeTray() {
      trayMenu = new ContextMenuStrip();
      trayMenu.Items.Add("Hide", null, (s, e) => HidePet());
      trayMenu.Items.Add("Settings"); // TODO
      trayMenu.Items.Add("Exit", null, (s, e) => Exit());

      trayIcon = new NotifyIcon {
        Text = "Tachikoma",
        ContextMenuStrip = trayMenu,
        Visible = true,
        Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath)
      };
    }

    private void HidePet() {
      root.Visible = false;
      trayMenu.Items.RemoveAt(0);
      trayMenu.Items.Insert(0, new ToolStripMenuItem("Show", null, (s, e) => ShowPet()));
    }

    private void ShowPet() {
      root.Visible = true;
      trayMenu.Items.RemoveAt(0);
      trayMenu.Items.Insert(0, new ToolStripMenuItem("Hide", null, (s, e) => HidePet()));
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

    protected override void OnExiting(object sender, ExitingEventArgs args) {
      trayMenu?.Dispose();
      trayIcon?.Dispose();
      base.OnExiting(sender, args);
    }
  }
}
