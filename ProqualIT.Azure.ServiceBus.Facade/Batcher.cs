using ProqualIT.Azure.ServiceBus.Facade.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class Batcher<T>
    {
        private readonly ILog log;

        private BufferBlock<T> buffer;

        private ActionBlock<T[]> actionBlock;

        public Batcher(
            Action<T[]> processBatch,
            int batchSize,
            TimeSpan maxTimeWaitingForBatch)
        {
            log = LogProvider.For<Batcher<T>>();
            Initialize(batchSize, processBatch, maxTimeWaitingForBatch);
        }

        private void Initialize(int batchSize,
            Action<T[]> processBatch,
            TimeSpan maxTimeWaitingForBatch)
        {
            buffer = new BufferBlock<T>();

            var batchStockEvents = new BatchBlock<T>(batchSize);

            // Use a timer to make sure that items do not remain in memory for too long
            var triggerBatchTimer = new Timer(delegate
            {
                log.Info("Timer expired. Triggering batch.");
                batchStockEvents.TriggerBatch();
            });

            // Use a transform block to reset the timer whenever an item is inserted to avoid unnessary batches
            var timeoutBlock = new TransformBlock<T, T>((value) =>
            {
                log.Info("Resetting timer");
                triggerBatchTimer.Change(maxTimeWaitingForBatch, TimeSpan.FromMilliseconds(-1));
                return value;
            });

            actionBlock = new ActionBlock<T[]>(processBatch);

            buffer.LinkTo(timeoutBlock);
            timeoutBlock.LinkTo(batchStockEvents);
            batchStockEvents.LinkTo(actionBlock);

            actionBlock.Completion.ContinueWith(x =>
            {
                if (x.Exception != null)
                {
                    log.ErrorException("Exiting batcher due to error.", x.Exception);
                }

            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void Post(T someObject)
        {
            if (actionBlock.Completion.IsCompleted)
            {
                throw new InvalidOperationException("The batcher is not accepting new objects as it is in a failed state");
            }

            buffer.Post(someObject);
        }
    }
}
