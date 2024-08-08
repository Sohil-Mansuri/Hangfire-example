using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace ScheduleJob.Controllers
{
    [Route("api/currency")]
    [ApiController]
    [APIKeyHeader]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyRepo _currencyRepo;
        private readonly IConfiguration _config;
        private readonly string _apiKey = string.Empty;

        public CurrencyController(ICurrencyRepo currencyRepo, IConfiguration configuration)
        {
            _currencyRepo = currencyRepo;
            _config = configuration;
            _apiKey = _config["APIKey"] ?? string.Empty;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var curriencies = await _currencyRepo.GetAllAsync();
            var result = curriencies.Adapt<IList<CurrencyDto>>();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id/*, [FromHeader(Name = "X-api-key")] string apiKey*/)
        {
            //if (apiKey != _apiKey) return Unauthorized();
            var currncy = await _currencyRepo.GetByIDAsync(id);
            if (currncy == null) return NotFound();

            return Ok(currncy);
        }

        [HttpPost]
        public async Task<IActionResult> PostCurrency([FromBody] CurrencyDto currencyDto)
        {
            var currency = currencyDto.Adapt<Currency>();
            await _currencyRepo.AddAsync(currency);
            return Created();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var currency = await _currencyRepo.DeleteAsync(id);
            if (currency is null) return NotFound();
            return Ok("deleted");
        }


        //delayJob
        //Delayed jobs are executed only once but not immediately, after a specified delay.
        [HttpPost("schedule/delay")]
        public IActionResult ScheduleJob([FromBody] CurrencyDto currencyDto)
        {
            var delay = currencyDto.ScheduleTime - DateTime.UtcNow;

            if (delay > TimeSpan.Zero)
            {
                var currency = currencyDto.Adapt<Currency>();
                BackgroundJob.Schedule(() => _currencyRepo.AddAsync(currency), delay);
                return Ok("your job is set");
            }
            else
            {
                return BadRequest("schedule time is not currect");
            }
        }

        //Fire and forget
        //Fire-and-forget jobs are executed only once and almost immediately after they are created.
        [HttpPost("schedule/fire")]
        public IActionResult FireJob([FromBody] CurrencyDto currencyDto)
        {
            var currency = currencyDto.Adapt<Currency>();
            BackgroundJob.Enqueue(() => _currencyRepo.AddAsync(currency));
            return Ok("Operation done");
        }

        //Recurring jobs
        //Recurring jobs are executed many times on the specified CRON schedule.
        //http://www.cronmaker.com/?1
        //https://crontab.guru/
        [HttpPost("schedule/recurring")]
        public IActionResult AddRecurringJob([FromBody] CurrencyDto currencyDto)
        {
            var currency = currencyDto.Adapt<Currency>();

            RecurringJob.AddOrUpdate<ICurrencyRepo>($"jobID:{0}", servie => servie.AddAsync(currency), "* * * * *");
            return Ok(" Recurring Job created");
        }


        //Countineu job 
        //Continuation jobs are executed when their parent job has finished.
        [HttpPost("schedule/continueJob")]
        public IActionResult ContinueJob([FromBody] CurrencyDto currencyDto)
        {
            var currency = currencyDto.Adapt<Currency>();

            //parentJob
            var parentJobId = BackgroundJob.Enqueue(() => _currencyRepo.AddAsync(currency));
            currency.Name = currency.Name + "1";
            BackgroundJob.ContinueJobWith(parentJobId, () => _currencyRepo.AddAsync(currency), JobContinuationOptions.OnlyOnSucceededState);
            return Ok("jobs are scheduled");
        }

        [HttpDelete("schedule/deleteRecurringJob")]
        public IActionResult StopRecurringJob(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId);
            return Ok($"{jobId} stopped ");
        }

        [HttpPut("schedule/updateRecurringJob")]
        public IActionResult UpdateRecussingJob([FromBody] CurrencyDto currencyDto, string jobId, string crontime)
        {
            var currency = currencyDto.Adapt<Currency>();

            RecurringJob.AddOrUpdate(jobId, () => _currencyRepo.AddAsync(currency), crontime);
            return Ok($"{jobId} is updated");
        }


    }
}
