using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Percsharp.Domain
{
    public class PerceptronTrainer
    {
        private static decimal[] DefaultInitWeight = new decimal[] { 0, 0 };
        private static decimal DefaultInitBias = 0;
        private static decimal DefaultLearningRate = 1;
        
        private Perceptron perceptron;
        public PerceptronTrainerState State { get; private set; }
        public int Runs = 0;
        public int MaxRuns = 1000;
        public int Errors = 0;
        public decimal LearningRate;

        private decimal initBias;
        public decimal InitBias => initBias;
        public Vector InitWeight { get; private set; }

        public decimal CurrentBias => perceptron.Bias;
        public Vector CurrentWeight => perceptron.W;        

        public bool Convergence = false;
        
        private int CurrentTrainStep = 0;
        public bool IsNewPass => CurrentTrainStep == 0;

        public PerceptronTrainer() :this(DefaultInitWeight, DefaultInitBias, DefaultLearningRate)
        {
        }

        public PerceptronTrainer(decimal[] initWeiht, decimal initBias, decimal learningRate)
        {
            this.Init(initWeiht, initBias, learningRate);
        }     

        

        public void Init(decimal[] initWeight, decimal initBias, decimal learningRate)
        {
            this.Runs = 0;
            this.InitWeight = initWeight;
            this.initBias = initBias;
            this.LearningRate = learningRate;
            this.perceptron = new Perceptron(new Vector(initWeight), initBias, learningRate);

            State = PerceptronTrainerState.Initialized;
        }

        public void Reset()
        {
            Init(DefaultInitWeight, DefaultInitBias, DefaultLearningRate);
        }

        /// <summary>
        /// Trains the perceptron
        /// </summary>
        /// <param name="positives">Positives</param>
        /// <param name="negatives">Negatives</param>
        /// <returns>true after convergence</returns>
        public bool TrainRun(List<Vector> positives, List<Vector> negatives)
        {
            this.State = PerceptronTrainerState.Training;

            this.Convergence = false;
            
            while (!TrainPass(positives, negatives))
            {
                if (Runs >= MaxRuns)
                {
                    return false;
                }
            }

            this.State = PerceptronTrainerState.Finished;

            return this.Convergence;
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
                State = PerceptronTrainerState.Finished;
                Convergence = true;
                return true;
            }

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
        }

        public Vector TrainStepCurrent(List<Vector> positives, List<Vector> negatives)
        {
            if (CurrentTrainStep < positives.Count)
            {
                return positives[CurrentTrainStep];
            }
            else if ((CurrentTrainStep - positives.Count) < negatives.Count)
            {
                return negatives[CurrentTrainStep - positives.Count];
            }

            return null;
        }

        public bool TrainStepPredict(List<Vector> positives, List<Vector> negatives)
        {           
            Vector v;
            if(CurrentTrainStep < positives.Count)
            {
                v = positives[CurrentTrainStep];
                if((v * perceptron.W) <= 0)
                {
                    return false;
                }
            }
            else if((CurrentTrainStep - positives.Count) < negatives.Count)
            {
                v = negatives[CurrentTrainStep - positives.Count];
                if((v * perceptron.W) > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void TrainStep(List<Vector> positives, List<Vector> negatives)
        {
            Vector v;
            if (CurrentTrainStep < positives.Count)
            {
                v = positives[CurrentTrainStep];
                if ((v * perceptron.W) <= 0)
                {
                    perceptron.W += LearningRate * v;
                    Errors++;
                }
            }
            else if ((CurrentTrainStep - positives.Count) < negatives.Count)
            {
                v = negatives[CurrentTrainStep - positives.Count];
                if ((v * perceptron.W) > 0)
                {
                    perceptron.W -= LearningRate * v;
                    Errors++;
                }
            }

            if(CurrentTrainStep > positives.Count + negatives.Count)
            {
                CurrentTrainStep = 0;
                if(Errors == 0)
                {
                    Convergence = true;
                }
            }
        }
    }
}
