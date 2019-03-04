﻿using System;
using System.Collections.Generic;
using LearningAPIFramework.Tensor;

namespace LearningAPIFramework.ConvolutionalNetze.Layers
{
	/// <summary>
	/// Some vector elements could be out of the bounds of classes set by supervised learning data
	/// after applying softmax, each element is in the interval bound by the available probability classes taught to 
	/// the network. Softmax is often used in neural networks, to map the non-normalized output to a probability distribution over
	/// predicted output classes
	/// </summary>
	public class SoftmaxLayer : LastLayerBase, IClassificationLayer 
    {
        public SoftmaxLayer(int classCount)
        {
            this.ClassCount = classCount;
        }

        public int ClassCount { get; set; }

        /// <summary>
        /// This computes the cross entropy loss and its gradient (not the softmax gradient)
        /// </summary>
        /// <param name="y"></param>
        /// <param name="loss"></param>
        public override void Backward(Volume y, out double loss)
        {
            // input gradient = pi - yi
            y.SubtractFrom(this.OutputActivation, this.InputActivationGradients.ReShape(this.OutputActivation.Shape.Dimensions));

			//loss is the class negative log likelihood
			loss = 0;
            for (var n = 0; n < y.Shape.Dimensions[3]; n++)
            {
                for (var d = 0; d < y.Shape.Dimensions[2]; d++)
                {
                    for (var h = 0; h < y.Shape.Dimensions[1]; h++)
                    {
                        for (var w = 0; w < y.Shape.Dimensions[0]; w++)
                        {
                            var expected = y.Get(w, h, d, n);
                            var actual = this.OutputActivation.Get(w, h, d, n);
                            if (Ops<double>.Zero.Equals(actual))
                                actual = Ops<double>.Epsilon;
                            var current = Ops<double>.Multiply(expected, Ops<double>.Log(actual));

                            loss = Ops<double>.Add(loss, current);
                        }
                    }
                }
            }

            loss = Ops<double>.Negate(loss);

            if (Ops<double>.IsInvalid(loss))
                throw new ArgumentException("Error during calculation!");
        }

        public override void Backward(Volume outputGradient)
        {
            throw new NotImplementedException();
        }

        protected override Volume Forward(Volume input, bool isTraining = false)
        {
            input.Softmax(this.OutputActivation);
            return this.OutputActivation;
        }

        public override void Init(int inputWidth, int inputHeight, int inputDepth)
        {
            base.Init(inputWidth, inputHeight, inputDepth);

            var inputCount = inputWidth * inputHeight * inputDepth;
            this.OutputWidth = 1;
            this.OutputHeight = 1;
            this.OutputDepth = inputCount;
        }
    }
}