using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
/*
namespace Xbox360Game
{

    public static class Menu
    {
        public static InputState newstate;
        public static InputState oldstate;
        public static Game g;
        public static SpriteFont fontMenu;
        public static MainMenu mainMenu;
        public static InGameMenu inGameMenu;
        public static GameOverMenu gameOverMenu;

        public static SoundEffect sMenu;

        //Static constructor will be called on first use of static class
        public static void create()
        {
            mainMenu = new MainMenu();
            inGameMenu = new InGameMenu();
            gameOverMenu = new GameOverMenu();
            oldstate = InputState.getCurrent();
            
        }
        public static void updateDrawMainMenu(SpriteBatch s)
        {
            newstate = InputState.getCurrent();
            mainMenu.updateDraw(s);
            oldstate = newstate;
        }
        public static void updateDrawInGameMenu(SpriteBatch s)
        {
            newstate = InputState.getCurrent();
            inGameMenu.updateDraw(s);
            oldstate = newstate;
        }
        public static void updateDrawGameOverMenu(SpriteBatch s)
        {
            newstate = InputState.getCurrent();
            gameOverMenu.updateDraw(s);
            oldstate = newstate;
        }
        public static bool anyDownClicked()
        {
            return newstate.anyDownPressed() && !oldstate.anyDownPressed();
        }
        public static bool anyUpClicked()
        {
            return newstate.anyUpPressed() && !oldstate.anyUpPressed();
        }
        public static bool anyRightClicked()
        {
            return newstate.anyRightPressed() && !oldstate.anyRightPressed();
        }
        public static bool anyLeftClicked()
        {
            return newstate.anyLeftPressed() && !oldstate.anyLeftPressed();
        }
        public static bool anyEnterClicked()
        {
            return newstate.anyEnterPressed() && !oldstate.anyEnterPressed();
        }
        public static bool anybClicked()
        {
            return newstate.anybPressed() && !oldstate.anybPressed();
        }
        public static bool anyStartClicked()
        {
            return newstate.anyStartPressed() && !oldstate.anyStartPressed();
        }
        public static bool anyBigButtonPressed()
        {
            return newstate.anyBigButtonPressed() && !oldstate.anyBigButtonPressed();
        }
    }

    public class MainMenu : MenuComponent
    {
        private MultiplayerMenu multiplayerMenu = new MultiplayerMenu();
        private InstructionsScreen instructionsScreen = new InstructionsScreen();
        private HighScoresScreen highScoresScreen = new HighScoresScreen();
        private CreditsScreen creditsScreen = new CreditsScreen();
        private List<MenuComponent> nextSubComponents = new List<MenuComponent>();
        private List<MenuOption> mainScreenOptions = new List<MenuOption>();
        private int selectedMainMenuOption = 0;
        public static Texture2D blueBlocker;
        public static Texture2D gameLogo;
        public static Texture2D screenLine;
        public static Texture2D shortLine;
        public static Texture2D bottomOptionOutline;
        public static Texture2D tinyLine;
        public static Texture2D halfTinyLine;
        public static Texture2D halfTinyLineDark;
        public static Texture2D tinyLineDark;
        public static Texture2D bottomOptionOutlineDark;
        public static Texture2D shortLineDark;
        public static Texture2D screenLineDark;

        public static Texture2D singlePlayerShip;
        public static Texture2D multiPlayerShip;

        public List<Vector3> stars = new List<Vector3>();   //With components (x, y, leftward velocity)

        public MainMenu()
        {
            mainScreenOptions.Add(new oSinglePlayer());
            mainScreenOptions.Add(new oMultiplayer());
            mainScreenOptions.Add(new oInstructions());
            mainScreenOptions.Add(new oHighScores());
            mainScreenOptions.Add(new oCredits());
            mainScreenOptions.Add(new oQuit());
            foreach (MenuOption mo in mainScreenOptions)
                mo.parent = this;

            for (int i = 0; i < 1000; i++)
                updateStars();

            setToMainScreen();
            select(0);
        }

        public void setToMainScreen()
        {
            nextSubComponents.Clear();
            foreach (MenuOption mo in mainScreenOptions)
                nextSubComponents.Add(mo);
        }

        public void setToInstructionsScreen()
        {
            nextSubComponents.Clear();
            instructionsScreen.parent = this;
            nextSubComponents.Add(instructionsScreen);
        }

        public void setToHighScoresScreen()
        {
            nextSubComponents.Clear();
            highScoresScreen.parent = this;
            nextSubComponents.Add(highScoresScreen);
        }

        public void setToCreditsScreen()
        {
            nextSubComponents.Clear();
            creditsScreen.parent = this;
            nextSubComponents.Add(creditsScreen);
        }

        public void setToMuliplayerMenu()
        {
            nextSubComponents.Clear();
            multiplayerMenu.parent = this;
            nextSubComponents.Add(multiplayerMenu);
        }

        private void select(int i)
        {
            if (Menu.sMenu != null) Menu.sMenu.Play();
            if (i >= mainScreenOptions.Count) i = 0;
            if (i < 0) i = mainScreenOptions.Count - 1;
            selectedMainMenuOption = i;
            foreach (MenuOption mo in mainScreenOptions)
                if (mo.selected) mo.deselect();
            mainScreenOptions[i].select();
        }

        public override void updateDraw(SpriteBatch s)
        {
            //Update
            subComponents.Clear();
            foreach (MenuComponent mc in nextSubComponents)
                subComponents.Add(mc);
            base.updateDraw(s);
            if (Menu.anyRightClicked() || Menu.anyDownClicked()) select(selectedMainMenuOption + 1);
            if (Menu.anyLeftClicked() || Menu.anyUpClicked()) select(selectedMainMenuOption - 1);
            
            subComponents.Clear();
           
            foreach (MenuComponent mc in subComponents)
                if (!nextSubComponents.Contains(mc))nextSubComponents.Add(mc);

            //Stars
            updateStars();
            foreach (Vector3 star in stars) s.Draw(Starfield.starTextures[0], new Rectangle((int)star.X, (int)star.Y, (int)(star.Z), (int)(star.Z)), Color.White);

            //Draw logo
            s.Draw(gameLogo, new Rectangle(Menu.g.width / 2 - gameLogo.Width / 2, 85, gameLogo.Width, gameLogo.Height), Color.White);
            
        }

        private void updateStars()
        {

            if (Game.randy.NextDouble() < .4)
            {
                stars.Add(new Vector3(Menu.g.width, (float)Game.randy.NextDouble() * Menu.g.height, 2f / ((float)Math.Pow((Game.randy.NextDouble() * 2 + .5), 2))));
            }
            for (int i = 0; i < stars.Count; i++)
            {
                //Update
                stars[i] = new Vector3(stars[i].X - stars[i].Z, stars[i].Y, stars[i].Z);
                if (stars[i].X < -10)
                {
                    stars.RemoveAt(i);
                    i--;
                }
            }
        }

        public class oSinglePlayer : MenuOption
        {
            int topLineY = 255;
            int bottomLineY = 317;
            int textY = 289;
            Color lightLineColor = Color.Cyan;
            Color darkLineColor = Color.DarkCyan;

            public static void beginSinglePlayerGame()
            {
                Menu.g.clearEverythingExceptCamera();
                //figure out which controller selected single player because that'll be the one playing
                PlayerIndex pi = PlayerIndex.One;
                if (GamePad.GetState(PlayerIndex.Two).Buttons.A == ButtonState.Pressed) pi = PlayerIndex.Two;
                if (GamePad.GetState(PlayerIndex.Three).Buttons.A == ButtonState.Pressed) pi = PlayerIndex.Three;
                if (GamePad.GetState(PlayerIndex.Four).Buttons.A == ButtonState.Pressed) pi = PlayerIndex.Four;
                //Start game
                Vector2 location = new Vector2(Menu.g.width / 2, Menu.g.height / 2);
                Player p = new Player(0, location, Menu.g, Game.playerKeyControls[0], pi);
                Menu.g.players.Add(p);
                Menu.g.targets.Add(p);
                Menu.g.pe.createPlayerIntro(location);
                Menu.g.gameType = Game.GameType.singlePlayer;
                Menu.g.st = Game.state.intro;
                MediaPlayer.Stop();
                MediaPlayer.Play(Game.themeSong);
            }

            public override void updateDraw(SpriteBatch s)
            {
                base.updateDraw(s);
                
                //Update
                if (selected && Menu.anyEnterClicked())
                {
                    Menu.sMenu.Play(1f, 1f, 0f);
                    //Start single player game:
                    Menu.g.pe.clear();
                    Menu.g.resetCamera();
                    beginSinglePlayerGame();
                }

                //Draw
                if (selected)
                {
                    s.DrawString(Menu.fontMenu, "Single Player", new Vector2((float)(440 + 00*Math.Sin(sel/255.0)), y + textY), Color.White);
                    s.Draw(screenLine, new Vector2(0, y + topLineY), darkLineColor);
                    s.Draw(screenLine, new Vector2(0, y + bottomLineY), darkLineColor);
                    s.Draw(screenLine, new Vector2(0, y + topLineY), new Color(lightLineColor, (byte)(sel)));
                    s.Draw(screenLine, new Vector2(0, y + bottomLineY), new Color(lightLineColor, (byte)(sel)));

                    for (int i = 0; i < 19; i++)
                    {
                        s.Draw(MainMenu.shortLine, new Vector2(i * 15, topLineY + 25), lightLineColor);
                        s.Draw(MainMenu.shortLine, new Vector2(Menu.g.width - i * 15, topLineY + 25), lightLineColor);
                    }

                }
                else
                {
                    s.DrawString(Menu.fontMenu, "Single Player", new Vector2((float)(440 + 00 * Math.Sin((255-des) / 255.0)), y + textY), Menu.g.fontColor);
                    s.Draw(screenLine, new Vector2(0, y + topLineY), darkLineColor);
                    s.Draw(screenLine, new Vector2(0, y + bottomLineY), darkLineColor);
                    s.Draw(screenLine, new Vector2(0, y + topLineY), new Color(lightLineColor, (byte)(255-des)));
                    s.Draw(screenLine, new Vector2(0, y + bottomLineY), new Color(lightLineColor, (byte)(255-des)));

                    for (int i = 0; i < 19; i++)
                    {
                        s.Draw(MainMenu.shortLine, new Vector2(i * 15, topLineY + 25), darkLineColor);
                        s.Draw(MainMenu.shortLine, new Vector2(Menu.g.width - i * 15, topLineY + 25), darkLineColor);
                    }
                }


            }
        }

        public class oMultiplayer : MenuOption
        {
            int topLineY = 340;
            int bottomLineY = 402;
            int textY = 374;
            Color lightLineColor = Color.Orange;
            Color darkLineColor = Color.Chocolate;

            public override void updateDraw(SpriteBatch s)
            {
                base.updateDraw(s);

                //Update
                if (selected && Menu.anyEnterClicked())
                {
                    Menu.sMenu.Play(1f, 1f, 0f);
                    ((MainMenu)parent).setToMuliplayerMenu();
                }
                
                //Draw
                if (selected)
                {
                    s.DrawString(Menu.fontMenu, "Multi Player", new Vector2((float)(448 + 00 * Math.Sin(sel / 255.0)), textY), Color.White);
                    s.Draw(screenLine, new Vector2(0, y + topLineY), darkLineColor);
                    s.Draw(screenLine, new Vector2(0, y + bottomLineY), darkLineColor);
                    s.Draw(screenLine, new Vector2(0, y + topLineY), new Color(lightLineColor, (byte)(sel)));
                    s.Draw(screenLine, new Vector2(0, y + bottomLineY), new Color(lightLineColor, (byte)(sel)));

                    for (int i = 0; i < 19; i++)
                    {
                        s.Draw(MainMenu.shortLine, new Vector2(i * 15, topLineY + 25), lightLineColor);
                        s.Draw(MainMenu.shortLine, new Vector2(Menu.g.width - i * 15, topLineY + 25), lightLineColor);
                    }
                }
                else
                {
                    s.DrawString(Menu.fontMenu, "Multi Player", new Vector2((float)(448 + 00 * Math.Sin((255 - des) / 255.0)), textY), Menu.g.fontColor);
                    s.Draw(screenLine, new Vector2(0, y + topLineY), darkLineColor);
                    s.Draw(screenLine, new Vector2(0, y + bottomLineY), darkLineColor);
                    s.Draw(screenLine, new Vector2(0, y + topLineY), new Color(lightLineColor, (byte)(255 - des)));
                    s.Draw(screenLine, new Vector2(0, y + bottomLineY), new Color(lightLineColor, (byte)(255 - des)));

                    for (int i = 0; i < 19; i++)
                    {
                        s.Draw(MainMenu.shortLine, new Vector2(i * 15, topLineY + 25), darkLineColor);
                        s.Draw(MainMenu.shortLine, new Vector2(Menu.g.width - i * 15, topLineY + 25), darkLineColor);
                    }
                }

            }
        }

        public class oInstructions : MenuOption
        {
            Color lightLineColor = Color.Red;
            Color darkLineColor = Color.DarkRed;
            int x = 140;

            public override void updateDraw(SpriteBatch s)
            {
                base.updateDraw(s);
                //Update
                if (selected && Menu.anyEnterClicked())
                {
                    Menu.sMenu.Play(1f, 1f, 0f);
                    ((MainMenu)parent).setToInstructionsScreen();
                }

                //Draw
                if (selected)
                {
                    s.DrawString(Game.font, "Instruction", new Vector2(x + 40, 575), Color.White);
                    s.Draw(bottomOptionOutline, new Vector2(x, 510), lightLineColor);
                    for (int i = 0; i < 17; i++)
                        s.Draw(tinyLine, new Vector2(x + 30 + 10.4f * i, 535), lightLineColor);
                    s.Draw(halfTinyLine, new Vector2(x + 30 + 10.4f * 17, 535), lightLineColor);
                }
                else
                {
                    s.DrawString(Game.font, "Instruction", new Vector2(x + 40, 575), Menu.g.fontColor);
                    s.Draw(bottomOptionOutline, new Vector2(x, 510), darkLineColor);
                    for (int i = 0; i < 17; i++)
                        s.Draw(tinyLine, new Vector2(x + 30 + 10.4f * i, 535), darkLineColor);
                    s.Draw(halfTinyLine, new Vector2(x + 30 + 10.4f * 17, 535), darkLineColor);

                }
            }
        }

        public class oHighScores : MenuOption
        {
            Color lightLineColor = Color.Green;
            Color darkLineColor = Color.DarkGreen;
            int x = 380;

            public override void updateDraw(SpriteBatch s)
            {
                base.updateDraw(s);
                //Update
                if (selected && Menu.anyEnterClicked())
                {
                    Menu.sMenu.Play(1f, 1f, 0f);
                    ((MainMenu)parent).setToHighScoresScreen();
                }

                //Draw
                if (selected)
                {
                    s.DrawString(Game.font, "High Scores", new Vector2(x + 50, 575), Color.White);
                    s.Draw(bottomOptionOutline, new Vector2(x, 510), lightLineColor);
                    for (int i = 0; i < 17; i++)
                        s.Draw(tinyLine, new Vector2(x + 30 + 10.4f * i, 535), lightLineColor);
                    s.Draw(halfTinyLine, new Vector2(x + 30 + 10.4f * 17, 535), lightLineColor);
                }
                else
                {
                    s.DrawString(Game.font, "High Scores", new Vector2(x + 50, 575), Menu.g.fontColor);
                    s.Draw(bottomOptionOutline, new Vector2(x, 510), darkLineColor);
                    for (int i = 0; i < 17; i++)
                        s.Draw(tinyLine, new Vector2(x + 30 + 10.4f * i, 535), darkLineColor);
                    s.Draw(halfTinyLine, new Vector2(x + 30 + 10.4f * 17, 535), darkLineColor);
                    
                }
            }
        }

        public class oCredits : MenuOption
        {
            Color lightLineColor = Color.Goldenrod;
            Color darkLineColor = Color.DarkGoldenrod;
            int x = 620;

            public override void updateDraw(SpriteBatch s)
            {
                base.updateDraw(s);
                //Update
                if (selected && Menu.anyEnterClicked())
                {
                    Menu.sMenu.Play(1f, 1f, 0f);
                    ((MainMenu)parent).setToCreditsScreen();
                }

                //Draw
                if (selected)
                {
                    s.DrawString(Game.font, "Credits", new Vector2(x + 80, 575), Color.White);
                    s.Draw(bottomOptionOutline, new Vector2(x, 510), lightLineColor);
                    for (int i = 0; i < 17; i++)
                        s.Draw(tinyLine, new Vector2(x + 30 + 10.4f * i, 535), lightLineColor);
                    s.Draw(halfTinyLine, new Vector2(x + 30 + 10.4f * 17, 535), lightLineColor);
                }
                else
                {
                    s.DrawString(Game.font, "Credits", new Vector2(x + 80, 575), Menu.g.fontColor);
                    s.Draw(bottomOptionOutline, new Vector2(x, 510), darkLineColor);
                    for (int i = 0; i < 17; i++)
                        s.Draw(tinyLine, new Vector2(x + 30 + 10.4f * i, 535), darkLineColor);
                    s.Draw(halfTinyLine, new Vector2(x + 30 + 10.4f * 17, 535), darkLineColor);

                }
            }
        }

        public class oQuit : MenuOption
        {
            Color lightLineColor = Color.Violet;
            Color darkLineColor = Color.DarkViolet;
            int x = 860;

            public override void updateDraw(SpriteBatch s)
            {

                base.updateDraw(s);
                //Update
                if (selected && Menu.anyEnterClicked())
                {
                    Menu.sMenu.Play(1f, 1f, 0f);
                    Menu.g.saveScore();
                    Menu.g.Exit();
                }

                //Draw
                if (selected)
                {
                    s.DrawString(Game.font, "Quit", new Vector2(x + 100, 575), Color.White);
                    s.Draw(bottomOptionOutline, new Vector2(x, 510), lightLineColor);
                    for (int i = 0; i < 17; i++)
                        s.Draw(tinyLine, new Vector2(x + 30 + 10.4f * i, 535), lightLineColor);
                    s.Draw(halfTinyLine, new Vector2(x + 30 + 10.4f * 17, 535), lightLineColor);
                }
                else
                {
                    s.DrawString(Game.font, "Quit", new Vector2(x + 100, 575), Menu.g.fontColor);
                    s.Draw(bottomOptionOutline, new Vector2(x, 510), darkLineColor);
                    for (int i = 0; i < 17; i++)
                        s.Draw(tinyLine, new Vector2(x + 30 + 10.4f * i, 535), darkLineColor);
                    s.Draw(halfTinyLine, new Vector2(x + 30 + 10.4f * 17, 535), darkLineColor);

                }
            }
        }

        public class InstructionsScreen : MenuComponent
        {
            public static Texture2D instructions;
          
            public override void updateDraw(SpriteBatch s)
            {
                base.updateDraw(s);

                if (Menu.anyEnterClicked() || Menu.anybClicked())
                {
                    Menu.sMenu.Play();
                    ((MainMenu)parent).setToMainScreen();
                }

                s.Draw(instructions, new Vector2(0, 65), Menu.g.fontColor);
              
            }
        }
        
        public class HighScoresScreen : MenuComponent
        {
            public override void updateDraw(SpriteBatch s)
            {
                base.updateDraw(s);


                if (Menu.anyEnterClicked() || Menu.anybClicked())
                {
                    Menu.sMenu.Play();
                    ((MainMenu)parent).setToMainScreen();
                }

              //  s.Draw(blueBlocker, new Vector2(-10, -10), Menu.g.fontColor);
                for (int i = 0; i < Menu.g.topTenScores.Count; i++)
                {

                    s.DrawString(Game.font, "High Scores", new Vector2(565, 270), new Color(80, 120, 200,100));
                    s.Draw(screenLineDark, new Rectangle(280, 295, 800, 3), Color.Gray);
                    s.DrawString(Menu.fontMenu, "" + Menu.g.topTenScores[i],
                        new Vector2(540, 300 + 35 * i), Menu.g.fontColor);
                }

            }
        }

        public class CreditsScreen : MenuComponent
        {
            public static Texture2D credits;
            public override void updateDraw(SpriteBatch s)
            {
                base.updateDraw(s);

                if (Menu.anyEnterClicked() || Menu.anybClicked())
                {
                    Menu.sMenu.Play();
                    ((MainMenu)parent).setToMainScreen();
                }

                s.Draw(credits, new Vector2(0, 65), Menu.g.fontColor);
            }
        }

        public class MultiplayerMenu : MenuComponent
        {
            public static Texture2D bigGrayPlayerImage;
            bool[] playerJoined = new bool[4];

            public override void updateDraw(SpriteBatch s)
            {
                base.updateDraw(s);

                if (Menu.anybClicked())
                {
                    Menu.sMenu.Play();
                    ((MainMenu)parent).setToMainScreen();
                }

                //Count joined players
                int joined = 0;
                foreach (bool pj in playerJoined) if (pj) joined++;

                if (joined > 1)
                {
                    s.DrawString(Game.font, "Press Start", new Vector2(544, 300), Color.LimeGreen);
                    if (Menu.anyStartClicked() || Menu.newstate.keyboardState.IsKeyDown(Keys.Enter))
                    {
                        Menu.g.pe.clear();
                        Menu.g.clearEverythingExceptCamera();

                        //Figure out players that joined
                        if (playerJoined[0]) Menu.g.players.Add(new Player(0, Vector2.Zero, Menu.g, Game.playerKeyControls[0], PlayerIndex.One));
                        if (playerJoined[1]) Menu.g.players.Add(new Player(1, Vector2.Zero, Menu.g, Game.playerKeyControls[1], PlayerIndex.Two));
                        if (playerJoined[2]) Menu.g.players.Add(new Player(2, Vector2.Zero, Menu.g, Game.playerKeyControls[2], PlayerIndex.Three));
                        if (playerJoined[3]) Menu.g.players.Add(new Player(3, Vector2.Zero, Menu.g, Game.playerKeyControls[3], PlayerIndex.Four));

                        //Space players out equally along screen, add to targets
                        foreach (Player p in Menu.g.players)
                        {
                            p.location = new Vector2(Menu.g.width / (Menu.g.players.Count + 1) * (Menu.g.players.IndexOf(p) + 1), Menu.g.height / 2);
                            Menu.g.targets.Add(p);
                        }

                        //Start game
                        Menu.g.gameType = Game.GameType.multiPlayerCoop;
                        Menu.g.st = Game.state.intro;
                        MediaPlayer.Stop();
                        MediaPlayer.Play(Game.themeSong);
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (Menu.newstate.gamePadStates[i].IsButtonDown(Buttons.A) && Menu.oldstate.gamePadStates[i].IsButtonUp(Buttons.A) ||
                        Menu.newstate.keyboardState.IsKeyDown(Game.playerKeyControls[i][0]) && Menu.oldstate.keyboardState.IsKeyUp(Game.playerKeyControls[i][0]))
                        playerJoined[i] = !playerJoined[i];
                }


                s.Draw(MainMenu.screenLine, new Rectangle(215, 445, Menu.g.width - 400, 15), Color.Gray);
                s.Draw(MainMenu.screenLine, new Rectangle(Menu.g.width/2 + 8, 273, 390, 15), null, Color.Gray, MathHelper.PiOver2, Vector2.Zero, SpriteEffects.None, 0);

                //Draw
                if (playerJoined[0])
                {
                    s.Draw(bigGrayPlayerImage, new Vector2(280, 260), Player.explosionColors[0]);
                    s.DrawString(Game.fontSmall, "Press A to Leave", new Vector2(310, 400), Color.LightGray);
                }
                else
                {
                    s.Draw(bigGrayPlayerImage, new Vector2(280, 260), Color.Gray);
                    s.DrawString(Game.fontSmall, "Press A to Join", new Vector2(310, 400), Color.Gray);
                }

                if (playerJoined[1])
                {
                    s.Draw(bigGrayPlayerImage, new Vector2(Menu.g.width - bigGrayPlayerImage.Width - 280, 260), Player.explosionColors[1]);
                    s.DrawString(Game.fontSmall, "Press A to Leave", new Vector2(855, 400), Color.LightGray);
                }
                else
                {
                    s.Draw(bigGrayPlayerImage, new Vector2(Menu.g.width - bigGrayPlayerImage.Width - 280, 260), Color.Gray);
                    s.DrawString(Game.fontSmall, "Press A to Join", new Vector2(855, 400), Color.Gray);
                }

                if (playerJoined[2])
                {
                    s.Draw(bigGrayPlayerImage, new Vector2(280, 480), Player.explosionColors[2]);
                    s.DrawString(Game.fontSmall, "Press A to Leave", new Vector2(310, 620), Color.LightGray);
                }
                else
                {
                    s.Draw(bigGrayPlayerImage, new Vector2(280, 480), Color.Gray);
                    s.DrawString(Game.fontSmall, "Press A to Join", new Vector2(310, 620), Color.Gray);
                }

                if (playerJoined[3])
                {
                    s.Draw(bigGrayPlayerImage, new Vector2(Menu.g.width - bigGrayPlayerImage.Width - 280, 480), Player.explosionColors[3]);
                    s.DrawString(Game.fontSmall, "Press A to Leave", new Vector2(850, 620), Color.LightGray);
                }
                else
                {
                    s.Draw(bigGrayPlayerImage, new Vector2(Menu.g.width - bigGrayPlayerImage.Width - 280, 480), Color.Gray);
                    s.DrawString(Game.fontSmall, "Press A to Join", new Vector2(850, 620), Color.Gray);
                }
            }

        }

    }
    
    public class InGameMenu : MenuComponent
    {
        public static bool displayInstructions = false;
        int selectedMenuOption = 0;

        public InGameMenu()
        {
            subComponents.Add(new oResume());
            subComponents.Add(new oInstructions());
            subComponents.Add(new oMainMenu());
            foreach (MenuComponent mc in subComponents)
                mc.parent = this;
            select(0);
        }

        public void select(int i)
        {
            if (Menu.sMenu != null) Menu.sMenu.Play();
            if (i >= 3) i = 0;
            if (i < 0) i = 2;
            selectedMenuOption = i;
            foreach (MenuComponent mo in subComponents)
                if (((MenuOption)mo).selected) ((MenuOption)mo).deselect();
            ((MenuOption)subComponents[i]).select();
        }

        public override void updateDraw(SpriteBatch s)
        {
            if (displayInstructions)
            {

                if (Menu.anybClicked())
                    displayInstructions = false;

            }
            else
            {
                if (Menu.anyRightClicked() || Menu.anyDownClicked()) select(selectedMenuOption + 1);
                if (Menu.anyLeftClicked() || Menu.anyUpClicked()) select(selectedMenuOption - 1);
            }

            base.updateDraw(s);
            s.DrawString(Game.fontBig, "Paused", new Vector2(450, 260), Menu.g.fontColor);

        }

        public class oResume : MenuOption
        {
            public override void updateDraw(SpriteBatch s)
            {
                base.updateDraw(s);

                if (selected && Menu.anyEnterClicked())
                {
                    Menu.sMenu.Play(1f, 1f, 0f);
                    Menu.g.st = Game.state.running;
                }
                //Draw
                if (selected) s.DrawString(Menu.fontMenu, "Resume", new Vector2(450 + 100 - 100f / (osc + 1), 340), Color.White);
                else s.DrawString(Menu.fontMenu, "Resume", new Vector2(550 - des/255f*100, 340), Color.LightGray);
            }
        }
        public class oInstructions : MenuOption
        {
            public override void updateDraw(SpriteBatch s)
            {
                base.updateDraw(s);
                if (selected && Menu.anyEnterClicked())
                {
                    Menu.sMenu.Play(1f, 1f, 0f);
                    InGameMenu.displayInstructions = !InGameMenu.displayInstructions;
                }
                //Draw
                if (selected) s.DrawString(Menu.fontMenu, "Instructions", new Vector2(450 + 100 - 100f / (osc + 1), 380), Color.White);
                else s.DrawString(Menu.fontMenu, "Instructions", new Vector2(550 - des/255f * 100, 380), Color.LightGray);
            }
        }
        public class oMainMenu : MenuOption
        {
            public override void updateDraw(SpriteBatch s)
            {
                base.updateDraw(s);
                if (selected && Menu.anyEnterClicked())
                {
                    Menu.sMenu.Play(1f, 1f, 0f);
                    Menu.mainMenu.setToMainScreen();
                    Menu.g.st = Game.state.menu;
                    Menu.g.resetCamera();
                    Menu.g.clearEverythingExceptCamera();
                }
                //Draw
                if (selected) s.DrawString(Menu.fontMenu, "Main Menu", new Vector2(450 + 100 - 100f / (osc + 1), 420), Color.White);
                else s.DrawString(Menu.fontMenu, "Main Menu", new Vector2(550 - des/255f * 100, 420), Color.LightGray);
            }
        }
        
    }

    public class GameOverMenu : MenuComponent
    {
        int selectedMenuOption = 0;

        public GameOverMenu()
        {
            subComponents.Add(new oRestart());
            subComponents.Add(new oMainmenu());
            select(0);
        }

        public void select(int i)
        {
            if (Menu.sMenu != null) Menu.sMenu.Play();
            if (i >= 2) i = 0;
            if (i < 0) i = 1;
            selectedMenuOption = i;
            foreach (MenuComponent mo in subComponents)
                if (((MenuOption)mo).selected) ((MenuOption)mo).deselect();
            ((MenuOption)subComponents[i]).select();
        }

        public override void updateDraw(SpriteBatch s)
        {
            base.updateDraw(s);

            s.DrawString(Game.fontBig, "Game Over", new Vector2(400, 240), Color.LightGray);

            if (Menu.anyRightClicked() || Menu.anyDownClicked()) select(selectedMenuOption + 1);
            if (Menu.anyLeftClicked() || Menu.anyUpClicked()) select(selectedMenuOption - 1);

        }

        public class oRestart : MenuOption
        {
            public override void updateDraw(SpriteBatch s)
            {
                base.updateDraw(s);
                if (selected && Menu.anyEnterClicked())
                {
                    if (Menu.g.gameType == Game.GameType.singlePlayer)
                    {
                        Menu.sMenu.Play(1f, 1f, 0f);
                        MainMenu.oSinglePlayer.beginSinglePlayerGame();
                    }
                    else
                    {
                        Menu.g.st = Game.state.menu;
                        Menu.g.resetCamera();
                        Menu.g.clearEverythingExceptCamera();
                    }
                }
                //Draw
                if (selected) s.DrawString(Menu.fontMenu, "Restart", new Vector2(450 + 100 - 100f / (osc + 1), 340), Color.White);
                else s.DrawString(Menu.fontMenu, "Restart", new Vector2(550 - des / 255f * 100, 340), Color.LightGray);
            }
        }
        public class oMainmenu : MenuOption
        {
            public override void updateDraw(SpriteBatch s)
            {
                base.updateDraw(s);
                if (selected && Menu.anyEnterClicked())
                {
                    Menu.sMenu.Play(1f, 1f, 0f);
                    Menu.mainMenu.setToMainScreen();
                    Menu.g.st = Game.state.menu;
                    Menu.g.resetCamera();
                    Menu.g.clearEverythingExceptCamera();
                }
                //Draw
                if (selected) s.DrawString(Menu.fontMenu, "Main Menu", new Vector2(450 + 100 - 100f / (osc + 1), 380), Color.White);
                else s.DrawString(Menu.fontMenu, "Main Menu", new Vector2(550 - des / 255f * 100, 380), Color.LightGray);
            }
        }
      
    }

    public class MenuOption : MenuComponent
    {
        public bool selected = false;

        //Select/deselect counters used for animation, [0, 255] for easy use with color values
        protected int sel = 0;
        protected int des = 255;
        protected int osc = 0;

        public override void updateDraw(SpriteBatch s)
        {
            base.updateDraw(s);
            if (selected)
            {
                osc++;
                if (sel < 255) sel += 40;
                if (sel > 255) sel = 255;
                des = 0;
            }
            else 
            {
                if (des < 255)
                {
                    des += 40;
                    osc++;
                }
                if (des > 255)
                {
                    des = 255;
                    osc = 0;
                }
                sel = 0;
            }
        }

        public void select()
        {
            selected = true;
        }
        public void deselect()
        {
            selected = false;
        }

    }

    public class MenuComponent
    {
        protected int x, y;
        public MenuComponent parent;
        public List<MenuComponent> subComponents = new List<MenuComponent>();

        public virtual void updateDraw(SpriteBatch s)
        {
            foreach (MenuComponent m in subComponents)
                m.updateDraw(s);
        }

    }

    public class InputState
    {
        public KeyboardState keyboardState;
        public List<GamePadState> gamePadStates = new List<GamePadState>();

        public static InputState getCurrent()
        {
            InputState i = new InputState();
            i.keyboardState = Keyboard.GetState();
            i.gamePadStates.Add(GamePad.GetState(PlayerIndex.One));
            i.gamePadStates.Add(GamePad.GetState(PlayerIndex.Two));
            i.gamePadStates.Add(GamePad.GetState(PlayerIndex.Three));
            i.gamePadStates.Add(GamePad.GetState(PlayerIndex.Four));
            return i;
        }
        public bool anyUpPressed()
        {
            if (keyboardState.IsKeyDown(Keys.Up)) return true;
            foreach (GamePadState gps in gamePadStates)
                if (gps.ThumbSticks.Left.Y > .4 || gps.DPad.Up == ButtonState.Pressed) return true;
            return false;
        }
        public bool anyDownPressed()
        {
            if (keyboardState.IsKeyDown(Keys.Down)) return true;
            foreach (GamePadState gps in gamePadStates)
                if (gps.ThumbSticks.Left.Y < -.4 || gps.DPad.Down == ButtonState.Pressed) return true;
            return false;
        }
        public bool anyRightPressed()
        {
            if (keyboardState.IsKeyDown(Keys.Right)) return true;
            foreach (GamePadState gps in gamePadStates)
                if (gps.ThumbSticks.Left.X > .4 || gps.DPad.Right == ButtonState.Pressed) return true;
            return false;
        }
        public bool anyLeftPressed()
        {
            if (keyboardState.IsKeyDown(Keys.Left)) return true;
            foreach (GamePadState gps in gamePadStates)
                if (gps.ThumbSticks.Left.X < -.4 || gps.DPad.Left == ButtonState.Pressed) return true;
            return false;
        }
        public bool anyEnterPressed()
        {
            if (keyboardState.IsKeyDown(Keys.Enter)) return true;
            foreach (GamePadState gps in gamePadStates)
                if (gps.Buttons.A == ButtonState.Pressed) return true;
            return false;
        }
        public bool anybPressed()
        {
            if (keyboardState.IsKeyDown(Keys.Back)) return true;
            foreach (GamePadState gps in gamePadStates)
                if (gps.Buttons.B == ButtonState.Pressed) return true;
            return false;
        }

        public bool anyStartPressed()
        {
            foreach (GamePadState gps in gamePadStates)
                if (gps.Buttons.Start == ButtonState.Pressed) return true;
            return false;
        }

        public bool anyBigButtonPressed()
        {
            foreach (GamePadState gps in gamePadStates)
                if (gps.Buttons.BigButton == ButtonState.Pressed) return true;
            return false;
        }
    }
}

*/