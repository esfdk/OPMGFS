namespace OPMGFS.Novelty
{
    using System.Collections.Generic;

    /// <summary>
    /// An archive of novel solutions in novelty search.
    /// </summary>
    public abstract class NovelArchive
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NovelArchive"/> class. 
        /// </summary>
        protected NovelArchive()
        {
            this.Archive = new List<Solution>();
        }

        /// <summary>
        /// Gets or sets the novel archive.
        /// </summary>
        public List<Solution> Archive { get; protected set; }
    }
}
