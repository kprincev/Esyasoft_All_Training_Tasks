
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Task6TbTOAPIToRmq_API_For_Publish_msg_;
using Serilog;

namespace MdmsApiCreateForDataSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RmqPublisher : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RmqPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("PublishToRmq")]
        public async Task<IActionResult> FetchData([FromBody] Payload payload )
        {
            Log.Information("Receive Request From client");
            string json = payload.JsonData;
            string queueName = payload.QueueName;
            
            RMQPublisherService ob=new RMQPublisherService();
            
            try {
                ob.PublishMeassage(json, queueName, _configuration);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return BadRequest(ex.Message);
            }
            Log.Information("Return success response to client");
            return Ok();
        }
    }
}