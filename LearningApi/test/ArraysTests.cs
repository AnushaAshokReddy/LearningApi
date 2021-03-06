﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using LearningFoundation;
using LearningFoundation.Statistics;
using LearningFoundation.Arrays;


namespace test.statistics
{
    /// <summary>
    /// Unit tests for validation and check corect result for advanced statistics calculation of various operations
    /// </summary>
    public class ArraysTests
    {
        [Fact]
        public void FindFirstTests()
        {
            var dataSample1 = new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 5, 6, 7, 2, 5, 7, 8, 3, 4, 5, 2, 3, 4 };

            var ocurrences = dataSample1.FindAllOcurrences();

            Assert.Equal(ocurrences.Length, 8);
        }

        /// <summary>
        /// Calculates hamming distance odf sparse arrays.
        /// </summary>
        [Fact]
        public void HemmingDistanceTests()
        {
            var dataSample1 = new double[] { 1.0, 1.0, 0.0, 0.0, 1.0 };
            var dataSample2 = new double[] { 1.0, 1.0, 0.0, 0.0, 1.0 };

            var acc = dataSample1.GetHammingDistance(dataSample2);

            Assert.True(acc == 100);

            dataSample2 = new double[] { 1.0, 1.0, 0.0, 0.0, 0.0 };

            acc = dataSample1.GetHammingDistance(dataSample2);

            Assert.True(acc == 80);

            dataSample2 = new double[] { 1.0, 0.0, 1.0, 1.0, 0.0 };

            acc = dataSample1.GetHammingDistance(dataSample2);

            Assert.True(acc == 20);
        }

        [Fact]
        public void TestBinaryConversion()
        {
            Assert.True(new double[] { 1, 1, 1 }.ToBinary() == 7);
            Assert.True(new double[] { 0, 1, 0 }.ToBinary() == 2);
            Assert.True(new double[] { 0, 0, 1 }.ToBinary() == 4); 
        }

    }
}
