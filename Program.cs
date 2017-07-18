using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace thread_demo
{
    class Program
    {


        static void Main(string[] args)
        {
            int state = 0;

            Action __delegate = null; 
            Task __task = null;
            IAsyncResult handle = null;
            ConcurrentQueue<int> exchange = new ConcurrentQueue<int>();

            int outValue = 0;
            //the main state machine
            while (state != 500)
            {
                switch (state)
                {
                    case 0:
                        // use Task 
                        __task = new Task(new Action(method));
                        __task.Start();
                        state +=10;
                        break;
                    case 10:
                        // wait until task done
                        if (__task.IsCompleted)
                            state +=10;
                        break;
                    case 20:
                        // use thread pool
                        ThreadPool.QueueUserWorkItem((object foo)=>{
                            method();
                            exchange.Enqueue(0); // enqueue some meaningfll result
                        },null);
                        state+=10;
                        break;
                    case 30:
                        //wait until method had been scheduled and executed
                        if (exchange.TryDequeue(out outValue))
                            state +=10;
                        break;
                    case 40:
                        // use delegate/IAsyncResult 
                        __delegate =  new Action(method);
                        handle = __delegate.BeginInvoke((IAsyncResult ar)=> 
                            {
                                method();
                                __delegate.EndInvoke(ar); // you had to call this to release ar
                            },null);
                        break;
                    case 50:
                        // wait until method had been executed
                        if (handle.IsCompleted)
                            state = 500;
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine("");
        }

        static void method()
        {
            //reflect the current thread
            Console.WriteLine(System.Threading.Thread.CurrentThread.Name);
        }

    }
}
