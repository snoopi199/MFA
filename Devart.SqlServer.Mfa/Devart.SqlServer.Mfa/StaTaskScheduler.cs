using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Devart.SqlServer.Mfa {

  internal sealed class StaTaskScheduler : TaskScheduler, IDisposable {

    private BlockingCollection<Task> tasks;
    private readonly List<Thread> threads;

    public StaTaskScheduler(int numberOfThreads) {

      Func<int, Thread> selector = null;
      if (numberOfThreads < 1)
        throw new ArgumentOutOfRangeException("numberOfThreads");

      this.tasks = new BlockingCollection<Task>();
      if (selector == null) {
        selector = delegate (int i) {
          Thread thread = new Thread(() => {
            foreach (Task task in this.tasks.GetConsumingEnumerable()) {
              base.TryExecuteTask(task);
            }
          }) {
            IsBackground = true
          };
          thread.SetApartmentState(ApartmentState.STA);
          return thread;
        };
      }
      this.threads = Enumerable.Range(0, numberOfThreads).Select<int, Thread>(selector).ToList<Thread>();
      this.threads.ForEach(t => t.Start());
    }

    public void Dispose() {

      if (this.tasks != null) {
        this.tasks.CompleteAdding();
        foreach (Thread thread in this.threads)
          thread.Join();

        this.tasks.Dispose();
        this.tasks = null;
      }
    }

    public override int MaximumConcurrencyLevel => this.threads.Count;

    protected override IEnumerable<Task> GetScheduledTasks() => this.tasks.ToArray();

    protected override void QueueTask(Task task) => this.tasks.Add(task);

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) =>
        ((Thread.CurrentThread.GetApartmentState() == ApartmentState.STA) && base.TryExecuteTask(task));
  }
}
