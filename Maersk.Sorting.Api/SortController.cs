using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.Controllers
{
    [ApiController]
    [Route("sort")]
    public class SortController : ControllerBase
    {
        private readonly ISortJobProcessor _sortJobProcessor;
        private static StackSortJob processingJob = new StackSortJob();
        private static StackSortJob completedJob = new StackSortJob();

        public SortController(ISortJobProcessor sortJobProcessor)
        {
            _sortJobProcessor = sortJobProcessor;
        }


        [HttpPost("EnqueueJob")]
        public async void EnqueueJob(int[] myNum)
        { 

            var pendingJob = new SortJob(
                id: Guid.NewGuid(),
                status: SortJobStatus.Pending,
                duration: null,
                input: myNum,
                output: myNum);

            processingJob.PushJobItem(pendingJob);

            while (!processingJob.IsEmpty())
            {
                var processed = await _sortJobProcessor.Process(processingJob.RemoveSortJobItem());
                completedJob.PushJobItem(processed);
            }         
        }

        [HttpGet("GetJobs")]
        public Task<List<SortJob>> GetJobs()
        {
            return Task.Run(() => {
                List<SortJob> result = new List<SortJob>();
                result.AddRange(processingJob.PrintStack());
                result.AddRange(completedJob.PrintStack());
                return result;
            });
           

        }

        [HttpGet("{jobId}")]
        public Task<SortJob> GetJob(Guid jobId)
        {
            List<SortJob> result = new List<SortJob>();
            result.AddRange(processingJob.PrintStack());
            result.AddRange(completedJob.PrintStack());
            
            return Task.Run(() => {
                return result.Where(it => it.Id == jobId).FirstOrDefault();
                
            });
        }

        public class StackSortJob
        {
            private const int MaxSize = 100;
            private SortJob[] _items = new SortJob[MaxSize];
            private int _currentIndex = -1;
            public StackSortJob()
            {
            }
            public void PushJobItem(SortJob element)
            {
                if (_currentIndex >= MaxSize - 1)
                {
                    throw new InvalidOperationException("Container is full");
                }
                _currentIndex++;
                _items[_currentIndex] = element;
            }
            public SortJob RemoveSortJobItem()
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("No elements Found");
                }
                SortJob element = _items[_currentIndex];
                _currentIndex--;
                return element;
            }
            public SortJob PeekSortJobItem()
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("No elements found");
                }
                return _items[_currentIndex];
            }
            public bool IsEmpty()
            {
                if (_currentIndex < 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public List<SortJob> PrintStack()
            {
                List<SortJob> result = new List<SortJob>();
                if (_currentIndex < 0)
                {
                    return result;
                }
                else
                {
                    
                    for (int i = _currentIndex; i >= 0; i--)
                    {
                        result.Add(_items[_currentIndex]);

                    }
                    return result;
                }
            }
        }


    }
}
