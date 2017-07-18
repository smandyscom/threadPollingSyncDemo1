using System;

namespace thread_demo
{
    class Program
    {

        static Func<int> __delegate =  new Func<int>(method);
        static void Main(string[] args)
        {
            var handle = __delegate.BeginInvoke((IAsyncResult ar)=> 
            {
                method();
                __delegate.EndInvoke(ar);
            },null);

            while (!handle.IsCompleted)
            {
                
            }

            Console.WriteLine("");
        }

        static int method()
        {
            //reflect the current thread
            Console.WriteLine(System.Threading.Thread.CurrentThread.Name);
            return 0;
        }

    }
}
