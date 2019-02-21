using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace percsharp.domain
{
    public class NeuralNetworkPerceptron
    {
        private static decimal[] DefaultInitWeight = new decimal[] { 0, 0 };
        private static decimal DefaultInitBias = 0;
        private static decimal DefaultLearningRate = 1;

        private Perceptron perceptron;

        public int Runs = 0;
        public int MaxRuns = 1000;
        public int Errors = 0;
        public decimal LearningRate;

        private decimal initBias;
        public decimal InitBias => initBias;

        private Vector initWeight;
        public Vector InitWeight => initWeight;

        public decimal CurrentBias => perceptron.Bias;
        public Vector CurrentWeight => perceptron.W;        

        public bool Convergence = false;

        private int CurrentTrainStep = 0;

        public NeuralNetworkPerceptron() :this(DefaultInitWeight, DefaultInitBias, DefaultLearningRate)
        {
        }

        public NeuralNetworkPerceptron(decimal[] initWeiht, decimal initBias, decimal learningRate)
        {
            this.initWeight = initWeiht;
            this.initBias = initBias;
            this.LearningRate = learningRate;

            perceptron = new Perceptron(new Vector(initWeiht), initBias, learningRate);
        }

        public void Reset()
        {
            Reset(DefaultInitWeight, DefaultInitBias);
        }

        public void Reset(decimal[] initWeights, decimal initBias)
        {
            Runs = 0;
        }

        /// <summary>
        /// Trains the perceptron
        /// </summary>
        /// <param name="positives">Positives</param>
        /// <param name="negatives">Negatives</param>
        /// <returns>true after convergence</returns>
        public bool TrainRun(List<Vector> positives, List<Vector> negatives)
        {
            Convergence = false;
            
            while (!TrainPass(positives, negatives))
            {
                if (Runs >= MaxRuns)
                {
                    return false;
                }
            }

            return Convergence;
        }

        /// <summary>
        /// Does one pass of leraning
        /// </summary>
        /// <param name="positives"></param>
        /// <param name="negatives"></param>
        /// <returns></returns>
        public bool TrainPass(List<Vector> positives, List<Vector> negatives)
        {
            Runs++;
            Errors = 0;

            positives.ForEach(v => TrainPassStep(v, true));
            negatives.ForEach(v => TrainPassStep(v, false));

            if (Errors <= 0)
            {
                Convergence = true;
                return true;
            }

            Console.WriteLine($"Run: {Runs}, Errors: {Errors}, Weight: {perceptron.W}");
            return false;
        }

        private void TrainPassStep(Vector v, bool expectPositive)
        {
            if(expectPositive)
            {
                if((v * perceptron.W) <= 0)
                {
                    perceptron.W += LearningRate * v;
                    Errors++;
                }                
            }
            else
            {
                if ((v * perceptron.W) > 0)
                {
                    perceptron.W -= LearningRate * v;
                    Errors++;
                }
            }

            CurrentTrainStep++;
        }
    }
}
