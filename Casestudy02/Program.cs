using System;
using System.Threading;

namespace OS_Problem_02
{
    class Thread_safe_buffer
    {
        static int[] TSBuffer = new int[10];
        static int Front = 0;
        static int Back = 0;
        static int Count = 0;
        static int Item = 0;

        static readonly object lockObj = new object();
        static List<string> exitLogs = new List<string>();
        static readonly object lockLogs = new object();

        static void EnQueue(int eq, object t)
        {
            lock (lockObj)
            {
                while (Count == TSBuffer.Length)
                {
                    Monitor.Wait(lockObj);
                    Console.WriteLine("..........[Threat-{0}]:Queue full, waiting..........", t);
                }

                TSBuffer[Back] = eq;
                Back = (Back + 1) % TSBuffer.Length;
                Count++;
                Item++;

                Monitor.PulseAll(lockObj);
            }
        }
        static int DeQueue()
        {
            lock (lockObj)
            {
                while (Count == 0)
                {
                    if (Item == 0)
                    {
                        Monitor.PulseAll(lockObj);
                        return -1;
                    }
                    Monitor.Wait(lockObj);
                }

                int x = TSBuffer[Front];
                Front = (Front + 1) % TSBuffer.Length;
                Count--;
                Item--;

                Monitor.PulseAll(lockObj);

                return x;
            }
        }

        static void th01(object t)
        {
            int i;

            for (i = 1; i < 51; i++)
            {
                EnQueue(i, t);
                Thread.Sleep(5); //ห้ามแก้ไขหรือเปลี่ยนแปลงบรรทัดนี้/Editing or Modification of this line is forbidden
            }
        }

        static void th011(object t)
        {
            int i;

            for (i = 100; i < 151; i++)
            {
                EnQueue(i, t);
                Thread.Sleep(7); //ห้ามแก้ไขหรือเปลี่ยนแปลงบรรทัดนี้/Editing or Modification of this line is forbidden
            }
        }


        static void th02(object t)
        {
            int i;
            int j;

            for (i = 0; i < 60; i++)
            {
                j = DeQueue();
                if (j == -1)
                {
                    lock (lockLogs)
                    {
                        exitLogs.Add($"thread-{t} exit");
                    }
                    return;
                }
                Console.WriteLine("j={0}, thread:{1}", j, t);
                Thread.Sleep(16); //ห้ามแก้ไขหรือเปลี่ยนแปลงบรรทัดนี้/Editing or Modification of this line is forbidden
            }
            lock (lockLogs)
            {
                exitLogs.Add($"thread-{t} exit");
            }
        }
        static void Main(string[] args)
        {
            Thread t1 = new Thread(th01);
            Thread t11 = new Thread(th011);
            Thread t2 = new Thread(th02);
            Thread t21 = new Thread(th02);
            Thread t22 = new Thread(th02);

            t1.Start(100);
            t11.Start(200);
            t2.Start(1);
            t21.Start(2);
            t22.Start(3);

            t1.Join();
            t11.Join();
            t2.Join();
            t21.Join();
            t22.Join();

            Console.WriteLine("Press any key to exit...");

            Console.ReadKey();

            foreach (string log in exitLogs)
            {
                Console.WriteLine(log);
            }
        }
    }
}
