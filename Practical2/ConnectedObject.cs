using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical2
{
    public class ConnectedObject
    {
        public string Name { get; set; }

        public AForge.Point Centroid { get; set; }
        public Rectangle BoundingRectangle { get; set; }
        public Bitmap Image { get; set; }

    }
}
