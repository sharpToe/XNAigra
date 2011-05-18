using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace KillEm
{
    class Igralec : Sprite
    {
        const string IME_TEKSTURE="pikapolonica";
        const int STARTPOS_X=120;
        const int STARTPOS_Y=200;
        const int HITROST=100;
        const int GOR = -1, DOL = 1, LEVO = -1, DESNO = 1;

        public string orozje = "pistola";
        public int health = 1000;
        Vector2 hitrost = Vector2.Zero;
        float angle = 0.0f;
        public List<Metek> ustreljeni_metki = new List<Metek>();
        public Dictionary<string, int> st_metkov = new Dictionary<string, int>();
        ContentManager cmngr;
        TimeSpan zadnji_metek_ms;

        KeyboardState previousKeyboardState;
        MouseState previousMouseState;
        

        public void LoadContent(ContentManager mngr)
        {
            pozicija = new Vector2(STARTPOS_X, STARTPOS_Y);
            cmngr = mngr;
            st_metkov.Add("pistola", 999);
            zadnji_metek_ms = TimeSpan.Zero;

            foreach (Metek m in ustreljeni_metki)  
                m.LoadContent(mngr);

            base.LoadContent(mngr, IME_TEKSTURE);
        }

        public void Update(GameTime cas)
        {
            KeyboardState trenutnaTipkovnica = Keyboard.GetState();
            MouseState trenutnaMiska = Mouse.GetState();

            UpdateMovement(trenutnaTipkovnica,trenutnaMiska);
            UpdateMetek(cas,trenutnaMiska);

            previousKeyboardState = trenutnaTipkovnica;
            previousMouseState = trenutnaMiska;

            base.Update(cas, hitrost, smer);
        }

        public override void Draw(SpriteBatch sprBatch)
        {
            sprBatch.Draw(tekstura, pozicija, new Rectangle(0, 0, tekstura.Width, tekstura.Height), Color.White, angle - 1.5f, new Vector2(this.velikost.Width / 2, this.velikost.Height / 2), povecava, SpriteEffects.None, 0);
            foreach (Metek m in ustreljeni_metki)
            {
                m.Draw(sprBatch);
            }
        }
        private void UpdateMovement(KeyboardState tipkovnica, MouseState miska)
        {
            hitrost = Vector2.Zero;
            smer = Vector2.Zero;

            if (tipkovnica.IsKeyDown(Keys.Left) || tipkovnica.IsKeyDown(Keys.A))
            {
                hitrost.X = HITROST;
                smer.X = LEVO;
            }
            else if (tipkovnica.IsKeyDown(Keys.Right) || tipkovnica.IsKeyDown(Keys.D))
            {
                hitrost.X=HITROST;
                smer.X=DESNO;
            }

            if (tipkovnica.IsKeyDown(Keys.Down) || tipkovnica.IsKeyDown(Keys.S))
            {
                hitrost.Y=HITROST;
                smer.Y=DOL;
            }
            else if (tipkovnica.IsKeyDown(Keys.Up) || tipkovnica.IsKeyDown(Keys.W))
            {
                hitrost.Y=HITROST;
                smer.Y=GOR;
            }

            //računanje kota igralca, da je usmerjen proti miški
            Vector2 usmerjenost = new Vector2(pozicija.X - miska.X, pozicija.Y - miska.Y);
            angle = (float)(Math.Atan2(usmerjenost.Y,usmerjenost.X));
        }

        public bool CheckCollision(Spawnbox box)
        {
            if (Vector2.Distance(pozicija, box.pozicija + new Vector2(box.velikost.Width / 2, box.velikost.Height / 2)) < 30) return true;
            else return false;
        }

        private void UpdateMetek(GameTime cas, MouseState miska)
        {
            foreach (Metek m in ustreljeni_metki)
            {
                if(m.Visible) m.Update(cas);
            }

            if (miska.LeftButton == ButtonState.Pressed)
            {
                int delay =150; //onemogočimo prepogosto streljanje
                switch(orozje){
                    case "pistola": delay = 150; break;
                    case "ognjena_krogla": delay = 80; break;
                    case "plazma_krogla": delay = 300; break;
                }
                if (cas.TotalGameTime - zadnji_metek_ms > TimeSpan.FromMilliseconds(delay)) 
                {
                    if (st_metkov[orozje] > 0)
                    {
                        if (orozje != "pistola") st_metkov[orozje]--;
                    }
                    else
                    {
                        orozje = "pistola";
                        ustreljeni_metki.Clear();
                    }

                    zadnji_metek_ms = cas.TotalGameTime;
                    Ustreli(miska);
                }
            }
        }

        private void Ustreli(MouseState miska)
        {
            bool narediNovo = true;
            Vector2 smer = new Vector2(miska.X, miska.Y);
            smer = -pozicija + smer;
            smer.Normalize();

            foreach (Metek m in ustreljeni_metki)
            {
                if (m.Visible == false)
                {
                    narediNovo = false;
                    m.SetType(orozje);
                    m.Fire(pozicija-new Vector2(velikost.Width/5,velikost.Height/5)+smer*25, smer);
                    break;
                }
            }
            if (narediNovo)
            {
                Metek metek = new Metek();
                metek.SetType(orozje);
                metek.LoadContent(cmngr);
                metek.Fire(pozicija - new Vector2(velikost.Width / 5, velikost.Height / 5) + smer * 25, smer);
                ustreljeni_metki.Add(metek);   
            }
        }

    }
}
