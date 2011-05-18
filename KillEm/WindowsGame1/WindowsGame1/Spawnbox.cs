using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KillEm
{
    class Spawnbox: Sprite
    {
        public string ime;
        public int metkov;
        public bool visible;

        public void LoadContent(ContentManager mngr)
        {
            base.LoadContent(mngr, "SPAWN_"+ime);
        }
    }
}
