using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;


namespace Fighter_Evolution_Simulatior
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Fighter> Fighters = new List<Fighter>(); //*BG => едни се мандръсат по екрана
        int k = 0; //Counter... Curently is Curently idle
        SpriteFont font; //Font for drawing purpeces
        String text = ""; //For debuging or whatever
        double frame = 1; //a frame counter
        string ChampionData=""; //For displaying the champion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Command.ResolutionWidth = 600;//Setting the game resolution.
            Command.ResolutionHeight = 400;
            Command.Rnd = new Random();//Setting the game random seed.

            //Change resolution
            graphics.PreferredBackBufferWidth = Command.ResolutionWidth;
            graphics.PreferredBackBufferHeight = Command.ResolutionHeight;
            graphics.ApplyChanges();

            TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);//increases maxium FPS.
            // graphics.SynchronizeWithVerticalRetrace = false; //This will remove the FPS reastriciton.

            Command.MobID = 0; //Intializes the MobID system.
          
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Generating fighters
            for (int i = 0; i < 250; i++) 
            {                                              
                var item = new Fighter(Content.Load<Texture2D>("warrior 15"));                
                Fighters.Add(item);
            }
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {            
            spriteBatch = new SpriteBatch(GraphicsDevice); // Create a new SpriteBatch, which can be used to draw textures.
            font = Content.Load<SpriteFont>("FontDescriptionFileArial");//loading a font            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            ///Press Escape key to quit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();           
            
            foreach (var fighter in Fighters)
                fighter.moveRandomly();

            foreach (var clone in Command.FightersFight(Fighters))
                Fighters.Add(clone);
            
            if (frame % 10 == 0)
            {
               ChampionData = "Current champion:\n\n";
               ChampionData += Fighters.Aggregate((i1, i2) => i1.kills > i2.kills ? i1 : i2).PrintItself();             
            }

            frame++;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Finds the center of the string in coordinates inside the text rectangle
            Vector2 textMiddlePoint = font.MeasureString(k.ToString()) / 2;
            Vector2 textMiddlePoint1 = font.MeasureString(text) / 2;

            spriteBatch.Begin();
            foreach (var i in Fighters)
            {
                spriteBatch.Draw(i.texture, i.location, null, i.color, i.rotation,
                    i.origin, 1f, new SpriteEffects(), i.layerDepth);
            }
            spriteBatch.DrawString(font, "Fighters left: "+Fighters.Count.ToString(), new Vector2(20,20), Color.Red, 0, textMiddlePoint, 1.4f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(font, ChampionData, new Vector2(420,20), Color.Black, 0, textMiddlePoint, 1.2f, SpriteEffects.None, 0.5f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
