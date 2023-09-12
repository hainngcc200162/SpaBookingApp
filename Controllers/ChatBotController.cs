using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Completions;

namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatBotController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> GetAIBasedResult(string SearchText)
        {
            string APIKey = "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";
            string answer = string.Empty;

            var openai = new OpenAIAPI(APIKey);
            CompletionRequest completion = new CompletionRequest();

            string promptWithRequest = "Please answer briefly: " + SearchText;

            completion.Prompt = promptWithRequest;
            completion.Model = OpenAI_API.Models.Model.DavinciText;
            completion.MaxTokens = 200;
            
            var result = openai.Completions.CreateCompletionAsync(completion);
            foreach(var item in result.Result.Completions)
            {
                answer = item.Text;
            }

            return Ok(answer);
        }
    }
}