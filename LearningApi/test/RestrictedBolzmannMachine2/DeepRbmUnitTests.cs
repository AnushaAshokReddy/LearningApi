﻿using LearningFoundation;
using System;
using System.Linq;
using Xunit;
using NeuralNet.RestrictedBolzmannMachine2;
using System.IO;
using LearningFoundation.DataProviders;
using LearningFoundation.DataMappers;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using LearningFoundation.Arrays;
using System.Threading;

namespace test.RestrictedBolzmannMachine2
{
    /// <summary>
    /// Tests for DeepRbm Algorithm.
    /// </summary>
    public class DeepRbmUnitTests
    {
       
        /// <summary>
        /// RBM is not supervised algorithm. This is why we do not have a label.
        /// </summary>
        /// <returns></returns>
        private DataDescriptor getDescriptorForRbm_sample1()
        {
            DataDescriptor des = new DataDescriptor();
            des.Features = new LearningFoundation.DataMappers.Column[6];

            // Label not used.
            des.LabelIndex = -1;

            des.Features = new Column[6];
            des.Features[0] = new Column { Id = 1, Name = "col1", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[1] = new Column { Id = 2, Name = "col2", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[2] = new Column { Id = 3, Name = "col3", Index = 2, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[3] = new Column { Id = 4, Name = "col4", Index = 3, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[4] = new Column { Id = 5, Name = "col5", Index = 4, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[5] = new Column { Id = 6, Name = "col6", Index = 5, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };

            return des;
        }
 

        /// <summary>
        /// RBM is not supervised algorithm. This is why we do not have a label.
        /// </summary>
        /// <returns></returns>
        private DataDescriptor getDescriptorForRbmTwoClassesClassifier()
        {
            DataDescriptor des = new DataDescriptor();
            des.Features = new LearningFoundation.DataMappers.Column[10];

            // Label not used.
            des.LabelIndex = -1;

            des.Features = new Column[10];
            des.Features[0] = new Column { Id = 1, Name = "col1", Index = 0, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[1] = new Column { Id = 2, Name = "col2", Index = 1, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[2] = new Column { Id = 3, Name = "col3", Index = 2, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[3] = new Column { Id = 4, Name = "col4", Index = 3, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[4] = new Column { Id = 5, Name = "col5", Index = 4, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[5] = new Column { Id = 6, Name = "col6", Index = 5, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[6] = new Column { Id = 7, Name = "col7", Index = 6, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[7] = new Column { Id = 8, Name = "col8", Index = 7, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[8] = new Column { Id = 9, Name = "col9", Index = 8, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
            des.Features[9] = new Column { Id = 10, Name = "col10", Index = 9, Type = ColumnType.NUMERIC, Values = null, DefaultMissingValue = 0 };
           
            return des;
        }

        /// <summary>
        /// Movies:
        /// 
        /// </summary>
        [Fact]
        public void RBMRecoomendationTest()
        {

        }


        /// <summary>
        /// TODO...
        /// </summary>
        [Theory]
        //[InlineData(1, 4096, new int[] { 4096, 250, 10 })]       
        [InlineData(1, 0.2, new int[] { 4096, 10 })]
        public void DigitRecognitionDeepTest(int iterations, double learningRate, int[] layers)
        {
            Debug.WriteLine($"{iterations}-{String.Join("", layers)}");

            LearningApi api = new LearningApi(RbmHandwrittenDigitUnitTests.GetDescriptorForDigits());

            // Initialize data provider
            // TODO: Describe Digit Dataset.
            api.UseCsvDataProvider(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\DigitDataset.csv"), ',', false, 0);
            api.UseDefaultDataMapper();

            api.UseDeepRbm(learningRate, iterations, layers);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            RbmDeepScore score = api.Run() as RbmDeepScore;
            watch.Stop();

            var testData = RbmHandwrittenDigitUnitTests.ReadData(Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\DigitTest.csv"));

            var result = api.Algorithm.Predict(testData, api.Context) as RbmDeepResult;
            var accList = new double[result.LayerResults.Count];
            var predictions = new double[result.LayerResults.Count][];

            int i = 0;
            foreach (var item in result.LayerResults)
            {
                predictions[i] = item.First().VisibleNodesPredictions;
                accList[i] = testData[i].GetHammingDistance(predictions[i]);

                i++;
            }

            RbmHandwrittenDigitUnitTests.WriteDeepResult(iterations, layers, accList, watch.ElapsedMilliseconds*1000);
            RbmHandwrittenDigitUnitTests.WriteOutputMatrix(iterations, layers, predictions, testData);
        }



        /// <summary>
        /// This test provides data, which contains two patterns.
        /// First pattern is concentrated on left and second pattern is concentrated on right.
        /// Sample data is stored in 'rbm_twoclass_sample.csv'.
        /// Data looks like:
        /// 011111000000
        /// 000000001110
        /// It is concentrated on left or on right.
        /// </summary>
        [Fact]
        public void Rbm_ClassifierTest()
        {
            var dataPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"RestrictedBolzmannMachine2\Data\rbm_twoclass_sample.csv");

            LearningApi api = new LearningApi(this.getDescriptorForRbmTwoClassesClassifier());

            // Initialize data provider
            api.UseCsvDataProvider(dataPath, ';', false, 1);
            api.UseDefaultDataMapper();
            api.UseDeepRbm(0.2, 1000, new int[] { 10, 2 });
             
            RbmResult score = api.Run() as RbmResult;

            double[][] testData = new double[5][];

            //
            // This test data contains two patterns. One is grouped at left and one at almost right.
            testData[0] = new double[] { 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 };
            testData[1] = new double[] { 1, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
            testData[2] = new double[] { 0, 0, 0, 0, 0, 1, 1, 1, 0, 0 };
            testData[3] = new double[] { 0, 0, 0, 0, 0, 1, 0, 1, 0, 0 };

            // This will be classified as third class.
            testData[4] = new double[] { 1, 1, 1, 0, 0, 1, 1, 1, 0, 0 };

            var result = api.Algorithm.Predict(testData, api.Context) as RbmResult;

            //
            // 2 * BIT1 + BIT2 of [0] and [1] should be same.
            // We don't know how RBM will classiffy data. We only expect that
            // same or similar pattern of data will be assigned to the same class.
            // Note, we have here two classes (two hiddne nodes).
            // First and second data sample are of same class. Third and fourth are also of same class.

            // Here we check first classs.
            Assert.True(2 * result.HiddenNodesPredictions[0][0] + result.HiddenNodesPredictions[0][1] ==
                2 * result.HiddenNodesPredictions[1][0] + result.HiddenNodesPredictions[1][1]);

            // Here is test for second class.
            Assert.True(2 * result.HiddenNodesPredictions[2][0] + result.HiddenNodesPredictions[2][1] ==
                2 * result.HiddenNodesPredictions[3][0] + result.HiddenNodesPredictions[3][1]);
        }
    }




}
