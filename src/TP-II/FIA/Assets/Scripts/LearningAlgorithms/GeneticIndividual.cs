using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MetaHeuristic;

public class GeneticIndividual : Individual {


	public GeneticIndividual(int[] topology, int numberOfEvaluations, MutationType mutation) : base(topology, numberOfEvaluations, mutation) {
	}

	public override void Initialize () 
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
        GeneticIndividual new_ind = new GeneticIndividual(this.topology, this.maxNumberOfEvaluations, this.mutation);

        genotype.CopyTo(new_ind.genotype, 0);
        new_ind.fitness = this.Fitness;
        new_ind.evaluated = false;

        return new_ind;
    }


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
        }
    }
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

    
    public void MutateGaussian(float probability)
    {
        /* YOUR CODE HERE! */
        throw new System.NotImplementedException();
    }

    public override void Crossover(Individual partner, float probability)
    {
        /* YOUR CODE HERE! */
        /* Nota: O crossover deverá alterar ambos os indivíduos */
        throw new System.NotImplementedException();
    }


}
