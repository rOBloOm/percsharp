using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace percsharp.domain
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
            perceptron = new Perceptron(initWeights);

            sonarData = new List<Vector>();
            sonarClassification = new List<bool>();

            csvData.ForEach(dataset =>
            {
                decimal[] data = new decimal[60];
                for(int i = 0; i < 60; i++)
                {
                    data[i] = decimal.Parse(dataset[i]);
                }

                sonarData.Add(new Vector(data));
                sonarClassification.Add(dataset[60] == "R");
                Console.WriteLine(dataset[60]=="R");
            });
        }

        public void Train()
        {
            Errors = 0;
            for(int i = 0; i < sonarData.Count; i++)
            {
                bool classification = perceptron.Classify(sonarData[i]);
                if (classification == sonarClassification[i])
                    continue;

                Errors++;
                perceptron.Learn(sonarData[i], sonarClassification[i] ? 1 : -1);
            }
        }
    }
}
