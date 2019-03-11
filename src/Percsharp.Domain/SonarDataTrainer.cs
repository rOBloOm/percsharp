using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Percsharp.Domain
{
    public class SonarDataTrainer
    {
        private List<Vector> sonarData;
        private List<bool> sonarClassification;

        private Perceptron perceptron;

        public List<Vector> SonarData => sonarData;
        public List<bool> SonarClassification => sonarClassification;

        public int Errors = 0;

        public SonarDataTrainer(List<string[]> csvData)
        {
            Vector initWeights = new Vector(60);
            perceptron = new Perceptron(initWeights, 0, 1);

            sonarData = new List<Vector>();
            sonarClassification = new List<bool>();

            csvData.ForEach(dataset =>
            {
                double[] data = new double[60];
                for(int i = 0; i < 60; i++)
                {
                    data[i] = double.Parse(dataset[i]);
                }

                sonarData.Add(new Vector(data));
                sonarClassification.Add(dataset[60] == "M");
            });
        }

        public void Train()
        {
            Errors = 0;
            for(int i = 0; i < sonarData.Count; i++)
            {
                if(sonarClassification[i])
                {
                    if(perceptron.W * sonarData[i] < 0)
                    {
                        perceptron.W += sonarData[i];
                        Errors++;
                    }
                }
                else
                {
                    if(perceptron.W * sonarData[i] >= 0)
                    {
                        perceptron.W -= sonarData[i];
                        Errors++;
                    }
                }
            }
        }
    }
}
