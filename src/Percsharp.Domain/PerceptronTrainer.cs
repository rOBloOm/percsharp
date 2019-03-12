﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Percsharp.Domain
{
    public class PerceptronTrainer
    {
        private static double[] DefaultInitWeight = new double[] { 0, 0 };
        private static double DefaultInitBias = 0;
        private static double DefaultLearningRate = 1;
        
        private Perceptron Perceptron;
        private List<Vector> Positives;
        private List<Vector> Negatives;

        public PerceptronTrainerState State { get; private set; }
        public int Runs = 0;
        public int MaxRuns = 5000;
        public int Errors = 0;
        public double LearningRate;

        public double InitBias { get; private set; }
        public Vector InitWeight { get; private set; }

        public double CurrentBias => Perceptron.Bias;
        public Vector CurrentWeight => Perceptron.W;        

        public bool Convergence = false;
        
        private int CurrentTrainStep = 0;
        public bool IsNewPass => CurrentTrainStep == 0;

        public int LastPassErrors { get; private set; }

        #region Constructor

        public PerceptronTrainerStepPrediction NextTrainStepPrediction => TrainStepPredict();

        public PerceptronTrainer(double[] initWeiht, double initBias, double learningRate, List<Vector> positives, List<Vector> negatives)
        {
            this.Init(initWeiht, initBias, learningRate, positives, negatives);
        }

        #endregion Constructor

        #region Initialization

        public void Init(double[] initWeight, double initBias, double learningRate, List<Vector> positives, List<Vector> negatives)
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

            return Convergence;
        }             

        public void TrainStep()
        {
            State = PerceptronTrainerState.Training;
            Vector v;
            if (CurrentTrainStep < Positives.Count)
            {
                v = Positives[CurrentTrainStep];
                if ((v * Perceptron.W) <= 0)
                {
                    Perceptron.W += LearningRate * v;
                    Errors++;
                }
            }
            else if ((CurrentTrainStep - Positives.Count) < Negatives.Count)
            {
                v = Negatives[CurrentTrainStep - Positives.Count];
                if ((v * Perceptron.W) > 0)
                {
                    Perceptron.W -= LearningRate * v;
                    Errors++;
                }
            }

            if(CurrentTrainStep >= Positives.Count + Negatives.Count - 1)
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
            if (CurrentTrainStep < Positives.Count)
            {                
                result.DataPoint = Positives[CurrentTrainStep];
                result.isPositiveDatapoint = true;
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
                result.isPositiveDatapoint = false;
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
