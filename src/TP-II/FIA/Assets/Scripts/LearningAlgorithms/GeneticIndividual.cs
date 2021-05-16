using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MetaHeuristic;

public class GeneticIndividual : Individual
{
    public GeneticIndividual(int[] topology, int numberOfEvaluations, MutationType mutation, CrossoverType crossover, FitnessType fitnessFunction) : base(topology, numberOfEvaluations, mutation, crossover, fitnessFunction)
    {
    }

    public override void Initialize()
    {
        for (int i = 0; i < totalSize; i++)
        {
            genotype[i] = Random.Range(-1.0f, 1.0f);
        }
    }

    public override void Initialize(NeuralNetwork nn)
    {
        int count = 0;
        for (int i = 0; i < topology.Length - 1; i++)
        {
            for (int j = 0; j < topology[i]; j++)
            {
                for (int k = 0; k < topology[i + 1]; k++)
                {
                    genotype[count++] = nn.weights[i][j][k];
                }

            }
        }
    }

    public override Individual Clone()
    {
        GeneticIndividual new_ind = new GeneticIndividual(this.topology, this.maxNumberOfEvaluations, this.mutation, this.crossover, this.fitnessFunction);

        genotype.CopyTo(new_ind.genotype, 0);
        new_ind.fitness = this.Fitness;
        new_ind.evaluated = false;

        return new_ind;
    }


    // Mutation Implementations.
    public override void Mutate(float probability)
    {
        switch (mutation)
        {
            case MetaHeuristic.MutationType.Gaussian:
                MutateGaussian(probability);
                break;
            case MetaHeuristic.MutationType.Random:
                MutateRandom(probability);
                break;
            case MetaHeuristic.MutationType.None:
                throw new System.Exception("No mutation method selected!");
        }
    }

    // Uniform Mutation
    // DESCRIPTION:
    // Mutate all the genes in the individual uniformly. The mutation occurrence is determined by
    // a certain user defined probability. The value for a given gene is a uniformly picked 
    // value from the range [-1.0f, 1.0f].
    public void MutateRandom(float probability)
    {
        for (int i = 0; i < totalSize; i++)
        {
            if (Random.Range(0.0f, 1.0f) < probability)
            {
                genotype[i] = Random.Range(-1.0f, 1.0f);
            }
        }
    }

    // NonUniform Mutation (Gaussian)
    // DESCRIPTION:
    // Mutate all the genes in the individual non uniformly. The mutation occurrence is determined by
    // a certain user defined probability. The value for a given gene is value from the range [-1.0f, 1.0f]
    // that is picked according to a non-uniform gaussian distribution.
    public void MutateGaussian(float probability)
    {
        float mean = 0;
        float stdev = 0.5f;
        for (var i = 0; i < totalSize; ++i)
        {
            if (Random.Range(0.0f, 1.0f) < probability)
            {
                genotype[i] += NextGaussian(mean, stdev);
            }
        }
    }

    // Crossover Implementations
    public override void Crossover(Individual partner, float probability)
    {
        switch (crossover)
        {
            case MetaHeuristic.CrossoverType.OnePoint:
                Crossover1(partner, probability);
                break;
            case MetaHeuristic.CrossoverType.TwoPoint:
                Crossover2(partner, probability);
                break;
            case MetaHeuristic.CrossoverType.None:
                throw new System.Exception("No crossover method selected!");
        }
    }


    // Single-point crossover
    // DESCRIPTION:
    // A point on both parents' chromosomes is picked randomly, and designated a 'crossover point'.
    // Bits to the right of that point are swapped between the two parent chromosomes.
    public void Crossover1(Individual partner, float probability)
    {
        GeneticIndividual other = ((GeneticIndividual)partner);
        if (Random.Range(0.0f, 1.0f) < probability)
        {
            int crosspoint = Random.Range(0, genotype.Length - 2);
            for (var i = crosspoint; i < genotype.Length; ++i)
            {
                float tmp = genotype[i];
                genotype[i] = other.genotype[i];
                other.genotype[i] = tmp;
            }
        }

    }

    // Two-point crossover
    // DESCRIPTION:
    // In two-point crossover, two crossover points are picked randomly from the parent chromosomes.
    // The bits in between the two points are swapped between the parent organisms.
    public void Crossover2(Individual partner, float probability)
    {
        GeneticIndividual other = ((GeneticIndividual)partner);
        if (Random.Range(0.0f, 1.0f) < probability)
        {
            var crosspoint1 = Random.Range(0, genotype.Length - 2);
            var crosspoint2 = Random.Range(crosspoint1, genotype.Length - 1);
            for (var i = crosspoint1; i < crosspoint2; ++i)
            {
                float tmp = genotype[i];
                genotype[i] = other.genotype[i];
                other.genotype[i] = tmp;
            }
        }
    }


}
