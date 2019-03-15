using System;
using System.Collections.Generic;
using Bloom.Percsharp.Domain.Extensions;

namespace Bloom.Percsharp.Domain
{
    public class PerceptronTrainer
    {
        private static double[] DefaultInitWeight = new double[] { 0, 0 };
        private static double DefaultInitBias = 0;
        private static double DefaultLearningRate = 1;
        
        private Perceptron Perceptron;
        private List<PerceptronTrainerDatapoint> Datapoints;

        public PerceptronTrainerState State { get; private set; }
        public int Runs = 0;
        public int MaxRuns = 20000;
        public int Errors = 0;
        public double LearningRate;
        public bool BiasedLearning = false;
        public int Seed;

        #region Properties

        public double InitBias { get; private set; }
        public Vector InitWeight { get; private set; }

        public double CurrentBias => Perceptron.Bias;
        public Vector CurrentWeight => Perceptron.W;        

        public bool Convergence = false;
        
        private int CurrentTrainStep = 0;

        public Vector SeparationLineUpperEnd => (CurrentWeight * 10).Rotate(0.5 * Math.PI).Add(new Vector(-XDeviation, 0));

        public Vector SeparationLineLowerEnd => (CurrentWeight * 10).Rotate(-0.5 * Math.PI).Add(new Vector(-XDeviation, 0));

        public double XDeviation => CurrentBias == 0 || CurrentWeight[0] == 0 ? 0 : CurrentBias / CurrentWeight[0];

        public bool IsNewPass => CurrentTrainStep == 0;

        public int LastPassErrors { get; private set; }

        #endregion Properties

        #region Constructor

        public PerceptronTrainerStepPrediction NextTrainStepPrediction => TrainStepPredict();

        public PerceptronTrainer(int seed, double[] initWeiht, double initBias, double learningRate, List<Vector> positives, List<Vector> negatives)
        {
            this.Init(seed, initWeiht, initBias, learningRate, positives, negatives);
        }

        #endregion Constructor

        #region Initialization

        public void Init(int seed, double[] initWeight, double initBias, double learningRate, List<Vector> positives, List<Vector> negatives)
        {
            this.Seed = seed;
            this.Runs = 0;
            this.InitWeight = initWeight;
            this.InitBias = initBias;
            this.LearningRate = learningRate;
            this.Perceptron = new Perceptron(new Vector(initWeight), initBias, learningRate);

            this.Datapoints = new List<PerceptronTrainerDatapoint>();
            positives.ForEach(p => this.Datapoints.Add(new PerceptronTrainerDatapoint() { Datapoint = p, IsPositive = true } ));
            negatives.ForEach(n => this.Datapoints.Add(new PerceptronTrainerDatapoint() { Datapoint = n, IsPositive = false } ));
            Datapoints.Shuffle(seed);

            this.BiasedLearning = false;

            State = PerceptronTrainerState.Initialized;
        }

        public void Reset()
        {
            Init(this.Seed, DefaultInitWeight, DefaultInitBias, DefaultLearningRate, null, null);
        }

        #endregion Initialization

        #region Train

        public bool TrainRun()
        {
            this.State = PerceptronTrainerState.Training;

            this.Convergence = false;
            
            while (!TrainPass())
            {
                if (Runs >= MaxRuns)
                {
                    return false;
                }
            }

            this.State = PerceptronTrainerState.Finished;

            return this.Convergence;
        }
        
        public bool TrainPass()
        {
            State = PerceptronTrainerState.Training;            

            do
            {
                TrainStep();
            }
            while (!IsNewPass);
            Datapoints.Shuffle(Seed++);

            return Convergence;
        }             

        public void TrainStep()
        {
            State = PerceptronTrainerState.Training;

            PerceptronTrainerDatapoint CurrentDatapoint = Datapoints[CurrentTrainStep];
            if(CurrentDatapoint.IsPositive)
            {
                if (BiasedLearning)
                {
                    LearnPositiveBiased(CurrentDatapoint.Datapoint);
                }
                else
                {
                    LearnPositive(CurrentDatapoint.Datapoint);
                }
            }
            else
            {
                if (BiasedLearning)
                {
                    LearnNegativeBiased(CurrentDatapoint.Datapoint);
                }
                else
                {
                    LearnNegative(CurrentDatapoint.Datapoint);
                }
            }            

            if(CurrentTrainStep >= Datapoints.Count - 1)
            {                
                CurrentTrainStep = 0;
                LastPassErrors = Errors;
                Runs++;
                if (Errors == 0)
                {
                    State = PerceptronTrainerState.Finished;
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
            State = PerceptronTrainerState.Training;
            PerceptronTrainerStepPrediction result = new PerceptronTrainerStepPrediction();

            PerceptronTrainerDatapoint CurrentDatapoint = Datapoints[CurrentTrainStep];
            if (CurrentDatapoint.IsPositive)
            {
                result.DataPoint = CurrentDatapoint.Datapoint;
                result.isPositiveDatapoint = true;
                if (BiasedLearning)
                {
                    return PredictPositiveBiased(result);
                }
                else
                {
                    return PredictPositive(result);
                }
            }
            else
            {
                result.DataPoint = CurrentDatapoint.Datapoint;
                result.isPositiveDatapoint = false;

                if (BiasedLearning)
                {
                    return PredictNegativeBiased(result);
                }
                else
                {
                    return PredictNegative(result);
                }
            }            
        }

        #region Learn
        private void LearnNegative(Vector v)
        {
            if ((v * Perceptron.W) > 0)
            {
                Perceptron.W -= LearningRate * v;
                Errors++;
            }
        }

        private void LearnNegativeBiased(Vector v)
        {
            if ((v * Perceptron.W + Perceptron.Bias) > 0)
            {
                Perceptron.W -= LearningRate * v;
                Perceptron.Bias -= LearningRate;
                Errors++;
            }
        }

        private void LearnPositive(Vector v)
        {
            if ((v * Perceptron.W) <= 0)
            {
                Perceptron.W += LearningRate * v;
                Errors++;
            }
        }

        private void LearnPositiveBiased(Vector v)
        {
            if ((v * Perceptron.W + Perceptron.Bias) <= 0)
            {
                Perceptron.W += LearningRate * v;
                Perceptron.Bias += LearningRate;
                Errors++;
            }
        }

        #endregion Learn        

        #region Predict

        private PerceptronTrainerStepPrediction PredictPositiveBiased(PerceptronTrainerStepPrediction result)
        {
            if ((result.DataPoint * this.Perceptron.W + this.Perceptron.Bias) <= 0)
            {
                result.Error = true;
                result.CurrentWeight = CurrentWeight;
                result.Correction = result.DataPoint * LearningRate;
                result.ResultingWeight = result.CurrentWeight + result.Correction;
                result.CurrentBias = CurrentBias;
                result.ResultingBias = CurrentBias + LearningRate;

                return result;
            }
            else
            {
                result.Error = false;
                return result;
            }
        }

        private PerceptronTrainerStepPrediction PredictPositive(PerceptronTrainerStepPrediction result)
        {
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

        private PerceptronTrainerStepPrediction PredictNegative(PerceptronTrainerStepPrediction result)
        {
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

        private PerceptronTrainerStepPrediction PredictNegativeBiased(PerceptronTrainerStepPrediction result)
        {
            if ((result.DataPoint * Perceptron.W + Perceptron.Bias) > 0)
            {
                result.Error = true;
                result.CurrentWeight = CurrentWeight;
                result.Correction = result.DataPoint * LearningRate * -1;
                result.ResultingWeight = result.CurrentWeight + result.Correction;
                result.CurrentBias = CurrentBias;
                result.ResultingBias = CurrentBias - LearningRate;

                return result;
            }
            else
            {
                result.Error = false;
                return result;
            }
        }

        #endregion Predict

        #endregion Train
    }
}
