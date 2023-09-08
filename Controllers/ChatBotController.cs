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