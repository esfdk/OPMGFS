namespace OPMGFS.Novelty
{
    using System.Collections.Generic;

    public abstract class NovelArchive
    {
        public NovelArchive()
        {
            Archive = new List<Solution>();
        }

        public List<Solution> Archive { get; protected set; }
    }
}
