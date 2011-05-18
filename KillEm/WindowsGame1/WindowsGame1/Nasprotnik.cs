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
    class Nasprotnik: Sprite
    {
        const int STARTPOS_X = 80;
        const int STARTPOS_Y = 130;
        public static float HITROST = 1.0f;
        Vector2 hitrost = Vector2.Zero;
        float angle = 0.0f;
        public int alpha = 255;
        public bool alive = true;

        public void LoadContent(ContentManager mngr, string ime)
        {
            base.LoadContent(mngr, ime);
        }

        public void Update(GameTime cas, Igralec igralec)
        {
            if (alive)
            {
                UpdatePozicija(igralec);
                smer.Normalize();
                smer.X = (float)(smer.X * (-1)) / (float)(1.5);
                smer.Y = (float)(smer.Y * (-1)) / (float)(1.5);
                if (Vector2.Distance(pozicija, igralec.pozicija) > 20) pozicija += smer * new Vector2(HITROST,HITROST);
                //base.Update(cas, new Vector2(-1,-1), smer);
            }
            else if(alpha>0) alpha--;
        }

        
        private void UpdatePozicija(Igralec igralec)
        {
            //računanje kota nasprotnika, da je usmerjen proti igralcu
            Vector2 usmerjenost = new Vector2(pozicija.X - igralec.pozicija.X, pozicija.Y - igralec.pozicija.Y);
            smer = usmerjenost;
            angle = (float)(Math.Atan2(usmerjenost.Y, usmerjenost.X));

        }

        public override void Draw(SpriteBatch sprBatch)
        {
            Color barva;
            if(alive) barva=Color.White;
            else barva=new Color(0, 0, 0, alpha);
           
            sprBatch.Draw(tekstura, pozicija, new Rectangle(0, 0, tekstura.Width, tekstura.Height), barva, angle - 1.5f, new Vector2(this.velikost.Width / 2, this.velikost.Height / 2), povecava, SpriteEffects.None, 0);
            
        }
    }
}
