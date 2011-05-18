using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KillEm
{
    class Sprite
    {
        public Vector2 pozicija = new Vector2(0, 0);
        protected Texture2D tekstura;
        public Rectangle velikost;
        public float povecava = 1.0f;
        public Vector2 smer = new Vector2(1, 0);

        public void LoadContent(ContentManager mngr, string imeTeksture)
        {
            tekstura = mngr.Load<Texture2D>(imeTeksture);
            velikost = new Rectangle(0, 0, (int)(tekstura.Width * povecava), (int)(tekstura.Height * povecava));
        }

        public virtual void Draw(SpriteBatch sprBatch)
        {
            sprBatch.Draw(tekstura, pozicija, new Rectangle(0, 0, tekstura.Width, tekstura.Height), Color.White, 0.0f, Vector2.Zero, povecava, SpriteEffects.None, 0); 
        }

        public void Scale()
        {
              velikost=new Rectangle(0,0,(int)(tekstura.Width*povecava),(int)(tekstura.Height*povecava));
        }

        public void Update(GameTime cas, Vector2 hitrost, Vector2 smer)
        {
            pozicija += (float)cas.ElapsedGameTime.TotalSeconds * hitrost * smer;
        }


    }

    
}
