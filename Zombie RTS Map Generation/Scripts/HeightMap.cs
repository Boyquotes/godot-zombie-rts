using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieRTSMapGeneration.Scripts
{
    public class HeightMap
    {
        private int _width;
        private int _height;
        private string _fileName;

        
        /**
         * 
         */
        public HeightMap(int width, int height, string fileName)
        {
            _width = width;
            _height = height;
            _fileName = fileName;
        }

        
        /**
         * 
         */
        public void MakeImage()
        {
            //bitmap
            var bitmap = new Bitmap(_width, _height);

            //random number
            var rand = new Random();

            //create random pixels
            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    //generate random ARGB value
                    var a = rand.Next(256);
                    var r = rand.Next(256);
                    var g = rand.Next(256);
                    var b = rand.Next(256);

                    //set ARGB value
                    bitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }
            
            //save (write) random pixel image
            bitmap.Save(string.Format("{0}.png", _fileName));
        }
    }
}
