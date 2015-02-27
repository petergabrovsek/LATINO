﻿/*=====================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    BinarySvm.cs
 *  Desc:    Tutorial 5.1: Supervised machine learning
 *  Created: Apr-2010
 *
 *  Authors: 
 *
 **********************************************************************/

using System;
using System.Threading.Tasks;
using Latino;
using Latino.Model;
using Latino.Model.Eval;
using Tutorial.Case.Model;

namespace Tutorial.Case.Validation
{
    public class NFoldParallel : Tutorial<NFoldParallel>
    {
        public override void Run(object[] args)
        {
            // get labeled data
            BinarySvm classifierInst = BinarySvm.RunInstanceNull(args);
            var labeledData = (LabeledDataset<string, SparseVector<double>>)classifierInst.Result["labeled_data"];

            // convert dataset to binary vector
            var ds = (LabeledDataset<string, BinaryVector>)labeledData.ConvertDataset(typeof(BinaryVector), false);

            // cross validation with task validator
            var validator = new TaskCrossValidator<string, BinaryVector>(new Func<IModel<string, BinaryVector>>[]
                {
                    // model instances are constructed on the fly
                    () => new NaiveBayesClassifier<string>()
                })
            {
                NumFolds = 10, // default
                IsStratified = true, // default
                ExpName = "", // default

                Dataset = ds,
                OnAfterTrain = (sender, foldN, model, trainSet) =>
                {
                    var m = (NaiveBayesClassifier<string>)model;
                    // do stuff after model is trained for a fold...
                },
                OnAfterPrediction = (sender, foldN, model, le, prediction) =>
                {
                    lock (Output) Output.WriteLine("actual: {0} \tpredicted: {1}\t score: {2:0.0000}", le.Label, prediction.BestClassLabel, prediction.BestScore);
                }
            };


            var cores = (int)(Math.Round(Environment.ProcessorCount * 0.9) - 1); // use 90% of cpu cores
            Output.WriteLine("Multi-threaded using {0} cores\n", cores);
            Output.Flush();

            Parallel.ForEach(
                validator.GetFoldAndModelTasks(),
                new ParallelOptions { MaxDegreeOfParallelism = cores },
                foldTask => Parallel.ForEach(
                    foldTask(),
                    new ParallelOptions { MaxDegreeOfParallelism = cores },
                    modelTask => modelTask()
                )
            );


            Output.WriteLine("Sum confusion matrix:");
            PerfMatrix<string> sumPerfMatrix = validator.PerfData.GetSumPerfMatrix("", validator.GetModelName(0));
            Output.WriteLine(sumPerfMatrix.ToString());
            Output.WriteLine("Average accuracy: {0:0.00}", sumPerfMatrix.GetAccuracy());
            foreach (string label in validator.PerfData.GetLabels("", validator.GetModelName(0)))
            {
                double stdDev;
                Output.WriteLine("Precision for '{0}': {1:0.00} std. dev: {2:0.00}", label,
                    validator.PerfData.GetAvg("", validator.GetModelName(0), ClassPerfMetric.Precision, label, out stdDev), stdDev);
            }
        }
    }
}