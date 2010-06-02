﻿
namespace GMap.NET.Internals
{
   using System;
   using System.Threading;

   public sealed class FastReaderWriterLock
   {
      Int32 busy = 0;
      Int32 readCount = 0;

      public void AcquireReaderLock()
      {
         Thread.BeginCriticalRegion();

         while(Interlocked.CompareExchange(ref busy, 1, 0) != 0)
         {
            Thread.Sleep(1);
         }
        
         Interlocked.Increment(ref readCount);
         
         // somehow this fix deadlock on heavy reads
         Thread.Sleep(0);
         Thread.Sleep(0);
         Thread.Sleep(0);
         Thread.Sleep(0);
         Thread.Sleep(0);
         Thread.Sleep(0);
         Thread.Sleep(0);

         Interlocked.Exchange(ref busy, 0);

         // Debug.WriteLine("AcquireReaderLock(" + numReads + "): " +  + Thread.CurrentThread.ManagedThreadId);
      }

      public void ReleaseReaderLock()
      {
         Interlocked.Decrement(ref readCount);
         Thread.EndCriticalRegion();

         // Debug.WriteLine("ReleaseReaderLock: " + Thread.CurrentThread.ManagedThreadId);
      }

      public void AcquireWriterLock()
      {
         Thread.BeginCriticalRegion();

         while(Interlocked.CompareExchange(ref busy, 1, 0) != 0)
         {
            Thread.Sleep(1);
         }

         while(Interlocked.CompareExchange(ref readCount, 0, 0) != 0)
         {
            Thread.Sleep(1);
         }

         // Debug.WriteLine("AcquireWriterLock: " + Thread.CurrentThread.ManagedThreadId);
      }

      public void ReleaseWriterLock()
      {
         Interlocked.Exchange(ref busy, 0);
         Thread.EndCriticalRegion();

         // Debug.WriteLine("ReleaseWriterLock: " + Thread.CurrentThread.ManagedThreadId);
      }
   }
}