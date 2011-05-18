using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace KillEm
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //objekti
        Igralec igralec;
        Spawnbox spawnbox;
        Spawnbox healthbox;
        List<Nasprotnik> seznam_nasprotnikov = new List<Nasprotnik>();
        Dictionary<string, Sprite> seznam_napisov = new Dictionary<string, Sprite>();
        Sprite ozadje, obris_izbran_napis;
        Texture2D healthbar;

        //dodatno
        SpriteFont Pisava;
        SpriteFont Pisava2;
        Vector2 pozicija_pisava;
        int st_napisov = 1;
        Random r1;
        bool konec = false;
        bool napis = false;
        TimeSpan cas_napisa, cas_upocasni;
        int level=0;

        //lastnosti
        int POGOSTOST_SPAWN = 500; //manjša številka, večja pogostost pojavitev spawnboxov
        int POGOSTOST_SPAWN_nasp = 2; //manjša številka, večja pogostost pojavitev nasprotnikov
        int POGOSTOST_SPAWN_health = 700; //manjša številka, večja pogostost pojavitev zdravja
        int st_tock = 0;

        int oskus = 0;
        int a;
        //poskus za github - zbriši me

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            this.Window.Title = "KillEm!";
            igralec = new Igralec();
            r1 = new Random();
            cas_napisa = TimeSpan.Zero;

            ozadje = new Sprite();
            ozadje.povecava = 1.2f;

            seznam_napisov.Add("pistola", new Sprite()); //pištola je osnovno orožje

            spawnbox = new Spawnbox();
            spawnbox.ime = "ognjena_krogla";
            spawnbox.visible = false;

            healthbox = new Spawnbox();
            healthbox.ime="zdravje";
            healthbox.visible=false;

            seznam_nasprotnikov.Add(new Nasprotnik());  

            obris_izbran_napis = new Sprite();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice); //objekt, s pomočjo katerega rišemo

            Pisava=Content.Load<SpriteFont>("Pisava"); //naložimo tip pisave za prikaz st. metkov
            Pisava2 = Content.Load<SpriteFont>("Pisava2"); //naložimo tip pisave za prikaz st. metkov
            pozicija_pisava = new Vector2(85, 31);

            igralec.LoadContent(this.Content);
            seznam_nasprotnikov[0].LoadContent(this.Content, "nasprotnik");
            seznam_nasprotnikov[0].pozicija = new Vector2(r1.Next(500), r1.Next(400));

            ozadje.LoadContent(this.Content, "tla");
            ozadje.pozicija = new Vector2(0, 0);

            seznam_napisov["pistola"].LoadContent(this.Content, "STAT_pistola");
            seznam_napisov["pistola"].pozicija = new Vector2(10, 10);

            healthbar = Content.Load<Texture2D>("healthbar");
            healthbox.LoadContent(this.Content);
            //spawnbox.pozicija = new Vector2(300, 300);

            obris_izbran_napis.LoadContent(this.Content, "obris");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            if (!konec)
            {
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                // TODO: Add your update logic here
                igralec.Update(gameTime);

                foreach (Metek m in igralec.ustreljeni_metki)
                {
                    if (m.Visible)
                    {
                        for (int i = 0; i < seznam_nasprotnikov.Count; i++)
                        {
                            if (seznam_nasprotnikov[i].alive)
                            {
                                if (Vector2.Distance(m.pozicija, seznam_nasprotnikov[i].pozicija) < 20)
                                {
                                    //seznam_nasprotnikov.RemoveAt(i);

                                    seznam_nasprotnikov[i].LoadContent(this.Content, "nasprotnik_splash" + r1.Next(1, 3));
                                    seznam_nasprotnikov[i].alive=false;
                                    seznam_nasprotnikov[i].alpha--;
                                    m.Visible = false;
                                    st_tock++;
                                }
                            }                            
                        }
                    }
                }
                if (spawnbox.visible && igralec.CheckCollision(spawnbox))
                { //pobiranje spawnboxov
                    if (spawnbox.ime == "upocasni")
                    {
                        cas_upocasni = TimeSpan.Zero;
                        Nasprotnik.HITROST = 0.2f;
                    }
                    else dodajOrozje(spawnbox.ime, spawnbox.metkov);
                    spawnbox.visible = false;
                }
                if (healthbox.visible && igralec.CheckCollision(healthbox))
                { //pobiranje zdravja
                    igralec.health += 200;
                    igralec.health = (int)MathHelper.Clamp(igralec.health, 0, 1000);
                    healthbox.visible = false;
                }

                if (!spawnbox.visible) spawnRandomBox(); //spawnanje spawnboxov
                if (!healthbox.visible) spawnHealthbox(); //spawnanje zdravja
                spawnNasprotnik(); //spawnanje nasprotnikov

                for(int i=0;i<seznam_nasprotnikov.Count;i++){
                    seznam_nasprotnikov[i].Update(gameTime, igralec);
                    if (Vector2.Distance(igralec.pozicija, seznam_nasprotnikov[i].pozicija) < 20)
                    {
                        if (igralec.health > 0) igralec.health--;
                    }
                    if (seznam_nasprotnikov[i].alpha == 0) seznam_nasprotnikov.RemoveAt(i);
                    else if (!seznam_nasprotnikov[i].alive)
                    {
                        seznam_nasprotnikov[i].alpha-=2;
                        seznam_nasprotnikov[i].alpha = (int)MathHelper.Clamp(seznam_nasprotnikov[i].alpha, 0, 255);
                    }
                }
                if (igralec.health == 0)
                {
                    konec = true;
                }

                if (st_tock % (20 + (level * 20)) == 0 && POGOSTOST_SPAWN_nasp-level >= 10 && !napis)
                {
                    level++;
                    POGOSTOST_SPAWN_nasp -= level*2;
                    cas_napisa = TimeSpan.Zero;
                    napis=true;
                }
                if(napis)
                {
                    cas_napisa += gameTime.ElapsedGameTime;
                }

                
                if(cas_upocasni.TotalSeconds<3) cas_upocasni += gameTime.ElapsedGameTime;
                else if (Nasprotnik.HITROST!=1.0f)
                {
                    Nasprotnik.HITROST = 1.0f;
                }

                base.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue); //vsakič na novo izrišemo

            //RISANJE
            spriteBatch.Begin();
            ozadje.Draw(this.spriteBatch);
            igralec.Draw(this.spriteBatch);


            foreach (Nasprotnik n in seznam_nasprotnikov)
            {
                n.Draw(this.spriteBatch);
            }

            if (spawnbox.visible)
            {
                spawnbox.Draw(this.spriteBatch);
            }
            if (healthbox.visible)
            {
                healthbox.Draw(this.spriteBatch);
            }

            izrisiNapise(); //izris stranskih napisov za orožja, ki jih imamo

            if (igralec.health == 0)
            {
                Vector2 sredisceNiza = Pisava2.MeasureString("KONEC") / 2;
                this.spriteBatch.DrawString(Pisava2, "KONEC", new Vector2(400, 250), Color.OrangeRed,
                     0, sredisceNiza, 1.0f, SpriteEffects.None, 0.5f);
            }

            if (napis)
            {
                if (cas_napisa.TotalSeconds < 3)
                {
                    Vector2 sredisceNiza = Pisava2.MeasureString("Level " + level) / 2;
                    this.spriteBatch.DrawString(Pisava2, "Level " + level, new Vector2(400, 250), Color.Black,
                         0, sredisceNiza, 1.0f, SpriteEffects.None, 0.5f);
                }
                else napis = false;
            }

            Vector2 sredisceNiz = Pisava2.MeasureString("Ubitih: " + st_tock) / 2;
            this.spriteBatch.DrawString(Pisava2, "Ubitih: " + st_tock, new Vector2(400, 45), Color.Black,
                 0, sredisceNiz, 0.5f, SpriteEffects.None, 0.5f);

            //healthbar
            if (igralec.health < 1000)
            {
                spriteBatch.Draw(healthbar, new Rectangle(this.Window.ClientBounds.Width / 2 - healthbar.Width / 2 + 120, 10, healthbar.Width / 2, 20),
                new Rectangle(0, 45, healthbar.Width, 44), Color.Gray);
            }
            spriteBatch.Draw(healthbar, new Rectangle(this.Window.ClientBounds.Width / 2 - healthbar.Width / 2 + 120, 10, (int)((healthbar.Width * ((double)igralec.health / 1000))) / 2, 20),
                new Rectangle(0, 45, healthbar.Width, 44), Color.Red);
            spriteBatch.Draw(healthbar, new Rectangle(this.Window.ClientBounds.Width / 2 - healthbar.Width / 2 + 120, 10, healthbar.Width / 2, 20),
                new Rectangle(0, 0, healthbar.Width, 44), Color.White);

            spriteBatch.End();
            //KONEC RISANJE

            base.Draw(gameTime);
        }

        protected void spawnRandomBox()
        {
            if (r1.Next(POGOSTOST_SPAWN) == 0)
            {
                int rand=r1.Next(0, 7);
                if (rand < 4)
                {
                    spawnbox.ime = "ognjena_krogla";
                    spawnbox.metkov = 80;
                }
                else if (rand > 3 && rand <5)
                {
                    spawnbox.ime = "plazma_krogla";
                    spawnbox.metkov = 40;
                }
                else if (rand > 5)
                {
                    spawnbox.ime = "upocasni";
                }
                spawnbox.LoadContent(this.Content);
                spawnbox.pozicija = new Vector2(r1.Next(20, this.GraphicsDevice.Viewport.Width-40), r1.Next(20, this.GraphicsDevice.Viewport.Height -40));
                spawnbox.visible = true;
            }
        }

        protected void spawnHealthbox()
        {
            if (r1.Next(POGOSTOST_SPAWN_health) == 0)
            {
                healthbox.pozicija = new Vector2(r1.Next(20, this.GraphicsDevice.Viewport.Width - 40), r1.Next(20, this.GraphicsDevice.Viewport.Height - 40));
                healthbox.visible = true;
            }
        }

        protected void spawnNasprotnik()
        {
            if (r1.Next(POGOSTOST_SPAWN_nasp) == 0)
            {
                seznam_nasprotnikov.Add(new Nasprotnik());
                seznam_nasprotnikov[seznam_nasprotnikov.Count - 1].LoadContent(this.Content, "nasprotnik");
                do
                {
                    seznam_nasprotnikov[seznam_nasprotnikov.Count - 1].pozicija = new Vector2(r1.Next(-20, this.GraphicsDevice.Viewport.Width +20), r1.Next(-20, this.GraphicsDevice.Viewport.Height + 20));
                }
                while (Vector2.Distance(seznam_nasprotnikov[seznam_nasprotnikov.Count - 1].pozicija, igralec.pozicija) < 200);
            }
        }

        protected void izrisiNapise()
        {
            //izris stranskih možnosti za orožja (napisov)
            int narisanih = 0;
            foreach (KeyValuePair<string, Sprite> napis in seznam_napisov) //gremo čez cel seznam napisov
            {
                if (igralec.st_metkov[napis.Key] > 0) //pokažemo samo napise za orožja, za katera imamo metke
                {
                    if (napis.Key == igralec.orozje) //najdemo ustrezen napis za trenutno orozje, okoli njega narisemo obris
                    {
                        obris_izbran_napis.pozicija = new Vector2(10, 10 + (narisanih * 40));
                    }

                    napis.Value.pozicija = new Vector2(10, 10 + (narisanih * 40));
                    string imeTeksture = "STAT_"+napis.Key;
                    napis.Value.LoadContent(this.Content, imeTeksture);

                    obris_izbran_napis.Draw(this.spriteBatch);
                    napis.Value.Draw(this.spriteBatch);

                    Vector2 sredisceNiza = Pisava.MeasureString(igralec.st_metkov[napis.Key].ToString()) / 2;
                    this.spriteBatch.DrawString(Pisava, igralec.st_metkov[napis.Key].ToString(), new Vector2(pozicija_pisava.X, pozicija_pisava.Y + (40 * narisanih)), Color.Black,
                     0, sredisceNiza, 1.0f, SpriteEffects.None, 0.5f);

                    narisanih++;
                }
                else //za napise orozij, pri katerih nimamo nic nabojev
                {
                    if (napis.Key == igralec.orozje) //ce je trenutno orozje igralca brez nabojev
                    {
                        obris_izbran_napis.pozicija = new Vector2(10, 10 - ((narisanih - 1) * 40));
                    }
                }
            }
        }

        protected void dodajOrozje(string id_orozja, int st_metkov)
        {
            igralec.orozje = id_orozja; //igralec dobi orozje, ki je bilo v skatli
            igralec.ustreljeni_metki.Clear(); //pobrišemo prejšen seznam izstreljenih metkov

            if (!seznam_napisov.ContainsKey(id_orozja)) //pogledamo, če že obstaja napis za trenutno orožje
            {
                seznam_napisov.Add(id_orozja, new Sprite());
                st_napisov++;
            }
            if (!igralec.st_metkov.ContainsKey(id_orozja)) igralec.st_metkov.Add(id_orozja, 0);  //če še nimamo vnosa za št. metkov za to orožje, ga naredimo

            igralec.st_metkov[id_orozja] += st_metkov; //povečamo št metkov za to orožje  
        }
    }
}
