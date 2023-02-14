
//Created by Alexander Fields 

using System;

namespace Optimization.Utility
{
    public static class Memory
    {
        /// <summary>
        /// Increase the heap size
        /// </summary>
        /// <param name="amount">1 is 1 GB</param>
        /// <returns></returns>
        public static long IncreaseMemorySize(long amount)
        {
            GC.AddMemoryPressure(amount * 1024 * 1024 * 1024);

            return GC.GetTotalMemory(false);
        }

        public static long IncreaseMemorySize2GB()
        {
            GC.AddMemoryPressure(2L * 1024 * 1024 * 1024);

            return GC.GetTotalMemory(false);
        }

        public static long IncreaseMemorySize4GB()
        {
            GC.AddMemoryPressure(4L * 1024 * 1024 * 1024);

            return GC.GetTotalMemory(false);
        }

      
    }
}
