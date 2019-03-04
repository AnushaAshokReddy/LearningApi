using System;
using System.Collections.Generic;
using LearningAPIFramework.Tensor;

namespace LearningAPIFramework.ConvolutionalNetze.Training
{
    /// <summary>
    ///     Gradient descent Algorithm for weight estimation
    /// </summary>
    public class GradientDescentTrainer : TrainerBase
    {
        private readonly List<Volume > regGrads = new List<Volume >();

        // last iteration gradients (used for momentum calculations)
        private readonly List<Volume > velocities = new List<Volume >();

        public GradientDescentTrainer(INet net) : base(net)
        {
        }

        public double L1Decay { get; set; }

        public double L2Decay { get; set; }

        public double Momentum { get; set; }

        public double LearningRate { get; set; }

        public void Dispose()
        {
            foreach (var v in this.velocities)
            {
                v.Dispose();
            }

            foreach (var r in this.regGrads)
            {
                r.Dispose();
            }
        }

        protected override void TrainImplem()
        {
            var parametersAndGradients = this.Net.GetParametersAndGradients();
            var isMomentumGreaterThanZero = Ops<double>.GreaterThan(this.Momentum, Ops<double>.Zero);

            // initialize lists for accumulators. Will only be done once on first iteration
            if (this.velocities.Count == 0)
            {
                foreach (var parameter in parametersAndGradients)
                {
                    this.velocities.Add(BuilderInstance .Volume.SameAs(parameter.Volume.Shape));
                    this.regGrads.Add(BuilderInstance .Volume.SameAs(parameter.Volume.Shape));
                }
            }

            // perform an update for all sets of weights
            for (var i = 0; i < parametersAndGradients.Count; i++)
            {
                var parametersAndGradient = parametersAndGradients[i];
                var parameters = parametersAndGradient.Volume;
                var gradients = parametersAndGradient.Gradient;
                var velocity = this.velocities[i];

                var batchAdjustedLearningRate = Ops<double>.Divide(this.LearningRate, Ops<double>.Cast(this.BatchSize));

                // delta = gradient + regularization;
                gradients.Multiply(batchAdjustedLearningRate, gradients);

                if (isMomentumGreaterThanZero)
                {
                    // sgd with momentum update
                    velocity.Multiply(this.Momentum, velocity); // step
                    velocity.Add(gradients, velocity);
                    velocity.SubtractFrom(parameters, parameters); // apply corrected gradient
                }
                else
                {
                    // vanilla sgd
                    gradients.SubtractFrom(parameters, parameters);
                }

                // zero out gradient so that we can begin accumulating anew
                gradients.Clear();
            }
        }
    }
}