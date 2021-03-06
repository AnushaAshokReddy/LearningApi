using LearningFoundation;
using MLPerceptron;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Xunit;
using System.Diagnostics;
using System.Globalization;
using NeuralNet.MLPerceptron;
using ImageBinarizer;

namespace test.MLPerceptron
{
    /// <summary>
    /// Class MLPerceptronUnitTests contains the unit test cases to test the ML Perceptron algorithm
    /// </summary>
    public class ImageBinarizationTests
    {
        
        /// <summary>
        /// Loads a single JPG and create a binarized version with zeros and ones.
        /// Look after executing of test for file binary.txt.
        /// </summary>
        [Fact]
        public void BinarizerTest()
        {
            Binarizer bizer = new Binarizer();

            foreach (var item in new string[] { "face.jpg", "daenet.png", "gab.png"})
            {
                string trainingImagesPath = Path.Combine(Path.Combine(AppContext.BaseDirectory, "ImageBinarization"), "TestImages");
             
                bizer.CreateBinary(Path.Combine(trainingImagesPath, item), $"binary_{item}.txt");
            }
        }


        /// <summary>
        /// Loads a single JPG and create a binarized version with zeros and ones in specified size, which is not image original size.
        /// </summary>
        [Fact]
        public void BinarizerWithTargetSizeTest()
        {
            Binarizer bizer = new Binarizer(targetHeight: 256, targetWidth: 256 );

            foreach (var item in new string[] { "face.jpg", "daenet.png", "gab.png" })
            {
                string trainingImagesPath = Path.Combine(Path.Combine(AppContext.BaseDirectory, "ImageBinarization"), "TestImages");

                bizer.CreateBinary(Path.Combine(trainingImagesPath, item), $"binary__256x256_{item}.txt");
            }
        }
    }

}




