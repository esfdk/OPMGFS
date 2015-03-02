using System;

namespace OPMGFS.Novelty
{
    public abstract class NoveltySearcher
    {
        protected Random Random;

        public Population FeasiblePopulation { get; protected set; }

        public Population InfeasiblePopulation { get; protected set; }

        public NovelArchive Archive { get; protected set; }

        protected abstract void NextGeneration();

    }
}
