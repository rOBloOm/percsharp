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
        
        private Perceptron Perceptron;
        private List<Vector> Positives;
        private List<Vector> Negatives;

        public PerceptronTrainerState State { get; private set; }
        public int Runs = 0;
        public int MaxRuns = 1000;
        public int Errors = 0;
        public decimal LearningRate;

        public decimal InitBias { get; private set; }
        public Vector InitWeight { get; private set; }

        public decimal CurrentBias => Perceptron.Bias;
        public Vector CurrentWeight => Perceptron.W;        

        public bool Convergence = false;
        
        private int CurrentTrainStep = 0;
        public bool IsNewPass => CurrentTrainStep == 0;

        public int LastPassErrors { get; private set; }

        #region Constructor

        public PerceptronTrainerStepPrediction NextTrainStepPrediction => TrainStepPredict();

        public PerceptronTrainer(decimal[] initWeiht, decimal initBias, decimal learningRate, List<Vector> positives, List<Vector> negatives)
        {
            this.Init(initWeiht, initBias, learningRate, positives, negatives);
        }

        #endregion Constructor

        #region Initialization

        public void Init(decimal[] initWeight, decimal initBias, decimal learningRate, List<Vector> positives, List<Vector> negatives)
        {
            this.Runs = 0;
            this.InitWeight = initWeight;
            this.InitBias = initBias;
            this.LearningRate = learningRate;
            this.Perceptron = new Perceptron(new Vector(initWeight), initBias, learningRate);
            this.Positives = positives;
            this.Negatives = negatives;

            State = PerceptronTrainerState.Initialized;
        }

        public void Reset()
        {
            Init(DefaultInitWeight, DefaultInitBias, DefaultLearningRate, null, null);
        }

        #endregion Initialization

        #region Train

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
                if((v * Perceptron.W) <= 0)
                {
                    Perceptron.W += LearningRate * v;
                    Errors++;
                }                
            }
            else
            {
                if ((v * Perceptron.W) > 0)
                {
                    Perceptron.W -= LearningRate * v;
                    Errors++;
                }
            }
        }        

        public void TrainStep(List<Vector> positives, List<Vector> negatives)
        {
            Vector v;
            if (CurrentTrainStep < positives.Count)
            {
                v = positives[CurrentTrainStep];
                if ((v * Perceptron.W) <= 0)
                {
                    Perceptron.W += LearningRate * v;
                    Errors++;
                }
            }
            else if ((CurrentTrainStep - positives.Count) < negatives.Count)
            {
                v = negatives[CurrentTrainStep - positives.Count];
                if ((v * Perceptron.W) > 0)
                {
                    Perceptron.W -= LearningRate * v;
                    Errors++;
                }
            }

            if(CurrentTrainStep >= positives.Count + negatives.Count - 1)
            {
                CurrentTrainStep = 0;
                LastPassErrors = Errors;
                Runs++;
                if (Errors == 0)
                {
                    Convergence = true;
                }
                else
                {                    
                    Errors = 0;
                }                
            }
            else
            {
                CurrentTrainStep++;
            }            
        }

        private PerceptronTrainerStepPrediction TrainStepPredict()
        {
            PerceptronTrainerStepPrediction result = new PerceptronTrainerStepPrediction();
            if (CurrentTrainStep < Positives.Count)
            {
                result.DataPoint = Positives[CurrentTrainStep];
                if ((result.DataPoint * this.Perceptron.W) <= 0)
                {
                    result.Error = true;
                    result.CurrentWeight = CurrentWeight;
                    result.Correction = result.DataPoint * LearningRate;
                    result.ResultingWeight = result.CurrentWeight + result.Correction;

                    return result;
                }
                else
                {
                    result.Error = false;
                    return result;
                }
            }
            else if ((CurrentTrainStep - Positives.Count) < Negatives.Count)
            {
                result.DataPoint = Negatives[CurrentTrainStep - Positives.Count];
                if ((result.DataPoint * Perceptron.W) > 0)
                {
                    result.Error = true;
                    result.CurrentWeight = CurrentWeight;
                    result.Correction = result.DataPoint * LearningRate * -1;
                    result.ResultingWeight = result.CurrentWeight + result.Correction;
                    return result;
                }
                else
                {
                    result.Error = false;
                    return result;
                }
            }

            return null;
        }

        #endregion Train
    }
}
