using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace JigsawPuzzle.Analysis
{
    public class Performance
    {
        /* const */


        /* field */
        public DateTime LastUpdateTime { get; private set; }
        protected internal CancellationToken CancellationToken { get; private set; }

        public static Performance History;

        /* inter */

        /* ctor */
        protected internal Performance()
        {
            LastUpdateTime = DateTime.Now;
        }

        /* func */
        public void OnEnable()
        {
            lock (this)
            {
                if (CancellationToken != default)
                    return;
                CancellationToken = new CancellationToken();
                Task.Run(Update, CancellationToken);
            }
        }
        public void OnDisable()
        {
            lock (this)
            {
                if (CancellationToken == default)
                    return;
                CancellationToken.ThrowIfCancellationRequested();
            }
        }
        public void Update()
        {

        }

        /* operator */

    }
}