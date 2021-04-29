using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class TournamentSelection : SelectionMethod
{
    private int tournamentSize;

    public TournamentSelection(int tournamentSize) : base()
    {
        this.tournamentSize = tournamentSize;
    }

    public override List<Individual> selectIndividuals(List<Individual> oldpop, int num)
    {
        if (oldpop.Count < tournamentSize)
        {
            throw new System.Exception("The population size is smaller than the tournament size.");
        }

        List<Individual> selectedInds = new List<Individual>();
        for (int i = 0; i < num; i++)
        {
            selectedInds.Add(tournamentSelection(oldpop, tournamentSize).Clone());
        }

        return selectedInds;
    }

    // Tournament Selection Algorithm
    // This algorithm is performs a tournament in a given population according to a size tournamentSize
    // DESCRIPTION:
    // A random individual is selected and competes with the best individual found in the population so far 
    // If the best performant individual hasn't been  found then the the first randomly picked individual will become it.
    // The algorithm returns the best individual found in all of the tournamentSize iterations .
    public Individual tournamentSelection(List<Individual> population, int tournamentSize)
    {
        GeneticIndividual best = null;
        for (var i = 0; i < tournamentSize; ++i)
        {
            GeneticIndividual individual = ((GeneticIndividual)population[Random.Range(0, population.Count) - 1]);
            if (best == null || individual.Fitness > best.Fitness)
            {
                best = (GeneticIndividual)individual.Clone();
            }
        }
        return best;
    }
}
