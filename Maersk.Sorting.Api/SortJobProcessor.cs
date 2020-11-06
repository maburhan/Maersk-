using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class SortJobProcessor : ISortJobProcessor
    {
        private readonly ILogger<SortJobProcessor> _logger;

        public SortJobProcessor(ILogger<SortJobProcessor> logger)
        {
            _logger = logger;
        }

        public async Task<SortJob> Process(SortJob job)
        {

            _logger.LogInformation("Processing job with ID '{JobId}'.", job.Id);

            var stopwatch = Stopwatch.StartNew();

            int[] arr = new int[job.Input.Length];
            Array.Copy(job.Input, 0, arr, 0, job.Input.Length);

            this.Sort(job.Input);

            await Task.Delay(5000); // NOTE: This is just to simulate a more expensive operation

            var duration = stopwatch.Elapsed;

            _logger.LogInformation("Completed processing job with ID '{JobId}'. Duration: '{Duration}'.", job.Id, duration);

            return new SortJob(
                id: job.Id,
                status: SortJobStatus.Completed,
                duration: duration,
                input: arr,
                output: job.Input);
        }

        //Implementing Bubble sort
        private void Sort(int[] arr)
        {
            int temp;
            for (int j = 0; j <= arr.Length - 2; j++)
            {
                for (int i = 0; i <= arr.Length - 2; i++)
                {
                    if (arr[i] > arr[i + 1])
                    {
                        temp = arr[i + 1];
                        arr[i + 1] = arr[i];
                        arr[i] = temp;
                    }
                }
            }
        }
     
    }
}
