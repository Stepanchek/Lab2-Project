using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PartyQuiz.Utils
{
    public static class UniTaskExtensions
    {
        private static readonly Action<Task> _handleFinishedTask = HandleFinishedTask;

        public static void HandleExceptions(this Task task)
        {
            task.ContinueWith(_handleFinishedTask);
        }

        private static void HandleFinishedTask(Task task)
        {
            if (task.IsFaulted)
                Debug.LogException(task.Exception);
        }

        public static void HandleExceptions(this UniTask task)
        {
            HandleExceptions(task.AsTask());
        }


        public static void HandleExceptions<T>(this UniTask<T> task) where T : class
        {
            HandleExceptions(task.AsTask());
        }
    }
}