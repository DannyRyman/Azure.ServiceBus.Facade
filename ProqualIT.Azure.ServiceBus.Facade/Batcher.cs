using ProqualIT.Azure.ServiceBus.Facade.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    public class Batcher<T>
    {
        private readonly ILog _log;

        private BufferBlock<T> _buffer;

        private ActionBlock<IEnumerable<T>> _actionBlock;

        public Batcher(
            Action<IEnumerable<T>> processBatch,
            int batchSize,
            TimeSpan maxTimeWaitingForBatch)
        {
            _log = LogProvider.For<Batcher<T>>();
            Initialize(batchSize, processBatch, maxTimeWaitingForBatch);
        }

        private void Initialize(int batchSize,
            Action<IEnumerable<T>> processBatch,
            TimeSpan maxTimeWaitingForBatch)
        {
            _buffer = new BufferBlock<T>();

            var batchStockEvents = new BatchBlock<T>(batchSize);

            // Use a timer to make sure that items do not remain in memory for too long
            var triggerBatchTimer = new Timer(delegate { batchStockEvents.TriggerBatch(); });

            // Use a transform block to reset the timer whenever an item is inserted to avoid unnessary batches
            var timeoutBlock = new TransformBlock<T, T>((value) =>
            {
                triggerBatchTimer.Change(maxTimeWaitingForBatch, TimeSpan.FromMilliseconds(-1));
                return value;
            });

            _actionBlock = new ActionBlock<IEnumerable<T>>(processBatch);

            _buffer.LinkTo(timeoutBlock);
            timeoutBlock.LinkTo(batchStockEvents);
            batchStockEvents.LinkTo(_actionBlock);

            _actionBlock.Completion.ContinueWith(x =>
            {
                if (x.Exception != null)
                {
                    _log.ErrorException("Exiting batcher due to error.", x.Exception);
                }

            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void Post(T someObject)
        {
            if (_actionBlock.Completion.IsCompleted)
            {
                throw new InvalidOperationException("The batcher is not accepting new objects as it is in a failed state");
            }

            _buffer.Post(someObject);
        }
    }
}
