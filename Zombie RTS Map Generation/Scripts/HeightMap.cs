using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Color = System.Drawing.Color;

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
                    var nx = x / _width - 0.5f;
                    var ny = y/_height - 0.5f;
                    
                    
                    //var pixelValue = RandomPixelValue(rand);
                    var pixelValue = FrequencyPixelValue(nx, ny);

                    //set ARGB value
                    bitmap.SetPixel(x, y, pixelValue);
                }
            }
            
            //save (write) random pixel image
            bitmap.Save(string.Format("{0}.png", _fileName));
        }

        
        /**
         * 
         */
        private Color FrequencyPixelValue(float nx, float ny)
        {
            //noise(1.00 * nx, 1.00 * ny);
            //Mathf.   todo: get perlin noise
            throw new NotImplementedException();
        }

        
        /**
         * Generate a random pixel value
         */
        private static Color RandomPixelValue(Random rand)
        {
            //generate random ARGB value
            var a = rand.Next(256);
            var r = rand.Next(256);
            var g = rand.Next(256);
            var b = rand.Next(256);

            return Color.FromArgb(a, r, g, b);
        }
    }
}
