using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace KillEm
{
    class Metek : Sprite
    {
        public int range = 500;
        public bool Visible = false;
        public string ime ="WPN_pistola";
        private string tip;
        Vector2 zacetnaPozicija;
        Vector2 hitrost = new Vector2(300, 300);

        public void SetType(string tip)
        {
            this.tip = tip;
            if (tip == "pistola")
            {
                ime="WPN_pistola";
                range = 400;
                povecava = 1.5f;
                hitrost = new Vector2(800, 800);
            }
            else if (tip == "ognjena_krogla")
            {
                ime = "WPN_ognjena_krogla";
                range = 600;
                hitrost = new Vector2(400, 400);
            }
            else if (tip == "plazma_krogla")
            {
                ime = "WPN_plazma_krogla";
                range = 250;
                hitrost = new Vector2(200, 200);
                povecava = 0.2f;
            }
        }
        public void LoadContent(ContentManager mngr)
        {
            base.LoadContent(mngr, ime);
        }

        public void Update(GameTime theGameTime)
        {
            if (Vector2.Distance(zacetnaPozicija, pozicija) > range)
            {
                Visible = false;
            }

            if (Visible == true)
            {
                if (tip == "plazma_krogla") povecava += 0.01f;
                base.Update(theGameTime, hitrost, smer);
            }
        }

        public override void Draw(SpriteBatch theSpriteBatch)
        {
            if (Visible == true)
            {
                base.Draw(theSpriteBatch);
            }
        }

        public void Fire(Vector2 theStartPosition, Vector2 theDirection)
        {
            pozicija = theStartPosition;
            zacetnaPozicija = theStartPosition;
            smer = theDirection;
            Visible = true;
        }
    }
}
